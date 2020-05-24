using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A controller class for building spawning.
/// </summary>
public class BuildingSpawningController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private Camera camera;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Building fields
    private EBuilding selectedBuildingType;
    private Building heldBuilding;
    private Vector3 rawBuildingMovement;

    //Spawning bools
    private bool cycleBuildingSelection;
    private bool cyclingBuildingSelection;
    private bool spawnBuilding;
    private bool placeBuilding;
    private bool cancelBuilding;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// BuildingSpawningController's singleton public property.
    /// </summary>
    public static BuildingSpawningController Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one BuildingSpawningController.");
        }

        Instance = this;
        cycleBuildingSelection = false;
        cyclingBuildingSelection = false;
        spawnBuilding = false;
        placeBuilding = false;
        cancelBuilding = false;
        selectedBuildingType = EBuilding.FusionReactor;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        StartCoroutine(UpdateBuildingSpawning());
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// UpdateBuildingSpawning() acts as an Update() method for BuildingSpawningController that doesn't run on the first frame, but runs every frame after that.
    /// </summary>
    private IEnumerator UpdateBuildingSpawning()
    {
        yield return null;

        while (true)
        {
            GetInput();
            CheckBuildingSpawning();
            yield return null;
        }
    }

    //Recurring Methods (UpdateBuildingSpawning())--------------------------------------------------------------------------------------------------- 


    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        //Building Movement Input
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
                    heldBuilding.transform.position = MousePositionToBuildingPosition(transform.position, heldBuilding.XSize, heldBuilding.ZSize);
                }
                else
                {
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

                BuildingFactory.Instance.DestroyBuilding(heldBuilding, false, false);
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
            if (placeBuilding && ResourceController.Instance.Ore >= heldBuilding.OreCost && !collision && MapController.Instance.PositionAvailableForBuilding(heldBuilding))
            {              
                Vector3 spawnPos = heldBuilding.transform.position;
                spawnPos.y = 0.02f;
                PipeManager.Instance.RegisterPipeBuilding(spawnPos);
                spawnPos.y = 0.5f;
                heldBuilding.Place(spawnPos);    

                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;
            }
            else if (cancelBuilding || (placeBuilding && (ResourceController.Instance.Ore < heldBuilding.OreCost || collision || !MapController.Instance.PositionAvailableForBuilding(heldBuilding))))
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
                    else if (!MapController.Instance.PositionAvailableForBuilding(heldBuilding))
                    {
                        Debug.Log("You cannot place a building there; it would either occupy the same space as something else, or exceed the bounds of the map.");
                    }

                }

                BuildingFactory.Instance.DestroyBuilding(heldBuilding, false, false);
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
        Vector3 worldPos = transform.position;
        Vector3 newOffset = rawBuildingMovement * Player.Instance.MovementSpeed * Time.deltaTime;
        Vector3 newWorldPos = transform.position + newOffset;
        Vector3 newScreenPos = Camera.main.WorldToViewportPoint(newWorldPos);

        if (newScreenPos.x > 0 && newScreenPos.x < 1 && newScreenPos.y > 0 && newScreenPos.y < 1)
        {
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
}
