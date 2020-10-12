using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A size script for box-shaped objects.
/// </summary>
public class BoxSize : Size
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Tooltip("This object's full length along the z-axis.")]
    [SerializeField] private float length;
    [Tooltip("This object's full width along the x-axis.")]
    [SerializeField] private float width;

    //Non-Serialized Fields------------------------------------------------------------------------

    private float halfLength;
    private float halfWidth;
    private float hypot;
    private float hypotSquared;

    private float cornerAngleTopRight;
    private float cornerAngleBottomRight;
    private float cornerAngleBottomLeft;
    private float cornerAngleTopLeft;

    private float degreesToRadiansMultiplier;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        degreesToRadiansMultiplier = Mathf.PI / 180;

        halfLength = length * 0.5f;
        halfWidth = width * 0.5f;
        hypotSquared = halfLength * halfWidth;
        hypot = Mathf.Sqrt(hypotSquared);

        cornerAngleTopRight = MathUtility.Instance.Angle(Vector2.zero, new Vector2(halfWidth, halfLength));
        cornerAngleBottomRight = MathUtility.Instance.Angle(Vector2.zero, new Vector2(halfWidth, -halfLength));
        cornerAngleBottomLeft = MathUtility.Instance.Angle(Vector2.zero, new Vector2(-halfWidth, -halfLength));
        cornerAngleTopLeft = MathUtility.Instance.Angle(Vector2.zero, new Vector2(-halfWidth, halfLength));
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Calculates the equivalent of the box object's radius, given the position of the object requesting the radius and, therefore, the angle from this object to that other object.
    /// </summary>
    /// <param name="position">The position of the object requesting this object's radius.</param>
    /// <returns>The "radius" of the box object.</returns>
    public override float Radius(Vector3? position)
    {
        if (!position.HasValue || Vector3.SqrMagnitude(position.Value - transform.position) > hypotSquared)
        {
            return hypot;
        }

        float angle = MathUtility.Instance.Angle(new Vector2(transform.position.x, transform.position.z), new Vector2(position.Value.x, position.Value.z));

        if (angle == 0 || angle == 360 || angle == 180)
        {
            return halfLength;
        }
        else if (angle == 90 || angle == 270)
        {
            return halfWidth;
        }
        else if (angle == cornerAngleTopLeft 
                || angle == cornerAngleTopRight 
                || angle == cornerAngleBottomLeft 
                || angle == cornerAngleBottomRight
        )
        {
            return hypot;
        }
        else if (MathUtility.Instance.AngleIsBetween(angle, cornerAngleTopLeft, cornerAngleTopRight)
                || MathUtility.Instance.AngleIsBetween(angle, cornerAngleBottomRight, cornerAngleBottomLeft) 
        )
        {
            CalculateHypotenuse(halfLength, angle);
        }
        else if (MathUtility.Instance.AngleIsBetween(angle, cornerAngleTopRight, cornerAngleBottomRight) 
                || MathUtility.Instance.AngleIsBetween(angle, cornerAngleBottomLeft, cornerAngleTopLeft)
        )
        {
            CalculateHypotenuse(halfWidth, angle);
        }

        return hypot;
    }

    /// <summary>
    /// Calculates the hypotenuse of a triangle where one side forming the right angle is either this object's half-length or half-width, and you have the angle from this object's position to the other's position.
    /// </summary>
    /// <param name="adjacent">The adjacent side of the triangle, i.e. the side that is this object's half-length or half-width.</param>
    /// <param name="angle">The angle from this object's position to the other's position.</param>
    /// <returns>The hypotenuse of the aforementioned triangle.</returns>
    private float CalculateHypotenuse(float adjacent, float angle)
    {
        return adjacent / Mathf.Cos(angle * degreesToRadiansMultiplier);
    }
}
