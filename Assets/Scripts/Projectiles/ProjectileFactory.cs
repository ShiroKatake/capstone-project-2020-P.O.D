using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for ammunition.
/// </summary>
public class ProjectileFactory : Factory<ProjectileFactory, Projectile, EProjectileType>
{
    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves Projectiles from a pool if there's any available, and instantiates a new Projectile if there isn't one.
    /// </summary>
    /// <param name="owner">The player or turret firing the projectile from their weapon.</param>
    /// <param name="position">The position the Projectile should be instantiated at.</param>
    /// <returns>A new projectile.</returns>
    public Projectile Get(EProjectileType type, Transform owner, Transform barrelTip)
    {
        Projectile projectile = Get(type, barrelTip.position);
        projectile.Owner = owner;
		projectile.transform.rotation = barrelTip.rotation;
		return projectile;
    }

    /// <summary>
    /// Instantiates a new Projectile.
    /// </summary>
    /// <returns>A new Projectile.</returns>
    protected override Projectile Create(EProjectileType type)
    {
        Projectile projectile = base.Create(type);
        projectile.Collider.enabled = false;
        return projectile;
    }

    /// <summary>
    /// Handles the destruction of projectiles.
    /// </summary>
    /// <param name="projectile">The projectile to destroy.</param>
    public void Destroy(Projectile projectile)
    {
        Destroy(projectile.Type, projectile);
    }

    /// <summary>
    /// Handles the destruction of projectiles.
    /// </summary>
    /// <param name="type">The type of projectile to destroy.</param>
    /// <param name="projectile">The projectile to destroy.</param>
    public override void Destroy(EProjectileType type, Projectile projectile)
    {
        ProjectileManager.Instance.DeRegisterProjectile(projectile);
        projectile.Active = false;
        projectile.Collider.enabled = false;
        projectile.Rigidbody.velocity = Vector3.zero;
        projectile.Rigidbody.isKinematic = true;
        base.Destroy(type, projectile);
    }
}
