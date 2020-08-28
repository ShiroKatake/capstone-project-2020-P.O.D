using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A factory class for buildings.
/// </summary>
public class BuildingFactory : Factory<BuildingFactory, Building, EBuilding>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields------------------------------------------------------------------------

    private Vector3 normalScale;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------

	public UnityAction<Transform> onGetTurret;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        normalScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected override void Start()
    {
        base.Start();

        foreach (List<Building> l in pool.Values)
        {
            foreach (Building b in l)
            {
                b.SetCollidersEnabled("Placement", false);
                b.SetMeshRenderersEnabled(false);
                b.SetParticleSystemsEnabled(false);
            }
        }
    }

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get buildings of a specified type from BuildingFactory.
    /// </summary>
    /// <param name="buildingType">The type of building you want BuildingFactory to get for you.</param>
    /// <returns>A building of the specified type.</returns>
    public override Building Get(EBuilding buildingType)
    {
        Building building = base.Get(buildingType);

        //Debug.Log($"BuildingFactory(), new building organised ({building}), building collider position is {building.Collider.position} (world) / {building.Collider.localPosition} (local), building model position is {building.Model.position} (world) / {building.Model.localPosition} (local)");

        building.Id = IdGenerator.Instance.GetNextId();
        building.Active = true;

        if (building.Terraformer != null)
        {
            EnvironmentalController.Instance.RegisterBuilding(building.Terraformer);
        }

        if (building.Model.localPosition != prefabs[buildingType].Model.localPosition)
        {
            //Debug.Log($"Correcting local position for {building}'s model");
            building.Model.localPosition = prefabs[buildingType].Model.localPosition;
        }

		if (buildingType == EBuilding.LongRangeTurret || buildingType == EBuilding.ShortRangeTurret)
		{
			//Debug.Log("Displaying Range.");
			//onGetTurret?.Invoke(building.transform);
			building.TurretRangeFX = TurretRangeFXFactory.Instance.Get();
			TurretRangeFXFactory.Instance.OnGetTurret(building.transform, building.TurretRangeFX);
		}

        //Debug.Log($"BuildingFactory(), returning building ({building}), building collider position is {building.Collider.position} (world) / {building.Collider.localPosition} (local), building model position is {building.Model.position} (world) / {building.Model.localPosition} (local)");

		return building;
    }

    /// <summary>
    /// Custom modifications to a building after Get() retrieves one from the pool.
    /// </summary>
    /// <param name="building">The building being modified.</param>
    /// <returns>The modified building.</returns>
    protected override Building GetRetrievalSetup(Building building)
    {
        building.SetCollidersEnabled("Placement", true);
        building.SetMeshRenderersEnabled(true);
        building.SetParticleSystemsEnabled(true);
        building.transform.localScale = normalScale;
        return building;
    }

    /// <summary>
    /// Destroy a building.
    /// Note: it's probably better to call this method or another overload of Destroy() defined in BuildingFactory than Factory's base version of destroy.
    /// </summary>
    /// <param name="building">The building to be destroyed.</param>
    /// <param name="consumingResources">Is the building consuming resources and does that consumption need to be cancelled now that it's being destroyed?</param>
    /// <param name="killed">Was the building destroyed while placed, and therefore needs to leave behind foundations?</param>
    public void Destroy(Building building, bool consumingResources, bool killed)
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

        building.Killed = killed;
        base.Destroy(building, building.BuildingType);
    }

    /// <summary>
    /// Pools the building passed to it.
    /// </summary>
    /// <param name="toPool">The building to be pooled.</param>
    /// <param name="type">The type of building.</param>
    protected override void PoolNextItem(Building toPool, EBuilding type)
    {
        if (deactivateGameObjectInPool)
        {
            toPool.gameObject.SetActive(true);
        }

        if (toPool.Killed)
        {
            foreach (Vector3 offset in toPool.BuildingFoundationOffsets)
            {
                BuildingFoundationFactory.Instance.Get(toPool.transform.position + offset);
            }

            toPool.Killed = false;
        }

        toPool.Reset();

        if (deactivateGameObjectInPool)
        {
            toPool.gameObject.SetActive(false);
        }

        base.PoolNextItem(toPool, type);
    }
}
