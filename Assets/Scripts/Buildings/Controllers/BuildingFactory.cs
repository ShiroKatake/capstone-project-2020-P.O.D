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

    [Header("Other Prefabs")]
    [SerializeField] private BuildingFoundation buildingFoundationPrefab;

    [Header("Other Objects")]
    [SerializeField] private Transform objectPool;

    [Header("Initially Pooled Buildings")]
    [SerializeField] private int pooledSolarPanels;
    [SerializeField] private int pooledWindTurbines;
    [SerializeField] private int pooledWaterDrills;
    [SerializeField] private int pooledGasDiffusers;
    [SerializeField] private int pooledHumidifiers;
    [SerializeField] private int pooledGreenhouses;
    [SerializeField] private int pooledTurrets;
    [SerializeField] private int pooledBuildingFoundations;

    //Non-Serialized Fields

    private Dictionary<EBuilding, List<Building>> buildings;
    private List<BuildingFoundation> buildingFoundations;

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
        buildings = new Dictionary<EBuilding, List<Building>>();
        buildings[EBuilding.SolarPanel] = new List<Building>();
        buildings[EBuilding.WindTurbine] = new List<Building>();
        buildings[EBuilding.WaterDrill] = new List<Building>();
        buildings[EBuilding.GasDiffuser] = new List<Building>();
        buildings[EBuilding.Humidifier] = new List<Building>();
        buildings[EBuilding.Greenhouse] = new List<Building>();
        buildings[EBuilding.Turret] = new List<Building>();
        buildingFoundations = new List<BuildingFoundation>();
        IdGenerator idGenerator = IdGenerator.Instance;

        for (int i = 0; i < pooledSolarPanels; i++)
        {
            buildings[EBuilding.SolarPanel].Add(CreateBuilding(EBuilding.SolarPanel, true));
        }

        for (int i = 0; i < pooledWindTurbines; i++)
        {
            buildings[EBuilding.WindTurbine].Add(CreateBuilding(EBuilding.WindTurbine, true));
        }

        for (int i = 0; i < pooledWaterDrills; i++)
        {
            buildings[EBuilding.WaterDrill].Add(CreateBuilding(EBuilding.WaterDrill, true));
        }

        for (int i = 0; i < pooledGasDiffusers; i++)
        {
            buildings[EBuilding.GasDiffuser].Add(CreateBuilding(EBuilding.GasDiffuser, true));
        }

        for (int i = 0; i < pooledHumidifiers; i++)
        {
            buildings[EBuilding.Humidifier].Add(CreateBuilding(EBuilding.Humidifier, true));
        }

        for (int i = 0; i < pooledGreenhouses; i++)
        {
            buildings[EBuilding.Greenhouse].Add(CreateBuilding(EBuilding.Greenhouse, true));
        }

        for (int i = 0; i < pooledTurrets; i++)
        {
            buildings[EBuilding.Turret].Add(CreateBuilding(EBuilding.Turret, true));
        }
        
        for (int i = 0; i < pooledBuildingFoundations; i++)
        {
            buildingFoundations.Add(CreateBuildingFoundation(true));
        }
    }

    //Triggered Methods (Buildings)------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get buildings of a specified type from BuildingFactory.
    /// </summary>
    /// <param name="buildingType">The type of building you want BuildingFactory to get for you.</param>
    /// <returns>A building of the specified type.</returns>
    public Building GetBuilding(EBuilding buildingType)
    {
        Building building;

        if (buildings[buildingType].Count > 0)
        {
            building = buildings[buildingType][0];
            buildings[buildingType].RemoveAt(0);
            building.transform.parent = null;
        }
        else
        {
            building = CreateBuilding(buildingType, false);
        }

        building.Id = IdGenerator.Instance.GetNextId();
        building.Active = true;
        BuildingController.Instance.RegisterBuilding(building);

        if (building.Terraformer != null)
        {
            EnvironmentalController.Instance.RegisterBuilding(building.Terraformer);
        }

        return building;
    }

    /// <summary>
    /// Creates buildings of a specified type.
    /// </summary>
    /// <param name="buildingType">The type of building you want to create.</param>
    /// <returns>A building of the specified type.</returns>
    private Building CreateBuilding(EBuilding buildingType, bool pooling)
    {
        Building building;

        switch (buildingType)
        {
            case EBuilding.SolarPanel:
                building = Instantiate(solarPanelPrefab);
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
                Debug.LogError("Invalid EBuildingType value passed to BuildingFactory.CreateBuilding().");
                return null;
        }

        building.Id = IdGenerator.Instance.GetNextId();

        if (pooling)
        {
            building.transform.position = objectPool.transform.position;
            building.transform.parent = objectPool;
        }

        return building;
    }

    /// <summary>
    /// Destroy a building.
    /// </summary>
    /// <param name="building">The building to be destroyed.</param>
    /// <param name="consumingResources">Is the building consuming resources and does that consumption need to be cancelled now that it's being destroyed?</param>
    public void DestroyBuilding(Building building, bool consumingResources)
    {
        Debug.Log($"Destroy Building. BuildingFactory.Buildings[{building.BuildingType}].Count pre-pooling is {buildings[building.BuildingType].Count}");
        //TODO: create building foundations if the building was killed.
        BuildingController.Instance.DeRegisterBuilding(building);

        if (building.Terraformer != null)
        {
            EnvironmentalController.Instance.RemoveBuilding(building.Id);
        }

        if (consumingResources)
        {
            ResourceController.Instance.PowerSupply += building.PowerConsumption;
            ResourceController.Instance.WaterSupply += building.WaterConsumption;
        }
        
        building.Reset();
        building.Collider.enabled = false;
        building.transform.position = objectPool.position;
        building.transform.parent = objectPool;
        buildings[building.BuildingType].Add(building);
        Debug.Log($"Destroy Building. BuildingFactory.Buildings[{building.BuildingType}].Count post-pooling is {buildings[building.BuildingType].Count}");
    }

    //Triggered Methods (Building Foundations)-------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get a building foundation from BuildingFactory.
    /// </summary>
    /// <returns>A building foundation.</returns>
    public BuildingFoundation GetBuildingFoundation()
    {
        BuildingFoundation buildingFoundation;

        if (buildingFoundations.Count > 0)
        {
            buildingFoundation = buildingFoundations[0];
            buildingFoundations.RemoveAt(0);
            buildingFoundation.transform.parent = null;
        }
        else
        {
            buildingFoundation = CreateBuildingFoundation(false);
        }

        buildingFoundation.Id = IdGenerator.Instance.GetNextId();
        return buildingFoundation;
    }

    /// <summary>
    /// Creates a building foundation.
    /// </summary>
    /// <param name="pooling">The created building foundation.</param>
    /// <returns></returns>
    private BuildingFoundation CreateBuildingFoundation(bool pooling)
    {
        BuildingFoundation buildingFoundation = Instantiate(buildingFoundationPrefab);

        if (pooling)
        {
            buildingFoundation.transform.position = objectPool.transform.position;
            buildingFoundation.transform.parent = objectPool;
        }

        return buildingFoundation;
    }

    /// <summary>
    /// Destroy a building foundation.
    /// </summary>
    /// <param name="buildingFoundation">The building foundation to be destroyed.</param>
    public void DestroyBuildingFoundation(BuildingFoundation buildingFoundation)
    {
        buildingFoundation.transform.position = objectPool.position;
        buildingFoundation.transform.parent = objectPool;
        buildingFoundations.Add(buildingFoundation);
    }
}
