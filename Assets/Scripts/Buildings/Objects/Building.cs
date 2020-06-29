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

    [Header("Building")]
    [SerializeField] private float buildTime;
    [SerializeField] private float buildStartHeight;
    [SerializeField] private float boingInterval;
    [SerializeField] private float smallBoingMultiplier;
    [SerializeField] private float largeBoingMultiplier;

    [Header("Offsets of Foundations from Position")]
    [SerializeField] private List<Vector3> buildingFoundationOffsets;

    [Header("Renderers and Materials")]
    [SerializeField] private List<RendererMaterialSet> rendererMaterialSets;
    [SerializeField] private Material buildingErrorMaterial;

    [Header("Sound Library")]
    [SerializeField] private AudioManager.ESound idleSound;

 

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    //Components
    private Health health;
    private MeshRenderer parentRenderer;
    private List<MeshRenderer> allRenderers;
    private ResourceCollector resourceCollector;
    private Size size;
    private Rigidbody rigidbody;
    private Terraformer terraformer;
    private TurretAiming turretAimer;
    private TurretShooting turretShooter;

    //Positioning
    //private Dictionary<string, Vector3> offsets;
    private bool colliding = false;
    [SerializeField] private List<Collider> otherColliders;
    Vector3 normalScale;

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

    ///// <summary>
    ///// The building's collider component.
    ///// </summary>
    //public Collider Collider { get => collider; }

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
    /// How much waste this building requires per second to function.
    /// </summary>
    public int WasteConsumption { get => wasteConsumption; }

    /// <summary>
    /// How much water this building requires per second to function.
    /// </summary>
    public int WaterConsumption { get => waterConsumption; }

    ///// <summary>
    ///// How many squares this building occupies along the x-axis.
    ///// </summary>
    //public int XSize { get => xSize; }

    ///// <summary>
    ///// How many squares this building occupies along the z-axis.
    ///// </summary>
    //public int ZSize { get => zSize; }

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
            //Debug.Log($"Pre-Setting: operational: {operational}, value: {value}, active: {active}");
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

            //Debug.Log($"Post-Setting: operational: {operational}, value: {value}, active: {active}");
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
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
        otherColliders = new List<Collider>();
        normalScale = transform.localScale;
        normalBuildTime = buildTime;

        if (size.DiameterRoundedUp < 1 || size.DiameterRoundedUp > 3)
        {
            Debug.LogError("Building.Size.RadiusRoundedUp is invalid. It needs to be between 1 and 3.");
        }

        //if (xSize < 1 || xSize > 3)
        //{
        //    Debug.LogError("xSize is invalid. It needs to be between 1 and 3.");
        //}

        //if (zSize < 1 || zSize > 3)
        //{
        //    Debug.LogError("zSize is invalid. It needs to be between 1 and 3.");
        //}
    }

    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Handles a visual effect of the building rising from the ground when it's placed, before going "boing" and then triggering the public property Operational.
    /// </summary>
    public IEnumerator Build()
    {
        Vector3 startPos = new Vector3(0, 0, buildStartHeight);
        Vector3 endPos = Vector3.zero;
        float buildTimeElapsed = 0;

        Vector3 smallScale = normalScale * smallBoingMultiplier;
        Vector3 largeScale = normalScale * largeBoingMultiplier;
        float boingTimeElapsed = 0;

        AudioManager.Instance.PlaySound(AudioManager.ESound.Building_Materialises, this.transform.position); //needs to be stopped when finished building
        while (buildTimeElapsed < buildTime)
        {
            buildTimeElapsed += Time.deltaTime;
            parentRenderer.transform.localPosition = Vector3.Lerp(startPos, endPos, buildTimeElapsed / buildTime);
            yield return null;
        }

        boinging = true;

        while (boingTimeElapsed < boingInterval)
        {
            boingTimeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(normalScale, smallScale, boingTimeElapsed / boingInterval);
            yield return null;
        }

        boingTimeElapsed -= boingInterval;

        while (boingTimeElapsed < boingInterval)
        {
            boingTimeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(smallScale, largeScale, boingTimeElapsed / boingInterval);
            yield return null;
        }

        boingTimeElapsed -= boingInterval;

        while (boingTimeElapsed < boingInterval)
        {
            boingTimeElapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(largeScale, normalScale, boingTimeElapsed / boingInterval);
            yield return null;
        }

        boinging = false;
        Operational = true; //Using property to trigger activation of any resource collector component attached.
        AudioManager.Instance.PlaySound(AudioManager.ESound.Building_Completes, this.transform.position);

        if (turretShooter != null)
        {
            turretShooter.Place();
        }
        AudioManager.Instance.PlaySound(idleSound, this.transform.position);
        yield return null;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Building Triggered Methods-------------------------------------------------------------------

    public void EnableColliders()
    {
        foreach (CollisionReporter c in collisionReporters)
        {
            c.Collider.enabled = true;
        }
    }

    public void DisableColliders()
    {
        foreach (CollisionReporter c in collisionReporters)
        {
            c.Collider.enabled = false;
        }
    }

    /// <summary>
    /// Checks if the building is colliding while being placed, and updates colour appropriately.
    /// </summary>
    /// <returns>Is this building colliding with something?</returns>
    public bool CollisionUpdate()
    {
        if (active)
        {
            if (!placed)
            {
                //Weird quirk of destroying one object and then instantating another and moving it to the same position: it triggers boths' OnTriggerEnter(),
                //even though one doesn't exist, and then the other doesn't have OnTriggerExit() triggered in the next frame. This checks for the existence of
                //the other collider and corrects the value of colliding if the other collider no longer exists.
                if (colliding )
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

                if (colliding)
                {
                    foreach (RendererMaterialSet r in rendererMaterialSets)
                    {
                        if (r.renderer.material != buildingErrorMaterial)
                        {
                            r.renderer.material = buildingErrorMaterial;
                        }
                    }
                }
                else
                {
                    foreach (RendererMaterialSet r in rendererMaterialSets)
                    {
                        if (r.renderer.material != r.transparent)
                        {
                            r.renderer.material = r.transparent;
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"Building {id} ran CollisionsUpdate(), though it's already placed.");
            }

            return colliding;
        }
        else
        {
            return false;
        }
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
        transform.position = position;

        foreach (RendererMaterialSet r in rendererMaterialSets)
        {
            r.renderer.material = r.opaque;
        }

        foreach (CollisionReporter c in collisionReporters)
        {
            c.Rigidbody.isKinematic = true;
            c.Collider.isTrigger = false;
            c.ReportOnTriggerEnter = false;
            c.ReportOnTriggerExit = false;
        }

        BuildingController.Instance.RegisterBuilding(this);
        StartCoroutine(Build());
    }

    /// <summary>
    /// Resets Building to its initial values when it is returned to the building pool.
    /// </summary>
    public void Reset()
    {
        placed = false; //Needs to occur first so that BuildingFoundations know to ignore this building
        active = false;
        colliding = false;

        StopCoroutine(Build());
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

        foreach (CollisionReporter c in collisionReporters)
        {
            c.Collider.isTrigger = true;
            c.Collider.enabled = false;
            c.Rigidbody.isKinematic = false;
            c.ReportOnTriggerEnter = true;
            c.ReportOnTriggerExit = true;
        }
    }

    //ICollisionListener Triggered Methods---------------------------------------------------------

    ///// <summary>
    ///// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    ///// </summary>
    ///// <param name="collision">The collision data associated with this event.</param>
    //public override void OnCollisionEnter(Collision collision)
    //{
    //    if (active)
    //    {
    //        Debug.Log($"Building {id} OnCollisionEnter()");
    //    }
    //}

    ///// <summary>
    ///// OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
    ///// </summary>
    ///// <param name="collision">The collision data associated with this event.</param>
    //public override void OnCollisionExit(Collision collision)
    //{
    //    if (active)
    //    {
    //        Debug.Log($"Building {id} OnCollisionExit()");
    //    }
    //}

    ///// <summary>
    ///// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    ///// </summary>
    ///// <param name="collision">The collision data associated with this event.</param>
    //public override void OnCollisionStay(Collision collision)
    //{
    //    if (active)
    //    {
    //        Debug.Log($"Building {id} OnCollisionStay()");
    //    }
    //}

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public override void OnTriggerEnter(Collider other)
    {
        if (active && !operational && !other.isTrigger)
        {
            //Debug.Log($"Building {id} OnTriggerEnter(). Other is {other}");
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
        if (active && !operational && !other.isTrigger)
        {            
            //Debug.Log($"Building {id} OnTriggerExit(). Other is {other}");
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

    ///// <summary>
    ///// OnTriggerStay is called almost all the frames for every Collider other that is touching the trigger. The function is on the physics timer so it won't necessarily run every frame.
    ///// </summary>
    ///// <param name="other">The other Collider involved in this collision.</param>
    //public override void OnTriggerStay(Collider other)
    //{
    //    if (active)
    //    {
    //        Debug.Log($"Building {id} OnTriggerStay()");
    //    }
    //}
}
