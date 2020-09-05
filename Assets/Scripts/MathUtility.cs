using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton class for reusable maths methods that don't seem to be included in Mathf or any other classes or libraries or that have been put together because people can't be bothered Googling the actual methods.
/// </summary>
public class MathUtility : Singleton<MathUtility>
{
    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// MathUtility's constructor method.
    /// WARNING: DO NOT CALL IN-CODE. IT IS ONLY PUBLIC FOR SINGLETON.INSTANCE TO USE.
    /// </summary>
    public MathUtility()
    {

    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  

    //Number Magnitude-----------------------------------------------------------------------------

    /// <summary>
    /// Returns the magnitude of a number.
    /// </summary>
    /// <param name="num">The number to calculate the magnitude of.</param>
    /// <returns>The magnitude of the number.</returns>
    public float FloatMagnitude(float num)
    {
        if (num < 0)
        {
            num *= -1;
        }

        return num;
    }

    /// <summary>
    /// Returns the magnitude of a number.
    /// </summary>
    /// <param name="num">The number to calculate the magnitude of.</param>
    /// <returns>The magnitude of the number.</returns>
    public int IntMagnitude(int num)
    {
        if (num < 0)
        {
            num *= -1;
        }

        return num;
    }

    //Number Properties----------------------------------------------------------------------------

    /// <summary>
    /// Returns the sign of a number, i.e. +1 if it's positive or 0, and -1 if it's negative.
    /// </summary>
    /// <param name="num">The number to determine the sign of.</param>
    /// <returns>The sign (+1 or -1) of the number.</returns>
    public int Sign(float num)
    {
        return (num < 0 ? -1 : 1);
    }

	/// <summary>
	/// Determines whether the number is odd or not.
	/// </summary>
	/// <param name="num">The number to determine.</param>
	/// <returns>whether the number is odd (true) or not (false).</returns>
	public bool IsOdd(float num)
	{
		return (num % 2 == 1);
	}

	/// <summary>
	/// Determines whether the number is even or not.
	/// </summary>
	/// <param name="num">The number to determine.</param>
	/// <returns>whether the number is even (true) or not (false).</returns>
	public bool IsEven(float num)
	{
		return (num % 2 == 0);
	}

    //Angles---------------------------------------------------------------------------------------

    /// <summary>
    /// Converts the provided angle to an angle between 0 degrees and 360 degrees
    /// </summary>
    /// <param name="angle">The raw angle.</param>
    /// <returns>The normalised angle.</returns>
    public float NormaliseAngle(float angle)
    {
        while (angle > 360)
        {
            angle -= 360;
        }

        while (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    /// <summary>
    /// Checks if an angle is between two specified angles.
    /// </summary>
    /// <param name="angle">The angle being checked.</param>
    /// <param name="a">The counter-clockwise-most bound of what's acceptable for the angle being checked.</param>
    /// <param name="b">The clockwise-most bound of what's acceptable for the angle being checked.</param>
    /// <returns>Whether the angle being checked is between a and b.</returns>
    public bool AngleIsBetween(float angle, float a, float b)
    {
        angle = NormaliseAngle(angle);

        if (a < b) //Range does not include 0/360 degrees, or 0/360 degrees is a or b.
        {
            return (angle >= a && angle <= b) || (angle == 0 && (a == 360 || b == 360)) || (angle == 360 && (a == 0 || b == 0));
        }
        else if (a > b) //Range does include 0/360 degrees, and 0/360 degrees doesn't need to be a or b.
        {
            return ((angle >= a && angle <= 360)) || (angle >= 0 && angle <= b);
        }
        else //a == b, range is only one angle.
        {
            return angle == a;
        }
    }
}
