using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component class for tracking something's size so that multiple unrelated things can have a radius/diameter where necessary.
/// </summary>
public class Size : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private float radius;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The exact/approximate radius of the object this component is attached to.
    /// </summary>
    public float Radius
    {
        get
        {
            return radius;
        }
    }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// The exact/approximate diameter of the object this component is attached to.
    /// </summary>
    public float Diameter { get => radius * 2; }
    
    /// <summary>
    /// The rounded-up diameter of the object this component is attached to.
    /// </summary>
    public int DiameterRoundedUp
    {
        get
        {
            return Mathf.CeilToInt(radius * 2);
        }
    }

    /// <summary>
    /// The rounded-up radius of the object this component is attached to.
    /// </summary>
    public int RadiusRoundedUp
    {
        get
        {
            return Mathf.CeilToInt(radius);
        }
    }
}
