using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// A health component for anything that needs to track health, durability, etc.
/// </summary>
public class Health : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[SerializeField] private float maxHealth;

	//Non-Serialized Fields------------------------------------------------------------------------
	private Actor actor;
	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------
	public UnityAction<float, Transform> onDamaged;
	public UnityAction<float> onHealed;
	public UnityAction onDie;

	public float currentHealth { get; set; }

	/// <summary>
	/// How much health, durability, etc. something currently has.
	/// </summary>
	public float Value
    {
        get => currentHealth; //set => health = value;

        set
        {
			currentHealth = value;
            //Debug.Log($"{gameObject.name}'s health updated to {health}");
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
		currentHealth = maxHealth;
		actor = GetComponent<Actor>();
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Damage the object and trigger things that would happen when the object gets damaged.
	/// </summary>
	public void TakeDamage(float amount, Actor attackerActor)
	{
		//Friendly fire OFF: If the attacker does not have the same tag as this object, take damage from the attacker
		if (actor.Affiliation != attackerActor.Affiliation)
		{
			float healthBefore = currentHealth;
			currentHealth -= amount;
			currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

			//If the object has taken damage, do damage related stuffs (ie. animations, sounds, etc)
			float trueDamageAmount = healthBefore - currentHealth;
			if (trueDamageAmount > 0f && onDamaged != null)
			{
				onDamaged.Invoke(trueDamageAmount, attackerActor.transform);
			}

			HandleDeath();
		}
	}

	/// <summary>
	/// Heal the object and trigger things that would happen when the object gets healed.
	/// </summary>
	public void Heal(float amount)
	{
		float healthBefore = currentHealth;
		currentHealth += amount;
		currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

		//If the object has been healed, do heal related stuffs (ie. animations, sounds, etc)
		float trueHealAmount = currentHealth - healthBefore;
		if (trueHealAmount > 0f && onHealed != null)
		{
			onHealed.Invoke(trueHealAmount);
		}
	}

	/// <summary>
	/// Trigger things that would happen when the obejct dies.
	/// </summary>
	public void HandleDeath()
	{
		if (IsDead() && onDie != null)
		{
			onDie?.Invoke();
		}
	}

    /// <summary>
    /// Checks if health is 0 or less.
    /// </summary>
    /// <returns>Is the object this health class is a component of dead?</returns>
    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    /// <summary>
    /// Resets health to its starting value.
    /// </summary>
    public void Reset()
    {
		currentHealth = maxHealth;
    }
}
