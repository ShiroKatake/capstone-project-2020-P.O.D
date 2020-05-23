using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory class for aliens.
/// </summary>
public class AlienFactory : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Game Objects")]
    [SerializeField] private Alien alienPrefab;
    [SerializeField] private Transform alienPoolParent;

    [Header("Stats")]
    [SerializeField] private float alienHoverHeight;

    //Non-Serialized Fields------------------------------------------------------------------------

    private List<Alien> alienPool;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// AlienController's singleton public property.
    /// </summary>
    public static AlienFactory Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The height at which aliens hover.
    /// </summary>
    public float AlienHoverHeight { get => alienHoverHeight; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more AlienFactories.");
        }

        Instance = this;
        alienPool = new List<Alien>();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves Enemies from a pool if there's any available, and instantiates a new alien if there isn't one. Provides a random position within the accepted bounds.
    /// </summary>
    /// <returns>A new alien.</returns>
    public Alien GetAlien()
    {
        return GetAlien(MapController.Instance.RandomAlienSpawnablePos());
    }

    /// <summary>
    /// Retrieves Enemies from a pool if there's any available, and instantiates a new alien if there isn't one.
    /// </summary>
    /// <param name="position">The position the alien should be instantiated at.</param>
    /// <returns>A new alien.</returns>
    public Alien GetAlien(Vector3 position)
    {
        Alien alien;        

        if (alienPool.Count > 0)
        {
            alien = alienPool[0];
            alienPool.Remove(alien);
            alien.transform.parent = null;
            alien.transform.position = position;
        }
        else
        {
            alien = Instantiate(alienPrefab, position, new Quaternion());
        }

        alien.Setup(IdGenerator.Instance.GetNextId());
        alien.Moving = true;
        return alien;
    }

    /// <summary>
    /// Handles the destruction of aliens.
    /// </summary>
    /// <param name="alien">The alien to be destroyed.</param>
    public void DestroyAlien(Alien alien)
    {
        AlienController.Instance.DeRegisterAlien(alien);
        alienPool.Add(alien);
        alien.Moving = false;
        alien.transform.position = alienPoolParent.position;
        alien.transform.parent = alienPoolParent;
    }
}
