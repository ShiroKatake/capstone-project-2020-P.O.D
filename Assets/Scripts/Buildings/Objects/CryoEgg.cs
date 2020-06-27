using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component class gathering all the bits of cryo egg needed by aliens because there's always that one stupid alien that can't find them.
/// </summary>
public class CryoEgg : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private Transform colliderTransform;
    [SerializeField] private Health health;
    [SerializeField] private Size size;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// CryoEgg's singleton public property.
    /// </summary>
    public static CryoEgg Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The transform of the CryoEgg's collider.
    /// </summary>
    public Transform ColliderTransform { get => colliderTransform; }

    /// <summary>
    /// The CryoEgg's Health component.
    /// </summary>
    public Health Health { get => health; }

    /// <summary>
    /// The CryoEgg's Size component.
    /// </summary>
    public Size Size { get => size; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one CryoEgg.");
        }

        Instance = this;
    }
}
