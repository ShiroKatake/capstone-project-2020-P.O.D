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

	[Header("Resource Supply")]
	[SerializeField] private int ore;
	[SerializeField] private int powerSupply;
	[SerializeField] private int waterSupply;
	[SerializeField] private int wasteSupply;

	[Header("Resource Consumption")]
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
	private Animator animator;

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
	public int PowerSupply { get => powerSupply; }
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
	public int WasteSupply { get => wasteSupply; }
	/// <summary>
	/// How much water this building requires per second to function.
	/// </summary>
	public int WaterConsumption { get => waterConsumption; }
	public int WaterSupply{ get => waterSupply; }
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
            Debug.Log($"Pre-Setting {gameObject.name}: operational: {operational}, value: {value}, active: {active}");
            if (operational != value)
            {
                operational = (value && active);
                if (resourceCollector != null)
                {
                    if (operational)
                    {
                        resourceCollector.Activate();
						Debug.Log($"{gameObject.name}: Resource Activated");
                    }
                    else
                    {
                        resourceCollector.Deactivate();
						Debug.Log($"{gameObject.name}: Resource Deactivated");
					}
                }
            }

            Debug.Log($"Post-Setting {gameObject.name}: operational: {operational}, value: {value}, active: {active}");
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
		animator = GetComponent<Animator>();
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

		//The initial animation duration is 10s in the animation panel.
		//Therefore 10 divided by whatever duration it needs to be will give us the speed multiplier (ie. 20s is twice as long as 10s, so the speed multiplier is 10/20 = 0.5, so if 10s is 1, 20s is only 0.5)
		animator.SetFloat("DurationMultiplier", 10f / buildTime);

		//if (xSize < 1 || xSize > 3)
		//{
		//    Debug.LogError("xSize is invalid. It needs to be between 1 and 3.");
		//}

		//if (zSize < 1 || zSize > 3)
		//{
		//    Debug.LogError("zSize is invalid. It needs to be between 1 and 3.");
		//}
	}

	//Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	private void Update()
	{
		animator.SetFloat("Health", health.Value);
		animator.SetBool("Operational", operational);
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	//Building Triggered Methods-------------------------------------------------------------------
	
	/// <summary>
	/// Handles a visual effect of the building rising from the ground when it's placed, before going "boing" and then triggering the public property Operational.
	/// </summary>
	public void Build()
	{
		animator.enabled = true;
	}

	public void Shutdown()
	{
		Operational = false;
		ResourceController.Instance.PowerSupply -= powerSupply;
		ResourceController.Instance.WaterSupply -= waterSupply;
		ResourceController.Instance.WasteSupply -= wasteSupply;
	}

	public void Restore()
	{
		Operational = true;
		ResourceController.Instance.PowerSupply += powerSupply;
		ResourceController.Instance.WaterSupply += waterSupply;
		ResourceController.Instance.WasteSupply += wasteSupply;
	}

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
        Build();
    }
    /// <summary>
    /// Resets Building to its initial values when it is returned to the building pool.
    /// </summary>
    public void Reset()
    {
        placed = false; //Needs to occur first so that BuildingFoundations know to ignore this building
        active = false;
        colliding = false;

        //StopCoroutine(Build());
        health.Reset();
        operational = false;
        
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
