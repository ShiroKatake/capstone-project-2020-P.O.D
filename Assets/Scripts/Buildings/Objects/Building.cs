using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;
using UnityEngine.Events;

[Serializable]
public struct RendererMaterialSet
{
    public MeshRenderer renderer;
    public Material opaque;
    public Material transparent;
}

/// <summary>
/// A building placed by the player.
/// </summary>
public class Building : CollisionListener
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Identification")]
    [SerializeField] private int id;
    [SerializeField] private EBuilding buildingType;
    [SerializeField] private string consoleName;

    [Header("Resource Requirements")]
    [SerializeField] private int oreCost;
    [SerializeField] private int powerConsumption;
    [SerializeField] private int waterConsumption;
    [SerializeField] private int wasteConsumption;

    //Note: if you need buildings to supply resources, the ResourceCollector component already has you covered 
    //there, and it should be interacted with on shutdown / restoration through Building.Operational. If it's 
    //not doing it's job, fix it rather than adding resource gathering to Building.

    [Header("Building")]
    [SerializeField] private float buildTime;

    [Header("Offsets of Cliff Detection Raycasts from Position")]
    [SerializeField] private List<Vector3> cliffRaycastOffsets;

    [Header("Offsets of Foundations from Position")]
    [SerializeField] private List<Vector3> buildingFoundationOffsets;

    [Header("Model, Materials, etc.")]
    [SerializeField] private Transform model;
    [SerializeField] private List<RendererMaterialSet> rendererMaterialSets;
    [SerializeField] private Material buildingErrorMaterial;

    [Header("Sound Library")]
    [SerializeField] private AudioManager.ESound idleSound;

    [Header("Effects")]
	[SerializeField] private FinishedFX constructionFinishedFX;
	[SerializeField] private float fxSize = 1f;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    [Header("Testing")]
	//Components
	private Animator animator;
    private TurretRangeFX turretRangeFX;
    private FusionReactorBeam fusionReactorBeam;
    private Health health;
    private List<GameObject> particleSystems;
    private MeshRenderer parentRenderer;
    private List<MeshRenderer> allRenderers;
    private ResourceCollector resourceCollector;
    private Rigidbody rigidbody;
    private Size size;
    private Terraformer terraformer;
    private TurretAiming turretAimer;
    private TurretShooting turretShooter;
    private Dictionary<string, List<CollisionReporter>> groupedReporters;

    //Positioning
    private bool colliding = false;
    private bool validPlacement = true;
	private bool placementCurrentValid = true;
	private bool materialChanged = false;
	private List<Collider> otherColliders;
    Vector3 normalScale;
    LayerMask groundLayerMask;

    //Other
    [SerializeField] private bool awake;
    [SerializeField] private bool active = false;
    [SerializeField] private bool placed = false;
    [SerializeField] private bool operational = false;
    [SerializeField] private bool built;
    private float normalBuildTime;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------
    
	/// <summary>
	/// Whether the building is active and in the scene, or has been pooled and is inactive. Active should only be set in BuildingFactory.
	/// </summary>
	public bool Active { get => active; set => active = value; }

    /// <summary>
    /// The position of building foundations relative to the building's transform.position value.
    /// </summary>
    public List<Vector3> BuildingFoundationOffsets { get => buildingFoundationOffsets; }

    /// <summary>
    /// The type of building this building is.
    /// </summary>
    public EBuilding BuildingType { get => buildingType; }

    /// <summary>
    /// How long this building takes to builds itself when the player places it in the scene. Should only be set by BuildingFactory.
    /// </summary>
    public float BuildTime { get => buildTime; set => buildTime = value; }
	
    /// <summary>
    /// Has the building been placed and been fully built?
    /// </summary>
    public bool Built { get => built; }

    /// <summary>
    /// What should this building's name be and how should it be formatted when printed in the console?
    /// </summary>
    public string ConsoleName { get => consoleName; }

    /// <summary>
    /// The Building's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// The transform of the building's model.
    /// </summary>
    public Transform Model { get => model; }

    /// <summary>
    /// How much ore it costs to build this building.
    /// </summary>
    public int OreCost { get => oreCost; }

    /// <summary>
    /// Whether the building has been placed or not.
    /// </summary>
    public bool Placed { get => placed; }

    /// <summary>
    /// How much power this building requires per second to function.
    /// </summary>
    public int PowerConsumption { get => powerConsumption; }

    /// <summary>
    /// Size information regarding this building.
    /// </summary>
    public Size Size { get => size; }

    /// <summary>
    /// The building's resource collector component, if it has one.
    /// </summary>
    public ResourceCollector ResourceCollector { get => resourceCollector; }

    /// <summary>
    /// The building's terraformer component, if it has one.
    /// </summary>
    public Terraformer Terraformer { get => terraformer; }

    /// <summary>
    /// The building's TurretShooting component, if it has one.
    /// </summary>
    public TurretShooting TurretShooter { get => turretShooter; }

    /// <summary>
    /// How much waste this building requires per second to function.
    /// </summary>
    public int WasteConsumption { get => wasteConsumption; }

    /// <summary>
    /// How much water this building requires per second to function.
    /// </summary>
    public int WaterConsumption { get => waterConsumption; }

    /// <summary>
    /// This building's TurretRangeFX decal if it's a turret.
    /// </summary>
    public TurretRangeFX TurretRangeFX { get => turretRangeFX; set => turretRangeFX = value; }

    //Complex Public Properties--------------------------------------------------------------------                                                    

    /// <summary>
    /// The Building's unique ID number. Id should only be set by BuildingFactory.GetBuilding().
    /// </summary>
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
            gameObject.name = $"{buildingType} {id}";
        }
    }

    /// <summary>
    /// Whether or not the building is operational and doing its job. When set, also triggers any appropriate resource collector state changes.
    /// </summary>
    public bool Operational
    {
        get
        {
            return operational;
        }

        set
        {
            if (operational != value)
            {
                operational = (value && active);

                if (resourceCollector != null)
                {
                    if (operational)
                    {
                        resourceCollector.Activate();
                    }
                    else
                    {
                        resourceCollector.Deactivate();
                    }
                }
            }
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (!awake)
        {
            //Debug.Log("Building Awake()");
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                Debug.Log($"{this} building is missing an animator component.");
            }

            fusionReactorBeam = GetComponent<FusionReactorBeam>();
            health = GetComponent<Health>();
            size = GetComponent<Size>();
            parentRenderer = GetComponentInChildren<MeshRenderer>();
            allRenderers = new List<MeshRenderer>(parentRenderer.GetComponentsInChildren<MeshRenderer>());
            rigidbody = GetComponentInChildren<Rigidbody>();
            resourceCollector = GetComponent<ResourceCollector>();
            terraformer = GetComponent<Terraformer>();
            turretAimer = GetComponent<TurretAiming>();
            turretShooter = GetComponent<TurretShooting>();
            collisionReporters = GetCollisionReporters();
            groupedReporters = new Dictionary<string, List<CollisionReporter>>();
            otherColliders = new List<Collider>();
            normalScale = transform.localScale;
            normalBuildTime = buildTime;
            groundLayerMask = LayerMask.GetMask("Ground");

            particleSystems = new List<GameObject>();
            ParticleSystem[] particleSystemsRaw = GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem p in particleSystemsRaw)
            {
                particleSystems.Add(p.gameObject);
            }

            foreach (CollisionReporter c in collisionReporters)
            {
                if (!groupedReporters.ContainsKey(c.Purpose))
                {
                    groupedReporters[c.Purpose] = new List<CollisionReporter>();
                }

                groupedReporters[c.Purpose].Add(c);
            }

            if (size.DiameterRoundedUp < 1 || size.DiameterRoundedUp > 5)
            {
                Debug.LogError("Building.Size.RadiusRoundedUp is invalid. It needs to be between 1 and 5.");
            }

            awake = true;
        }
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            if (buildingType != EBuilding.CryoEgg && animator.enabled)
            {
                animator.SetFloat("Health", health.CurrentHealth);
                animator.SetBool("Operational", operational);
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Building Triggered Methods-------------------------------------------------------------------

    /// <summary>
    /// Enables or disables all colliders attached to the building's collision reporters with the listed purpose.
    /// </summary>
    /// <param name="purpose">The purpose of the collision reporters to have their colliders enabled or disabled.</param>
    /// <param name="enabled">Whether the collision reporters' colliders will be enabled or disabled.</param>
    public void SetCollidersEnabled(string purpose, bool enabled)
    {
        foreach (CollisionReporter r in groupedReporters[purpose])
        {
            r.SetCollidersEnabled(enabled);
        }
    }

    /// <summary>
    /// Enables or disables all mesh renderers attached to the building's models.
    /// </summary>
    /// <param name="enabled">Whether the mesh renderers will be enabled or disabled.</param>
    public void SetMeshRenderersEnabled(bool enabled)
    {
        foreach (RendererMaterialSet s in rendererMaterialSets)
        {
            s.renderer.enabled = enabled;
        }
    }

    /// <summary>
    /// Enables or disables the game objects of all particle systems attached to the building's models.
    /// </summary>
    /// <param name="enabled">Whether the game objects of the particle systems will be enabled or disabled.</param>
    public void SetParticleSystemsEnabled(bool enabled)
    {
        foreach (GameObject p in particleSystems)
        {
            p.SetActive(enabled);
        }
    }

    /// <summary>
    /// Checks if the building is colliding while being placed, and updates colour appropriately.
    /// </summary>
    /// <returns>Is this building colliding with something?</returns>
    public bool IsPlacementValid()
    {
        if (active)
        {
            if (!placed)
            {
                validPlacement = !(CheckInPit() || CheckColliding() || CheckOnCliff() || CheckMouseOverUI()) && MapController.Instance.PositionAvailableForBuilding(this);

				if (!validPlacement && placementCurrentValid)
				{
					BuildingFactory.Instance.onPlacementInvalid?.Invoke();
					placementCurrentValid = false;
					materialChanged = false;
				}

				else if (validPlacement && !placementCurrentValid)
				{
					BuildingFactory.Instance.onPlacementValid?.Invoke();
					placementCurrentValid = true;
					materialChanged = false;
				}

				if (!materialChanged)
				{
					foreach (RendererMaterialSet r in rendererMaterialSets)
					{
						Material currentMaterial = (validPlacement ? r.transparent : buildingErrorMaterial);

						for (int i = 0; i < r.renderer.materials.Length; i++)
						{
							UpdateRendererMaterials(r.renderer, currentMaterial, r.renderer.materials.Length);
							break;
						}
					}

					materialChanged = true;
				}

				return validPlacement;
            }
            else
            {
                Debug.Log($"Building {id} ran IsPlacementValid(), even though it's already placed.");
                return false;
            }
        }
        else
        {
            return true;
        }

    }

    /// <summary>
    /// Checks if the mouse is over the UI before placement.
    /// </summary>
    private bool CheckMouseOverUI()
    {        
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = ReInput.controllers.Mouse.screenPosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        foreach (RaycastResult r in raycastResultList)
        {
            if (r.gameObject.GetComponent<MouseClickThrough>() != null)
            {
                //Debug.Log("Over UI");
                return true;
            }
        }

        //Debug.Log("Not Over UI");
        return false;
    }

    /// <summary>
    /// Checks if this building is currently in a pit.
    /// </summary>
    private bool CheckInPit()
    {
        return transform.position.y < -0.1f;
        //bool result = transform.position.y < -0.1f;
        //Debug.Log($"{this} in pit: {result}");
        //return result;
    }

    /// <summary>
    /// Verifies if this building should be considered to be colliding with another object.
    /// </summary>
    private bool CheckColliding()
    {
        //Weird quirk of destroying one object and then instantating another and moving it to the same position: it triggers boths' OnTriggerEnter(),
        //even though one doesn't exist, and then the other doesn't have OnTriggerExit() triggered in the next frame. This checks for the existence of
        //the other collider and corrects the value of colliding if the other collider no longer exists.
        if (colliding)
        {
            if (otherColliders.Count == 0)
            {
                colliding = false;
            }
            else
            {
                colliding = false;

                for (int i = 0, j = otherColliders.Count; i < j; i++)
                {
                    if (otherColliders[i] == null)
                    {
                        otherColliders.RemoveAt(i);
                        i--;
                        j--;
                    }
                    else
                    {
                        colliding = true;
                        break;
                    }
                }
            }
        }

        //Debug.Log($"{this} touching another collider: {colliding}");

        //if (!colliding)
        //{



        //    Debug.Log($"{this} occupying an already occupied space: {colliding}");
        //}

        //Debug.Log($"{this} colliding: {colliding}");
        return colliding;
    }

    /// <summary>
    /// Verifies if this building is extending over a cliff edge.
    /// </summary>
    private bool CheckOnCliff()
    {
        RaycastHit hit;
        Vector3 raycastPos;
        float maxDistance = 0.68f;

        foreach (Vector3 offset in cliffRaycastOffsets)
        {
            raycastPos = transform.position + offset;

            if (!Physics.Raycast(raycastPos, Vector3.down, out hit, 20, groundLayerMask) || hit.distance > maxDistance)
            {
                //Debug.Log($"{this} on cliff");
                return true;
            }
        }

        //Debug.Log($"{this} not on cliff");
        return false;
    }

    /// <summary>
    /// Gives a renderer a specified number of copies of the required material.
    /// </summary>
    /// <param name="renderer">The renderer getting its material(s) updated.</param>
    /// <param name="material">The material to apply to the renderer.</param>
    /// <param name="count">How many times the material needs to be applied to the renderer for every model the renderer is responsible for to be covered.</param>
    private void UpdateRendererMaterials(Renderer renderer, Material material, int count)
    {
        List<Material> materials = new List<Material>();

        for (int i = 0; i < count; i++)
        {
            materials.Add(material);
        }

        renderer.materials = materials.ToArray();
    }

    /// <summary>
    /// Places the building, using up the appropriate resources, positioning and solidifying it, and triggering Build().
    /// </summary>
    /// <param name="position">Where the building is to be placed.</param>
    public void Place(Vector3 position)
    {
        placed = true; //Needs to occur before its position gets set to be on the ground so that it triggers the building Foundation at the proper time.
        ResourceController.Instance.Ore -= oreCost;
		ResourceController.Instance.PowerConsumption += powerConsumption;
		ResourceController.Instance.WaterConsumption += waterConsumption;
		ResourceController.Instance.WasteConsumption += wasteConsumption;
		SetCollidersEnabled("Placement", false);
        SetCollidersEnabled("Body", true);
        transform.position = position;
        BuildingController.Instance.RegisterBuilding(this);
		BuildingFactory.Instance.onPlacementFinished?.Invoke();

        foreach (RendererMaterialSet r in rendererMaterialSets)
        {
            UpdateRendererMaterials(r.renderer, r.opaque, r.renderer.materials.Length);
        }

        health.CurrentHealth = 0.01f;
        StartCoroutine(ProgressUpdate());

        animator.enabled = true;
    }

    private IEnumerator ProgressUpdate(){
        Debug.Log("Progress starting; starting health is: " + health.CurrentHealth);
        /*Material shd = null;
        for (int i = 0; i < gameObject.transform.childCount; i++){
            Debug.Log("Child " + i + ": " + gameObject.transform.GetChild(i));
            if (gameObject.transform.GetChild(i).name.Equals("Body Collider")){
                Debug.Log("Found the correct Child");
                for (int z = 0; z < gameObject.transform.GetChild(i).childCount; z++){
                    Debug.Log("Child " + z + ": " + gameObject.transform.GetChild(i).GetChild(z));
                    if (gameObject.transform.GetChild(i).GetChild(z).name.Equals("Base Model")){
                        shd = gameObject.transform.GetChild(i).GetChild(z).GetComponent<Material>();
                        Debug.Log("Child Shader: " + gameObject.transform.GetChild(i).GetChild(z).GetComponent<Material>());
                        break;
                    }
                }
                break;
            }
        }
        if (shd != null){
            shd.GetFloat("_DissolveAmount");
        } else {
            Debug.Log("shd is null");
        }*/
        while (health.CurrentHealth < health.MaxHealth){
            if (!PauseMenuManager.Paused){
                Debug.Log("health is: " + health.CurrentHealth + " ; Renderer is: " + rendererMaterialSets[0].renderer.name + " ; Material is: " + rendererMaterialSets[0].renderer.materials[0]);
                
                }
                if (rendererMaterialSets[0].renderer.materials[0].GetFloat("_DissolveAmount") == 1){
                    health.CurrentHealth = 0.01f;
                } else {
                    health.CurrentHealth = health.MaxHealth * rendererMaterialSets[0].renderer.materials[0].GetFloat("_DissolveAmount");
                    //health.CurrentHealth = health.MaxHealth * shd.GetPropertyDefaultFloatValue();
                }
                Debug.Log("health is: " + health.CurrentHealth);

            yield return null;
        }
    }

    /// <summary>
    /// Spawns a "building finished" particle effect.
    /// </summary>
	public void SpawnFinishedFX()
	{
		FinishedFX fx = FinishedFXFactory.Instance.Get();
		fx.transform.position = transform.position;
		fx.transform.localScale = new Vector3(fxSize, fxSize, fxSize);
		fx.gameObject.SetActive(true);
    }

    /// <summary>
    /// Handles what should happen once the building has been built.
    /// </summary>
    public void FinishBuilding()
    {
        built = true;
        Operational = true; //Using property to trigger activation of any resource collector component attached.

        if (turretShooter != null)
        {
            turretShooter.Place();
        }

        AudioManager.Instance.PlaySound(idleSound, gameObject);
        AudioManager.Instance.PlaySound(AudioManager.ESound.Building_Completes, gameObject);
    }

    /// <summary>
    /// Resets Building to its initial values when it is returned to the building pool.
    /// </summary>
    public void Reset()
    {
        StopCoroutine(ProgressUpdate());
        placed = false; //Needs to occur first so that BuildingFoundations know to ignore this building
        active = false;
        colliding = false;
        built = false;

        animator.enabled = false;
        health.Reset();
        Operational = false;

        otherColliders.Clear();
        parentRenderer.transform.localPosition = Vector3.zero;
        transform.localScale = normalScale;
        buildTime = normalBuildTime;

        switch (buildingType)
        {
            case EBuilding.FusionReactor:
                fusionReactorBeam.SetBeamActive(false);
                break;
            case EBuilding.ShortRangeTurret:
            case EBuilding.LongRangeTurret:
                turretAimer.Reset();
                turretShooter.Reset();
				BuildingFactory.Instance.onPlacementFinished?.Invoke();
				break;
        }

        foreach (RendererMaterialSet r in rendererMaterialSets)
        {
            UpdateRendererMaterials(r.renderer, r.opaque, r.renderer.materials.Length);
            r.renderer.enabled = false;
        }

        SetCollidersEnabled("Body", false);
        SetParticleSystemsEnabled(false);		
	}

    //ICollisionListener Triggered Methods---------------------------------------------------------

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{this}.OnTriggerEnter, other is {other}");

        if (active && !operational && !other.isTrigger)
        {
            //Debug.Log($"Active, not operational, and !other.isTrigger.");
            colliding = true;

            if (!otherColliders.Contains(other))
            {
                otherColliders.Add(other);
            }
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerExit(Collider other)
    {
        //Debug.Log($"{this}.OnTriggerExit, other is {other}");

        if (active && !operational && !other.isTrigger)
        {
            //Debug.Log($"Active, not operational, and !other.isTrigger");

            if (otherColliders.Contains(other))
            {
                otherColliders.Remove(other);
            }

            if (otherColliders.Count == 0)
            {
                colliding = false;
            }
        }
    }
}