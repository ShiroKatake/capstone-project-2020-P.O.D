using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for resource gathering and usage.
/// </summary>
public class ResourceController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private int ore;
    [SerializeField] private int powerConsumption;
    [SerializeField] private int powerSupply;
    [SerializeField] private int waterConsumption;
    [SerializeField] private int waterSupply;
    [SerializeField] private int wasteConsumption;
    [SerializeField] private int wasteSupply;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    //Resource Consumption

    //Resource Availability
    private bool powerAvailable = false;
    private bool wasteAvailable = false;
    private bool waterAvailable = false;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// ResourceController's singleton public property.
    /// </summary>
    public static ResourceController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// How much ore the player has collected
    /// </summary>
    public int Ore { get => ore; set => ore = value; }

    //Complex Public Properties--------------------------------------------------------------------                                                    

    /// <summary>
    /// How much power the player is consuming per second.
    /// </summary>
    public int PowerConsumption
    {
        get
        {
            return powerConsumption;
        }

        set
        {
            powerConsumption = value;
            CheckResourceSupply();
        }
    }

    /// <summary>
    /// How much power the player is generating per second.
    /// </summary>
    public int PowerSupply
    {
        get
        {
            return powerSupply;
        }

        set
        {
            powerSupply = value;
            CheckResourceSupply();
        }
    }

    /// <summary>
    /// How much waste the player is consuming per second.
    /// </summary>
    public int WasteConsumption
    {
        get
        {
            return wasteConsumption;
        }

        set
        {
            wasteConsumption = value;
            CheckResourceSupply();
        }
    }

    /// <summary>
    /// How much waste the player is collecting per second.
    /// </summary>
    public int WasteSupply
    {
        get
        {
            return wasteSupply;
        }

        set
        {
            wasteSupply = value;
            CheckResourceSupply();
        }
    }

    /// <summary>
    /// How much water the player is consuming per second.
    /// </summary>
    public int WaterConsumption
    {
        get
        {
            return waterConsumption;
        }

        set
        {
            waterConsumption = value;
            CheckResourceSupply();
        }
    }

    /// <summary>
    /// How much water the player is collecting per second.
    /// </summary>
    public int WaterSupply
    {
        get
        {
            return waterSupply;
        }

        set
        {
            waterSupply = value;
            CheckResourceSupply();
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one [CLASSNAME].");
        }

        Instance = this;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        //For testing by changing resource values in the inspector.
        CheckResourceSupply();
	}

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Check if there is sufficient or insufficient supplies of either resource, and shutdown and restore buildings appropriately.
    /// </summary>
    private void CheckResourceSupply()
    {
        //Get initial values
        bool initialPowerStatus = powerAvailable;
        bool initialWaterStatus = waterAvailable;
        bool initialWasteStatus = wasteAvailable;

        //Check if power needs to be updated
        if ((powerAvailable && powerSupply < powerConsumption) || (!powerAvailable && powerSupply >= powerConsumption && powerSupply != 0))
        {
            powerAvailable = !powerAvailable;
        }

        //Check if water needs to be updated
        if ((waterAvailable && waterSupply < waterConsumption) || (!waterAvailable && waterSupply >= waterConsumption && waterSupply != 0))
        {
            waterAvailable = !waterAvailable;
        }

        //Check if waste needs to be updated
        if ((wasteAvailable && wasteSupply < wasteConsumption) || (!wasteAvailable && wasteSupply >= wasteConsumption && wasteSupply != 0))
        {
            wasteAvailable = !wasteAvailable;
        }

        //Check if there's been a change
        if (initialPowerStatus != powerAvailable || initialWaterStatus != waterAvailable || initialWasteStatus != wasteAvailable)
        {
            //Check if buildings need to be shutdown
            if (!powerAvailable || !wasteAvailable || !waterAvailable)
            {
                Debug.Log("Shutdown Buildings.");
                BuildingController.Instance.ShutdownBuildings(powerAvailable, waterAvailable, wasteAvailable);
            }

            //Check if buildings can be restored
            if (powerAvailable || waterAvailable || wasteAvailable)
            {
                Debug.Log("Restore Buildings");
                BuildingController.Instance.RestoreBuildings(powerAvailable, waterAvailable, wasteAvailable);
            }
        }
    }
}
