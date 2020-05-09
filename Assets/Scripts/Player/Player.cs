using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player. Player controls the player's movement and shooting. For building spawning, see BuildingSpawningController.
/// </summary>
public class Player : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Objects")]
    [SerializeField] private Transform drone;
    //[SerializeField] private Transform droneModel;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform terraformerHoldPoint;
    [SerializeField] private Transform laserCannonTip;

    [Header("Player Stats")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float shootCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Variables for moving & determining if rotation is necessary
    private Vector3 movement;
    private Vector3 previousMovement = Vector3.zero;

    //Variables for rotating smoothly
    private Quaternion newRotation;
    private Quaternion oldRotation;
    private float slerpProgress = 1;

    private Rigidbody rigidbody;
    private float hoverHeight;

    //Projectile Variables
    private bool shooting;
    private float timeOfLastShot;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    public float GetMovementSpeed {get => movementSpeed;}

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// Singleton public property for the player.
    /// </summary>
    public static Player Instance { get; protected set; }

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
        rigidbody = GetComponent<Rigidbody>();
        timeOfLastShot = shootCooldown * -1;
        hoverHeight = drone.position.y;
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
        movement = new Vector3(InputController.Instance.GetAxis("MoveLeftRight"), 0, InputController.Instance.GetAxis("MoveForwardsBackwards"));
        shooting = InputController.Instance.ButtonHeld("Shoot");
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Updates the player based on their input.
    /// </summary>
    private void UpdateDrone()
    {
        Look();
        Move();
        CheckShooting();
    }

    /// <summary>
    /// Changes where the player is looking based on their input.
    /// </summary>
    private void Look()
    {
        //Player wants to move in a new direction? Update Slerp variables.
        if (movement != previousMovement)
        {
            slerpProgress = 0;
            oldRotation = drone.rotation;
            newRotation = Quaternion.LookRotation(movement);
        }

        //Still turning? Rotate towards direction player wants to move in, but smoothly.
        if (movement == Vector3.zero)
        {
            rigidbody.velocity = movement;
        }
        else if (slerpProgress < 1)
        {
            slerpProgress = Mathf.Min(1, slerpProgress + rotationSpeed * Time.deltaTime);
            drone.rotation = Quaternion.Slerp(oldRotation, newRotation, slerpProgress);
        }
    }

    /// <summary>
    /// Moves the player forward based on their input.
    /// </summary>
    private void Move()
    {
        if (movement != Vector3.zero)
        {
            drone.Translate(new Vector3(0, 0, movementSpeed * movement.magnitude * Time.deltaTime), Space.Self);
        }

        //Toggle gravity if something has pushed the player up above hoverHeight
        if (rigidbody.useGravity)
        {
            if (drone.position.y <= hoverHeight)
            {
                drone.position = new Vector3(drone.position.x, hoverHeight, drone.position.z);
                rigidbody.useGravity = false;
            }
        }
        else
        {
            if (drone.position.y > hoverHeight)   //TODO: account for terrain pushing the player up
            {
                rigidbody.useGravity = true;
            }
        }

        //Positioning this line here accounts for the player having been moved by an external force (e.g. pushed by enemies)
        cameraTarget.position = drone.position;     
    }

    /// <summary>
    /// Checks if the player wants to shoot based on their input, and fires projectiles if they do.
    /// </summary>
    private void CheckShooting()
    {
        if (shooting && Time.time - timeOfLastShot > shootCooldown)
        {
            timeOfLastShot = Time.time;
            Projectile projectile = ProjectileFactory.Instance.GetProjectile(transform, laserCannonTip.position);
            projectile.Shoot((transform.forward * 2 - transform.up).normalized);
        }
    }
}
