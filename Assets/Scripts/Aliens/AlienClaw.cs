using UnityEngine;

/// <summary>
/// An alien's melee weapon.
/// </summary>
public class AlienClaw : MonoBehaviour
{
	[SerializeField] private Actor alienActor;
	[SerializeField] private float damage;

    [Header("Testing")]
    [SerializeField] private bool debugging;

	/// <summary>
	/// When a GameObject collides with another GameObject, Unity calls OnTriggerEnter.
	/// Deal damage to any thing that is in the hit box at moment of enabling.
	/// </summary>
	/// <param name="collidedWith">The other Collider involved in this collision.</param>
	private void OnTriggerEnter(Collider collidedWith)
	{
        if (!collidedWith.isTrigger)
        {
            Health damageable = collidedWith.GetComponentInParent<Health>();    //Gets component in itself or its parent(s)

            if (damageable == null)
            {
                Debug.Log($"{this} cannot find {collidedWith.gameObject}'s Health component in it or its parent.");
            }
            else if (alienActor == null)
            {
                Debug.Log($"{this} is missing it's alien's Actor component.");
            }
            else
            {
                AudioManager.Instance.PlaySound(AudioManager.ESound.Damage_To_Building, this.gameObject);
                damageable.TakeDamage(damage, alienActor);
                gameObject.SetActive(false);

                UIColorManager.Instance.UITriggerAttackFlash();
            }
        }
        else if (debugging)
        {
            Debug.Log($"{this}.AlienClaw.OnTriggerEnter() ignoring trigger collider of {collidedWith.gameObject}");
        }
    }
}
