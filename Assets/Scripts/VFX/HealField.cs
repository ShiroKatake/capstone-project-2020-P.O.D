using UnityEngine;

/// <summary>
/// A controller for the healing FX.
/// </summary>
public class HealField : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------  

	//Serialized Fields----------------------------------------------------------------------------                                                    

	[SerializeField] private ParticleSystem healFieldFX;
	[SerializeField] private ParticleSystem healCircleFX;
	[SerializeField] private ParticleSystem healSparksFX;

	//Non-Serialized Fields------------------------------------------------------------------------                                                    

	private POD player;

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	///// <summary>
	///// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	///// Awake() runs before Start().
	///// </summary>
	//private void Awake()
	//{
		
	//}

	/// <summary>
	/// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
	/// Start() runs after Awake().
	/// </summary>
	private void Start()
	{
        player = FindPlayer();

        if (player != null)
        {
            player.HealthController.onPlayerHeal += ActivateHealingFX;
            player.HealthController.onPlayerHealCancelled += DeactivateHealingFX;
        }

        InitializeHealingFieldSize();
	}

	//Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	private void Update()
	{
		//If the player respawns, we'll need to find the player again
		if (player == null)
		{
			player = FindPlayer();

			if (player != null)
			{
				player.HealthController.onPlayerHeal += ActivateHealingFX;
				player.HealthController.onPlayerHealCancelled += DeactivateHealingFX;
			}
		}
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Finds the player and subscribe to the healing events to trigger healing FX.
	/// </summary>
	private POD FindPlayer()
	{
        //Find the Player
        POD player = FindObjectOfType<POD>();
		if (player == null)
			Debug.Log("CryoEgg can't find the player . . .");
		return player;
	}

	/// <summary>
	/// Plays the Healing FX (along with its children FXs).
	/// </summary>
	private void ActivateHealingFX()
	{
		if (healFieldFX.isStopped)
			healFieldFX.Play(true);
	}

	/// <summary>
	/// Stops the Healing FX (along with its children FXs).
	/// </summary>
	private void DeactivateHealingFX()
	{
		if (healFieldFX.isPlaying)
			healFieldFX.Stop(true);
	}

	/// <summary>
	/// Set the scale of the FX to be relative to the Cryo Egg and set the size according to the player healing range.
	/// </summary>
	private void InitializeHealingFieldSize()
	{
		//Make sure the scale is relative to the Cryo Egg
		transform.localScale = new Vector3(
			1 / Tower.Instance.transform.localScale.x,
			1 / Tower.Instance.transform.localScale.y,
			1 / Tower.Instance.transform.localScale.z
		);

		//Then set the start size according to the player's healing interaction range from the Cryo Egg
		SetHealingFXRadius(healFieldFX, player.HealthController.HealingRange);
		SetHealingFXStartSize(healCircleFX, player.HealthController.HealingRange);
		SetHealingFXRadius(healSparksFX, player.HealthController.HealingRange);
	}

	/// <summary>
	/// Set the radius of the FX according to the player healing range.
	/// </summary>
	private void SetHealingFXRadius(ParticleSystem particleSystem, float range)
	{
		ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
		shapeModule.radius = range;
	}

	/// <summary>
	/// Set the start size of the FX according to the player healing range.
	/// </summary>
	private void SetHealingFXStartSize(ParticleSystem particleSystem, float range)
	{
		ParticleSystem.MainModule mainModule = particleSystem.main;
		mainModule.startSize = range;
	}

	/// <summary>
	/// Draw the healing range of the FX according to the player healing range.
	/// </summary>
	void OnDrawGizmosSelected()
	{
		// Draw a green sphere at the transform's position
		float healingRange = FindObjectOfType<POD>().GetComponent<PODHealthController>().HealingRange;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, healingRange);
	}
}
