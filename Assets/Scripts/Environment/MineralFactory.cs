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

    [Header("Mineral Stats")]
	[SerializeField] private int oreCount;
    [SerializeField] private bool destroySpentMinerals;

    //Non-Serialized Fields------------------------------------------------------------------------

    private List<Mineral> despawningMinerals;
    private List<Mineral> despawnedMinerals;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// How much ore should a single mineral node yield in total.
    /// </summary>
	public int OreCount { get => oreCount;}

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	protected override void Awake()
    {
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
            m.SetCollidersEnabled(false);
            m.SetMeshRenderersEnabled(false);
        }
    }

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Get a new mineral node.
    /// </summary>
    /// <param name="position">The position the mineral should be instantiated at.</param>
    /// <param name="type">The type of mineral to retrieve. Should be left as default value of ENone.None.</param>
    /// <returns>A mineral node.</returns>
    public override Mineral Get(Vector3 position, ENone type = ENone.None)
    {
        Mineral mineral = base.Get(position, type);
        mineral.Id = IdGenerator.Instance.GetNextId();
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
        mineral.SetMeshRenderersEnabled(true);
        mineral.SetCollidersEnabled(true);
        return mineral;
    }

    /// <summary>
    /// Destroy a mineral node.
    /// </summary>
    /// <param name="mineral">The mineral to destroy.</param>
    /// <param name="type">The type of mineral to destroy. Should be left as default value of ENone.None.</param>
    public override void Destroy(Mineral mineral, ENone type = ENone.None)
    {
        MapController.Instance.DeRegisterMineral(mineral);
        base.Destroy(mineral, type);
    }

    /// <summary>
    /// Transfers despawned minerals to the minerals pool when they're finished despawning.
    /// </summary>
    /// <param name="toPool">The mineral to be pooled.</param> 
    /// <param name="type">The type of mineral.</param>
    protected override void PoolNextItem(Mineral toPool, ENone type)
    {        
        if (destroySpentMinerals)
        {
            GameObject.Destroy(toPool.gameObject);
        }
        else
        {
            toPool.Reset();
            base.PoolNextItem(toPool, type);
        }  
    }
}
