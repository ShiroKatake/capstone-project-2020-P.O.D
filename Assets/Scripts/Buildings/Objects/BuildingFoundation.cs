using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Building foundations left behind when a building is destroyed. Building on top of building foundations reduces the building's build time.
/// </summary>
public class BuildingFoundation : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private int id;
    [SerializeField] private float buildTimeReduction;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// BuildingFoundation's unique ID number.
    /// </summary>
    public int Id { get => id; set => id = value; }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        Building building = other.gameObject.GetComponent<Building>();

        if (building != null)
        {
            building.BuildTime -= buildTimeReduction;
            BuildingFactory.Instance.DestroyBuildingFoundation(this);
        }
    }

    //TODO: disable trigger collider while pooled.
}
