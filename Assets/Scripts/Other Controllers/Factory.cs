﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An empty enum for factories with only one product prefab.
/// </summary>
public enum ENoEnum
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
    //Would use a single list of this, but Unity doesn't want to serialize two variable types together, it seems. Separate lists seems to be the only way it'll let me do it.
    ///// <summary>
    ///// A serializable key-value pair for prefabs and their enum values.
    ///// </summary>
    //[SerializeField]
    //public class ProductEnumPrefabPair
    //{
    //    //Private Fields-----------------------------------------------------------------------------------------------------------------------------

    //    //Serialized Fields------------------------------------------------------------------------

    //    [SerializeField] private ProductEnum productEnum;
    //    [SerializeField] private ProductType productPrefab;

    //    //Public Properties--------------------------------------------------------------------------------------------------------------------------

    //    //Basic Public Properties------------------------------------------------------------------

    //    /// <summary>
    //    /// The [ProductEnum] value of this prefab.
    //    /// </summary>
    //    public ProductEnum ProductEnum { get => productEnum; }

    //    /// <summary>
    //    /// The [ProductType] prefab for [ProductEnum].
    //    /// </summary>
    //    public ProductType ProductPrefab { get => productPrefab; }
    //}

    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] protected List<ProductEnum> productEnums;
    [SerializeField] protected List<ProductType> productPrefabs;

    //Non-Serialized Fields------------------------------------------------------------------------

    protected string id;
    protected Dictionary<ProductEnum, ProductType> prefabs;
    protected Dictionary<ProductEnum, List<ProductType>> pool;
    protected Transform objectPool;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (id == "")
        {
            Debug.LogError("Please set this factory's ID before calling Factory.Awake().");
        }

        if (productEnums.Count != productPrefabs.Count)
        {
            Debug.LogError($"{id}'s enum and prefab lists do not match in their lengths.");
        }

        pool = new Dictionary<ProductEnum, List<ProductType>>();
        prefabs = new Dictionary<ProductEnum, ProductType>();

        for (int i = 0; i < productEnums.Count; i++)
        {
            pool[productEnums[i]] = new List<ProductType>();
            prefabs[productEnums[i]] = productPrefabs[i];
        }
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
    /// Retrieves a [ProductType] from the pool if there's any available, instantiates a new one if none are available, then sets its position.
    /// </summary>
    /// <param name="type">The type of [ProductType] that you want to retrieve.</param>
    /// <param name="position">The position [ProductType] should be instantiated at.</param>
    /// <returns>A new instance of [ProductType].</returns>
    public virtual ProductType Get(ProductEnum type, Vector3 position)
    {
        ProductType result = Get(type);
        result.transform.position = position;
        result.transform.rotation = new Quaternion();
        return result;
    }

    /// <summary>
    /// Retrieves a [ProductType] from the pool if there's any available, and instantiates a new one if none are available.
    /// </summary>
    /// <param name="type">The type of [ProductType] that you want to retrieve.</param>
    /// <returns>A new instance of [ProductType].</returns>
    public virtual ProductType Get(ProductEnum type)
    {
        ProductType result;

        if (pool.ContainsKey(type) && pool[type].Count > 0)
        {
            result = pool[type][0];
            pool[type].Remove(result);
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
    /// <param name="position">The position [ProductType] should be instantiated at.</param>
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
            Debug.LogError($"{id} does not have a prefab of [ProductEnum] value {type}");
            return null;
        }
    }

    /// <summary>
    /// Handles the destruction of [ProductType]s.
    /// </summary>
    /// <param name="type">The type of the [ProductType] to be destroyed.</param>
    /// <param name="toDestroy">The [ProductType] to be destroyed.</param>
    public virtual void Destroy(ProductEnum type, ProductType toDestroy)
    {
        toDestroy.transform.position = objectPool.position;
        toDestroy.transform.parent = objectPool;

        if (pool.ContainsKey(type))
        {
            pool[type].Add(toDestroy);
        }
        else
        {
            Debug.LogError($"{id} does not have a list objects of [ProductEnum] value {type}.");
        }
    }
}