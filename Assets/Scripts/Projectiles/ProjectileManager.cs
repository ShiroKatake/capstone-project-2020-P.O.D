using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for projectiles.
/// </summary>
public class ProjectileManager : SerializableSingleton<ProjectileManager>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<Projectile> projectiles = new List<Projectile>();

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    ///// <summary>
    ///// ProjectileManager's singleton public property.
    ///// </summary>
    //public static ProjectileManager Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    ///// Awake() runs before Start().
    ///// </summary>
    //private void Awake()
    //{
    //    if (Instance != null)
    //    {
    //        Debug.LogError("There should never be more than one BuildingController.");
    //    }

    //    Instance = this;
    //}

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if a projectile with the specified owner has been shot.
    /// </summary>
    /// <param name="owner">The owner of the projectile.</param>
    /// <returns>Whether a projectile with the specified owner has been shot.</returns>
    public bool HasProjectileWithOwner(Transform owner)
    {
        foreach (Projectile p in projectiles)
        {
            if (p.Owner == owner)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Registers a Projectile with ProjectileManager. ProjectileManager adds it to its list of Projectiles in a scene.
    /// <param name="projectile">The projectile being registered with ProjectileManager.</param>
    /// </summary>
    public void RegisterProjectile(Projectile projectile)
    {
        if (!projectiles.Contains(projectile))
        {
            projectiles.Add(projectile);
        }
    }

    /// <summary>
    /// De-registers a Projectile from ProjectileManager. ProjectileManager removes it from its list of Projectiles in a scene.
    /// <param name="projectile">The projectile being de-registered from ProjectileManager.</param>
    /// </summary>
    public void DeRegisterProjectile(Projectile projectile)
    {
        if (projectiles.Contains(projectile))
        {
            projectiles.Remove(projectile);
        }
    }
}
