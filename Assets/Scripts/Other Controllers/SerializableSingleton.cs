using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for serializable singletons.
/// </summary>
/// <typeparam name="T">The type of the singleton.</typeparam>
public abstract class SerializableSingleton<T> : MonoBehaviour where T: SerializableSingleton<T>, new()
{
    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// This singleton's singleton public property of type T.
    /// </summary>
    public static T Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.Log($"There should never be 2 or more instances of {this}");
        }

        Instance = this as T;
    }
}
