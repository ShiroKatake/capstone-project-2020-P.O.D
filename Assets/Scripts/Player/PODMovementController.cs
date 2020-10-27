using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player controller class for the player's movement.
/// </summary>
public class PODMovementController : PrivateInstanceSerializableSingleton<PODMovementController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Objects")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform mouseTargeter;
    [SerializeField] private Transform audioListener;

    [Header("Movement Stats")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Testing")]
    [SerializeField] private bool printInputs;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private CharacterController characterController;

    //Movement variables
    private Vector3 movement;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// POD's movement speed.
    /// </summary>
    public float MovementSpeed { get => movementSpeed; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponent<CharacterController>();
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
        Look();
        Move();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        float moveHorizontal = POD.Instance.PlayerInputManager.GetAxis("Horizontal");
        float moveVertical = POD.Instance.PlayerInputManager.GetAxis("Vertical");
        movement = new Vector3(moveHorizontal, 0, -moveVertical);
        if (printInputs) Debug.Log($"Rewired, PlayerMovementController.GetInput() (called by Update()), movement: {movement}");
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

        if (movement != Vector3.zero && !POD.Instance.HealthController.IsHealing)
        {
            characterController.SimpleMove(movement * movementSpeed);
        }

        cameraTarget.position = transform.position;
    }
}
