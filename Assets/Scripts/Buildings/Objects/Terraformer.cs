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
    private bool operational;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The ID of the building this terraformer class is a component of. Should only be set by Building.Id, which in turn should
    /// only be set by BuildingFactory.GetBuilding().
    /// </summary>
    public int BuildingId { get => buildingId; set => buildingId = value; }

    /// <summary>
    /// How quickly this building affects the environment.
    /// </summary>
    public float EnvironmentalAffect { get => environmentalAffect; }

    /// <summary>
    /// The aspect of the environment this buildng affects, if any.
    /// </summary>
    public EEnvironmentParameter EnvironmentParameter { get => environmentParameter; }

    /// <summary>
    /// Is the building currently operational?
    /// </summary>
    public bool Operational { get => operational; set => operational = value; }
}
