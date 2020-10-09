using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Factory class for aliens.
/// </summary>
public class AlienFactory : Factory<AlienFactory, Alien, EAlien>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Alien Stats")]
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

        foreach (List<Alien> l in pool.Values)
        {
            foreach (Alien a in l)
            {
                foreach (Collider c in a.GetComponents<Collider>())
                {
                    c.enabled = false;
                }
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves an alien from the pool if there's any available, and instantiates a new alien if there isn't one.
    /// </summary>
    /// <param name="position">The position the alien should be instantiated at.</param>
    /// <param name="type">The type of alien to instantiate.</param>
    /// <returns>A new alien.</returns>
    public override Alien Get(Vector3 position, EAlien type)
    {
        return base.Get(position, type);
    }

    /// <summary>
    /// Does extra setup for the alien before returning it from Get().
    /// </summary>
    /// <param name="result">The alien to return.</param>
    /// <returns>The now-setup alien.</returns>
    protected override Alien GetRetrievalSetup(Alien result)
    {
        result.Renderer.enabled = true;

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
    /// <param name="type">The type of the alien to be destroyed.</param>
    public override void Destroy(Alien alien, EAlien type)
    {
        alien.Reset();
        AlienController.Instance.DeRegisterAlien(alien);
        base.Destroy(alien, type);
    }

    /// <summary>
    /// Get a prefab from the factory.
    /// Note: only use if you need to access the prefab directly; if you need an instance of an alien, use Get().
    /// </summary>
    /// <param name="type">The type of the alien prefab.</param>
    /// <returns>The requested alien prefab.</returns>
    public Alien GetPrefab(EAlien type)
    {
        if (prefabs.ContainsKey(type))
        {
            return prefabs[type];
        }

        return null;
    }
}
