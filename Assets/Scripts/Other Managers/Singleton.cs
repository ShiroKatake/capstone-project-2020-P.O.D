using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for non-serializable singletons with a public Instance property.
/// </summary>
/// <typeparam name="Type">The type of the singleton.</typeparam>
public abstract class Singleton<Type> where Type : Singleton<Type>, new()
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields------------------------------------------------------------------------

    private static Type instance = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// This singleton's singleton public property of type T.
    /// </summary>
    public static Type Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Type();
            }

            return instance;
        }
    }
}
