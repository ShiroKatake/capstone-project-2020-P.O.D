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
    public float CurrentHealth {get => health;}

    public float MaxHealth { get => startingHealth; }


    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
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
    /// Resets health to its starting value.
    /// </summary>
    public void Reset()
    {
        health = startingHealth;
    }

    // var checking -----------------------------------------------------------------------------------------------------------------------------------

    public void ChangeHealthValue(float val){
        if (health + val > MaxHealth) {
            health = MaxHealth;
        } else if (health + val <= 0){
            health = 0;
        } else {
            health += val;
        }
    }
}
