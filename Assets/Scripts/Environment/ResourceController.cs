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
    [SerializeField] private int powerSupply;
    [SerializeField] private int waterSupply;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool powerAvailable = true;
    private bool waterAvailable = true;

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
    private void Update()
    {
        //For testing by changing resource values in the inspector.
        CheckResourceSupply();
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    //private void FixedUpdate()
    //{

    //}

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Check if there is sufficient or insufficient supplies of either resource, and shutdown and restore buildings appropriately.
    /// </summary>
    private void CheckResourceSupply()
    {
        //Get initial values
        bool initialPowerStatus = powerAvailable;
        bool initialWaterStatus = waterAvailable;

        //Check if power needs to be updated
        if ((powerAvailable && powerSupply < 0) || (!powerAvailable && powerSupply >= 0))
        {
            powerAvailable = !powerAvailable;
        }

        //Check if water needs to be updated
        if ((waterAvailable && waterSupply < 0) || (!waterAvailable && waterSupply >= 0))
        {
            waterAvailable = !waterAvailable;
        }

        //Check if there's been a change
        if (initialPowerStatus != powerAvailable || initialWaterStatus != waterAvailable)
        {
            //Check if buildings need to be shutdown
            if (!powerAvailable || !waterAvailable)
            {
                Debug.Log("Shutdown Buildings.");
                BuildingController.Instance.ShutdownBuildings(powerAvailable, waterAvailable);
            }

            //Check if buildings can be restored
            if (powerAvailable || waterAvailable)
            {
                Debug.Log("Restore Buildings");
                BuildingController.Instance.RestoreBuildings(powerAvailable, waterAvailable);
            }
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------



    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------



    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------



    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
