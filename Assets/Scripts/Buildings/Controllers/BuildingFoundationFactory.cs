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
    /// <param name="type">The type of building foundation to instantiate. Should be left as default value of ENone.None.</param>
    /// <returns>A building foundation.</returns>
    public override BuildingFoundation Get(Vector3 position, ENone type = ENone.None)
    {
        BuildingFoundation buildingFoundation = base.Get(position, type);
        buildingFoundation.Id = IdGenerator.Instance.GetNextId();
        buildingFoundation.Activate();
        return buildingFoundation;
    }

    /// <summary>
    /// Destroy a building foundation.
    /// </summary>
    /// <param name="buildingFoundation">The building foundation to be destroyed.</param>
    /// <param name="type">The type of building foundation to destroy. Should be left as default value of ENone.None.</param>
    public override void Destroy(BuildingFoundation buildingFoundation, ENone type = ENone.None)
    {
        base.Destroy(buildingFoundation, type);
    }

    /// <summary>
    /// Pools the building foundation passed to it.
    /// </summary>
    /// <param name="toPool">The building foundation to be pooled.</param>
    /// <param name="type">The type of building foundation.</param>
    protected override void PoolNextItem(BuildingFoundation toPool, ENone type)
    {
        toPool.Collider.enabled = false;
        base.PoolNextItem(toPool, type);
    }
}
