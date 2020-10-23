using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// A controller class for building spawning.
/// </summary>
public class BuildingSpawnController : SerializableSingleton<BuildingSpawnController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private bool printInputs;
    [SerializeField] private Camera camera;    

    //Non-Serialized Fields------------------------------------------------------------------------

    //Building variables
    private EBuilding selectedBuildingType;
    private ToolTips.Etooltips tooltip;
    private Building heldBuilding;
    private Vector3 rawBuildingMovement;

    //Spawning variables
    private bool cycleBuildingSelection;
    private bool cyclingBuildingSelection;
    private bool spawnBuilding;
    private bool placeBuilding;
    private bool cancelBuilding;
    private LayerMask groundLayerMask;

    //Other
    private DialogueBox console;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

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
    protected override void Awake()
    {
        base.Awake();
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
        console = DialogueBoxManager.Instance.GetDialogueBox("Console");
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
            if (!PauseMenuManager.Paused)
            {
                GetInput();
                CheckBuildingSpawning();
            }

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
        rawBuildingMovement = new Vector3(InputManager.Instance.GetAxis("LookLeftRight"), 0, InputManager.Instance.GetAxis("LookUpDown"));

        //Building Selection Input
        if (!cyclingBuildingSelection && InputManager.Instance.ButtonPressed("CycleBuilding"))
        {
            cyclingBuildingSelection = true;
            selectedBuildingType = InputManager.Instance.SelectBuilding(selectedBuildingType);
        }
        else if (cyclingBuildingSelection && !InputManager.Instance.ButtonPressed("CycleBuilding"))
        {
            cyclingBuildingSelection = false;
        }

        //Building Placement Input
        if (!spawnBuilding)
        {
            spawnBuilding = InputManager.Instance.ButtonPressed("SpawnBuilding");
        }
        else
        {
            placeBuilding = InputManager.Instance.ButtonPressed("PlaceBuilding");
            cancelBuilding = InputManager.Instance.ButtonPressed("CancelBuilding");

            if (printInputs)
            {
                Debug.Log($"Rewired via InputController, BuildingSpawningController.GetInput() (called by IEnumerator UpdateBuildingSpawning()), placeBuilding: {placeBuilding}");
                Debug.Log($"Rewired via InputController, BuildingSpawningController.GetInput() (called by IEnumerator UpdateBuildingSpawning()), cancelBuilding: {cancelBuilding}");
            }
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
                heldBuilding = BuildingFactory.Instance.Get(selectedBuildingType);
                heldBuilding.transform.position = MousePositionToPotentialBuildingPosition(transform.position, heldBuilding.Size.DiameterRoundedUp(null));
                ChangeTooltip(selectedBuildingType);
                //Debug.Log($"BuildingSpawningController(), new heldBuilding ({heldBuilding}) (from null), building collider position is {heldBuilding.Collider.position} (world) / {heldBuilding.Collider.localPosition} (local), building model position is {heldBuilding.Model.position} (world) / {heldBuilding.Model.localPosition} (local)");

                // put tooltip spawn here
            }
            //Instantiate the appropriate building and postion it properly, replacing the old one.
            else if (heldBuilding.BuildingType != selectedBuildingType)
            {
                Vector3 pos = MousePositionToPotentialBuildingPosition(heldBuilding.transform.position, heldBuilding.Size.DiameterRoundedUp(null));
                BuildingFactory.Instance.Destroy(heldBuilding, false, false);
                heldBuilding = BuildingFactory.Instance.Get(selectedBuildingType);
                heldBuilding.transform.position = pos;
                //Debug.Log($"BuildingSpawningController(), new heldBuilding ({heldBuilding}) (from update), building collider position is {heldBuilding.Collider.position} (world) / {heldBuilding.Collider.localPosition} (local), building model position is {heldBuilding.Model.position} (world) / {heldBuilding.Model.localPosition} (local)");

                ChangeTooltip(selectedBuildingType);
               
                // hide and then show new tooltip
            }
            else //Move the building where you want it
            {
                heldBuilding.transform.position = MousePositionToPotentialBuildingPosition(heldBuilding.transform.position, heldBuilding.Size.DiameterRoundedUp(null));
                //Debug.Log($"BuildingSpawningController(), update heldBuilding ({heldBuilding}) position, building collider position is {heldBuilding.Collider.position} (world) / {heldBuilding.Collider.localPosition} (local), building model position is {heldBuilding.Model.position} (world) / {heldBuilding.Model.localPosition} (local)");
            }

            //Debug.Log($"BuildingSpawningController.CheckBuildingSpawning(), {heldBuilding}.transform.position is {heldBuilding.transform.position}");

            bool placementValid = heldBuilding.IsPlacementValid();
            bool resourcesAvailable = CheckResourcesAvailable();
            //TODO: check if all the methods below should be asking for "radius" or "diameter"

            //Place it or cancel building it
            if (placeBuilding && resourcesAvailable && placementValid)
            {
                //Debug.Log($"BuildingSpawningController(), placeBuilding successful ({heldBuilding}) (start), building collider position is {heldBuilding.Collider.position} (world) / {heldBuilding.Collider.localPosition} (local), building model position is {heldBuilding.Model.position} (world) / {heldBuilding.Model.localPosition} (local)");
                console.SubmitCustomMessage($"Placement successful. Constructing {heldBuilding.ConsoleName}.", false, 0);

                Vector3 spawnPos = heldBuilding.transform.position;
                spawnPos.y = 0.02f;
                PipeManager.Instance.RegisterPipeBuilding(spawnPos);
                spawnPos.y = GetStandardisedPlacementHeight(spawnPos, true);
                //Debug.Log($"BuildingSpawningController(), placeBuilding ({heldBuilding}) successful (ready to place), new spawnPos is {spawnPos}, building collider position is {heldBuilding.Collider.position} (world) / {heldBuilding.Collider.localPosition} (local), building model position is {heldBuilding.Model.position} (world) / {heldBuilding.Model.localPosition} (local)");
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
                    string errorMessage = $"<Cannot build {heldBuilding.ConsoleName}.>";

                    if (!placementValid)
                    {
                        errorMessage += "~<- Invalid location.>";
                    }

                    if (ResourceManager.Instance.Ore < heldBuilding.OreCost)
                    {
                        errorMessage += "~<- Insufficient +minerals&.>";
                    }

                    if (ResourceManager.Instance.PowerSupply < ResourceManager.Instance.PowerConsumption + heldBuilding.PowerConsumption)
                    {
                        errorMessage += "~<- Insufficient [power].>";
                    }

                    if (ResourceManager.Instance.WaterSupply < ResourceManager.Instance.WaterConsumption + heldBuilding.WaterConsumption)
                    {
                        errorMessage += "~<- Insufficient /water\\.>";
                    }

                    if (ResourceManager.Instance.PlantsSupply < ResourceManager.Instance.PlantsConsumption + heldBuilding.PlantsConsumption)
                    {
                        errorMessage += "~<- Insufficient {plants}.>";
                    }

                    if (ResourceManager.Instance.PlantsSupply < ResourceManager.Instance.PlantsConsumption + heldBuilding.GasConsumption)
                    {
                        errorMessage += "~<- Insufficient @gas$.>";
                    }

                    console.SubmitCustomMessage(errorMessage, true, 0);
                }

                BuildingFactory.Instance.Destroy(heldBuilding, false, false);
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
    private Vector3 MousePositionToPotentialBuildingPosition(Vector3 backup, int radius)
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
        Vector3 newOffset = rawBuildingMovement * PODController.Instance.MovementSpeed * Time.deltaTime;
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
        //float errorMargin = 0.01f;
        Vector3 raycastPos = new Vector3(pos.x, 4, pos.z);
        RaycastHit hit;

        if (Physics.Raycast(raycastPos, Vector3.down, out hit, 20, groundLayerMask))
        {
            ////Debug.Log($"BuildingSpawningController.GetStandardisedPlacementHeight() raycast from {raycastPos} hit {hit.collider} at {hit.point}");

            //if (hit.point.y >= 2.5f - errorMargin)
            //{
            //    //Debug.Log($"BuildingSpawningController.GetStandardisedPlacementHeight(), hit at ~2.5f, setting height to 2.5f");
            //    result = 2.5f;
            //}
            //else if (hit.point.y >= -errorMargin)
            //{
            //    //Debug.Log($"BuildingSpawningController.GetStandardisedPlacementHeight(), hit at ~0f, setting height to 0f");
            //    result = 0;
            //}
            //else if (hit.point.y >= -2.5f - errorMargin)
            //{
            //    //Debug.Log($"BuildingSpawningController.GetStandardisedPlacementHeight(), hit at ~-2.5f, setting height to -2.5f");
            //    result = -2.5f;
            //}
            //else
            //{
            //    Debug.LogError($"{this}.SnapBuildingToGrid() cannot account for a screen-to-ground raycast of height-snapped position {raycastPos}. RaycastHit.point is {hit.point}");
            //}

            result = hit.point.y;
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
        return ResourceManager.Instance.Ore >= heldBuilding.OreCost
            && (ResourceManager.Instance.PowerSupply >= ResourceManager.Instance.PowerConsumption + heldBuilding.PowerConsumption || heldBuilding.PowerConsumption == 0)
            && (ResourceManager.Instance.PlantsSupply >= ResourceManager.Instance.PlantsConsumption + heldBuilding.PlantsConsumption || heldBuilding.PlantsConsumption == 0)
            && (ResourceManager.Instance.WaterSupply >= ResourceManager.Instance.WaterConsumption + heldBuilding.WaterConsumption || heldBuilding.WaterConsumption == 0)
            && (ResourceManager.Instance.GasSupply >= ResourceManager.Instance.GasConsumption + heldBuilding.GasConsumption || heldBuilding.GasConsumption == 0);
    }

    /// <summary>
    /// Change the active tooltip.
    /// </summary>
    /// <param name="selectedBuilding">The building the player has selected to build.</param>
    private void ChangeTooltip(EBuilding selectedBuilding)
    {
        switch(selectedBuilding)
        {
            case EBuilding.FusionReactor:
                tooltip = ToolTips.Etooltips.FusionReactor;
                break;
            case EBuilding.IceDrill:
                tooltip = ToolTips.Etooltips.IceDrill;
                break;
            case EBuilding.Greenhouse:
                tooltip = ToolTips.Etooltips.Greenhouse;
                break;
            case EBuilding.Boiler:
                tooltip = ToolTips.Etooltips.Boiler;
                break;
            case EBuilding.Incinerator:
                tooltip = ToolTips.Etooltips.Incinerator;
                break;
            case EBuilding.ShotgunTurret:
                tooltip = ToolTips.Etooltips.Shotgun;
                break;
            case EBuilding.MachineGunTurret:
                tooltip = ToolTips.Etooltips.MachineGun;
                break;
            default:
                break;
        }
    }
}
