using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for factories.
/// </summary>
/// <typeparam name="FactoryType">The type of the factory.</typeparam>
/// <typeparam name="ProductType">The type of the product that the factory makes.</typeparam>
public class Factory<FactoryType, ProductType> : SerializableSingleton<FactoryType> 
    where FactoryType : Factory<FactoryType, ProductType>, new()
    where ProductType : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] protected ProductType prefab;

    //Non-Serialized Fields------------------------------------------------------------------------

    protected List<ProductType> pool;
    protected Transform objectPool;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        pool = new List<ProductType>();
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected virtual void Start()
    {
        objectPool = ObjectPool.Instance.transform;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves a [ProductType] from the pool if there's any available, and instantiates a new one if none are available.
    /// </summary>
    /// <param name="position">The position [ProductType] should be instantiated at.</param>
    /// <returns>A new instance of [ProductType].</returns>
    public virtual ProductType Get(Vector3 position)
    {
        ProductType result;

        if (pool.Count > 0)
        {
            result = pool[0];
            pool.Remove(result);
            result.transform.parent = null;
            result.transform.position = position;
            result = GetRetrievalSetup(result);
        }
        else
        {
            result = Create(position);
        }

        return result;
    }

    /// <summary>
    /// Custom modifications to a [ProductType] after Get() retrieves it from the pool.
    /// </summary>
    /// <param name="result">The [ProductType] being modified.</param>
    /// <returns>The modified [ProductType]</returns>
    protected virtual ProductType GetRetrievalSetup(ProductType result)
    {
        return result;
    }

    /// <summary>
    /// Creates a new [ProductType].
    /// </summary>
    /// <param name="position">The position [ProductType] should be instantiated at.</param>
    /// <returns>A building of the specified type.</returns>
    protected virtual ProductType Create(Vector3 position)
    {
        ProductType result = Instantiate(prefab, position, new Quaternion());
        return result;
    }

    /// <summary>
    /// Handles the destruction of [ProductType]s.
    /// </summary>
    /// <param name="toDestroy">The [ProductType] to be destroyed.</param>
    public virtual void Destroy(ProductType toDestroy)
    {
        toDestroy.transform.position = objectPool.position;
        toDestroy.transform.parent = objectPool;
        pool.Add(toDestroy);
    }
}
