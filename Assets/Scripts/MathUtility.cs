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
    /// Returns the sign of a number, i.e. +1 if it's positive or 0, and -1 if it's negative.
    /// </summary>
    /// <param name="num">The number to determine the sign of.</param>
    /// <returns>The sign (+1 or -1) of the number.</returns>
    public int Sign(float num)
    {
        return (num < 0 ? -1 : 1);
    } 
}
