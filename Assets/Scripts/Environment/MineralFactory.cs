using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for minerals.
/// </summary>
public class MineralFactory : Factory<MineralFactory, Mineral, ENone>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    //[SerializeField] private Mineral mineralPrefab;
    //[SerializeField] private int pooledMinerals;
    [Header("Mineral Stats")]
	[SerializeField] private int oreCount;
    [SerializeField] private bool destroySpentMinerals;

    //Non-Serialized Fields------------------------------------------------------------------------

    //private Transform objectPool;
    //private List<Mineral> minerals;
    private List<Mineral> despawningMinerals;
    private List<Mineral> despawnedMinerals;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    ///// <summary>
    ///// MineralFactory's singleton public property.
    ///// </summary>
    //public static MineralFactory Instance { get; protected set; }
	public int OreCount { get => oreCount;}

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	protected override void Awake()
    {
        //if (Instance != null)
        //{
        //    Debug.LogError("There should never be more than one BuildingFactory.");
        //}

        //Instance = this;
        //IdGenerator idGenerator = IdGenerator.Instance;
        //minerals = new List<Mineral>();
        base.Awake();
        despawningMinerals = new List<Mineral>();
        despawnedMinerals = new List<Mineral>();        
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected override void Start()
    {
        base.Start();

        foreach (Mineral m in pool[ENone.None])
        {
            m.DisableColliders();
        }
    }

    ///// <summary>
    ///// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    ///// Start() runs after Awake().
    ///// </summary>
    //private void Start()
    //{
    //    objectPool = ObjectPool.Instance.transform;

    //    for (int i = 0; i < pooledMinerals; i++)
    //    {
    //        minerals.Add(CreateMineral(true));
    //    }
    //}

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get a new mineral node.
    /// </summary>
    /// <param name="position">The position the mineral should be instantiated at.</param>
    /// <returns>A mineral node.</returns>
    public Mineral Get(Vector3 position)
    {
        return Get(ENone.None, position);
    }

    /// <summary>
    /// Get a new mineral node.
    /// </summary>
    /// <param name="type">The type of mineral to retrieve.</param>
    /// <param name="position">The position the mineral should be instantiated at.</param>
    /// <returns>A mineral node.</returns>
    public override Mineral Get(ENone type, Vector3 position)
    {
        Mineral mineral = base.Get(type, position);

        //if (minerals.Count > 0)
        //{
        //    mineral = minerals[0];
        //    minerals.RemoveAt(0);
        //    mineral.transform.parent = null;
        //    mineral.EnableColliders();
        //}
        //else
        //{
        //    mineral = CreateMineral(false);
        //}

        mineral.Id = IdGenerator.Instance.GetNextId();
        //mineral.transform.position = position;
        MapController.Instance.RegisterMineral(mineral);
        return mineral;
    }

    /// <summary>
    /// Custom modifications to a mineral after Get() retrieves it from the pool.
    /// </summary>
    /// <param name="mineral">The mineral being modified.</param>
    /// <returns>The modified mineral.</returns>
    protected override Mineral GetRetrievalSetup(Mineral mineral)
    {
        mineral.EnableColliders();
        return mineral;
    }

    ///// <summary>
    ///// Creates mineral nodes.
    ///// </summary>
    ///// <returns>A mineral node.</returns>
    //private Mineral CreateMineral(bool pooling)
    //{
    //    Mineral mineral = Instantiate(mineralPrefab);

    //    if (pooling)
    //    {
    //        mineral.transform.position = objectPool.transform.position;
    //        mineral.transform.parent = objectPool;
    //        mineral.DisableColliders();
    //    }

    //    return mineral;
    //}

    /// <summary>
    /// Destroy a mineral node.
    /// </summary>
    /// <param name="mineral">The mineral to destroy.</param>
    public void Destroy(Mineral mineral)
    {
        Destroy(ENone.None, mineral);
    }

    /// <summary>
    /// Destroy a mineral node.
    /// </summary>
    /// <param name="mineral">The mineral to destroy.</param>
    public override void Destroy(ENone type, Mineral mineral)
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
            for (int i = 0; i < despawningMinerals.Count; i++)
            {
                if (!despawningMinerals[i].Despawning)
                {
                    despawnedMinerals.Add(despawningMinerals[i]);
                    despawningMinerals.RemoveAt(i);
                    i--;
                }
            }

            if (despawnedMinerals.Count > 0)
            {
                do
                {
                    Mineral toDestroy = despawnedMinerals[0];
                    despawnedMinerals.RemoveAt(0);

                    if (destroySpentMinerals)
                    {
                        GameObject.Destroy(toDestroy.gameObject);
                    }
                    else
                    {
                        base.Destroy(ENone.None, toDestroy);
                    }
                }
                while (!destroySpentMinerals && despawnedMinerals.Count > 0);
            }

            yield return null;
        }        
    }
}
