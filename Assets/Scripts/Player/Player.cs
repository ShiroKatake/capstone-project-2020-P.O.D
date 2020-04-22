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
    [SerializeField] private Camera camera;
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

    //Variables for Building Spawning
    private EBuilding selectedBuildingType;
    private Building heldBuilding;
    private Vector3 rawBuildingOffset;
    private Vector3 rawBuildingMovement;
    private bool cycleBuildingSelection = false;
    private bool cyclingBuildingSelection = false;
    private bool spawnBuilding = false;
    private bool placeBuilding = false;
    private bool cancelBuilding = false;

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
            LaserBolt l = Instantiate<LaserBolt>(laserBoltPrefab, laserBatteryPoint.position, laserBoltPrefab.transform.rotation);
            l.transform.parent = laserBatteryPoint;
            laserBattery.Add(l);
        }

        selectedBuildingType = EBuilding.SolarPanel;
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
        //Movement Input
        movement = new Vector3(InputController.Instance.GetAxis("MoveLeftRight"), 0, InputController.Instance.GetAxis("MoveForwardsBackwards"));
        rawBuildingMovement = new Vector3(InputController.Instance.GetAxis("LookLeftRight"), 0, InputController.Instance.GetAxis("LookUpDown"));

        //Building Selection Input
        if (!cyclingBuildingSelection && InputController.Instance.ButtonPressed("CycleBuilding"))
        {
            cyclingBuildingSelection = true;
            selectedBuildingType = InputController.Instance.SelectBuilding(selectedBuildingType);
        }
        else if (cyclingBuildingSelection && !InputController.Instance.ButtonPressed("CycleBuilding"))
        {
            cyclingBuildingSelection = false;
        }

        //Building Placement Input
        if (!spawnBuilding)
        {
            spawnBuilding = InputController.Instance.ButtonPressed("SpawnBuilding");
        }
        else
        {
            placeBuilding = InputController.Instance.ButtonPressed("PlaceBuilding");
            cancelBuilding = InputController.Instance.ButtonPressed("CancelBuilding");
        }

        //Shooting Input
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
        CheckBuildingSpawning();
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
        if (slerpProgress < 1 && movement != Vector3.zero)
        {
            slerpProgress = Mathf.Min(1, slerpProgress + rotationSpeed * Time.deltaTime);
            drone.transform.rotation = Quaternion.Slerp(oldRotation, newRotation, slerpProgress);
        }
    }

    /// <summary>
    /// Moves the player forward based on their input.
    /// </summary>
    private void Move()
    {
        if (movement != Vector3.zero)
        {
            drone.transform.Translate(new Vector3(0, 0, movementSpeed * movement.magnitude * Time.deltaTime), Space.Self);
            cameraTarget.transform.position = drone.transform.position;
        }
    }

    /// <summary>
    /// Checks if the player wants to spawn a building, and spawns and moves it if they do.
    /// </summary>
    private void CheckBuildingSpawning()
    {
        if (spawnBuilding)
        {
            //Instantiate the appropriate building, positioning it properly.
            if (heldBuilding == null)
            {
                heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);

                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
                {
                    heldBuilding.transform.position = MousePositionToBuildingPosition(transform.position + heldBuilding.GetOffset(transform.rotation.eulerAngles.y), heldBuilding.XSize, heldBuilding.ZSize);
                }
                else
                {
                    rawBuildingOffset = heldBuilding.GetOffset(transform.rotation.eulerAngles.y);
                    heldBuilding.transform.position = RawBuildingPositionToBuildingPosition(heldBuilding.XSize, heldBuilding.ZSize);
                }
            }
            //Instantiate the appropriate building and postion it properly, replacing the old one.
            else if (heldBuilding.BuildingType != selectedBuildingType)
            {
                Vector3 pos;

                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
                {
                    pos = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.XSize, heldBuilding.ZSize);
                }
                else
                {
                    pos = RawBuildingPositionToBuildingPosition(heldBuilding.XSize, heldBuilding.ZSize);
                }

                BuildingFactory.Instance.DestroyBuilding(heldBuilding, false);
                heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);
                heldBuilding.transform.position = pos;
            }
            else //Move the building where you want it
            {
                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
                {
                    heldBuilding.transform.position = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.XSize, heldBuilding.ZSize);
                }
                else
                {
                    heldBuilding.transform.position = RawBuildingPositionToBuildingPosition(heldBuilding.XSize, heldBuilding.ZSize);
                }                
            }

            bool collision = heldBuilding.CollisionUpdate();

            //Place it or cancel building it
            if (placeBuilding && ResourceController.Instance.Ore >= heldBuilding.OreCost && !collision)
            {              
                Vector3 spawnPos = heldBuilding.transform.position;
                spawnPos.y = 0.5f;
                heldBuilding.Place(spawnPos);    
                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;
            }
            else if (cancelBuilding || (placeBuilding && (collision || ResourceController.Instance.Ore < heldBuilding.OreCost)))
            {
                if (placeBuilding)
                {
                    if (ResourceController.Instance.Ore < heldBuilding.OreCost)
                    {
                        Debug.Log("You have insufficient ore to build this building.");
                    }
                    
                    if (collision)
                    {
                        Debug.Log("You cannot place a building there; it would occupy the same space as something else.");
                    }
                }

                BuildingFactory.Instance.DestroyBuilding(heldBuilding, false);
                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;                
            }
        }
    }

    /// <summary>
    /// Gets the position of the mouse in the scene based on its on-screen position, and uses that and the building's size to determine the building's position.
    /// </summary>
    /// <param name="backup">The value to return if the mouse is off the screen or something else fails.</param>
    /// <param name="xSize">The building's size along the x-axis (Building.XSize).</param>
    /// <param name="zSize">The building's size along the z-axis (Building.ZSize).</param>
    /// <returns>Snapped-to-grid building position.</returns>
    private Vector3 MousePositionToBuildingPosition(Vector3 backup, int xSize, int zSize)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {            
            return SnapBuildingToGrid(hit.point, xSize, zSize);
        }

        return backup;
    }

    /// <summary>
    /// Gets the position of the building based on the player's position and the offset according to the right analog stick's movement input, while keeping it within the player's field of view.
    /// </summary>
    /// <param name="xSize">The building's size along the x-axis (Building.XSize).</param>
    /// <param name="zSize">The building's size along the z-axis (Building.ZSize).</param>
    /// <returns>Snapped-to-grid building position.</returns>
    private Vector3 RawBuildingPositionToBuildingPosition(int xSize, int zSize)
    {
        Vector3 worldPos = transform.position + rawBuildingOffset;
        Vector3 newOffset = rawBuildingOffset + rawBuildingMovement * movementSpeed * Time.deltaTime;
        Vector3 newWorldPos = transform.position + newOffset;
        Vector3 newScreenPos = Camera.main.WorldToViewportPoint(newWorldPos);

        if (newScreenPos.x > 0 && newScreenPos.x < 1 && newScreenPos.y > 0 && newScreenPos.y < 1)
        {
            rawBuildingOffset = newOffset;
            worldPos = newWorldPos;
        }

        return SnapBuildingToGrid(worldPos, xSize, zSize);
    }

    /// <summary>
    /// Snaps the building's position to the grid of integer positions by rounding the X and Z values, and adjusting depending on the building's dimensions.
    /// </summary>
    /// <param name="pos">The position to be snapped-to-grid from.</param>
    /// <param name="xSize">The building's size along the x-axis (Building.XSize).</param>
    /// <param name="zSize">The building's size along the z-axis (Building.ZSize).</param>
    /// <returns>Snapped-to-grid building position.</returns>
    private Vector3 SnapBuildingToGrid(Vector3 pos, int xSize, int zSize)
    {
        pos.x = Mathf.Round(pos.x) + (xSize == 2 ? 0.5f : 0);
        pos.y = 0.67f;
        pos.z = Mathf.Round(pos.z) + (zSize == 2 ? 0.5f : 0);
        return pos;
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
            laserBolt.transform.parent = null;
            laserBolt.transform.position = laserCannonTip.position;
            laserBolt.Shoot((transform.forward * 2 - transform.up).normalized);
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Handles the destruction of laser bolts.
    /// </summary>
    /// <param name="laserBolt">The laser bolt to destroy.</param>
    public void DestroyLaserBolt(LaserBolt laserBolt)
    {
        laserBolt.Active = false;
        laserBolt.Rigidbody.isKinematic = true;
        laserBolt.transform.position = laserBatteryPoint.position;
        laserBolt.transform.parent = laserBatteryPoint;
        laserBattery.Add(laserBolt);
    }
}
