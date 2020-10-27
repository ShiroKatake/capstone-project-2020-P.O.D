//using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// A player controller class for the management of the player's health.
/// </summary>
public class PODHealthController : PrivateInstanceSerializableSingleton<PODHealthController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    //[Header("Player Objects")]
    //[SerializeField] private Camera camera;
    //[SerializeField] private Transform cameraTarget;
    //[SerializeField] private Transform mouseTargeter;
    //[SerializeField] private Transform barrelTip;
    //[SerializeField] private Transform barrelMagazine;
    //[SerializeField] private List<Transform> cliffDetectionMarkers;
    //[SerializeField] private List<Vector3> cliffTestOffsets;
    //[SerializeField] private Transform audioListener;

    //[Header("Movement Stats")]
    //[SerializeField] private float movementSpeed;
    //[SerializeField] private float rotationSpeed;

    //[Header("Shooting Stats")]
    //[SerializeField] private float shootCooldown;

    [Header("Healing Stats")]
    [SerializeField] private float healingSpeed;
    [SerializeField] private float healingRange;

    //[Header("Testing")]
    //[SerializeField] private bool printInputs;

    ////Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private Health health;
    //private Rigidbody rigidbody;
    //private CharacterController charCon;

    ////Variables for moving & determining if rotation is necessary
    //private Vector3 movement;
    //private Vector3 previousMovement = Vector3.zero;
    //private float defaultHoverHeight;
    //private LayerMask groundLayerMask;

    ////Variables for rotating smoothly
    //private Quaternion newRotation;
    //private Quaternion oldRotation;
    //private float slerpProgress = 1;

    ////Projectile Variables
    //private bool shooting;
    //private float timeOfLastShot;

    //Healing variables
    private bool heal;
    private bool isHealing;

    ////Other
    //private Player playerInputManager;
    private bool repsawn;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    public UnityAction onPlayerHeal;
    public UnityAction onPlayerHealCancelled;

    /// <summary>
    /// How close the player needs to be to the tower to heal themselves.
    /// </summary>
    public float HealingRange { get => healingRange; }

    /// <summary>
    /// Is the player currently healing themselves?
    /// </summary>
    public bool IsHealing { get => isHealing; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
        //rigidbody = GetComponent<Rigidbody>();
        //charCon = GetComponent<CharacterController>();
        //groundLayerMask = LayerMask.GetMask("Ground");
        health.onDie += OnDie;
        //timeOfLastShot = shootCooldown * -1;
        //defaultHoverHeight = transform.position.y;
        repsawn = false;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        //Input manager checks every frame, not at fixed update's rate, so calls to input manager should be made every frame
        if (!PauseMenuManager.Paused)
        {
            GetInput();
        }
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// If Time.timeScale == 0, FixedUpdate will not be called.
    /// </summary>
    private void FixedUpdate()
    {
        CheckHealing();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        heal = POD.Instance.PlayerInputManager.GetButton("Heal") && Vector3.Distance(transform.position, Tower.Instance.transform.position) < healingRange && health.CurrentHealth < health.MaxHealth;
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if the player wants to heal themselves based on their input, and heals them if they do.
    /// </summary>
    private void CheckHealing() //No-healing conditions checked for in GetInput() when determining the value of healing.
    {
        if (heal)
        {
            health.Heal(healingSpeed * Time.deltaTime);
            onPlayerHeal?.Invoke();
            isHealing = true;
        }
        else
        {
            if (isHealing)
            {
                onPlayerHealCancelled?.Invoke();
                isHealing = false;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks the player's health and if they're still alive.
    /// </summary>
    private void OnDie()
    {
        if (!repsawn)
        {
            AudioManager.Instance.PlaySound(AudioManager.ESound.Explosion, this.gameObject);
            Debug.Log("The player's health has reached 0. Respawn!!!");
            MessageManager.Instance.SendMessage("Alien", new Message(gameObject.name, "Player", gameObject, "Dead"));
            repsawn = true;
        }
    }
}
