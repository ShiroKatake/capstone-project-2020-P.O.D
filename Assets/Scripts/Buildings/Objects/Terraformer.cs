using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Terraformers to terraform the planet.
/// </summary>
public class Terraformer : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private EEnvironmentParameter environmentParameter;
    [SerializeField] private float environmentalAffect;

    //Non-Serialized Fields------------------------------------------------------------------------

    private int buildingId;
    private Building building;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The building this terraformer class is a component of.
    /// </summary>
    public Building Building { get => building; }

    /// <summary>
    /// How quickly this building affects the environment.
    /// </summary>
    public float EnvironmentalAffect { get => environmentalAffect; }

    /// <summary>
    /// The aspect of the environment this buildng affects, if any.
    /// </summary>
    public EEnvironmentParameter EnvironmentParameter { get => environmentParameter; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        building = gameObject.GetComponent<Building>();
    }
}
