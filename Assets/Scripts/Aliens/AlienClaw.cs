using UnityEngine;

/// <summary>
/// An alien's melee weapon.
/// </summary>
public class AlienClaw : MonoBehaviour
{
	[SerializeField] private Actor alienActor;
	[SerializeField] private float damage;

	/// <summary>
	/// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
	/// Deal damage to any thing that is in the hit box at moment of enabling.
	/// </summary>
	/// <param name="collidedWith">The other Collider involved in this collision.</param>
	private void OnTriggerEnter(Collider collidedWith)
	{
		Health damageable = collidedWith.GetComponent<Health>();
		if (damageable != null)
		{
			damageable.TakeDamage(damage, alienActor);
			gameObject.SetActive(false);
			//Debug.Log(damageable + " " + damageable.CurrentHealth);
		}
	}
}
