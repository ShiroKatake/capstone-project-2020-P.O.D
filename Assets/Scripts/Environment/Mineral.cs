using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Nodes of minerals for the player to collect for building.
/// </summary>
public class Mineral : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private int count;

    //Non-Serialized Fields------------------------------------------------------------------------

    private int initialCount;
    private int id;
    private List<Collider> colliders;
    private bool despawning;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Whether the mineral node is in the process of despawning or not.
    /// </summary>
    public bool Despawning { get => despawning; }

    //Complex Public Properties----------------------------------------------------------------------

    /// <summary>
    /// How many minerals are remaining in this mineral node.
    /// </summary>
    public int Count
    {
        get
        {
            return count;
        }

        set
        {
            if (count > 0)
            {
                count = value;

                if (count <= 0)
                {
                    MineralFactory.Instance.DestroyMineral(this);
                }
            }
        }
    }

    /// <summary>
    /// The Mineral node's unique ID number. Id should only be set by MineralFactory.GetMineral().
    /// </summary>
    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
            gameObject.name = $"Mineral {id}";
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        initialCount = count;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Enables the mineral node's colliders
    /// </summary>
    public void EnableColliders()
    {
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
    }

    /// <summary>
    /// Resets the mineral node to its default state.
    /// </summary>
    public void Reset()
    {
        DisableColliders();
        count = initialCount;
        StartCoroutine(DespawnMineral());
    }

    /// <summary>
    /// Disables the mineral node's colliders
    /// </summary>
    public void DisableColliders()
    {
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
    }

    /// <summary>
    /// Handles the visual dissapation of the mineral
    /// </summary>
    /// <returns></returns>
    private IEnumerator DespawnMineral()
    {
        despawning = true;

        while (transform.position.y > -1.1)
        {
            transform.position += new Vector3(0, -1 * Time.deltaTime, 0);//TODO: swap for de-spawn shader/animation
            yield return null;
        }

        transform.position = ObjectPool.Instance.transform.position;
        transform.parent = ObjectPool.Instance.transform;
        despawning = false;
    }
}
