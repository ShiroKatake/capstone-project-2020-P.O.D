using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

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
    private LayerMask groundLayerMask;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// BuildingSpawningController's singleton public property.
    /// </summary>
    public static BuildingSpawningController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Is the player in the middle of spawning a building or not?
    /// </summary>
    public bool SpawningBuilding { get => spawnBuilding; }

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
        groundLayerMask = LayerMask.GetMask("Ground");
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
                heldBuilding.transform.position = MousePositionToBuildingPosition(transform.position, heldBuilding.Size.DiameterRoundedUp);
            }
            //Instantiate the appropriate building and postion it properly, replacing the old one.
            else if (heldBuilding.BuildingType != selectedBuildingType)
            {
                Vector3 pos;
                pos = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.Size.DiameterRoundedUp);
                BuildingFactory.Instance.DestroyBuilding(heldBuilding, false, false);
                heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);
                heldBuilding.transform.position = pos;
            }
            else //Move the building where you want it
            {
                heldBuilding.transform.position = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.Size.DiameterRoundedUp);
            }

            bool placementValid = heldBuilding.IsPlacementValid();
            bool resourcesAvailable = CheckResourcesAvailable();
            //TODO: check if all the methods below should be asking for "radius" or "diameter"

            //Place it or cancel building it
            if (placeBuilding && resourcesAvailable && placementValid)
            {
                Vector3 spawnPos = heldBuilding.transform.position;
                spawnPos.y = 0.02f;
                PipeManager.Instance.RegisterPipeBuilding(spawnPos);
                spawnPos.y = GetStandardisedPlacementHeight(spawnPos, true);
                heldBuilding.Place(spawnPos);

                heldBuilding = null;
                spawnBuilding = false;
                placeBuilding = false;
                cancelBuilding = false;
            }
            else if (cancelBuilding || (placeBuilding && (!resourcesAvailable || !placementValid)))
            {
                if (placeBuilding)
                {
                    AudioManager.Instance.PlaySound(AudioManager.ESound.Negative_UI);

                    if (ResourceController.Instance.Ore < heldBuilding.OreCost)
                    {
                        Debug.Log("You have insufficient ore to build this building.");
                    }

                    if (ResourceController.Instance.PowerSupply < ResourceController.Instance.PowerConsumption + heldBuilding.PowerConsumption)
                    {
                        Debug.Log("You have insufficient power to maintain this building.");
                    }

                    if (ResourceController.Instance.WasteSupply < ResourceController.Instance.WasteConsumption + heldBuilding.WasteConsumption)
                    {
                        Debug.Log("You have insufficient water to maintain this building.");
                    }

                    if (ResourceController.Instance.WaterSupply < ResourceController.Instance.WaterConsumption + heldBuilding.WaterConsumption)
                    {
                        Debug.Log("You have insufficient waste to maintain this building.");
                    }

                    if (!placementValid)
                    {
                        Debug.Log("You cannot place a building there; it would occupy the same space as something else, or exceed the bounds of the map.");
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
    /// <param name="radius">The building's radius.</param>
    /// <returns>Snapped-to-grid building position.</returns>
    private Vector3 MousePositionToBuildingPosition(Vector3 backup, int radius)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(ReInput.controllers.Mouse.screenPosition);

        if (Physics.Raycast(ray, out hit, 200, groundLayerMask))
        {
            return SnapBuildingToGrid(hit.point, radius);
        }

        return backup;
    }

    /// <summary>
    /// Gets the position of the building based on the player's position and the offset according to the right analog stick's movement input, while keeping it within the player's field of view.
    /// </summary>
    /// <param name="radius">The building's radius.</param>
    /// <returns>Snapped-to-grid building position.</returns>
    private Vector3 RawBuildingPositionToBuildingPosition(int radius)
    {
        Vector3 worldPos = transform.position;
        Vector3 newOffset = rawBuildingMovement * PlayerMovementController.Instance.MovementSpeed * Time.deltaTime;
        Vector3 newWorldPos = transform.position + newOffset;
        Vector3 newScreenPos = Camera.main.WorldToViewportPoint(newWorldPos);

        if (newScreenPos.x > 0 && newScreenPos.x < 1 && newScreenPos.y > 0 && newScreenPos.y < 1)
        {
            worldPos = newWorldPos;
        }

        return SnapBuildingToGrid(worldPos, radius);
    }

    /// <summary>
    /// Snaps the building's position to the grid of integer positions by rounding the X and Z values, and adjusting depending on the building's dimensions.
    /// </summary>
    /// <param name="pos">The position to be snapped-to-grid from.</param>
    /// <param name="radius">The building's "radius".</param>
    /// <returns>Snapped-to-grid building position.</returns>
    private Vector3 SnapBuildingToGrid(Vector3 pos, int radius)
    {
        pos.x = Mathf.Round(pos.x) + (radius == 2 ? 0.5f : 0);
        pos.z = Mathf.Round(pos.z) + (radius == 2 ? 0.5f : 0);
        pos.y = GetStandardisedPlacementHeight(pos, false);
        return pos;
    }

    /// <summary>
    /// Snaps the y component of the position to the height of the proper area of the terrain.
    /// </summary>
    /// <param name="pos">The building's un-snapped position.</param>
    /// <param name="placed">Has the building been placed.</param>
    /// <returns>The terrain-snapped y component of the building's position.</returns>
    private float GetStandardisedPlacementHeight(Vector3 pos, bool placed)
    {
        float result = 3;
        float errorMargin = 0.01f;
        Vector3 raycastPos = new Vector3(pos.x, 3, pos.z);
        RaycastHit hit;

        if (Physics.Raycast(raycastPos, Vector3.down, out hit, 20, groundLayerMask))
        {
            //Debug.Log($"BuildingSpawningController.GetStandardisedPlacementHeight() raycast from {raycastPos} hit {hit.collider} at {hit.point}");

            if (hit.point.y >= 2.5f - errorMargin)
            {
                result = 2.5f;
            }
            else if (hit.point.y >= -errorMargin)
            {
                result = 0;
            }
            else if (hit.point.y >= -2.5f - errorMargin)
            {
                result = -2.5f;
            }
            else
            {
                Debug.LogError($"{this}.SnapBuildingToGrid() cannot account for a screen-to-ground raycast of height-snapped position {raycastPos}. RaycastHit.point is {hit.point}");
            }
        }
        else
        {
            Debug.LogError($"{this}.SnapBuildingToGrid() cannot account for a screen-to-ground raycast of non height-snapped hit position {pos}");
        }

        result += placed ? 0.5f : 0.67f;
        return result;
    }

    /// <summary>
    /// Checks if there are enough resources available to build and maintain this building.
    /// </summary>
    /// <returns>Whether there are enough resources available to build and maintain this building.</returns>
    private bool CheckResourcesAvailable()
    {
        return ResourceController.Instance.Ore >= heldBuilding.OreCost
            && ResourceController.Instance.PowerSupply >= ResourceController.Instance.PowerConsumption + heldBuilding.PowerConsumption
            && ResourceController.Instance.WasteSupply >= ResourceController.Instance.WasteConsumption + heldBuilding.WasteConsumption
            && ResourceController.Instance.WaterSupply >= ResourceController.Instance.WaterConsumption + heldBuilding.WaterConsumption;
    }
}
