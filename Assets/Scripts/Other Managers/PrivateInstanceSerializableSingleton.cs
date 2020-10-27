using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for serializable singletons with a private Instance property.
/// </summary>
/// <typeparam name="Type">The type of the singleton.</typeparam>
public abstract class PrivateInstanceSerializableSingleton<Type> : MonoBehaviour where Type : PrivateInstanceSerializableSingleton<Type>, new()
{
    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// This singleton's singleton public property of type T.
    /// </summary>
    private static Type instance;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.Log($"There should never be 2 or more instances of {this}");
        }

        instance = this as Type;
    }
}
