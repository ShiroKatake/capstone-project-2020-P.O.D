using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class for ores.
/// </summary>
public class OreFactory : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Serialized Fields----------------------------------------------------------------------------                                                    

	[SerializeField] private Ore orePrefab;
	[SerializeField] private int pooledOres;

	//Non-Serialized Fields------------------------------------------------------------------------

	private Transform objectPool;
	private Queue<Ore> ores = new Queue<Ore>();

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Singleton Public Property--------------------------------------------------------------------                                                    

	/// <summary>
	/// MineralFactory's singleton public property.
	/// </summary>
	public static OreFactory Instance { get; protected set; }

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	void Awake()
    {
		if (Instance != null)
		{
			Debug.LogError("There should never be more than one OreFactory.");
		}

		Instance = this;
		objectPool = ObjectPool.Instance.transform;
		IdGenerator idGenerator = IdGenerator.Instance;

		AddOres(pooledOres);
	}

	//Triggered Methods -----------------------------------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Generate a mineral ore from the pool. If there's no ores in the pool add one.
	/// </summary>
	public Ore Get()
	{
		if (ores.Count == 0)
		{
			AddOres(1);
		}
		return ores.Dequeue();
	}

	/// <summary>
	/// Add ores into the pool.
	/// </summary>
	private void AddOres(int count)
	{
		for (int i = 0; i < count; i++)
		{
			Ore ore = Instantiate(orePrefab);
			ore.gameObject.SetActive(false);
			ores.Enqueue(ore);
		}
	}

	/// <summary>
	/// Return an ore back into the pool.
	/// </summary>
	public void ReturnToPool(Ore ore)
	{
		ore.gameObject.SetActive(false);
		ores.Enqueue(ore);
	}
}
