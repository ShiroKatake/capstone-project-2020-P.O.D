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
    [SerializeField] private float miningCooldown;
    [SerializeField] private float afkTimeout;
    [SerializeField] private float rotationSpeed;

    //Non-Serialized Fields------------------------------------------------------------------------

    private int initialCount;
    private int id;
    private List<Collider> colliders;
    private bool despawning;
    private float timeSpentMining;
    private float timeOfLastMining;
    private Vector3 rotationUpdate;

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
        rotationUpdate = new Vector3(0, rotationSpeed, 0);
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Allows the player to mine a mineral node for minerals.
    /// </summary>
    /// <returns>The number of minerals that the player has mined during this frame.</returns>
    public int Mine()
    {
        if (count > 0 && !despawning)
        {
            transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationUpdate * Time.deltaTime);

            if (Time.time - timeOfLastMining > afkTimeout)
            {
                timeSpentMining = 0;
            }
            else
            {
                timeSpentMining += Time.deltaTime;
            }

            timeOfLastMining = Time.time;

            if (timeSpentMining >= miningCooldown)
            {
                timeSpentMining -= miningCooldown;
                count--;

                if (count <= 0)
                {
                    MineralFactory.Instance.DestroyMineral(this);
                }

                return 1;
            }            
        }

        return 0;
    }

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
