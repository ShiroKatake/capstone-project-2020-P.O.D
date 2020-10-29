using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for buildings.
/// </summary>
public class BuildingManager : SerializableSingleton<BuildingManager>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private Building tower;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<Building> buildings = new List<Building>();
    private List<Building> destroyedBuildings = new List<Building>();

    private float timeLastDefenceWasBuilt;
    private float timeLastNonDefenceWasBuilt;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The number of buildings that are registered with BuildingController.
    /// </summary>
    public int BuildingCount { get => buildings.Count; }

    /// <summary>
    /// The time in seconds that the last defensive building was built.
    /// </summary>
    public float TimeLastDefenceWasBuilt { get => timeLastDefenceWasBuilt; }

    /// <summary>
    /// The time in seconds that the last non-defensive building was built.
    /// </summary>
    public float TimeLastNonDefenceWasBuilt { get => timeLastNonDefenceWasBuilt; }

    /// <summary>
    /// The tower.
    /// </summary>
    public Building Tower { get => tower; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        if (!MapManager.Instance.Initialised)
        {
            MapManager.Instance.Initialise();
        }

        RegisterBuilding(tower);
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            foreach (Building b in buildings)
            {
                CheckBuildingHealth(b);
            }

            CleanupBuildings();
        }
    }

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Checks the building's health, and passes it to BuildingFactory to be destroyed if it falls below 0.
    /// </summary>
    /// <param name="building">The building whose health is being checked.</param>
    private void CheckBuildingHealth(Building building)
    {
        if (building.Health.IsDead() && building.BuildingType != EBuilding.Tower)
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
            BuildingFactory.Instance.Destroy(b, true, true);
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Checks if a building of the specified type has been placed and built.
    /// </summary>
    /// <param name="buildingType">The type of building you want to check for.</param>
    /// <returns>Whether a building of the specified type has been placed and built.</returns>
    public int BuiltBuildingsCount(EBuilding buildingType)
    {
        int result = 0;

        foreach (Building b in buildings)
        {
            if (b.BuildingType == buildingType && b.Built)
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// Checks the number of buildings of a specified type that have been placed.
    /// </summary>
    /// <param name="buildingType">The type of building you want to check for.</param>
    /// <returns>Whether a building of the specified type has been placed.</returns>
    public int PlacedBuildingsCount(EBuilding buildingType)
    {
        int result = 0;

        foreach (Building b in buildings)
        {
            if (b.BuildingType == buildingType && b.Placed)
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// Checks the number of buildings of a specified type that are built and operational.
    /// </summary>
    /// <param name="buildingType">The type of building you want to check for.</param>
    /// <returns>Whether a building of the specified type has been placed.</returns>
    public int BuiltAndOperationalBuildingsCount(EBuilding buildingType)
    {
        int result = 0;

        foreach (Building b in buildings)
        {
            if (b.BuildingType == buildingType && b.Built && b.Operational)
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// Checks the number of buildings of a specified type that are built but not operational.
    /// </summary>
    /// <param name="buildingType">The type of building you want to check for.</param>
    /// <returns>Whether a building of the specified type has been placed.</returns>
    public int BuiltAndNonOperationalBuildingsCount(EBuilding buildingType)
    {
        int result = 0;

        foreach (Building b in buildings)
        {
            if (b.BuildingType == buildingType && b.Built && !b.Operational)
            {
                result++;
            }
        }

        return result;
    }

    /// <summary>
    /// Registers a Building with BuildingController. BuildingController adds it to its list of Buildings in the scene.
    /// </summary>
    /// <param name="building">The building being registered with BuildingController.</param>
    public void RegisterBuilding(Building building)
    {
        if (!buildings.Contains(building))
        {
            buildings.Add(building);
            MapManager.Instance.RegisterBuilding(building);

            if (building.BuildingType == EBuilding.ShotgunTurret || building.BuildingType == EBuilding.MachineGunTurret)
            {
                timeLastDefenceWasBuilt = Time.time;
            }
            else if (building.BuildingType != EBuilding.Tower)
            {
                timeLastNonDefenceWasBuilt = Time.time;
            }
        }
    }

    /// <summary>
    /// De-registers a Building from BuildingController. BuildingController removes it from its list of Buildings in a scene.
    /// </summary>
    /// <param name="building">The building being de-registered from BuildingController.</param>
    public void DeRegisterBuilding(Building building)
    {
        if (buildings.Contains(building))
        {
            buildings.Remove(building);

            if (building.Placed)
            {
                MapManager.Instance.DeRegisterBuilding(building);
            }
        }
    }

    /// <summary>
    /// Shutdown buildings depending on which resources are overtaxed.
    /// </summary>
    /// <param name="power">Is there sufficient power to supply all buildings?</param>
    /// <param name="water">Is there sufficient water to supply all buildings?</param>
    /// <param name="water">Is there sufficient plants to supply all buildings?</param>
    /// <param name="gas">Is there sufficient gas to supply all buildings?</param>
    public void ShutdownBuildings(bool power, bool water, bool plants, bool gas)
    {
		Debug.Log("Disabling Buildings");

        foreach (Building b in buildings)
        {			
			if (b.Operational && ((!power && b.PowerConsumption > 0) || (!water && b.WaterConsumption > 0) || (!plants && b.PlantsConsumption > 0) || (!gas && b.GasConsumption > 0)))
            {
                Debug.Log($"Disabling {b.name}");

                b.Operational = false;
				RatioManager.Instance.UpdateCurrentRatio();
			}
        }
    }

    /// <summary>
    /// Restore buildings depending on which resources are available.
    /// </summary>
    /// <param name="power">Is there sufficient power to supply all buildings?</param>
    /// <param name="water">Is there sufficient water to supply all buildings?</param>
    /// <param name="water">Is there sufficient waste to supply all buildings?</param>
    /// <param name="gas">Is there sufficient gas to supply all buildings?</param>
    public void RestoreBuildings(bool power, bool water, bool waste, bool gas)
    {
        Debug.Log("Enabling Buildings");

        foreach (Building b in buildings)
        {
            if (!b.Operational && (power || b.PowerConsumption == 0) && (water || b.WaterConsumption == 0) && (waste || b.PlantsConsumption == 0) || (gas || b.GasConsumption == 0))
            {
                Debug.Log($"Enabling {b.name}");

                b.Operational = true;
				RatioManager.Instance.UpdateCurrentRatio();
			}
        }
    }
}
