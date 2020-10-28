using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Snaps objects' x and z coordinates to an integer position.
/// </summary>
[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] Transform gameObjectToPosition;
    //[SerializeField] Vector3 offset;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (!Application.isEditor || Application.isPlaying)
        {
            Debug.LogError($"You left SnapToGrid enabled for {this}, which is gonna impact performance. Please disable this script on the prefab / game object and go again. This should only be enabled for the instant you need to snap-to-grid a lot of somethings quickly.");
            this.enabled = false;
        }
    }

#if UNITY_EDITOR
    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        Vector3 position = gameObjectToPosition.position;
        position.x = Mathf.Round(position.x);
        position.z = Mathf.Round(position.z);
        //position += offset;
        gameObjectToPosition.position = position;
        this.enabled = false;
    }
#endif
}
