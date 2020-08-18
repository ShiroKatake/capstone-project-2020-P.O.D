using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for FX when buildings finished constructing.
/// </summary>
public class FinishedFXFactory : SerializableSingleton<FinishedFXFactory>
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Serialized Fields----------------------------------------------------------------------------                                                    

	[SerializeField] private GameObject constructionFinishedFX;
	[SerializeField] private int pooledFX;

	//Non-Serialized Fields------------------------------------------------------------------------

	private Transform objectPool;
	private Queue<GameObject> finishedFXs = new Queue<GameObject>();

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Singleton Public Property--------------------------------------------------------------------                                                    

	///// <summary>
	///// MineralFactory's singleton public property.
	///// </summary>
	//public static FinishedFXFactory Instance { get; protected set; }

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	protected override void Awake()
	{
        //if (Instance != null)
        //{
        //	Debug.LogError("There should never be more than one OreFactory.");
        //}

        //Instance = this;
        base.Awake();
		IdGenerator idGenerator = IdGenerator.Instance;

		Add(pooledFX);
	}

	/// <summary>
	/// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
	/// Start() runs after Awake().
	/// </summary>
	private void Start()
	{
		objectPool = ObjectPool.Instance.transform;
	}

	//Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Generate a mineral ore from the pool. If there's no ores in the pool add one.
	/// </summary>
	public GameObject Get()
	{
		if (finishedFXs.Count == 0)
		{
			Add(1);
		}
		return finishedFXs.Dequeue();
	}

	/// <summary>
	/// Add ores into the pool.
	/// </summary>
	private void Add(int count)
	{
		for (int i = 0; i < count; i++)
		{
			GameObject fx = Instantiate(constructionFinishedFX);
			fx.gameObject.SetActive(false);
			finishedFXs.Enqueue(fx);
		}
	}

	/// <summary>
	/// Return an ore back into the pool.
	/// </summary>
	public void ReturnToPool(GameObject fx)
	{
		fx.gameObject.SetActive(false);
		finishedFXs.Enqueue(fx);
	}
}
