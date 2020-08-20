using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NameByPosition : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private string name;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (!Application.isEditor || Application.isPlaying)
        {
            Debug.LogError($"You left NameByPosition enabled for {this}, which is gonna impact performance. Please disable this script on the prefab / game object and go again. This should only be enabled for the instant you need to rename a lot of somethings quickly.");
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
        gameObject.name = $"{name} ({transform.position.x}, {transform.position.y}, {transform.position.z})";
        this.enabled = false;
    }
#endif
}
