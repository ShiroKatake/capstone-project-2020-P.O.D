using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for buildings.
/// </summary>
public class BuildingController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private Building cryoEgg;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<Building> buildings = new List<Building>();
    private List<Building> destroyedBuildings = new List<Building>();

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// BuildingController's singleton public property.
    /// </summary>
    public static BuildingController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The cryo egg.
    /// </summary>
    public Building CryoEgg { get => cryoEgg; }

    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one BuildingController.");
        }

        Instance = this;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        buildings.Add(cryoEgg);
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        foreach (Building b in buildings)
        {
            ExecuteBuildingBehaviour(b);
            CheckBuildingHealth(b);
        }

        CleanupBuildings();
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    //private void FixedUpdate()
    //{
    //If having building controller execute building behaviour, for each building in list "buildings", check its type and execute behaviour for it using the value of its public properties
    //}

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    private void ExecuteBuildingBehaviour(Building building)
    {
        switch (building.BuildingType)
        {
            //case EBuilding.CryoEgg:
            //    CryoEggBehaviour.Instance.Execute(building);
            //    break;
            //case EBuilding.FusionReactor:
            //    SolarPanelBehaviour.Instance.Execute(building);
            //    break;
            //case EBuilding.IceDrill:
            //    IceDrillBehaviour.Instance.Execute(building);
            //    break;
            //case EBuilding.Boiler:
            //    GasDiffuserBehaviour.Instance.Execute(building);
            //    break;
            //case EBuilding.Greenhouse:
            //    GreenhouseBehaviour.Instance.Execute(building);
            //    break;
            //case EBuilding.Incinerator:
            //    HumidifierBehaviour.Instance.Execute(building);
            //    break;
            case EBuilding.ShortRangeTurret:
                ShortRangeTurretBehaviour.Instance.Execute(building);
                break;
            case EBuilding.LongRangeTurret:
                LongRangeTurretBehaviour.Instance.Execute(building);
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// Checks the building's health, and passes it to BuildingFactory to be destroyed if it falls below 0.
    /// <param name="building">The building whose health is being checked.</param>
    /// </summary>
    private void CheckBuildingHealth(Building building)
    {
        if (building.Health.IsDead() && building.BuildingType != EBuilding.CryoEgg)
        {
            destroyedBuildings.Add(building);
        }
    }

    /// <summary>
    /// Processes all Buildings that have been destroyed.
    /// </summary>
    private void CleanupBuildings()
    {
        while (destroyedBuildings.Count > 0)
        {
            Building b = destroyedBuildings[0];
            destroyedBuildings.RemoveAt(0);
            BuildingFactory.Instance.DestroyBuilding(b, true, true);
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------



    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------



    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Registers a Building with BuildingController. BuildingController adds it to its list of Buildings in the scene.
    /// <param name="building">The building being registered with BuildingController.</param>
    /// </summary>
    public void RegisterBuilding(Building building)
    {
        if (!buildings.Contains(building))
        {
            buildings.Add(building);
        }
    }

    /// <summary>
    /// De-registers a Building from BuildingController. BuildingController removes it from its list of Buildings in a scene.
    /// <param name="building">The building being de-registered from BuildingController.</param>
    /// </summary>
    public void DeRegisterBuilding(Building building)
    {
        if (buildings.Contains(building))
        {
            buildings.Remove(building);
        }
    }

    /// <summary>
    /// Shutdown buildings depending on which resources are overtaxed.
    /// </summary>
    /// <param name="power">Is there sufficient power to supply all buildings?</param>
    /// <param name="water">Is there sufficient water to supply all buildings?</param>
    /// <param name="water">Is there sufficient waste to supply all buildings?</param>
    public void ShutdownBuildings(bool power, bool water, bool waste)
    {
        foreach (Building b in buildings)
        {
            if (b.Operational && ((!power && b.PowerConsumption > 0) || (!water && b.WaterConsumption > 0) || (!waste && b.WasteConsumption > 0)))
            {
                b.Operational = false;
            }
        }
    }

    /// <summary>
    /// Restore buildings depending on which resources are available.
    /// </summary>
    /// <param name="power">Is there sufficient power to supply all buildings?</param>
    /// <param name="water">Is there sufficient water to supply all buildings?</param>
    /// <param name="water">Is there sufficient waste to supply all buildings?</param>
    public void RestoreBuildings(bool power, bool water, bool waste)
    {
        foreach (Building b in buildings)
        {
            if (!b.Operational && (power || b.PowerConsumption == 0) && (water || b.WaterConsumption == 0) && (waste || b.WasteConsumption == 0))
            {
                b.Operational = true;
            }
        }
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
