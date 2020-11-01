using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A factory class for buildings.
/// </summary>
public class BuildingFactory : Factory<BuildingFactory, Building, EBuilding>
{
	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------

	public UnityAction<Building> onPlacementStarted;
	public UnityAction<Building> onBuildingHasRange;
	public UnityAction onPlacementValid;
	public UnityAction onPlacementInvalid;
	public UnityAction onPlacementFinished;

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

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

        if (building.Terraformer != null) EnvironmentManager.Instance.RegisterBuilding(building.Terraformer);
        if (building.Model.localPosition != prefabs[buildingType].Model.localPosition) building.Model.localPosition = prefabs[buildingType].Model.localPosition;
		if (buildingType == EBuilding.MachineGunTurret || buildingType == EBuilding.ShotgunTurret) onBuildingHasRange?.Invoke(building);

		onPlacementStarted?.Invoke(building);        
        building.CanPlace();
		
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
        return building;
    }

    /// <summary>
    /// Destroy a building.
    /// Note: it's probably better to call this method or another overload of Destroy() defined in BuildingFactory than Factory's base version of destroy.
    /// </summary>
    /// <param name="building">The building to be destroyed.</param>
    /// <param name="consumingResources">Is the building in a state where it can consume resources and does that consumption need to be verified and cancelled now that it's being destroyed?</param>
    /// <param name="killed">Has the building been placed successfully and therefore needs to leave behind foundations now that it's being destroyed?</param>
    public void Destroy(Building building, bool consumingResources, bool killed)
    {
        BuildingManager.Instance.DeRegisterBuilding(building);

        if (BuildingDemolitionController.Instance.SelectedBuilding == building)
        {
            BuildingDemolitionController.Instance.HideDemolitionMenu();
        }

        if (building.Terraformer != null)
        {
            EnvironmentManager.Instance.RemoveBuilding(building.Id);
        }

        if (consumingResources && !building.DisabledByPlayer)
        {
            ResourceManager.Instance.PowerConsumption -= building.PowerConsumption;
            ResourceManager.Instance.WaterConsumption -= building.WaterConsumption;
            ResourceManager.Instance.PlantsConsumption -= building.PlantsConsumption;
            ResourceManager.Instance.PlantsConsumption -= building.GasConsumption;
        }

        if (killed)
        {
            foreach (Vector3 offset in building.BuildingFoundationOffsets)
            {
                BuildingFoundationFactory.Instance.Get(building.transform.position + offset);
            }
        }

        building.Reset();
        Destroy(building, building.BuildingType);
    }
}
