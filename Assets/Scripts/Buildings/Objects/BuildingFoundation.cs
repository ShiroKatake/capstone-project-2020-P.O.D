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

    //Non-Serialized Fields------------------------------------------------------------------------

    private Collider collider;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The building foundation's collider component.
    /// </summary>
    public Collider Collider { get => collider; }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// BuildingFoundation's unique ID number. Id should only be set by BuildingFactory.GetBuildingFoundation().
    /// </summary>
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
            gameObject.name = $"BuildingFoundation {id}";
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Starts the coroutine that enables the building foundation's collider in the next frame.
    /// </summary>
    public void Activate()
    {
        StartCoroutine(EnableCollider());
    }

    /// <summary>
    /// A coroutine that enables the building foundation's collider in the frame after it's started.
    /// </summary>
    IEnumerator EnableCollider()
    {
        yield return null;
        collider.enabled = true;
    }

    /// <summary>
    /// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        Building building = other.gameObject.GetComponentInParent<Building>();
        //Debug.Log($"BuildingFoundation {id} OnTriggerEnter()");

        if (building != null && building.Placed)
        {
            //Debug.Log($"BuildingFoundation {id} OnTriggerEnter() triggered by a building.");
            building.BuildTime -= buildTimeReduction;
            BuildingFactory.Instance.DestroyBuildingFoundation(this);
        }
    }
}
