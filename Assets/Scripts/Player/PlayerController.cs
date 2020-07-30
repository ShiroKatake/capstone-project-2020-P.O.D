using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The player. Player controls the player's movement, shooting and healing. For building spawning, see BuildingSpawningController.
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Objects")]
    [SerializeField] private Camera camera;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform mouseTargeter;
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Transform barrelMagazine;
    [SerializeField] private List<Transform> cliffDetectionMarkers;
    [SerializeField] private List<Vector3> cliffTestOffsets;
    [SerializeField] private Transform audioListener;

    [Header("Movement Stats")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Shooting Stats")]
    [SerializeField] private float shootCooldown;

    [Header("Healing Stats")]
    [SerializeField] private float healingSpeed;
    [SerializeField] private float healingRange;

    [Header("Testing")]
    [SerializeField] private bool printInputs;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private Health health;
    private Rigidbody rigidbody;
    private CharacterController charCon;

    //Variables for moving & determining if rotation is necessary
    private Vector3 movement;
    private Vector3 previousMovement = Vector3.zero;
    private float defaultHoverHeight;
    private LayerMask groundLayerMask;

    //Variables for rotating smoothly
    private Quaternion newRotation;
    private Quaternion oldRotation;
    private float slerpProgress = 1;

    //Projectile Variables
    private bool shooting;
    private float timeOfLastShot;

    //Healing variables
    private bool healing;

    //Other
    private Player playerInputManager;
    private bool repsawn;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// Singleton public property for the player controller.
    /// </summary>
    public static PlayerController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// How close the player needs to be to the cryo egg to heal themselves.
    /// </summary>
    public float HealingRange { get => healingRange; }

    /// <summary>
    /// POD's movement speed.
    /// </summary>
    public float MovementSpeed { get => movementSpeed; }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// POD's Rewired player input manager.
    /// </summary>
    public Player PlayerInputManager
    {
        get
        {
            if (playerInputManager == null)
            {
                playerInputManager = ReInput.players.GetPlayer(GetComponent<PlayerID>().Value);
            }

            return playerInputManager;
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more Players.");
        }

        Instance = this;
        health = GetComponent<Health>();
        rigidbody = GetComponent<Rigidbody>();
        charCon = GetComponent<CharacterController>();
        groundLayerMask = LayerMask.GetMask("Ground");
		health.onDie += OnDie;
        timeOfLastShot = shootCooldown * -1;
        defaultHoverHeight = transform.position.y;
        repsawn = false;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    void Start()
    {
        playerInputManager = ReInput.players.GetPlayer(GetComponent<PlayerID>().Value);
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
        UpdateDrone();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        float moveHorizontal = playerInputManager.GetAxis("Horizontal");
        float moveVertical = playerInputManager.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0, -moveVertical);

        healing = playerInputManager.GetButton("Heal") && Vector3.Distance(transform.position, CryoEgg.Instance.transform.position) < healingRange && health.CurrentHealth < health.MaxHealth;
        shooting = InputController.Instance.ButtonHeld("Shoot") && !healing && Time.time - timeOfLastShot > shootCooldown;

        if (printInputs)
        {
            Debug.Log($"Rewired, PlayerMovementController.GetInput() (called by Update()), movement: {movement}");
            Debug.Log($"Rewired via InputController, PlayerMovementController.GetInput() (called by Update()), shooting: {shooting}");
        }
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Updates the player based on their input.
    /// </summary>
    private void UpdateDrone()
    {
        audioListener.position = transform.position;
        Look();
        Move();
        CheckShooting();
        CheckHealing();
    }

    /// <summary>
    /// Changes where the player is looking based on their input.
    /// </summary>
    private void Look()
    {
        Vector3 lookPos = MousePositionOnTerrain.Instance.GetWorldPosition;
        mouseTargeter.LookAt(lookPos);
        float yRotation = MathUtility.Instance.NormaliseAngle(mouseTargeter.eulerAngles.y);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    /// <summary>
    /// Moves the player forward based on their input.
    /// </summary>
    private void Move()
    {
        AudioManager.Instance.PlaySound(AudioManager.ESound.Player_Hover, gameObject);

        if (movement != Vector3.zero && !healing)
        {
            charCon.SimpleMove(movement * movementSpeed);
        }

        cameraTarget.position = transform.position;  
    }

    /// <summary>
    /// Checks if the player wants to shoot based on their input, and fires projectiles if they do.
    /// </summary>
    private void CheckShooting()
    {
        if (shooting) //No-shooting conditions checked for in GetInput() when determining the value of shooting.
        {
            timeOfLastShot = Time.time;
            Projectile projectile = ProjectileFactory.Instance.GetProjectile(EProjectileType.PODLaserBolt, transform, barrelTip.position);
            AudioManager.Instance.PlaySound(AudioManager.ESound.Laser_POD, this.gameObject);
            Vector3 vector = barrelTip.position - barrelMagazine.position;
            projectile.Shoot(vector.normalized, 0);
        }
    }

    /// <summary>
    /// Checks if the player wants to heal themselves based on their input, and heals them if they do.
    /// </summary>
    private void CheckHealing() //No-healing conditions checked for in GetInput() when determining the value of healing.
    {
        if (healing)
        {
            health.Heal(healingSpeed * Time.deltaTime);
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
            MessageDispatcher.Instance.SendMessage("Alien", new Message(gameObject.name, "Player", this.gameObject, "Dead"));
            repsawn = true;
        }
    }
}
