using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resource collectors to collect resources.
/// </summary>
public class ResourceCollector : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private EResource resource;
    [SerializeField] private int collectionRate;
    [Tooltip("Does this resource collector generate its own resources? Or does it need to be placed on a resource patch?")]
    [SerializeField] private bool isResourceGenerator;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Building building;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The building this resource collector class is a component of.
    /// </summary>
    public Building Building { get => building; }

    /// <summary>
    /// The rate at which this building collects its resource.
    /// </summary>
    public int CollectionRate { get => collectionRate; }

    /// <summary>
    /// The resource this building collects.
    /// </summary>
    public EResource Resource { get => resource; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        building = gameObject.GetComponent<Building>();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Is this building over a position with a resource that matches what this resource collector can collect?
    /// </summary>
    public bool CanCollectResourcesAtPosition()
    {
        //Debug.Log($"{this}.ResourceCollector.CanCollectResourcesAtPosition()");

        if (isResourceGenerator)
        {
            //Debug.Log($"{this}.ResourceCollector.CanCollectResourcesAtPosition(), isResourceGenerator is true, returning true.");
            return true;
        }

        foreach (Vector3 offset in building.BuildingFoundationOffsets)
        {
            Vector3 testPos = transform.position + offset;
            //Debug.Log($"{this}.ResourceCollector.CanCollectResourcesAtPosition(), checking position {testPos}.");
            PositionData posData = MapManager.Instance.GetPositionData(testPos);

            if (posData == null)
            {
                //Debug.Log($"{this}.ResourceCollector.CanCollectResourcesAtPosition(), MapManager.GetPositionData() returned null for position {testPos}, returning false.");
                return false;
            }

            if (posData.Resource != resource)
            {
                //Debug.Log($"{this}.ResourceCollector.CanCollectResourcesAtPosition(), resource at position {testPos} is {posData.Resource} not {resource}, returning false.");
                return false;
            }
        }

        //Debug.Log($"{this}.ResourceCollector.CanCollectResourcesAtPosition(), resource for all offsets from position {transform.position} is {resource}, returning true");
        return true;
    }

    /// <summary>
    /// Update the resource contribution of the building this resource collector class is a component of.
    /// </summary>
    public void Activate()
    {
        switch(resource)
        {
            case EResource.Power:
                ResourceManager.Instance.PowerSupply += collectionRate;
                break;
            case EResource.Water:
                ResourceManager.Instance.WaterSupply += collectionRate;
                break;
            case EResource.Plants:
                ResourceManager.Instance.PlantsSupply += collectionRate;
                break;
            case EResource.Gas:
                ResourceManager.Instance.GasSupply += collectionRate;
                break;
        }
    }

    /// <summary>
    /// Update the resource contribution of the building this resource collector class is a component of.
    /// </summary>
    public void Deactivate()
    {
        switch (resource)
        {
            case EResource.Power:
                ResourceManager.Instance.PowerSupply -= collectionRate;
                break;
            case EResource.Water:
                ResourceManager.Instance.WaterSupply -= collectionRate;
                break;
            case EResource.Plants:
                ResourceManager.Instance.PlantsSupply -= collectionRate;
                break;
            case EResource.Gas:
                ResourceManager.Instance.GasSupply -= collectionRate;
                break;
        }
    }
}
