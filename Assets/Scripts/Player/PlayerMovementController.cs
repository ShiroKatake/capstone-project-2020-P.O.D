using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The player. Player controls the player's movement and shooting. For building spawning, see BuildingSpawningController.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Objects")]
    [SerializeField] private Camera camera;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Transform barrelMagazine;
    [SerializeField] private List<Transform> cliffDetectionMarkers;
    [SerializeField] private List<Vector3> cliffTestOffsets;

    [Header("Player Stats")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float shootCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private Health health;
    private Rigidbody rigidbody;

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

    //Other
    private Rewired.Player playerInputManager;
    private bool gameOver;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// Singleton public property for the player.
    /// </summary>
    public static PlayerMovementController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// POD's movement speed.
    /// </summary>
    public float MovementSpeed { get => movementSpeed; }

    /// <summary>
    /// POD's Rewired player input manager.
    /// </summary>
    public Player PlayerInputManager { get => playerInputManager; }

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
        timeOfLastShot = shootCooldown * -1;
        defaultHoverHeight = transform.position.y;
        gameOver = false;
        groundLayerMask = LayerMask.GetMask("Ground");
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
        GetInput();
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
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
        shooting = InputController.Instance.ButtonHeld("Shoot");
        //Debug.Log($"Movement input: {movement}");
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Updates the player based on their input.
    /// </summary>
    private void UpdateDrone()
    {
        CheckHealth();
        Look();
        Move();
        CheckShooting();
    }

    /// <summary>
    /// Checks the player's health and if they're still alive.
    /// </summary>
    private void CheckHealth()
    {
        if (health.IsDead())
        {
            Debug.Log("The player's health has reached 0. GAME OVER!!!");

            if (!gameOver)
            {
                MessageDispatcher.Instance.SendMessage("Alien", new Message(gameObject.name, "Player", this.gameObject, "Dead"));
                gameOver = true;
            }
        }
    }

    /// <summary>
    /// Changes where the player is looking based on their input.
    /// </summary>
    private void Look()
    {
        transform.LookAt(MousePositionOnTerrain.Instance.GetWorldPosition);
    }

    /// <summary>
    /// Moves the player forward based on their input.
    /// </summary>
    private void Move()
    {
        //NavMeshHit navHit;
        RaycastHit rayHit;
        Vector3 oldPos = transform.position;
        float errorMargin = 0.01f;
        AudioManager.Instance.PlaySound(AudioManager.Sound.Player_Hover, transform.position);

        if (movement != Vector3.zero)
        {
            transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
        }

        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, 20, groundLayerMask))
        {
            //RaycastHit cliffRayHit;
            //Debug.Log($"POD hover height is {rayHit.distance}, default is {defaultHoverHeight}");

            //if (Physics.Raycast(transform.position, Vector3.down, out rayHit, 20, groundLayerMask))
            //{
                
            //}
            //else
            //{
            //    Debug.LogError($"{this}.PlayerMovementController.Move() could not raycast to the ground from position {transform.position}");
            //}

            //if (NavMesh.SamplePosition(transform.position, out navHit, 4, NavMesh.AllAreas))
            //{
            //    Debug.Log($"NavMesh.SamplePosition() from {transform.position} hit mesh at {navHit.position}"); 

            //    if (transform.position.x != navHit.position.x && transform.position.z != navHit.position.z)
            //    {
            //        Debug.LogError($"NavHit position {navHit.position} is not in line with {transform.position}. Cancelling movement to that position.");
            //        transform.position = oldPos;
            //    }
            //}
            //else
            //{
            //    Debug.LogError($"Position {transform.position} is not on the nav mesh. Cancelling movement to that position.");
            //    transform.position = oldPos;
            //}

            //Toggle gravity if something has pushed the player up above hoverHeight
            if (rigidbody.useGravity)
            {
                if (rayHit.distance <= defaultHoverHeight - errorMargin)
                {
                    transform.position = new Vector3(transform.position.x, defaultHoverHeight, transform.position.z);
                    rigidbody.useGravity = false;
                    rigidbody.drag = 100;
                    Debug.Log("POD below hover height");
                }
                else
                {
                    Debug.Log("POD above or at hover height");
                }
            }
            else
            {
                if (rayHit.distance > defaultHoverHeight + errorMargin)
                {
                    rigidbody.useGravity = true;
                    rigidbody.drag = 0;
                    rigidbody.velocity = Vector3.zero;
                    Debug.Log("POD above hover height");
                }
                else
                {
                    Debug.Log("POD below  or at hover height");
                }
            }
        }
        else
        {
            Debug.LogError($"{this}.PlayerMovementController.Move() could not raycast to the ground from position {transform.position}");
        }

        cameraTarget.transform.position = transform.position;

        //AudioManager.Instance.PlaySound(AudioManager.Sound.Player_Hover, this.transform.position);

        //if (movement != Vector3.zero)
        //{
        //    drone.transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
        //    cameraTarget.transform.position = drone.transform.position;
        //}

        ////Toggle gravity if something has pushed the player up above hoverHeight
        //if (rigidbody.useGravity)
        //{
        //    if (drone.position.y <= hoverHeight)
        //    {
        //        drone.position = new Vector3(drone.position.x, hoverHeight, drone.position.z);
        //        rigidbody.useGravity = false;
        //        rigidbody.drag = 100;
        //    }
        //}
        //else
        //{
        //    if (drone.position.y > hoverHeight)   //TODO: account for terrain pushing the player up
        //    {
        //        rigidbody.useGravity = true;
        //        rigidbody.drag = 0;
        //        rigidbody.velocity = Vector3.zero;
        //    }
        //}

        ////Positioning this line here accounts for the player having been moved by an external force (e.g. pushed by enemies)
        //cameraTarget.position = drone.position;     
    }

    /// <summary>
    /// Checks if the player wants to shoot based on their input, and fires projectiles if they do.
    /// </summary>
    private void CheckShooting()
    {
        if (shooting && Time.time - timeOfLastShot > shootCooldown)
        {
            timeOfLastShot = Time.time;
            Projectile projectile = ProjectileFactory.Instance.GetProjectile(EProjectileType.PODLaserBolt, transform, barrelTip.position);
            Vector3 vector = barrelTip.position - barrelMagazine.position;
            projectile.Shoot(vector.normalized, 0);
            //TODO: use overload that incorporates shooter movement speed, and calculate current movement speed in the direction of the shot vector.
        }
    }
}
