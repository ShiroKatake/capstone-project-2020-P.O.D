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

    //[Header("Cryo Egg Prefab")]
    //[SerializeField] private Building cryoEggPrefab;

    [Header("Resource Building Prefabs")]
    [SerializeField] private Building solarPanelPrefab;
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
    /// <param name="position">Where you want BuildingFactory to position the building.</param>
    /// <param name="rotation">The rotation you want BuildingFactory to give the building.</param>
    /// <returns></returns>
    public Building GetBuilding(EBuilding buildingType, Vector3 position, Quaternion rotation)
    {
        Debug.Log("Getting Building");
        Building building;

        switch(buildingType)
        {
            //case EBuilding.CryoEgg:
            //    building = Instantiate(cryoEggPrefab);
            //    break;
            case EBuilding.SolarPanel:
                building = Instantiate<Building>(solarPanelPrefab, position, rotation) ;
                break;
            case EBuilding.WindTurbine:
                building = Instantiate<Building>(windTurbinePrefab, position, rotation);
                break;
            case EBuilding.WaterDrill:
                building = Instantiate<Building>(waterDrillPrefab, position, rotation);
                break;
            case EBuilding.GasDiffuser:
                building = Instantiate<Building>(gasDiffuserPrefab, position, rotation);
                break;
            case EBuilding.Humidifier:
                building = Instantiate<Building>(humidifierPrefab, position, rotation);
                break;
            case EBuilding.Greenhouse:
                building = Instantiate<Building>(greenhousePrefab, position, rotation);
                break;
            case EBuilding.Turret:
                building = Instantiate<Building>(turretPrefab, position, rotation);
                break;
            default:
                Debug.LogError("Invalid EBuildingType value passed to BuildingFactory.GetBuilding().");
                return null;
        }

        building.Id = IdGenerator.Instance.GetNextId();
        BuildingController.Instance.RegisterBuilding(building);
        return building;
    }

    //public GameObject GetBuilding(EBuilding buildingType, Vector3 position, Quaternion rotation, bool eh)
    //{
    //    Debug.Log("Getting Building");
    //    GameObject o;
    //    Building b;

    //    switch (buildingType)
    //    {
    //        //case EBuilding.CryoEgg:
    //        //    building = Instantiate(cryoEggPrefab);
    //        //    break;
    //        //case EBuilding.SolarPanel:
    //        //    building = Instantiate(solarPanelPrefab, position, rotation);
    //        //    break;
    //        //case EBuilding.WindTurbine:
    //        //    building = Instantiate(windTurbinePrefab, position, rotation);
    //        //    break;
    //        //case EBuilding.WaterDrill:
    //        //    building = Instantiate(waterDrillPrefab, position, rotation);
    //        //    break;
    //        //case EBuilding.GasDiffuser:
    //        //    building = Instantiate(gasDiffuserPrefab, position, rotation);
    //        //    break;
    //        //case EBuilding.Humidifier:
    //        //    building = Instantiate(humidifierPrefab, position, rotation);
    //        //    break;
    //        //case EBuilding.Greenhouse:
    //        //    building = Instantiate(greenhousePrefab, position, rotation);
    //        //    break;
    //        case EBuilding.Turret:
    //            o = Instantiate(turretPrefab, position, rotation);
    //            break;
    //        default:
    //            Debug.LogError("Invalid EBuildingType value passed to BuildingFactory.GetBuilding().");
    //            return null;
    //    }

    //    b = o.GetComponent<Building>();

    //    b.Id = IdGenerator.Instance.GetNextId();
    //    BuildingController.Instance.RegisterBuilding(b);
    //    return o;
    //}

    /// <summary>
    /// Destroy a building.
    /// </summary>
    /// <param name="building">The building to be destroyed.</param>
    public void DestroyBuilding(Building building)
    {
        BuildingController.Instance.DeRegisterBuilding(building);
        building.Health.Die();
    }
}
