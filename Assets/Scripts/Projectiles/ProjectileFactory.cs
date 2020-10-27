using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for ammunition.
/// </summary>
public class ProjectileFactory : Factory<ProjectileFactory, Projectile, EProjectileType>
{
    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected override void Start()
    {
        base.Start();

        foreach (List<Projectile> l in pool.Values)
        {
            foreach (Projectile p in l)
            {
                p.Light.enabled = false;
                p.Renderer.enabled = false;
                p.Collider.enabled = false;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves Projectiles from a pool if there's any available, and instantiates a new Projectile if there isn't one.
    /// </summary>
    /// <param name="owner">The player or turret firing the projectile from their weapon.</param>
    /// <param name="barrelTip">The transform of the barrel tip it's being fired from.</param>
    /// <param name="type">The type of projectile to get.</param>
    /// <returns>A new projectile.</returns>
    public Projectile Get(Transform owner, Transform barrelTip, EProjectileType type)
    {
        Projectile projectile = Get(barrelTip.position, type);
        projectile.Owner = owner;
		projectile.transform.rotation = barrelTip.rotation;
        Debug.Log($"{this}.Get(), projectile is {projectile}");
		return projectile;
    }

    /// <summary>
    /// Custom modifications to a projectile after Get() retrieves it from the pool.
    /// </summary>
    /// <param name="projectile">The projectile being modified.</param>
    /// <returns>The modified projectile.</returns>
    protected override Projectile GetRetrievalSetup(Projectile projectile)
    {
        projectile.Light.enabled = true;
        projectile.Renderer.enabled = true;
        return projectile;
    }

    /// <summary>
    /// Handles the destruction of projectiles.
    /// </summary>
    /// <param name="projectile">The projectile to destroy.</param>
    public void Destroy(Projectile projectile)
    {
        Destroy(projectile, projectile.Type);
    }

    /// <summary>
    /// Handles the destruction of projectiles.
    /// </summary>
    /// <param name="projectile">The projectile to destroy.</param>
    /// <param name="type">The type of projectile to destroy.</param>
    public override void Destroy(Projectile projectile, EProjectileType type)
    {
        ProjectileManager.Instance.DeRegisterProjectile(projectile);
        projectile.Active = false;
        projectile.Collider.enabled = false;
        projectile.Light.enabled = false;
        projectile.Renderer.enabled = false;
        projectile.Rigidbody.velocity = Vector3.zero;
        projectile.Rigidbody.isKinematic = true;
        base.Destroy(projectile, type);
    }
}
