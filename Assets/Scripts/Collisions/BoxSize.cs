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
    private float halfLengthSquared;
    private float halfWidthSquared;

    //private float cornerAngleTopRight;
    //private float cornerAngleBottomRight;
    //private float cornerAngleBottomLeft;
    //private float cornerAngleTopLeft;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// This object's full length along the z-axis.
    /// </summary>
    public float Length { get => length; }

    /// <summary>
    /// This object's full width along the x-axis.
    /// </summary>
    public float Width { get => width; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        halfLength = length * 0.5f;
        halfWidth = width * 0.5f;
        halfLengthSquared = halfLength * halfLength;
        halfWidthSquared = halfWidth * halfWidth;
        hypotSquared = halfLength * halfLength + halfWidth * halfWidth;
        hypot = Mathf.Sqrt(hypotSquared);

        //cornerAngleTopRight = MathUtility.Instance.Angle(Vector2.zero, new Vector2(halfWidth, halfLength));
        //cornerAngleBottomRight = MathUtility.Instance.Angle(Vector2.zero, new Vector2(halfWidth, -halfLength));
        //cornerAngleBottomLeft = MathUtility.Instance.Angle(Vector2.zero, new Vector2(-halfWidth, -halfLength));
        //cornerAngleTopLeft = MathUtility.Instance.Angle(Vector2.zero, new Vector2(-halfWidth, halfLength));
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Calculates the equivalent of the box object's radius, given the position of the object requesting the radius and, therefore, the angle from this object to that other object.
    /// </summary>
    /// <param name="position">The position of the object requesting this object's radius.</param>
    /// <returns>The "radius" of the box object.</returns>
    public override float Radius(Vector3? position)
    {
        //Debug.Log("BoxSize.Radius");

        if (!position.HasValue || MathUtility.Instance.Square(position.Value.x - transform.position.x) + MathUtility.Instance.Square(position.Value.z - transform.position.z) > hypotSquared)
        {
            //Debug.Log($"Returning hypot because too far away or position is null, position is null: {position == null}");
            return hypot;
        }

        float deltaX = MathUtility.Instance.FloatMagnitude(position.Value.x - transform.position.x);
        float deltaZ = MathUtility.Instance.FloatMagnitude(position.Value.z - transform.position.z);

        if (deltaX >= halfWidth && deltaZ >= halfLength)
        {
            //Debug.Log($"deltaX {deltaX} and deltaX {deltaZ} both meet or exceed halfWidth {halfWidth} and halfLength {halfLength} respectively, in hypot quadrants, returning hypot {hypot}");
            return hypot;
        }
        else if (deltaX < halfWidth)
        {
            return CalculateHypotenuse(deltaX, halfLengthSquared);
            //Debug.Log($"DeltaX {deltaX} < halfWidth {halfWidth}, calculated hypotenuse from deltaX and halfLengthSquared, returning result {result}");
            //return result;
        }
        else if (deltaZ < halfLength)
        {
            return CalculateHypotenuse(deltaZ, halfWidthSquared);
            //Debug.Log($"DeltaZ {deltaZ} < halfLength {halfLength}, calculated hypotenuse from deltaZ and halfWidthSquared, returning result {result}");
            //return result;
        }

        //Debug.Log($"Exhausted all other checks for deltaX and deltaZ, returning hypot {hypot}");
        return hypot;
    }

    /// <summary>
    /// Calculates the hypotenuse of a triangle given the delta x or delta z between their positions, and the square of the other non-hypotenuse side (delta x should get half z squared, and vice versa).
    /// </summary>
    /// <param name="deltaDynamicSide">The delta x or delta z between the positions of this object and the other object.</param>
    /// <param name="squaredFixedSide">The square of the opposite side to deltaDynamicSide.</param>
    /// <returns>The hypotenuse.</returns>
    private float CalculateHypotenuse(float deltaDynamicSide, float squaredFixedSide)
    {
        return Mathf.Sqrt(deltaDynamicSide * deltaDynamicSide + squaredFixedSide);
    }
}
