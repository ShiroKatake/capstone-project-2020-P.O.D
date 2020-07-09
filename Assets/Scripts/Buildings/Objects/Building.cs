using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("ID")]
    [SerializeField] private int id;

    [Header("Building Type")]
    [SerializeField] private EBuilding buildingType;

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
    [SerializeField] private float buildStartHeight;
    [SerializeField] private float boingInterval;
    [SerializeField] private float smallBoingMultiplier;
    [SerializeField] private float largeBoingMultiplier;

    [Header("Offsets of Cliff Detection Raycasts from Position")]
    [SerializeField] private List<Vector3> cliffRaycastOffsets;

    [Header("Offsets of Foundations from Position")]
    [SerializeField] private List<Vector3> buildingFoundationOffsets;

    [Header("Renderers and Materials")]
    [SerializeField] private List<RendererMaterialSet> rendererMaterialSets;
    [SerializeField] private Material buildingErrorMaterial;

    [Header("Sound Library")]
    [SerializeField] private AudioManager.ESound idleSound;



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    //Components
    private Animator animator;
    private Health health;
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
    private List<Collider> otherColliders;
    Vector3 normalScale;
    LayerMask groundLayerMask;

    //Other
    [SerializeField] private bool active = false;
    private bool placed = false;
    [SerializeField] private bool operational = false;
    private float normalBuildTime;
    private bool boinging = false;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Whether the building is active and in the scene, or has been pooled and is inactive. Active should only be set in BuildingFactory.
    /// </summary>
    public bool Active { get => active; set => active = value; }

    /// <summary>
    /// Is the building going "boing" to indicate that it has finished building?
    /// </summary>
    public bool Boinging { get => boinging; }

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
    /// The Building's Health component.
    /// </summary>
    public Health Health { get => health; }

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
        animator = GetComponent<Animator>();
		if (animator == null)
		{
			Debug.Log($"{this} building is missing an animator component.");
		}
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

        foreach (CollisionReporter c in collisionReporters)
        {
            if (!groupedReporters.ContainsKey(c.Purpose))
            {
                groupedReporters[c.Purpose] = new List<CollisionReporter>();
            }

            groupedReporters[c.Purpose].Add(c);
        }

        if (size.DiameterRoundedUp < 1 || size.DiameterRoundedUp > 3)
        {
            Debug.LogError("Building.Size.RadiusRoundedUp is invalid. It needs to be between 1 and 3.");
        }
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
		if (buildingType != EBuilding.CryoEgg && animator.enabled)
		{
			animator.SetFloat("Health", health.Value);
			animator.SetBool("Operational", operational);
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
    /// Checks if the building is colliding while being placed, and updates colour appropriately.
    /// </summary>
    /// <returns>Is this building colliding with something?</returns>
    public bool IsPlacementValid()
    {
        if (active)
        {
            if (!placed)
            {
                validPlacement = !(CheckInPit() || CheckColliding() || CheckOnCliff()) && MapController.Instance.PositionAvailableForBuilding(this);

                if (validPlacement)
                {
                    foreach (RendererMaterialSet r in rendererMaterialSets)
                    {
                        if (r.renderer.material != r.transparent)
                        {
                            r.renderer.material = r.transparent;
                        }
                    }
                }
                else
                {
                    foreach (RendererMaterialSet r in rendererMaterialSets)
                    {
                        if (r.renderer.material != buildingErrorMaterial)
                        {
                            r.renderer.material = buildingErrorMaterial;
                        }
                    }
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
    /// Checks if this building is currently in a pit.
    /// </summary>
    private bool CheckInPit()
    {
        return transform.position.y < -0.1f;
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
                return true;
            }
        }

        return false;
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

        foreach (RendererMaterialSet r in rendererMaterialSets)
        {
            r.renderer.material = r.opaque;
        }

        transform.position = position;
        BuildingController.Instance.RegisterBuilding(this);
        animator.enabled = true;
    }

    /// <summary>
    /// Handles what should happen once the building has been built.
    /// </summary>
    public void Built()
    {
        Operational = true; //Using property to trigger activation of any resource collector component attached.

        if (turretShooter != null)
        {
            turretShooter.Place();
        }

        AudioManager.Instance.PlaySound(idleSound, gameObject);
        AudioManager.Instance.PlaySound(AudioManager.ESound.Building_Completes, gameObject);

        //Vector3 startPos = new Vector3(0, 0, buildStartHeight);
        //Vector3 endPos = Vector3.zero;
        //float buildTimeElapsed = 0;

        //Vector3 smallScale = normalScale * smallBoingMultiplier;
        //Vector3 largeScale = normalScale * largeBoingMultiplier;
        //float boingTimeElapsed = 0;

        //AudioManager.Instance.PlaySound(AudioManager.ESound.Building_Materialises, this.gameObject); //needs to be stopped when finished building
        //while (buildTimeElapsed < buildTime)
        //{
        //    buildTimeElapsed += Time.deltaTime;
        //    parentRenderer.transform.localPosition = Vector3.Lerp(startPos, endPos, buildTimeElapsed / buildTime);
        //    yield return null;
        //}

        //boinging = true;

        //while (boingTimeElapsed < boingInterval)
        //{
        //    boingTimeElapsed += Time.deltaTime;
        //    transform.localScale = Vector3.Lerp(normalScale, smallScale, boingTimeElapsed / boingInterval);
        //    yield return null;
        //}

        //boingTimeElapsed -= boingInterval;

        //while (boingTimeElapsed < boingInterval)
        //{
        //    boingTimeElapsed += Time.deltaTime;
        //    transform.localScale = Vector3.Lerp(smallScale, largeScale, boingTimeElapsed / boingInterval);
        //    yield return null;
        //}

        //boingTimeElapsed -= boingInterval;

        //while (boingTimeElapsed < boingInterval)
        //{
        //    boingTimeElapsed += Time.deltaTime;
        //    transform.localScale = Vector3.Lerp(largeScale, normalScale, boingTimeElapsed / boingInterval);
        //    yield return null;
        //}

        //boinging = false;
        //yield return null;
    }

    /// <summary>
    /// Resets Building to its initial values when it is returned to the building pool.
    /// </summary>
    public void Reset()
    {
        placed = false; //Needs to occur first so that BuildingFoundations know to ignore this building
        active = false;
        colliding = false;

        //TODO: reset animator? i.e. disable and set animation progress back to 0?
        animator.enabled = false;

        health.Reset();
        Operational = false;

        otherColliders.Clear();
        parentRenderer.transform.localPosition = Vector3.zero;
        transform.localScale = normalScale;
        buildTime = normalBuildTime;

        if (buildingType == EBuilding.ShortRangeTurret || buildingType == EBuilding.LongRangeTurret)
        {
            turretAimer.Reset();
            turretShooter.Reset();
        }

        foreach (RendererMaterialSet r in rendererMaterialSets)
        {
            r.renderer.material = r.transparent;
        }

        SetCollidersEnabled("Body", false);
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