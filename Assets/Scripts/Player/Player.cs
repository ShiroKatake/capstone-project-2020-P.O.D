using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player.
/// </summary>
public class Player : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Objects")]
    [SerializeField] private GameObject drone;
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private Transform terraformerHoldPoint;
    [SerializeField] private Transform laserCannonTip;
    [SerializeField] private Transform laserBatteryPoint;

    [Header("Prefabs")]
    [SerializeField] private LaserBolt laserBoltPrefab;

    [Header("Player Stats")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int laserBatteryCapacity;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Variables for moving & determining if rotation is necessary
    private Vector3 movement;
    private Vector3 previousMovement = Vector3.zero;

    //Variables for rotating smoothly
    private Quaternion newRotation;
    private Quaternion oldRotation;
    private float slerpProgress = 1;

    //Variables for Terraformer Spawning
    private Building heldBuilding;
    private EBuilding heldBuildingType;
    private bool holdingBuilding;
    private bool spawnBuilding;

    //Laser Bolt Variables

    private bool shooting;
    private List<LaserBolt> laserBattery = new List<LaserBolt>();

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// Singleton public property for the player.
    /// </summary>
    public static Player Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// A pool for laser bolts that aren't in use.
    /// </summary>
    public List<LaserBolt> LaserBattery { get => laserBattery; }

    /// <summary>
    /// The physical location of the pool of laser bolts in-scene.
    /// </summary>
    public Transform LaserBatteryPoint { get => laserBatteryPoint; }

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

        for (int i = 0; i < laserBatteryCapacity; i++)
        {
            laserBattery.Add(Instantiate<LaserBolt>(laserBoltPrefab, laserBatteryPoint.position, laserBoltPrefab.transform.rotation));
        }
    }


    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {
        GetInput();
        UpdateDrone();
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        //Movement Input
        movement = new Vector3(InputController.Instance.GetAxis("MoveLeftRight"), 0, InputController.Instance.GetAxis("MoveForwardsBackwards"));
        heldBuildingType = InputController.Instance.SpawnBuilding();

        //Terraformer Input
        if (heldBuildingType != EBuilding.None)
        {
            if (!holdingBuilding)
            {
                holdingBuilding = true;
                spawnBuilding = true;
            }
        }
        else if (holdingBuilding)
        {
            holdingBuilding = false;
        }

        //if (InputController.Instance.ButtonPressed("HoldTerraformer"))
        //{
        //    spawnTerraformer = true;
        //}

        //Shooting Input
        shooting = InputController.Instance.ButtonHeld("Shoot");
    }

    /// <summary>
    /// Updates the player based on their input.
    /// </summary>
    private void UpdateDrone()
    {
        Look();
        Move();
        CheckTerraformerSpawning();
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
            oldRotation = drone.transform.rotation;
            newRotation = Quaternion.LookRotation(movement);
        }

        //Still turning? Rotate towards direction player wants to move in, but smoothly.
        if (slerpProgress < 1/* && movement != Vector3.zero*/)
        {
            slerpProgress = Mathf.Min(1, slerpProgress + rotationSpeed * Time.deltaTime);
            drone.transform.rotation = Quaternion.Slerp(oldRotation, newRotation, slerpProgress);
        }
    }


    /// <summary>
    /// Moves the player based on their input.
    /// </summary>
    private void Move()
    {
        //Player wants to move? Move the drone.
        if (movement != Vector3.zero && heldBuilding == null)
        {
            drone.transform.Translate(new Vector3(0, 0, movementSpeed * movement.magnitude * Time.deltaTime), Space.Self);
            cameraTarget.transform.position = drone.transform.position;
        }
    }

    /// <summary>
    /// Checks if the player wants to spawn a building.
    /// </summary>
    private void CheckTerraformerSpawning()
    {
        if (!shooting || laserBattery.Count == 0)
        {
            if (heldBuilding == null)
            {
                if (spawnBuilding)
                {
                    heldBuilding = BuildingFactory.Instance.GetBuilding(heldBuildingType, terraformerHoldPoint.position, terraformerHoldPoint.rotation);
                    spawnBuilding = false;
                }
            }
            else
            {
                if (heldBuildingType != EBuilding.None && heldBuilding.BuildingType != heldBuildingType)
                {
                    BuildingFactory.Instance.DestroyBuilding(heldBuilding);
                    heldBuilding = BuildingFactory.Instance.GetBuilding(heldBuildingType, terraformerHoldPoint.position, terraformerHoldPoint.rotation);
                }
            }
        }

        if (heldBuilding != null)
        {
            heldBuilding.transform.rotation = terraformerHoldPoint.rotation;

            if (holdingBuilding && (!shooting || laserBattery.Count == 0))
            {
                heldBuilding.transform.position = terraformerHoldPoint.position;
            }
            else
            {
                Vector3 spawnPos = terraformerHoldPoint.position;
                spawnPos.y = 0.5f;
                heldBuilding.transform.position = spawnPos;
                //heldBuilding.Terraforming = Planet.Instance.TerraformingProgress < 1;
                //Planet.Instance.Terraformers.Add(heldBuilding);
                heldBuilding = null;
                heldBuildingType = EBuilding.None;
            }
        }
    }

    /// <summary>
    /// Checks if the player wants to shoot based on their input, and fires laser bolts if they do.
    /// </summary>
    private void CheckShooting()
    {
        if (shooting && laserBattery.Count > 0)
        {
            LaserBolt laserBolt = laserBattery[0];
            laserBattery.Remove(laserBolt);
            laserBolt.transform.position = laserCannonTip.position;
            laserBolt.Shoot((transform.forward * 2 - transform.up).normalized);
        }
    }
}
