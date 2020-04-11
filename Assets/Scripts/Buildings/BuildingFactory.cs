using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for buildings.
/// </summary>
public class BuildingFactory : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Cryo Egg Prefab")]
    [SerializeField] private Building cryoEggPrefab;

    [Header("Resource Building Prefabs")]
    [SerializeField] private Building solarPanelsPrefab;
    [SerializeField] private Building windTurbinePrefab;
    [SerializeField] private Building waterDrillPrefab;

    [Header("Terraforming Building Prefabs")]
    [SerializeField] private Building gasDiffuserPrefab;
    [SerializeField] private Building humidifierPrefab;
    [SerializeField] private Building greenhousePrefab;

    [Header("Defence Building Prefabs")]
    [SerializeField] private Building turretPrefab;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// BuildingFactory's singleton public property.
    /// </summary>
    public static BuildingFactory Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one BuildingFactory.");
        }

        Instance = this;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get buildings of a specified type from BuildingFactory.
    /// </summary>
    /// <param name="buildingType">The type of building you want BuildingFactory to get for you.</param>
    /// <returns></returns>
    public Building GetBuilding(EBuilding buildingType)
    {
        Building building;

        switch(buildingType)
        {
            case EBuilding.CryoEgg:
                building = Instantiate(cryoEggPrefab);
                break;
            case EBuilding.SolarPanels:
                building = Instantiate(solarPanelsPrefab);
                break;
            case EBuilding.WindTurbine:
                building = Instantiate(windTurbinePrefab);
                break;
            case EBuilding.WaterDrill:
                building = Instantiate(waterDrillPrefab);
                break;
            case EBuilding.GasDiffuser:
                building = Instantiate(gasDiffuserPrefab);
                break;
            case EBuilding.Humidifier:
                building = Instantiate(humidifierPrefab);
                break;
            case EBuilding.Greenhouse:
                building = Instantiate(greenhousePrefab);
                break;
            case EBuilding.Turret:
                building = Instantiate(turretPrefab);
                break;
            default:
                Debug.LogError("Invalid EBuildingType value passed to BuildingFactory.GetBuilding().");
                return null;
        }

        BuildingController.Instance.RegisterBuilding(building);
        return building;
    }

    /// <summary>
    /// Destroy a building.
    /// </summary>
    /// <param name="building">The building to be destroyed.</param>
    public void DestroyBuilding(Building building)
    {
        BuildingController.Instance.DeRegisterBuilding(building);
        Destroy(building.gameObject);
    }
}
