using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPod : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields

    private Health health;
    private Material material;

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
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    void Update()
    {
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (health.IsDead())
        {
            //health.Die();
            material.color = Color.red;
        }
    }
}
