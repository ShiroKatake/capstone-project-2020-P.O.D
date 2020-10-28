using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A size script for circular objects.
/// </summary>
public class CircleSize : Size
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private float radius;

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The exact/approximate radius of the object this component is attached to.
    /// </summary>
    /// <param name="position">The position of the object requesting this object's radius.</param>
    public override float Radius(Vector3? position)
    {
        return radius;
    }
}
