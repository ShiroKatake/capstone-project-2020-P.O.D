using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NameByPosition : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private string name;
    [SerializeField] private bool executing;
    [SerializeField] private bool update;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Vector3 currentPosition;
    private Vector3 previousPosition;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        currentPosition = Vector3.zero;
        previousPosition = Vector3.zero;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (executing)
        {
            previousPosition = currentPosition;
            currentPosition = transform.position;

            if (update || previousPosition != currentPosition)
            {
                gameObject.name = $"{name} ({currentPosition.x}, {currentPosition.y}, {currentPosition.z})";
            }
        }
    }
}
