using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A building placed by the player.
/// </summary>
public class Building : MonoBehaviour, ICollisionListener
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

    [Header("Size")]
    [SerializeField] [Range(1, 3)] private int xSize;
    [SerializeField] [Range(1, 3)] private int zSize;

    [Header("Building")]
    [SerializeField] private float buildTime;
    [SerializeField] private float buildStartHeight;
    [SerializeField] private float boingInterval;
    [SerializeField] private float smallBoingMultiplier;
    [SerializeField] private float largeBoingMultiplier;

    [Header("Offsets of Foundations from Position")]
    [SerializeField] private List<Vector3> buildingFoundationOffsets;

    [Header("Materials")]
    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material buildingErrorMaterial;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    //Components
    private Collider collider;
    private Health health;
    private MeshRenderer renderer;
    private ResourceCollector resourceCollector;
    private Rigidbody rigidbody;
    private Terraformer terraformer;

    //Positioning
    //private Dictionary<string, Vector3> offsets;
    private bool colliding = false;
    [SerializeField] private List<Collider> otherColliders;
    private List<CollisionReporter> collisionReporters;
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

    /// <summary>
    /// The building's collider component.
    /// </summary>
    public Collider Collider { get => collider; }

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

    /// <summary>
    /// How many squares this building occupies along the x-axis.
    /// </summary>
    public int XSize { get => xSize; }

    /// <summary>
    /// How many squares this building occupies along the z-axis.
    /// </summary>
    public int ZSize { get => zSize; }

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
                operational = value && active;

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
        collider = GetComponentInChildren<Collider>();
        health = GetComponent<Health>();
        renderer = GetComponentInChildren<MeshRenderer>();
        rigidbody = GetComponentInChildren<Rigidbody>();
        resourceCollector = GetComponent<ResourceCollector>();
        terraformer = GetComponent<Terraformer>();
        collisionReporters = new List<CollisionReporter>(GetComponentsInChildren<CollisionReporter>());
        otherColliders = new List<Collider>();

        normalScale = transform.localScale;
        normalBuildTime = buildTime;

        if (xSize < 1 || xSize > 3)
        {
            Debug.LogError("xSize is invalid. It needs to be between 1 and 3.");
        }

        if (zSize < 1 || zSize > 3)
        {
            Debug.LogError("zSize is invalid. It needs to be between 1 and 3.");
        }

        //CalculateOffsets();
    }

    /// <summary>
    /// Calculates the offset this building will need when being placed in any given direction relative to the player.
    /// </summary>
    //private void CalculateOffsets()
    //{
    //    offsets = new Dictionary<string, Vector3>();
    //    offsets["N"]  = new Vector3( xSize == 2 ? 0.5f  : 0                       , 0, zSize == 1 ? 1f    : zSize == 2 ? 1.5f  : 2f  );
    //    offsets["NE"] = new Vector3( xSize == 1 ? 1f    : xSize == 2 ? 1.5f  : 2f , 0, zSize == 1 ? 1f    : zSize == 2 ? 1.5f  : 2f  );
    //    offsets["E"]  = new Vector3( xSize == 1 ? 1f    : xSize == 2 ? 1.5f  : 2f , 0, zSize == 2 ? -0.5f : 0                        );
    //    offsets["SE"] = new Vector3( xSize == 1 ? 1f    : xSize == 2 ? 1.5f  : 2f , 0, zSize == 1 ? -1f   : zSize == 2 ? -1.5f : -2f );
    //    offsets["S"]  = new Vector3( xSize == 2 ? -0.5f : 0                       , 0, zSize == 1 ? -1f   : zSize == 2 ? -1.5f : -2f );
    //    offsets["SW"] = new Vector3( xSize == 1 ? -1f   : xSize == 2 ? -1.5f : -2f, 0, zSize == 1 ? -1f   : zSize == 2 ? -1.5f : -2f );
    //    offsets["W"]  = new Vector3( xSize == 1 ? -1f   : xSize == 2 ? -1.5f : -2f, 0, zSize == 2 ? 0.5f  : 0                        );
    //    offsets["NW"] = new Vector3( xSize == 1 ? -1f   : xSize == 2 ? -1.5f : -2f, 0, zSize == 1 ? 1f    : zSize == 2 ? 1.5f  : 2f  );
    //}

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

        while (buildTimeElapsed < buildTime)
        {
            buildTimeElapsed += Time.deltaTime;
            renderer.transform.localPosition = Vector3.Lerp(startPos, endPos, buildTimeElapsed / buildTime);
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
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Building Triggered Methods-------------------------------------------------------------------

    /// <summary>
    ///  Gets the offset appropriate at any angle for this building, given its xSize and zSize.
    ///  <param name="angle">The player's rotation on the y-axis in degrees clockwise from due North.</param>
    /// </summary>
    //public Vector3 GetOffset(float angle)
    //{
    //    if (angle >= 337.5 || angle < 22.5)
    //    {
    //        return offsets["N"];
    //    }
    //    else if (angle < 67.5)
    //    {
    //        return offsets["NE"];
    //    }
    //    else if (angle < 112.5) 
    //    {
    //        return offsets["E"];
    //    }
    //    else if (angle < 157.5)
    //    {
    //        return offsets["SE"];
    //    }
    //    else if (angle < 202.5)
    //    {
    //        return offsets["S"];
    //    }
    //    else if (angle < 247.5)
    //    {
    //        return offsets["SW"];
    //    }
    //    else if (angle < 292.5)
    //    {
    //        return offsets["W"];
    //    }
    //    else
    //    {
    //        return offsets["NW"];
    //    }
    //}

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
                    if (renderer.material != buildingErrorMaterial)
                    {
                        renderer.material = buildingErrorMaterial;
                    }
                }
                else
                {
                    if (renderer.material != transparentMaterial)
                    {
                        renderer.material = transparentMaterial;
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
        ResourceController.Instance.Ore -= oreCost;
        ResourceController.Instance.PowerConsumption += powerConsumption;
        ResourceController.Instance.WaterConsumption += waterConsumption;
        ResourceController.Instance.WasteConsumption += wasteConsumption;
        transform.position = position;
        renderer.material = opaqueMaterial;
        rigidbody.isKinematic = true;
        collider.isTrigger = false;
        placed = true;

        foreach (CollisionReporter c in collisionReporters)
        {
            c.ReportOnTriggerEnter = false;
            c.ReportOnTriggerExit = false;
        }

        StartCoroutine(Build());
    }

    /// <summary>
    /// Resets Building to its initial values when it is returned to the building pool.
    /// </summary>
    public void Reset()
    {
        StopCoroutine(Build());
        health.Reset();
        Operational = false;

        active = false;
        colliding = false;
        placed = false;
        
        otherColliders.Clear();
        renderer.transform.localPosition = Vector3.zero;
        transform.localScale = normalScale;
        buildTime = normalBuildTime;
        renderer.material = transparentMaterial;
        collider.isTrigger = true;
        collider.enabled = false;
        rigidbody.isKinematic = false;

        foreach (CollisionReporter c in collisionReporters)
        {
            c.ReportOnTriggerEnter = true;
            c.ReportOnTriggerExit = true;
        }
    }

    //ICollisionListener Triggered Methods---------------------------------------------------------

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public void OnCollisionEnter(Collision collision)
    {
        if (active)
        {
            Debug.Log($"Building {id} OnCollisionEnter()");
        }
    }

    /// <summary>
    /// OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public void OnCollisionExit(Collision collision)
    {
        if (active)
        {
            Debug.Log($"Building {id} OnCollisionExit()");
        }
    }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
    /// </summary>
    /// <param name="collision">The collision data associated with this event.</param>
    public void OnCollisionStay(Collision collision)
    {
        if (active)
        {
            Debug.Log($"Building {id} OnCollisionStay()");
        }
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public void OnTriggerEnter(Collider other)
    {
        if (active && !other.isTrigger)
        {
            Debug.Log($"Building {id} OnTriggerEnter(). Other is {other}");
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
    public void OnTriggerExit(Collider other)
    {
        if (active && !other.isTrigger)
        {            
            Debug.Log($"Building {id} OnTriggerExit(). Other is {other}");
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

    /// <summary>
    /// OnTriggerStay is called almost all the frames for every Collider other that is touching the trigger. The function is on the physics timer so it won't necessarily run every frame.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    public void OnTriggerStay(Collider other)
    {
        if (active)
        {
            Debug.Log($"Building {id} OnTriggerStay()");
        }
    }
}
