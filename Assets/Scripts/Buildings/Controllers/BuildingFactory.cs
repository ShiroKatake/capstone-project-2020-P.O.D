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

    [Header("Building Prefabs")]
    [SerializeField] private Building fusionReactorPrefab;
    [SerializeField] private Building iceDrillPrefab;
    [SerializeField] private Building boilerPrefab;
    [SerializeField] private Building greenhousePrefab;
    [SerializeField] private Building incineratorPrefab;
    [SerializeField] private Building shortRangeTurretPrefab;
    [SerializeField] private Building longRangeTurretPrefab;

    [Header("Other Prefabs")]
    [SerializeField] private BuildingFoundation buildingFoundationPrefab;
    //[SerializeField] private GameObject pipePrefab;
    //[SerializeField] private GameObject pipeBoxPrefab;

    [Header("Initially Pooled Buildings")]
    [SerializeField] private int pooledFusionReactors;
    [SerializeField] private int pooledIceDrills;
    [SerializeField] private int pooledBoilers;
    [SerializeField] private int pooledGreenhouses;
    [SerializeField] private int pooledIncinerators;
    [SerializeField] private int pooledShortRangeTurrets;
    [SerializeField] private int pooledLongRangeTurrets;
    [SerializeField] private int pooledBuildingFoundations;
    //[SerializeField] private int pooledPipes;
    //[SerializeField] private int pooledPipeBoxes;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Dictionary<EBuilding, List<Building>> buildings;
    private List<BuildingFoundation> buildingFoundations;
    //private List<GameObject> pipes;
    //private List<GameObject> pipeBoxes;
    private Transform objectPool;

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
        buildings[EBuilding.FusionReactor] = new List<Building>();
        buildings[EBuilding.IceDrill] = new List<Building>();
        buildings[EBuilding.Boiler] = new List<Building>();
        buildings[EBuilding.Greenhouse] = new List<Building>();
        buildings[EBuilding.Incinerator] = new List<Building>();
        buildings[EBuilding.ShortRangeTurret] = new List<Building>();
        buildings[EBuilding.LongRangeTurret] = new List<Building>();
        buildingFoundations = new List<BuildingFoundation>();        
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        objectPool = ObjectPool.Instance.transform;

        for (int i = 0; i < pooledFusionReactors; i++)
        {
            buildings[EBuilding.FusionReactor].Add(CreateBuilding(EBuilding.FusionReactor, true));
        }

        for (int i = 0; i < pooledIceDrills; i++)
        {
            buildings[EBuilding.IceDrill].Add(CreateBuilding(EBuilding.IceDrill, true));
        }

        for (int i = 0; i < pooledBoilers; i++)
        {
            buildings[EBuilding.Boiler].Add(CreateBuilding(EBuilding.Boiler, true));
        }

        for (int i = 0; i < pooledGreenhouses; i++)
        {
            buildings[EBuilding.Greenhouse].Add(CreateBuilding(EBuilding.Greenhouse, true));
        }

        for (int i = 0; i < pooledIncinerators; i++)
        {
            buildings[EBuilding.Incinerator].Add(CreateBuilding(EBuilding.Incinerator, true));
        }

        for (int i = 0; i < pooledShortRangeTurrets; i++)
        {
            buildings[EBuilding.ShortRangeTurret].Add(CreateBuilding(EBuilding.ShortRangeTurret, true));
        }

        for (int i = 0; i < pooledLongRangeTurrets; i++)
        {
            buildings[EBuilding.LongRangeTurret].Add(CreateBuilding(EBuilding.LongRangeTurret, true));
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
        //Debug.Log($"BuildingFactory.GetBuilding({buildingType})");
        Building building;

        if (buildings[buildingType].Count > 0)
        {
            Debug.Log("BuildingFactory.GetBuilding(), retrieving building");
            building = buildings[buildingType][0];
            buildings[buildingType].RemoveAt(0);
            building.transform.parent = null;
            building.SetCollidersEnabled("Placement", true);
        }
        else
        {
            Debug.Log("BuildingFactory.GetBuilding(), creating building");
            building = CreateBuilding(buildingType, false);
        }

        Debug.Log($"BuildingFactory(), new building organised ({building}), building collider position is {building.Collider.position} (world) / {building.Collider.localPosition} (local), building model position is {building.Model.position} (world) / {building.Model.localPosition} (local)");


        building.Id = IdGenerator.Instance.GetNextId();
        building.Active = true;

        if (building.Terraformer != null)
        {
            EnvironmentalController.Instance.RegisterBuilding(building.Terraformer);
        }

        Debug.Log($"BuildingFactory(), returning building ({building}), building collider position is {building.Collider.position} (world) / {building.Collider.localPosition} (local), building model position is {building.Model.position} (world) / {building.Model.localPosition} (local)");
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
            case EBuilding.FusionReactor:
                building = Instantiate(fusionReactorPrefab);
                break;
            case EBuilding.IceDrill:
                building = Instantiate(iceDrillPrefab);
                break;
            case EBuilding.Boiler:
                building = Instantiate(boilerPrefab);
                break;
            case EBuilding.Greenhouse:
                building = Instantiate(greenhousePrefab);
                break;
            case EBuilding.Incinerator:
                building = Instantiate(incineratorPrefab);
                break;
            case EBuilding.ShortRangeTurret:
                building = Instantiate(shortRangeTurretPrefab);
                break;
            case EBuilding.LongRangeTurret:
                building = Instantiate(longRangeTurretPrefab);
                break;
            default:
                Debug.LogError("Invalid EBuildingType value passed to BuildingFactory.CreateBuilding().");
                return null;
        }

        if (pooling)
        {
            building.transform.position = objectPool.position;
            building.transform.parent = objectPool;
            building.SetCollidersEnabled("Placement", false);
        }

        return building;
    }

    /// <summary>
    /// Destroy a building.
    /// </summary>
    /// <param name="building">The building to be destroyed.</param>
    /// <param name="consumingResources">Is the building consuming resources and does that consumption need to be cancelled now that it's being destroyed?</param>
    /// <param name="consumingResources">Was the building destroyed while placed, and therefore needs to leave behind foundations?</param>
    public void DestroyBuilding(Building building, bool consumingResources, bool killed)
    {
        BuildingController.Instance.DeRegisterBuilding(building);

        if (building.Terraformer != null)
        {
            EnvironmentalController.Instance.RemoveBuilding(building.Id);
        }

        if (consumingResources)
        {
            ResourceController.Instance.PowerConsumption -= building.PowerConsumption;
            ResourceController.Instance.WaterConsumption -= building.WaterConsumption;
            ResourceController.Instance.WasteConsumption -= building.WasteConsumption;
        }

        if (killed)
        {
            foreach (Vector3 offset in building.BuildingFoundationOffsets)
            {
                GetBuildingFoundation().transform.position = building.transform.position + offset;
            }
        }

        building.Reset();
        building.transform.position = objectPool.position;
        building.transform.parent = objectPool;
        buildings[building.BuildingType].Add(building);
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
        buildingFoundation.Activate();
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
        buildingFoundation.Collider.enabled = false;
        buildingFoundation.transform.position = objectPool.position;
        buildingFoundation.transform.parent = objectPool;
        buildingFoundations.Add(buildingFoundation);
    }
}
