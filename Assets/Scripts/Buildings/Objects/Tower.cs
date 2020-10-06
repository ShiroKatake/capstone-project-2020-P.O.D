using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component class gathering all the bits of the tower needed by aliens because there's always that one stupid alien that can't find them.
/// </summary>
public class Tower : SerializableSingleton<Tower>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private Transform colliderTransform;
    [SerializeField] private Health health;
    [SerializeField] private Size size;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

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
}
