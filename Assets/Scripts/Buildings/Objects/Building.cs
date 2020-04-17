using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A building placed by the player.
/// </summary>
public class Building : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Building Category")]
    [SerializeField] private EBuildingCategory buildingCategory;
    [SerializeField] private EBuilding buildingType;

    [Header("Resource Requirements")]
    [SerializeField] private int oreCost;
    [SerializeField] private int powerConsumption;
    [SerializeField] private int waterConsumption;

    [Header("Other Stats")]
    [SerializeField] private int id;
    [SerializeField] [Range(1, 3)] private int xSize;
    [SerializeField] [Range(1, 3)] private int zSize;
    [SerializeField] private float buildTime;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Health health;
    private Terraformer terraformer;
    private ResourceCollector resourceCollector;
    private bool operational = false;

    private Dictionary<string, Vector3> offsets;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The broad category of building (cryo egg, resources, terraforming, defence) that this building falls under.
    /// </summary>
    public EBuildingCategory BuildingCategory { get => buildingCategory; }

    /// <summary>
    /// The type of building this building is.
    /// </summary>
    public EBuilding BuildingType { get => buildingType; }     
    
    /// <summary>
    /// How long this building takes to builds itself when the player places it in the scene.
    /// </summary>
    public float BuildTime { get => buildTime; }

    /// <summary>
    /// The Building's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// The Building's unique ID number. Should only be set in BuildingFactory.
    /// </summary>
    public int Id { get => id; set => id = value; }

    /// <summary>
    /// How much ore it costs to build this building.
    /// </summary>
    public int OreCost { get => oreCost; }

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
                operational = value;

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
        health = GetComponent<Health>();
        resourceCollector = GetComponent<ResourceCollector>();
        terraformer = GetComponent<Terraformer>();

        if (xSize < 1 || xSize > 3)
        {
            Debug.LogError("xSize is invalid. It needs to be between 1 and 3.");
        }

        if (zSize < 1 || zSize > 3)
        {
            Debug.LogError("zSize is invalid. It needs to be between 1 and 3.");
        }

        CalculateOffsets();
    }

    /// <summary>
    /// Calculates the offset this building will need when being placed in any given direction relative to the player.
    /// </summary>
    private void CalculateOffsets()
    {
        offsets = new Dictionary<string, Vector3>();
        offsets["N"]  = new Vector3( xSize == 2 ? 0.5f  : 0                       , 0, zSize == 1 ? 1f    : zSize == 2 ? 1.5f  : 2f  );
        offsets["NE"] = new Vector3( xSize == 1 ? 1f    : xSize == 2 ? 1.5f  : 2f , 0, zSize == 1 ? 1f    : zSize == 2 ? 1.5f  : 2f  );
        offsets["E"]  = new Vector3( xSize == 1 ? 1f    : xSize == 2 ? 1.5f  : 2f , 0, zSize == 2 ? -0.5f : 0                        );
        offsets["SE"] = new Vector3( xSize == 1 ? 1f    : xSize == 2 ? 1.5f  : 2f , 0, zSize == 1 ? -1f   : zSize == 2 ? -1.5f : -2f );
        offsets["S"]  = new Vector3( xSize == 2 ? -0.5f : 0                       , 0, zSize == 1 ? -1f   : zSize == 2 ? -1.5f : -2f );
        offsets["SW"] = new Vector3( xSize == 1 ? -1f   : xSize == 2 ? -1.5f : -2f, 0, zSize == 1 ? -1f   : zSize == 2 ? -1.5f : -2f );
        offsets["W"]  = new Vector3( xSize == 1 ? -1f   : xSize == 2 ? -1.5f : -2f, 0, zSize == 2 ? 0.5f  : 0                        );
        offsets["NW"] = new Vector3( xSize == 1 ? -1f   : xSize == 2 ? -1.5f : -2f, 0, zSize == 1 ? 1f    : zSize == 2 ? 1.5f  : 2f  );
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    //private void Start()
    //{

    //}

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    //private void Update()
    //{

    //}

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    //private void FixedUpdate()
    //{

    //}

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  



    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------



    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------



    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///  Gets the offset appropriate at any angle for this building, given its xSize and zSize.
    ///  <param name="angle">The player's rotation on the y-axis in degrees clockwise from due North.</param>
    /// </summary>
    public Vector3 GetOffset(float angle)
    {
        if (angle >= 337.5 || angle < 22.5)
        {
            return offsets["N"];
        }
        else if (angle < 67.5)
        {
            return offsets["NE"];
        }
        else if (angle < 112.5) 
        {
            return offsets["E"];
        }
        else if (angle < 157.5)
        {
            return offsets["SE"];
        }
        else if (angle < 202.5)
        {
            return offsets["S"];
        }
        else if (angle < 247.5)
        {
            return offsets["SW"];
        }
        else if (angle < 292.5)
        {
            return offsets["W"];
        }
        else
        {
            return offsets["NW"];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public void Place(Vector3 position)
    {
        ResourceController.Instance.Ore -= oreCost;
        ResourceController.Instance.PowerSupply -= powerConsumption;
        ResourceController.Instance.WaterSupply -= waterConsumption;
        transform.position = position;
        Operational = true; //Using property to trigger activation of any resource collector component attached.
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
