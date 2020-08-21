using UnityEngine;
using kTools.Decals;
using System.Collections.Generic;

public class TurretRangeFXFactory : Factory<TurretRangeFXFactory, TurretRangeFX, ENone>
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	////Serialized Fields----------------------------------------------------------------------------                                                    

	//[SerializeField] private GameObject turretRangeFX;
	//[SerializeField] private int pooledFX;

	//Non-Serialized Fields------------------------------------------------------------------------

	//private Transform objectPool;
	//private Queue<GameObject> objects = new Queue<GameObject>();
	private const float BASE_SPRITE_RADIUS = 2.1f;
	private const float DECAL_HEIGHT = 4.5f;

    ////Public Properties------------------------------------------------------------------------------------------------------------------------------

    ////Singleton Public Property--------------------------------------------------------------------                                                    

    ///// <summary>
    ///// TurretRangeFXFactory's singleton public property.
    ///// </summary>
    //public static TurretRangeFXFactory Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

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

    //	Add(pooledFX);
    //}

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected override void Start()
    {
        base.Start();

        foreach (TurretRangeFX fx in pool[ENone.None])
        {
            fx.gameObject.SetActive(false);
        }

        //objectPool = ObjectPool.Instance.transform;
    }

    //Triggered Methods -----------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves a TurretRangeFX from the pool if there's any available, or instantiates a new one if none are available.
    /// </summary>
    /// <returns>A new instance of TurretRangeFX.</returns>
    public TurretRangeFX Get()
    {
        return base.Get(ENone.None);
    }

    //   /// <summary>
    //   /// Generate an objectfrom the pool. If there's no object in the pool add one.
    //   /// </summary>
    //   public GameObject Get()
    //{
    //	if (objects.Count == 0)
    //	{
    //		Add(1);
    //	}
    //	return objects.Dequeue();
    //}

    ///// <summary>
    ///// Add object into the pool.
    ///// </summary>
    //private void Add(int count)
    //{
    //	for (int i = 0; i < count; i++)
    //	{
    //		GameObject fx = Instantiate(turretRangeFX);
    //		fx.gameObject.SetActive(false);
    //		objects.Enqueue(fx);
    //	}
    //}

    /// <summary>
    /// Handles the destruction of TurretRangeFXs.
    /// </summary>
    /// <param name="fx">The TurretRangeFX to be destroyed.</param>
    public void Destroy(TurretRangeFX fx)
    {
        Destroy(ENone.None, fx);
    }

    /// <summary>
    /// Handles the destruction of TurretRangeFXs.
    /// </summary>
    /// <param name="type">The type of TurretRangeFX to be destroyed.</param>
    /// <param name="fx">The TurretRangeFX to be destroyed.</param>
    public override void Destroy(ENone type, TurretRangeFX fx)
    {
        fx.gameObject.SetActive(false);
        base.Destroy(type, fx);
    }

	///// <summary>
	///// Return object back into the pool.
	///// </summary>
	//public void ReturnToPool(GameObject fx)
	//{
	//	if (fx != null)
	//	{
	//		fx.transform.SetParent(null);
	//		fx.gameObject.SetActive(false);
	//		objects.Enqueue(fx);
	//	}
	//}

	/// <summary>
	/// If the player is placing a turret, display the turret range.
	/// </summary
	public void OnGetTurret(Transform turretTransform, TurretRangeFX currentFX)
	{
		float radius = turretTransform.GetComponent<TurretShooting>().DetectionRadius;

		currentFX.transform.position = new Vector3(0f, DECAL_HEIGHT, 0f);
		currentFX.transform.localScale = turretTransform.localScale * radius * BASE_SPRITE_RADIUS;
		currentFX.transform.SetParent(turretTransform, false);
		currentFX.gameObject.SetActive(true);
	}
}
