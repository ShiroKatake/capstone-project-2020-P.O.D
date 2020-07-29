using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for minerals.
/// </summary>
public class MineralFactory : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private Mineral mineralPrefab;
    [SerializeField] private int pooledMinerals;
	[SerializeField] private int oreCount;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Transform objectPool;
    private List<Mineral> minerals;
    private List<Mineral> despawningMinerals;
    private List<Mineral> despawnedMinerals;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// MineralFactory's singleton public property.
    /// </summary>
    public static MineralFactory Instance { get; protected set; }
	public int OreCount { get => oreCount;}

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one BuildingFactory.");
        }

        Instance = this;
        IdGenerator idGenerator = IdGenerator.Instance;
        minerals = new List<Mineral>();
        despawningMinerals = new List<Mineral>();
        despawnedMinerals = new List<Mineral>();        
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        objectPool = ObjectPool.Instance.transform;

        for (int i = 0; i < pooledMinerals; i++)
        {
            minerals.Add(CreateMineral(true));
        }
    }

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get mineral nodes from BuildingFactory.
    /// </summary>
    /// <returns>A mineral node.</returns>
    public Mineral GetMineral(Vector3 position)
    {
        Mineral mineral;

        if (minerals.Count > 0)
        {
            mineral = minerals[0];
            minerals.RemoveAt(0);
            mineral.transform.parent = null;
            mineral.EnableColliders();
        }
        else
        {
            mineral = CreateMineral(false);
        }

        mineral.Id = IdGenerator.Instance.GetNextId();
        mineral.transform.position = position;
        MapController.Instance.RegisterMineral(mineral);
        return mineral;
    }

    /// <summary>
    /// Creates mineral nodes.
    /// </summary>
    /// <returns>A mineral node.</returns>
    private Mineral CreateMineral(bool pooling)
    {
        Mineral mineral = Instantiate(mineralPrefab);

        if (pooling)
        {
            mineral.transform.position = objectPool.transform.position;
            mineral.transform.parent = objectPool;
            mineral.DisableColliders();
        }

        return mineral;
    }

    /// <summary>
    /// Destroy a mineral node.
    /// </summary>
    public void DestroyMineral(Mineral mineral)
    {
        MapController.Instance.DeRegisterMineral(mineral);
        mineral.Reset();
        despawningMinerals.Add(mineral);

        if (despawningMinerals.Count == 1)
        {
            StartCoroutine(PoolDespawningMinerals());
        }
    }

    /// <summary>
    /// Transfers despawned minerals to the minerals pool when they're finished despawning.
    /// </summary>
    private IEnumerator PoolDespawningMinerals()
    {
        while (despawningMinerals.Count > 0)
        {
            foreach (Mineral m in despawningMinerals)
            {
                if (!m.Despawning)
                {
                    despawnedMinerals.Add(m);
                }
            }

            if (despawnedMinerals.Count > 0)
            {
                foreach (Mineral m in despawnedMinerals)
                {
                    despawningMinerals.Remove(m);
                }

                despawnedMinerals.Clear();
            }

            yield return null;
        }        
    }
}
