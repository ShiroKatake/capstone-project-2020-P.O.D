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

    [Header("Shooting Stats")]
    [SerializeField] private float spreadAngle;
    [SerializeField] private float maxElevation;
    [SerializeField] private float shotForce;
    [SerializeField] private float shotCooldown;

    [Header("Overheating Stats")]
    [SerializeField] private float heatPerShot;
    [SerializeField] private float coolingPerSecond;
    [SerializeField] private float overheatingThreshold;
    [SerializeField] private float overheatingCooldown;

    [Header("Testing")]
    [SerializeField] private bool printInputs;

    //Non-Serialized Fields------------------------------------------------------------------------

    private float timeOfLastShot;
    private float timeOfLastOverheat;
    private bool wantToShoot;
    private float barrelHeat;
    private bool overheated;
    private bool canShoot = true;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Is the game in a stage where POD is allowed to shoot?
    /// </summary>
    public bool CanShoot { get => canShoot; set => canShoot = value; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        timeOfLastShot = -1;
        timeOfLastOverheat = -1;
        barrelHeat = 0;
        overheated = false;
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
        CheckOverheating();
        CheckShooting();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        wantToShoot = InputManager.Instance.ButtonHeld("Shoot");
        if (printInputs) Debug.Log($"Rewired via InputController, PlayerMovementController.GetInput() (called by Update()), wantToShoot: {wantToShoot}");
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    private void CheckOverheating()
    {
        if (overheated)
        {
            float timeSinceOverheat = Time.time - timeOfLastOverheat;
            //Debug.Log($"{this}.CheckShooting(), can't shoot, waiting for barrel to cool down. Progress is {timeSinceOverheat}s / {overheatingCooldown}s");

            if (timeSinceOverheat > overheatingCooldown)
            {
                overheated = false;
                barrelHeat = 0;
            }
        }
        else
        {
            barrelHeat -= Mathf.Min(barrelHeat, coolingPerSecond * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Checks if the player wants to and can shoot based on their input, and fires projectiles if they do.
    /// </summary>
    private void CheckShooting()
    {
        if (wantToShoot && ReadyToShoot()) //No-shooting conditions checked for in GetInput() when determining the value of shooting.
        {
            //Debug.Log($"{this}.CheckShooting(), can and wants to shoot, calling Shoot()");
            Shoot();
        }
        //else
        //{
        //    Debug.Log($"{this}.CheckShooting(), can't and/or don't want to shoot");
        //}
    }

    /// <summary>
    /// Checks if the player is ready to shoot.
    /// </summary>
    /// <returns>Whether or not the player can shoot.</returns>
    private bool ReadyToShoot()
    {
        return canShoot && !overheated && !POD.Instance.HealthController.IsHealing && Time.time - timeOfLastShot > shotCooldown;
    }

    /// <summary>
    /// Fires a projectile from POD's laser cannon.
    /// </summary>
    private void Shoot()
    {
        //Quaternion randomRotation = Random.rotation;
        //Quaternion projectileRotation = Quaternion.RotateTowards(barrelTip.transform.rotation, randomRotation, spreadAngle);
        Quaternion projectileRotation = Quaternion.RotateTowards(barrelTip.transform.rotation, Random.rotation, spreadAngle);
        //Debug.Log($"randomRotation is {randomRotation} (Quaternion) / {randomRotation.eulerAngles} (EulerAngles)");
        //Debug.Log($"projectileRotation is {projectileRotation} (Quaternion) / {projectileRotation.eulerAngles} (EulerAngles)");
        Projectile projectile = ProjectileFactory.Instance.Get(transform, barrelTip.position, projectileRotation, EProjectileType.PODLaserBolt);
        projectile.Shoot(shotForce);
        //Debug.Log($"{this}.PODShootingController.Shoot(), projectile is {projectile}");
        AudioManager.Instance.PlaySound(AudioManager.ESound.Laser_POD, this.gameObject);
        timeOfLastShot = Time.time;
        barrelHeat += heatPerShot;

        if (barrelHeat > overheatingThreshold)
        {
            overheated = true;
            timeOfLastOverheat = Time.time;
        }
    }
}
