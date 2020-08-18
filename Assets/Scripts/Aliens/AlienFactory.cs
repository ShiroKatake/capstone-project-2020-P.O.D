using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Factory class for aliens.
/// </summary>
public class AlienFactory : Factory<AlienFactory, Alien, ENone>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private float alienSpawnHeight;
   
    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The height at which aliens spawn.
    /// </summary>
    public float AlienSpawnHeight { get => alienSpawnHeight; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected override void Start()
    {
        base.Start();

        foreach (Alien a in pool[ENone.None])
        {
            foreach (Collider c in a.GetComponents<Collider>())
            {
                c.enabled = false;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Retrieves an alien from the pool if there's any available, and instantiates a new alien if there isn't one.
    /// </summary>
    /// <param name="position">The position the alien should be instantiated at.</param>
    /// <returns>A new alien.</returns>
    public Alien Get(Vector3 position)
    {
        return Get(ENone.None, position);
    }

    /// <summary>
    /// Does extra setup for the alien before returning it from Get().
    /// </summary>
    /// <param name="result">The alien to return.</param>
    /// <returns>The now-setup alien.</returns>
    protected override Alien GetRetrievalSetup(Alien result)
    {
        foreach (Collider c in result.GetComponents<Collider>())
        {
            c.enabled = true;
        }

        return result;
    }

    /// <summary>
    /// Handles the destruction of aliens.
    /// </summary>
    /// <param name="alien">The alien to be destroyed.</param>
    public void Destroy(Alien alien)
    {
        Destroy(ENone.None, alien);
    }

    /// <summary>
    /// Handles the destruction of aliens.
    /// </summary>
    /// <param name="type">The type of the alien to be destroyed.</param>
    /// <param name="alien">The alien to be destroyed.</param>
    public override void Destroy(ENone type, Alien alien)
    {
        alien.Reset();
        AlienController.Instance.DeRegisterAlien(alien);
        base.Destroy(type, alien);
    }
}
