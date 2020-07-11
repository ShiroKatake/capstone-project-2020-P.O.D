using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienFX : MonoBehaviour
{
	[SerializeField] private Renderer alienRenderer;
	[SerializeField] private Color onDamagedFlashColor;
	[SerializeField] private float flashSpeed;

	private Animator animator;
	private Alien alien;
	private Color materialFlashColor;
	private Material material;

	private const string ANIMATOR_DIE = "Die";
	private const string ANIMATOR_ATTACK = "Attack";
	private const string ANIMATOR_DAMAGED = "Damaged";
	private const string MATERIAL_FLASH = "_FlashColor";

	void Awake()
	{
		animator = GetComponent<Animator>();
		alien = GetComponent<Alien>();

		alien.onDie += OnDie;
		alien.onDamaged += OnDamaged;
		alien.onAttack += OnAttack;

		materialFlashColor = new Color(1, 0, 0, 0);
		material = alienRenderer.material;
	}

	private void Update()
	{
		if (materialFlashColor.a > 0)
		{
			materialFlashColor.a = Mathf.Clamp01(materialFlashColor.a - flashSpeed * Time.deltaTime);
			material.SetColor("_FlashColor", materialFlashColor);
			//Debug.Log("Flash");
		}
	}

	private void OnDamaged()
	{
		AudioManager.Instance.PlaySound(AudioManager.ESound.Alien_Takes_Damage, this.gameObject);
		SetFlashColor(onDamagedFlashColor);
		animator.SetTrigger(ANIMATOR_DAMAGED);
		//Debug.Log("Damaged");
	}

	private void OnAttack()
	{
		AudioManager.Instance.PlaySound(AudioManager.ESound.Damage_To_Building, this.gameObject);
		animator.SetTrigger(ANIMATOR_ATTACK);
		//Debug.Log("Attack");
	}

	private void OnDie()
	{
		AudioManager.Instance.PlaySound(AudioManager.ESound.Alien_Dies, this.gameObject);
		animator.SetTrigger(ANIMATOR_DIE);
		//Debug.Log("Die");
	}

	private void SetFlashColor(Color color)
	{
		alienRenderer.material = material;
		materialFlashColor = color;
		material.SetColor("_FlashColor", materialFlashColor);
	}
}
