using UnityEngine;
using kTools.Decals;
using System.Collections.Generic;

public class TurretRangeFXFactory : MonoBehaviour
{
	////Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	////Serialized Fields----------------------------------------------------------------------------

	//[SerializeField] private DecalData turretRangeDecal;

	////Non-Serialized Fields------------------------------------------------------------------------

	//private Decal currentDecal;
	//private const float BASE_SPRITE_RADIUS = 2.1f;
	//private const float DECAL_HEIGHT = 4.5f;

	////Singleton Public Property--------------------------------------------------------------------                                                    

	///// <summary>
	///// MineralFactory's singleton public property.
	///// </summary>
	//public static TurretRangeFXFactory Instance { get; protected set; }

	////Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	///// <summary>
	///// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	///// Awake() runs before Start().
	///// </summary>
	//void Awake()
	//{
	//	if (Instance != null)
	//	{
	//		Debug.LogError("There should never be more than one TurretRangeFXFactory.");
	//	}

	//	Instance = this;
	//	IdGenerator idGenerator = IdGenerator.Instance;
	//}

	///// <summary>
	///// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
	///// Start() runs after Awake().
	///// </summary>
	//private void Start()
	//{
	//	BuildingFactory.Instance.onGetTurret += OnGetTurret;
	//}

	///// <summary>
	///// If the player is placing a turret, display the turret range.
	///// </summary
	//private void OnGetTurret(Transform turretTransform)
	//{
	//	float radius = turretTransform.GetComponent<TurretShooting>().DetectionRadius;
	//	currentDecal = DecalSystem.GetDecal(turretRangeDecal, new Vector3(0f, DECAL_HEIGHT, 0f), -turretTransform.up, turretTransform.localScale * radius * BASE_SPRITE_RADIUS);
	//	currentDecal.transform.SetParent(turretTransform, false);
	//}

	///// <summary>
	///// Hides the turret range.
	///// </summary
	//public void HideRange()
	//{
	//	if (currentDecal != null)
	//	{
	//		DecalSystem.RemoveDecal(currentDecal);
	//	}
	//}

	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Serialized Fields----------------------------------------------------------------------------                                                    

	[SerializeField] private GameObject turretRangeFX;
	[SerializeField] private int pooledFX;

	//Non-Serialized Fields------------------------------------------------------------------------

	private Transform objectPool;
	private Queue<GameObject> objects = new Queue<GameObject>();
	private const float BASE_SPRITE_RADIUS = 2.1f;
	private const float DECAL_HEIGHT = 4.5f;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Singleton Public Property--------------------------------------------------------------------                                                    

	/// <summary>
	/// TurretRangeFXFactory's singleton public property.
	/// </summary>
	public static TurretRangeFXFactory Instance { get; protected set; }

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There should never be more than one TurretRangeFXFactory.");
		}

		Instance = this;
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
	/// Generate an objectfrom the pool. If there's no object in the pool add one.
	/// </summary>
	public GameObject Get()
	{
		if (objects.Count == 0)
		{
			Add(1);
		}
		return objects.Dequeue();
	}

	/// <summary>
	/// Add object into the pool.
	/// </summary>
	private void Add(int count)
	{
		for (int i = 0; i < count; i++)
		{
			GameObject fx = Instantiate(turretRangeFX);
			fx.gameObject.SetActive(false);
			objects.Enqueue(fx);
		}
	}

	/// <summary>
	/// Return object back into the pool.
	/// </summary>
	public void ReturnToPool(GameObject fx)
	{
		if (fx != null)
		{
			fx.transform.SetParent(null);
			fx.gameObject.SetActive(false);
			objects.Enqueue(fx);
		}
	}

	/// <summary>
	/// If the player is placing a turret, display the turret range.
	/// </summary
	public void OnGetTurret(Transform turretTransform, GameObject currentFX)
	{
		float radius = turretTransform.GetComponent<TurretShooting>().DetectionRadius;

		currentFX.transform.position = new Vector3(0f, DECAL_HEIGHT, 0f);
		currentFX.transform.localScale = turretTransform.localScale * radius * BASE_SPRITE_RADIUS;
		currentFX.transform.SetParent(turretTransform, false);
		currentFX.SetActive(true);
	}
}
