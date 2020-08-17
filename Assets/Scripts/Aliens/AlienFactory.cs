﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Factory class for aliens.
/// </summary>
public class AlienFactory : Factory<AlienFactory, Alien>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    //[Header("Game Objects")]
    //[SerializeField] private Alien alienPrefab;

    [Header("Stats")]
    [SerializeField] private int pooledAliens;
    [SerializeField] private float alienSpawnHeight;


    //Non-Serialized Fields------------------------------------------------------------------------

    //private Transform objectPool;
    //private List<Alien> alienPool;
   
    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    ///// <summary>
    ///// AlienController's singleton public property.
    ///// </summary>
    //public static AlienFactory Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The height at which aliens spawn.
    /// </summary>
    public float AlienSpawnHeight { get => alienSpawnHeight; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        //if (Instance != null)
        //{
        //    Debug.LogError("There should never be 2 or more AlienFactories.");
        //}

        //Instance = this;
        base.Awake();
        //alienPool = new List<Alien>();        
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        //objectPool = ObjectPool.Instance.transform;
        base.Start();

        for (int i = 0; i < pooledAliens; i++)
        {
            Alien alien = Instantiate(prefab, objectPool.position, new Quaternion()); 
            alien.transform.parent = objectPool;

            foreach (Collider c in alien.GetComponents<Collider>())
            {
                c.enabled = false;
            }

            pool.Add(alien);            
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// Retrieves Enemies from a pool if there's any available, and instantiates a new alien if there isn't one.
    ///// </summary>
    ///// <param name="position">The position the alien should be instantiated at.</param>
    ///// <returns>A new alien.</returns>
    //public Alien Get(Vector3 position)
    ////public Alien GetAlien(Vector3 position)
    //{
    //    Alien alien;        

    //    if (pool.Count > 0)
    //    {
    //        alien = pool[0];
    //        pool.Remove(alien);
    //        alien.transform.parent = null;
    //        alien.transform.position = position;

    //        foreach (Collider c in alien.GetComponents<Collider>())
    //        {
    //            c.enabled = true;
    //        }
    //    }
    //    else
    //    {
    //        alien = Instantiate(prefab, position, new Quaternion());
    //    }

    //    return alien;
    //}

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
    public override void Destroy(Alien alien)
    //public void DestroyAlien(Alien alien)
    {
        alien.Reset();
        AlienController.Instance.DeRegisterAlien(alien);
        base.Destroy(alien);
        //alien.transform.position = objectPool.position;
        //alien.transform.parent = objectPool;
        //pool.Add(alien);
    }
}
