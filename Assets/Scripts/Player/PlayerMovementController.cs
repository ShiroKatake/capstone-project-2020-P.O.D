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

    [SerializeField] private bool printInputs;

    [Header("Player Objects")]
    [SerializeField] private Camera camera;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Transform barrelMagazine;
    [SerializeField] private List<Transform> cliffDetectionMarkers;
    [SerializeField] private List<Vector3> cliffTestOffsets;
    [SerializeField] private Transform audioListener;

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
    private Player playerInputManager;
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
        playerInputManager = ReInput.players.GetPlayer(GetComponent<PlayerID>().Value);  //Needs to run in Awake() or the tutorial breaks
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
            AudioManager.Instance.PlaySound(AudioManager.ESound.Explosion, this.gameObject);
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
        RaycastHit rayHit;
        Vector3 oldPos = transform.position;
        float errorMargin = 0.01f;
        AudioManager.Instance.PlaySound(AudioManager.ESound.Player_Hover, gameObject);

        if (movement != Vector3.zero)
        {
            transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
        }

        if (Physics.Raycast(transform.position, Vector3.down, out rayHit, 20, groundLayerMask))
        {
            //Debug.Log($"POD hover height is {rayHit.distance}, default is {defaultHoverHeight}");
            CheckForCliff(rayHit, errorMargin, oldPos);
            CheckHoverHeight(rayHit, errorMargin);
        }
        else
        {
            Debug.LogError($"{this}.PlayerMovementController.Move() could not raycast to the ground from position {transform.position}");
        }

        cameraTarget.position = transform.position;  
    }

    /// <summary>
    /// Checks if POD's current movement would put it too close to a cliff, and cancels the movement if it would.
    /// </summary>
    /// <param name="rayHit">The RaycastHit from POD's position to gauge distance to the ground and the normal of the hit surface.</param>
    /// <param name="errorMargin">The margin of error for A == B float comparisons.</param>
    /// <param name="oldPos">POD's position before attempting to move.</param>
    private void CheckForCliff(RaycastHit rayHit, float errorMargin, Vector3 oldPos)
    {
        bool moveOkay = true;
        RaycastHit cliffRayHit;

        foreach (Vector3 offset in cliffTestOffsets)
        {
            if (Physics.Raycast(transform.position + offset, Vector3.down, out cliffRayHit, 20, groundLayerMask))
            {
                //Confirm ray hit heights differ; rule out OK: level to level - normal.y 1.0 to 1.0 and heights match
                if (cliffRayHit.point.y < rayHit.point.y - errorMargin || cliffRayHit.point.y > rayHit.point.y + errorMargin)   
                {
                    //Debug.Log($"POD at {rayHit.point} detecting cliff or ramp at {cliffRayHit.point}. POD ground normal: {rayHit.normal}, cliff/ramp normal: {cliffRayHit.normal}");

                    //Confirm normal.y's differ
                    if (cliffRayHit.normal.y < rayHit.normal.y - errorMargin || cliffRayHit.normal.y > rayHit.normal.y + errorMargin)     
                    {
                        //OK:  - Level to ramp  - normal.y 1.0 to 0.9
                        //OK:  - Ramp to level  - normal.y 0.9 to 1.0
                        //NOT: - Level to cliff - normal.y (0.9 OR 1.0) to !(0.9 OR 1.0)
                        
                        //Confirm cliffRayHit normal.y is outside of accepted bounds
                        if (cliffRayHit.normal.y < 0.9f - errorMargin || cliffRayHit.normal.y > 1 + errorMargin || (0.9f + errorMargin < cliffRayHit.normal.y && cliffRayHit.normal.y < 1 - errorMargin))
                        {
                            //Debug.LogError($"cliffRayHit normal.y is outside of expected bounds for level ground (~1.0) and ramps (~0.9), therefore cliff, therefore cancelling movement.");
                            moveOkay = false;
                            break;
                        }
                    }
                    else
                    {
                        //OK:  - Ramp to ramp   - normal.y 0.9 to 0.9 and heights don't match
                        //NOT: - Level to cliff - normal.y 1.0 to 1.0 but heights don't match

                        if (rayHit.normal.y >= 1 - errorMargin && rayHit.normal.y <= 1 + errorMargin                  //Confirm ray hit is on level ground
                            && cliffRayHit.normal.y >= 1 - errorMargin && cliffRayHit.normal.y <= 1 + errorMargin)    //Confirm cliff ray hit is on level ground
                        {
                            //Debug.Log($"POD and cliff rayhit normal y values within the margin of error ({errorMargin}) of each other and of level ground normal y (1), but the rayhit heights differ beyond the margin of error. Therefore sheer cliff, therefore not moving.");
                            moveOkay = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"{this}.PlayerMovementController.CheckForCliff(), could not raycast to the ground from position {transform.position + offset}");
                moveOkay = false;
                break;
            }
        }

        if (!moveOkay)
        {
            //Debug.Log("Move disallowed, returning to original position");
            transform.position = oldPos;
        }
    }

    /// <summary>
    /// Toggles gravity affecting POD if something has pushed POD up above its default hover height, and shuts it off when POD is back to its normal hover height.
    /// </summary>
    /// <param name="rayHit">The RaycastHit from POD's position to gauge distance to the ground.</param>
    /// <param name="errorMargin">The margin of error for A == B float comparisons.</param>
    private void CheckHoverHeight(RaycastHit rayHit, float errorMargin)
    {
        if (rigidbody.useGravity)
        {
            if (rayHit.distance <= defaultHoverHeight - errorMargin)
            {
                transform.position = new Vector3(transform.position.x, defaultHoverHeight, transform.position.z);
                rigidbody.useGravity = false;
                rigidbody.drag = 100;
                //Debug.Log("POD below hover height");
            }
            //else
            //{
            //    Debug.Log("POD above or at hover height");
            //}
        }
        else
        {
            if (rayHit.distance > defaultHoverHeight + errorMargin)
            {
                rigidbody.useGravity = true;
                rigidbody.drag = 0;
                rigidbody.velocity = Vector3.zero;
                //Debug.Log("POD above hover height");
            }
            //else
            //{
            //    Debug.Log("POD below  or at hover height");
            //}
        }
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
            AudioManager.Instance.PlaySound(AudioManager.ESound.Laser_POD, this.gameObject);
            Vector3 vector = barrelTip.position - barrelMagazine.position;
            projectile.Shoot(vector.normalized, 0);
            //TODO: use overload that incorporates shooter movement speed, and calculate current movement speed in the direction of the shot vector.
        }
        
    }
}
