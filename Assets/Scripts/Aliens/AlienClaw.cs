using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienClaw : MonoBehaviour
{
	[SerializeField] private Actor alienActor;
	[SerializeField] private float damage;

	private void OnTriggerEnter(Collider collidedWith)
	{
		Health damageable = collidedWith.GetComponent<Health>();
		if (damageable != null)
		{
			damageable.TakeDamage(damage, alienActor);
			gameObject.SetActive(false);
			Debug.Log(damageable + " " + damageable.currentHealth);
		}
	}
}
