using UnityEngine;

/// <summary>
/// Class for doing SFX, VFX and Animations for aliens.
/// </summary>
public class AlienFX : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Serialized Fields----------------------------------------------------------------------------

	[SerializeField] private Renderer alienRenderer;
	[SerializeField] private Color onDamagedFlashColor;
	[SerializeField] private float flashSpeed;

	//Non-Serialized Fields------------------------------------------------------------------------  

	private Animator animator;
	private Alien alien;
	private Color materialFlashColor;
	private Material material;

	private const string ANIMATOR_DIE = "Die";
	private const string ANIMATOR_ATTACK = "Attack";
	private const string ANIMATOR_DAMAGED = "Damaged";
	private const string MATERIAL_FLASH = "_FlashColor";

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// </summary>
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

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	private void Update()
	{
		if (materialFlashColor.a > 0)
		{
			materialFlashColor.a = Mathf.Clamp01(materialFlashColor.a - flashSpeed * Time.deltaTime);
			material.SetColor("_FlashColor", materialFlashColor);
			//Debug.Log("Flash");
		}
	}

	/// <summary>
	/// OnDamaged() is called whenever it receives a message from Alien.cs saying it's taken damage.
	/// </summary>
	private void OnDamaged()
	{
		AudioManager.Instance.PlaySound(AudioManager.ESound.Alien_Takes_Damage, this.gameObject);
		SetFlashColor(onDamagedFlashColor);
		animator.SetTrigger(ANIMATOR_DAMAGED);
		//Debug.Log("Damaged");
	}

	/// <summary>
	/// OnAttack() is called whenever it receives a message from Alien.cs saying it needs to deal damage.
	/// </summary>
	private void OnAttack()
	{
		AudioManager.Instance.PlaySound(AudioManager.ESound.Damage_To_Building, this.gameObject);
		animator.SetTrigger(ANIMATOR_ATTACK);
		//Debug.Log("Attack");
	}

	/// <summary>
	/// OnDie() is called whenever it receives a message from Alien.cs saying it has reached 0 hp.
	/// </summary>
	private void OnDie()
	{
		AudioManager.Instance.PlaySound(AudioManager.ESound.Alien_Dies, this.gameObject);
		animator.SetTrigger(ANIMATOR_DIE);
		//Debug.Log("Die");
	}

	/// <summary>
	/// Set the flash color of the alien's material to start the flashing FX.
	/// </summary>
	private void SetFlashColor(Color color)
	{
		alienRenderer.material = material;
		materialFlashColor = color;
		material.SetColor("_FlashColor", materialFlashColor);
	}
}
