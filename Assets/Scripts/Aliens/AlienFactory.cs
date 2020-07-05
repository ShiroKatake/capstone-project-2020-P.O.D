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

    [Header("Stats")]
    [SerializeField] private int pooledAliens;
    [SerializeField] private float alienSpawnHeight;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Transform objectPoolParent;
    private List<Alien> alienPool;

    //PublicProperties-------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// AlienController's singleton public property.
    /// </summary>
    public static AlienFactory Instance { get; protected set; }

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
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more AlienFactories.");
        }

        Instance = this;
        alienPool = new List<Alien>();

        
    }

    private void Start() {
        objectPoolParent = ObjectPool.Instance.transform;

        for (int i = 0; i < pooledAliens; i++)
        {
            Alien alien = Instantiate(alienPrefab, objectPoolParent.position, new Quaternion()); 
            alien.transform.parent = objectPoolParent;

            foreach (Collider c in alien.GetComponents<Collider>())
            {
                c.enabled = false;
            }

            alienPool.Add(alien);
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

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

            foreach (Collider c in alien.GetComponents<Collider>())
            {
                c.enabled = true;
            }
        }
        else
        {
            alien = Instantiate(alienPrefab, position, new Quaternion());
        }

        alien.Setup(IdGenerator.Instance.GetNextId());
        return alien;
    }

    /// <summary>
    /// Handles the destruction of aliens.
    /// </summary>
    /// <param name="alien">The alien to be destroyed.</param>
    public void DestroyAlien(Alien alien)
    {
        alien.Reset();
        AlienController.Instance.DeRegisterAlien(alien);
        alien.transform.position = objectPoolParent.position;
        alien.transform.parent = objectPoolParent;
        alienPool.Add(alien);
    }
}
