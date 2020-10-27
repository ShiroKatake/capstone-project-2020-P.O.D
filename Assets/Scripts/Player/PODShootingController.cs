//using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.Events;

/// <summary>
/// A player controller class for the player's shooting.
/// </summary>
public class PODShootingController : PrivateInstanceSerializableSingleton<PODShootingController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Objects")]
    //[SerializeField] private Camera camera;
    //[SerializeField] private Transform cameraTarget;
    //[SerializeField] private Transform mouseTargeter;
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Transform barrelMagazine;
    //[SerializeField] private List<Transform> cliffDetectionMarkers;
    //[SerializeField] private List<Vector3> cliffTestOffsets;
    //[SerializeField] private Transform audioListener;

    //[Header("Movement Stats")]
    //[SerializeField] private float movementSpeed;
    //[SerializeField] private float rotationSpeed;

    [Header("Shooting Stats")]
    [SerializeField] private float shootCooldown;

    //[Header("Healing Stats")]
    //[SerializeField] private float healingSpeed;
    //[SerializeField] private float healingRange;

    [Header("Testing")]
    [SerializeField] private bool printInputs;

    //Non-Serialized Fields------------------------------------------------------------------------

    ////Components
    //private Health health;
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

    //Projectile Variables
    private bool shooting;
    private float timeOfLastShot;

    ////Healing variables
    //private bool healing;
    //private bool isHealing;

    ////Other
    //private Player playerInputManager;
    //private bool repsawn;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        //health = GetComponent<Health>();
        //rigidbody = GetComponent<Rigidbody>();
        //charCon = GetComponent<CharacterController>();
        //groundLayerMask = LayerMask.GetMask("Ground");
        //health.onDie += OnDie;
        timeOfLastShot = shootCooldown * -1;
        //defaultHoverHeight = transform.position.y;
        //repsawn = false;
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
        CheckShooting();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        shooting = InputManager.Instance.ButtonHeld("Shoot") && !POD.Instance.HealthController.IsHealing && Time.time - timeOfLastShot > shootCooldown;
        if (printInputs) Debug.Log($"Rewired via InputController, PlayerMovementController.GetInput() (called by Update()), shooting: {shooting}");
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if the player wants to shoot based on their input, and fires projectiles if they do.
    /// </summary>
    private void CheckShooting()
    {
        if (shooting) //No-shooting conditions checked for in GetInput() when determining the value of shooting.
        {
            timeOfLastShot = Time.time;
            Projectile projectile = ProjectileFactory.Instance.Get(transform, barrelTip, EProjectileType.PODLaserBolt);
            AudioManager.Instance.PlaySound(AudioManager.ESound.Laser_POD, this.gameObject);
            Vector3 vector = barrelTip.position - barrelMagazine.position;
            projectile.Shoot(vector.normalized, 0);
        }
    }
}
