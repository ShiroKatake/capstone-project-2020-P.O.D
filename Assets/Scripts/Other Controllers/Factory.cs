using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An empty enum for factories with only one product prefab.
/// </summary>
public enum ENone
{
    None
}

/// <summary>
/// A base class for factories.
/// </summary>
/// <typeparam name="FactoryType">The type of the factory.</typeparam>
/// <typeparam name="ProductType">The type of the product that the factory makes.</typeparam>
/// <typeparam name="ProductEnum">The enum of the product that the factory makes.</typeparam>
public class Factory<FactoryType, ProductType, ProductEnum> : SerializableSingleton<FactoryType>
    where FactoryType : Factory<FactoryType, ProductType, ProductEnum>, new()
    where ProductType : MonoBehaviour
    where ProductEnum : Enum
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Pooling")]
    [SerializeField] protected bool deactivateGameObjectInPool;
    [SerializeField] protected List<ProductEnum> productEnums;
    [SerializeField] protected List<ProductType> productPrefabs;
    [SerializeField] protected List<int> productQuantities;

    //Non-Serialized Fields------------------------------------------------------------------------

    protected Dictionary<ProductEnum, ProductType> prefabs;
    protected Dictionary<ProductEnum, List<ProductType>> pool;
    protected Transform objectPool;
    protected List<(ProductEnum, ProductType)> toPool;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (productEnums.Count != productPrefabs.Count || productEnums.Count != productQuantities.Count || productPrefabs.Count != productQuantities.Count)
        {
            Debug.LogError($"The lengths of {this}'s enum, prefab and quantity lists do not match.");
        }

        pool = new Dictionary<ProductEnum, List<ProductType>>();
        prefabs = new Dictionary<ProductEnum, ProductType>();
        toPool = new List<(ProductEnum, ProductType)>();
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    protected virtual void Start()
    {
        objectPool = ObjectPool.Instance.transform;

        for (int i = 0; i < productEnums.Count; i++)
        {
            pool[productEnums[i]] = new List<ProductType>();
            prefabs[productEnums[i]] = productPrefabs[i];

            for (int j = 0; j < productQuantities[i]; j++)
            {
                Destroy(Create(productEnums[i]), productEnums[i]);
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves a [ProductType] from the pool if there's any available, instantiates a new one if none are available, then sets its position.
    /// Note: if overloaded by a child class rather than overridden, it'll usually be better to use the overload than this unless calling from within the overloaded method.
    /// </summary>
    /// <param name="position">The position [ProductType] should be instantiated at.</param>
    /// <param name="type">The type of [ProductType] that you want to retrieve.</param>
    /// <returns>A new instance of [ProductType].</returns>
    public virtual ProductType Get(Vector3 position, ProductEnum type)
    {
        ProductType result = Get(type);
        result.transform.position = position;
        result.transform.rotation = new Quaternion();
        return result;
    }

    /// <summary>
    /// Retrieves a [ProductType] from the pool if there's any available, and instantiates a new one if none are available.
    /// Note: if overloaded by a child class rather than overridden, it'll usually be better to use the overload than this unless calling from within the overloaded method.
    /// </summary>
    /// <param name="type">The type of [ProductType] that you want to retrieve.</param>
    /// <returns>A new instance of [ProductType].</returns>
    public virtual ProductType Get(ProductEnum type)
    {
        ProductType result;

        if (pool.ContainsKey(type) && pool[type].Count > 0)
        {
            result = pool[type][0];
            pool[type].RemoveAt(0);

            if (deactivateGameObjectInPool)
            {
                result.gameObject.SetActive(true);
            }

            result.transform.parent = null;
            result = GetRetrievalSetup(result);
        }
        else
        {
            result = Create(type);
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
    /// <returns>A building of the specified type.</returns>
    protected virtual ProductType Create(ProductEnum type)
    {
        if (prefabs.ContainsKey(type))
        {
            ProductType result = Instantiate(prefabs[type]);
            return result;
        }
        else
        {
            Debug.LogError($"{this} does not have a prefab of [ProductEnum] value {type}");
            return null;
        }
    }

    /// <summary>
    /// Handles the destruction of [ProductType]s.
    /// Note: if overloaded by a child class rather than overridden, it'll usually be better to use the overload than this unless calling from within the overloaded method.
    /// </summary>
    /// <param name="toDestroy">The [ProductType] to be destroyed.</param>
    /// <param name="type">The type of the [ProductType] to be destroyed.</param>
    public virtual void Destroy(ProductType toDestroy, ProductEnum type)
    {
        toDestroy.transform.position = objectPool.position;
        toDestroy.transform.parent = objectPool;

        if (deactivateGameObjectInPool)
        {
            toDestroy.gameObject.SetActive(false);
        }

        toPool.Add((type, toDestroy));
        
        if (toPool.Count == 1)
        {
            StartCoroutine(Pooling());
        }
    }

    /// <summary>
    /// Pools [ProductType]s at the Factory's own pace, so they're not retrieved from the pool before the [ProductType] is ready.
    /// </summary>
    private IEnumerator Pooling()
    {
        while (toPool.Count > 0)
        {
            yield return null;
            ProductEnum e = toPool[0].Item1;
            ProductType p = toPool[0].Item2;
            toPool.RemoveAt(0);
            PoolNextItem(p, e);            
        }
    }

    /// <summary>
    /// Pools a [ProductType] that is ready to be pooled.
    /// </summary>
    /// <param name="toPool">The [ProductType] being pooled.</param>
    /// <param name="type">The type of the [ProductType] being pooled.</param>
    protected virtual void PoolNextItem(ProductType toPool, ProductEnum type)
    {
        if (pool.ContainsKey(type))
        {
            pool[type].Add(toPool);
        }
        else
        {
            Debug.LogError($"{this} does not have a list objects of [ProductEnum] value {type}. Destroying {toPool}.");
            GameObject.Destroy(toPool.gameObject);
        }
    }
}
