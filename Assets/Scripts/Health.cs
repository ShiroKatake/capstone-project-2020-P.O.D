using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A health component for anything that needs to track health, durability, etc.
/// </summary>
public class Health : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private float health;

    //Non-Serialized Fields------------------------------------------------------------------------

    private float startingHealth;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// How much health, durability, etc. something currently has.
    /// </summary>
    public float Value { get => health; set => health = value; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        startingHealth = health;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if health is 0 or less.
    /// </summary>
    /// <returns>Is the object this health class is a component of dead?</returns>
    public bool IsDead()
    {
        return health <= 0;
    }

    /// <summary>
    /// Destroys the game object Health is attached to.
    /// </summary>
    //public void Die()
    //{
    //    Destroy(gameObject);
    //}

    /// <summary>
    /// Resets health to its starting value.
    /// </summary>
    public void Reset()
    {
        health = startingHealth;
    }
}
