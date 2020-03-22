using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPod : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields

    private Health health;
    private Material material;
    private Color startColour;
    private float colourLerpProgress = 0;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property

    public static HumanPod Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

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

    //Recurring Methods (Framerate)------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (colourLerpProgress != health.Value * 0.01)
        {
            material.color = Color.Lerp(Color.red, startColour, health.Value * 0.01f);
        }
    }
}
