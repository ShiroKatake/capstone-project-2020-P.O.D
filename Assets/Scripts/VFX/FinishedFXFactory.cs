using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for FX when buildings finished constructing.
/// </summary>
public class FinishedFXFactory : Factory<FinishedFXFactory, FinishedFX, ENone>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    //[SerializeField] private GameObject constructionFinishedFX;
    //[SerializeField] private int pooledFX;

    //Non-Serialized Fields------------------------------------------------------------------------

    //private Transform objectPool;
    //private Queue<GameObject> finishedFXs = new Queue<GameObject>();

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    ///// Awake() runs before Start().
    ///// </summary>
    //protected override void Awake()
    //{
    //       base.Awake();
    //	//IdGenerator idGenerator = IdGenerator.Instance;
    //	//Add(pooledFX);
    //}

    ///// <summary>
    ///// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    ///// Start() runs after Awake().
    ///// </summary>
    //private void Start()
    //{
    //	objectPool = ObjectPool.Instance.transform;
    //}

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// Generate a mineral ore from the pool. If there's no ores in the pool add one.
    ///// </summary>
    //public GameObject Get()
    //{
    //	if (finishedFXs.Count == 0)
    //	{
    //		Add(1);
    //	}
    //	return finishedFXs.Dequeue();
    //}


    /// <summary>
    /// Retrieves a FinishedFX from the pool if there's any available, instantiates a new one if none are available, then sets its position.
    /// </summary>
    /// <param name="position">The position FinishedFX should be instantiated at.</param>
    /// <returns>A new instance of FinishedFX.</returns>
    public FinishedFX Get(Vector3 position)
    {
        FinishedFX fx = base.Get(ENone.None);
        fx.transform.position = position;//base.Get(ENone.None, position) adjusts the rotation weirdly, which is not what we want, so just applying the position here instead.
        return fx;
    }

    /// <summary>
    /// Creates a new FinishedFX.
    /// </summary>
    /// <returns>A building of the specified type.</returns>
    protected override FinishedFX Create(ENone type)
    {
        FinishedFX fx = base.Create(type);
        fx.gameObject.SetActive(false);
        return fx;
    }

    ///// <summary>
    ///// Add ores into the pool.
    ///// </summary>
    //private void Add(int count)
    //{
    //	for (int i = 0; i < count; i++)
    //	{
    //		GameObject fx = Instantiate(constructionFinishedFX);
    //		fx.gameObject.SetActive(false);
    //		finishedFXs.Enqueue(fx);
    //	}
    //}

    ///// <summary>
    ///// Return an ore back into the pool.
    ///// </summary>
    //public void ReturnToPool(GameObject fx)
    //{
    //	fx.gameObject.SetActive(false);
    //	finishedFXs.Enqueue(fx);
    //}

    /// <summary>
    /// Handles the destruction of FinishedFXs.
    /// </summary>
    /// <param name="fx">The FinishedFX to be destroyed.</param>
    public void Destroy(FinishedFX fx)
    {
        Destroy(ENone.None, fx);
    }

    /// <summary>
    /// Handles the destruction of FinishedFXs.
    /// </summary>
    /// <param name="type">The type of the FinishedFX to be destroyed.</param>
    /// <param name="fx">The FinishedFX to be destroyed.</param>
    public override void Destroy(ENone type, FinishedFX fx)
    {
        fx.gameObject.SetActive(false);
        base.Destroy(type, fx);
    }
}
