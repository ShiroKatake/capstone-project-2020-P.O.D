using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player controller class for the player's shooting.
/// </summary>
public class PODShootingController : PrivateInstanceSerializableSingleton<PODShootingController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Objects")]
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Transform barrelMagazine;

    [Header("Shooting Stats")]
    [SerializeField] private float shootCooldown;

    [Header("Testing")]
    [SerializeField] private bool printInputs;

    //Non-Serialized Fields------------------------------------------------------------------------

    private bool shooting;
    private float timeOfLastShot;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        timeOfLastShot = shootCooldown * -1;
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
