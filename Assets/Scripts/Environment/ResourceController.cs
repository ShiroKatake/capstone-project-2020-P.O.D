using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for resource gathering and usage.
/// </summary>
public class ResourceController : SerializableSingleton<ResourceController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Resource Supplies")]
    [SerializeField] private int ore;
    [SerializeField] private int powerSupply;
    [SerializeField] private int wasteSupply;
    [SerializeField] private int waterSupply;

    [Header("Resource Consumption")]
    [SerializeField] private int powerConsumption;
    [SerializeField] private int wasteConsumption;
    [SerializeField] private int waterConsumption;

    [Header("Testing")]
    [SerializeField] private bool getDeveloperResources;
    [SerializeField] private int developerResources;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    //Resource Consumption

    //Resource Availability
    private bool powerAvailable = false;
    private bool wasteAvailable = false;
    private bool waterAvailable = false;

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
    /// How much power the player has to spare.
    /// </summary>
    public int SurplusPower
    {
        get
        {
            return powerSupply - PowerConsumption;
        }
    }

    /// <summary>
    /// How much waste the player has to spare.
    /// </summary>
    public int SurplusWaste
    {
        get
        {
            return wasteSupply - wasteConsumption;
        }
    }

    /// <summary>
    /// How much water the player has to spare.
    /// </summary>
    public int SurplusWater
    {
        get
        {
            return waterSupply - waterConsumption;
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

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            CheckDeveloperResources();
            CheckResourceSupply();
        }
    }

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Checks if the developer has requested extra resources in the inspector, and grants them one batch of them if so.
    /// </summary>
    private void CheckDeveloperResources()
    {
        if (getDeveloperResources)
        {
            ore += developerResources;
            powerSupply += developerResources;
            wasteSupply += developerResources;
            waterSupply += developerResources;
            getDeveloperResources = false;
        }
    }

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
