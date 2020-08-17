using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for non-serializable singletons.
/// </summary>
/// <typeparam name="T">The type of the singleton.</typeparam>
public abstract class Singleton<T> where T : Singleton<T>, new()
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields------------------------------------------------------------------------

    private static T instance = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// This singleton's singleton public property of type T.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}
