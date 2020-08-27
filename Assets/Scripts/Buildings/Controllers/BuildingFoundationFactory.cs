using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for building foundations.
/// </summary>
public class BuildingFoundationFactory : Factory<BuildingFoundationFactory, BuildingFoundation, ENone>
{
    //Triggered Methods (Building Foundations)-------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get a building foundation from BuildingFactory.
    /// </summary>
    /// <param name="position">The position the building foundation should be instantiated at.</param>
    /// <returns>A building foundation.</returns>
    public BuildingFoundation Get(Vector3 position)
    {
        return Get(ENone.None, position);
    }

    /// <summary>
    /// Get a building foundation from BuildingFactory.
    /// </summary>
    /// <param name="position">The position the building foundation should be instantiated at.</param>
    /// <returns>A building foundation.</returns>
    public override BuildingFoundation Get(ENone type, Vector3 position)
    {
        BuildingFoundation buildingFoundation = base.Get(ENone.None, position);
        buildingFoundation.Id = IdGenerator.Instance.GetNextId();
        buildingFoundation.Activate();
        return buildingFoundation;
    }

    /// <summary>
    /// Destroy a building foundation.
    /// </summary>
    /// <param name="buildingFoundation">The building foundation to be destroyed.</param>
    public void Destroy(BuildingFoundation buildingFoundation)
    {
        Destroy(ENone.None, buildingFoundation);
    }

    /// <summary>
    /// Destroy a building foundation.
    /// </summary>
    /// <param name="buildingFoundation">The building foundation to be destroyed.</param>
    public override void Destroy(ENone type, BuildingFoundation buildingFoundation)
    {
        buildingFoundation.Collider.enabled = false;
        base.Destroy(type, buildingFoundation);
    }
}
