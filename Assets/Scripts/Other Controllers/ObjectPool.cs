using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A blank singleton to make the object pool parent object easily referenceable in-code.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// ObjectPoolParent's singleton public property.
    /// </summary>
    public static ObjectPool Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one ObjectPoolParent.");
        }

        Instance = this;
    }
}
