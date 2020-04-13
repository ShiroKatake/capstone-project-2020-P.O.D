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

    //Variables for Terraformer Spawning
    private EBuilding selectedBuildingType;
    private Building heldBuilding;
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
            laserBattery.Add(Instantiate<LaserBolt>(laserBoltPrefab, laserBatteryPoint.position, laserBoltPrefab.transform.rotation));
        }

        selectedBuildingType = EBuilding.SolarPanel;
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
        
        Debug.Log($"Selected building type: {selectedBuildingType}, spawning: {spawnBuilding}, placing: {placeBuilding}, canceling: {cancelBuilding}.");

        //if (heldBuildingType != EBuilding.None)
        //{
        //    if (!holdingBuilding)
        //    {
        //        holdingBuilding = true;
        //        spawnBuilding = true;
        //    }
        //}
        //else if (holdingBuilding)
        //{
        //    holdingBuilding = false;
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
    /// Moves the player based on their input.
    /// </summary>
    private void Move()
    {
        //Player wants to move? Move the drone.
        if (heldBuilding == null)
        {
            if (movement != Vector3.zero)
            {
                drone.transform.Translate(new Vector3(0, 0, movementSpeed * movement.magnitude * Time.deltaTime), Space.Self);
                cameraTarget.transform.position = drone.transform.position;
            }
        }
        else
        {
            //TODO: if x and z position values don't place the drone squarely on a square, move it to the centre of the nearest square.
            //If x or z is an even distance between one square and another, favour reversing to the right.
        }
    }

    /// <summary>
    /// Checks if the player wants to spawn a building.
    /// </summary>
    private void CheckBuildingSpawning()
    {
        if (spawnBuilding)
        {
            //Instantiate the appropriate building
            if (heldBuilding == null)
            {
                heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);

                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
                {
                    heldBuilding.transform.position = GetMousePositionInWorld();
                }
                else
                {
                    heldBuilding.transform.position = transform.position + heldBuilding.GetOffset(transform.rotation.eulerAngles.y);//TODO: snap to grid based on player's snap to grid position, not their actual position.
                }
            }
            else if (heldBuilding.BuildingType != selectedBuildingType)
            {
                Vector3 pos;

                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
                {
                    pos = GetMousePositionInWorld();
                }
                else
                {
                    pos = heldBuilding.transform.position /* plus movement according to the right analog stick*/;//TODO: snap to grid based on player's snap to grid position, not their actual position.
                }

                BuildingFactory.Instance.DestroyBuilding(heldBuilding);
                heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);
                heldBuilding.transform.position = pos;
            }
            else //Move it where you want it
            {
                //TODO: snap to grid based on player's snap to grid position, not their actual position.
                if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
                {
                    heldBuilding.transform.position = GetMousePositionInWorld();
                }
                else
                {
                    heldBuilding.transform.position = transform.position + heldBuilding.GetOffset(transform.rotation.eulerAngles.y);//TODO: snap to grid based on player's snap to grid position, not their actual position.
                }

                //TODO: differentiate between mouse and keyboard positioning and controller positioning
                //Mouse and keyboard: follow the mouse, but keep within the screen bounds
                //Controller: move according to the right analog stick, but keep within the screen bounds

                //TODO: check for building collisions with drone / other buildings / enemies.
            }

            //Place it or cancel building it
            if (placeBuilding) //and there's not a collision
            {
                Vector3 spawnPos = heldBuilding.transform.position;
                spawnPos.y = 0.5f;
                heldBuilding.transform.position = spawnPos;
                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;
            }
            else if (cancelBuilding) // or place building but there's a collision
            {
                BuildingFactory.Instance.DestroyBuilding(heldBuilding);
                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;
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

    private Vector3 GetMousePositionInWorld()
    {
        //RaycastHit hit;
        //Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        //{
        //    return hit.point;
        //}
        //    return Vector3.zero;

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }

        return Vector3.zero;

        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //worldPos.y = 0.67f;
        //Debug.Log($"Mouse position in world is {worldPos}");
        //return worldPos;
    }
}
