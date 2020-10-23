using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract component class for tracking something's size so that multiple unrelated things can have a radius/diameter or equivalent where necessary.
/// </summary>
public abstract class Size : MonoBehaviour
{
    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The exact/approximate diameter of the object this component is attached to.
    /// </summary>
    /// <param name="position">The position of the object requesting this object's diameter.</param>
    public virtual float Diameter(Vector3? position)
    {
        return Radius(position) * 2;
    }

    /// <summary>
    /// The rounded-up diameter of the object this component is attached to.
    /// </summary>
    /// <param name="position">The position of the object requesting this object's diameter.</param>
    public int DiameterRoundedUp(Vector3? position)
    {
        return Mathf.CeilToInt(Radius(position) * 2);
    }

    /// <summary>
    /// The exact/approximate radius of the object this component is attached to, or equivalent.
    /// </summary>
    /// <param name="position">The position of the object requesting this object's radius.</param>
    public abstract float Radius(Vector3? position);

    /// <summary>
    /// The rounded-up radius of the object this component is attached to.
    /// </summary>
    /// <param name="position">The position of the object requesting this object's radius.</param>
    public int RadiusRoundedUp(Vector3? position)
    {
        return Mathf.CeilToInt(Radius(position));
    }
}
