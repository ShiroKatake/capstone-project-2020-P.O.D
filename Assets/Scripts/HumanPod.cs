using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo class for the Cryo Egg the drone's humans are inside.
/// </summary>
public class HumanPod : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields------------------------------------------------------------------------

    private Health health;
    private Material material;
    private Color startColour;
    private float colourLerpProgress = 0;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// HumanPod's singleton public property.
    /// </summary>
    public static HumanPod Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more HumanPods.");
        }

        Instance = this;

        health = GetComponent<Health>();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(meshRenderer.material);
        material = meshRenderer.material;
        startColour = material.color;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    void Update()
    {
        CheckHealth();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks the health of HumanPod, changing its colour if it changes.
    /// </summary>
    private void CheckHealth()
    {
        if (colourLerpProgress != health.Value * 0.01)
        {
            material.color = Color.Lerp(Color.red, startColour, health.Value * 0.01f);
        }
    }
}
