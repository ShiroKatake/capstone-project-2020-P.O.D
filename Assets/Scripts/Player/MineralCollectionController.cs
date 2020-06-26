using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player script for collecting minerals.
/// </summary>
public class MineralCollectionController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Mineral Collection Stats")]
    [SerializeField] private int collectionCount;
    [SerializeField] private int collectionCooldown;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool collectMinerals;
    private bool collectingMinerals;

    private float timeOfLastCollection;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// MineralCollectionController's singleton public property.
    /// </summary>
    public static MineralCollectionController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          



    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one MineralCollectionController.");
        }

        Instance = this;
        timeOfLastCollection = -1 * collectionCooldown;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        GetInput();
        CollectMinerals();
    }

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Gets the player's input from the keyboard and mouse / gamepad they're using.
    /// </summary>
    private void GetInput()
    {
        //Mineral Collection Input
        collectMinerals = InputController.Instance.ButtonHeld("CollectMinerals");
    }

    /// <summary>
    /// Checks if the player wants to spawn a building, and spawns and moves it if they do.
    /// </summary>
    private void CollectMinerals()
    {
        if (collectMinerals)
        {
            ////Instantiate the appropriate building, positioning it properly.
            //if (heldBuilding == null)
            //{
            //    heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);

            //    if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
            //    {
            //        heldBuilding.transform.position = MousePositionToBuildingPosition(transform.position, heldBuilding.Size.DiameterRoundedUp);// heldBuilding.XSize, heldBuilding.ZSize);
            //    }
            //    else
            //    {
            //        heldBuilding.transform.position = RawBuildingPositionToBuildingPosition(heldBuilding.Size.DiameterRoundedUp);// heldBuilding.XSize, heldBuilding.ZSize);
            //    }
            //}
            ////Instantiate the appropriate building and postion it properly, replacing the old one.
            //else if (heldBuilding.BuildingType != selectedBuildingType)
            //{
            //    Vector3 pos;

            //    if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
            //    {
            //        pos = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.Size.DiameterRoundedUp);// heldBuilding.XSize, heldBuilding.ZSize);
            //    }
            //    else
            //    {
            //        pos = RawBuildingPositionToBuildingPosition(heldBuilding.Size.DiameterRoundedUp);// heldBuilding.XSize, heldBuilding.ZSize);
            //    }

            //    BuildingFactory.Instance.DestroyBuilding(heldBuilding, false, false);
            //    heldBuilding = BuildingFactory.Instance.GetBuilding(selectedBuildingType);
            //    heldBuilding.transform.position = pos;
            //}
            //else //Move the building where you want it
            //{
            //    if (InputController.Instance.Gamepad == EGamepad.MouseAndKeyboard)
            //    {
            //        heldBuilding.transform.position = MousePositionToBuildingPosition(heldBuilding.transform.position, heldBuilding.Size.DiameterRoundedUp);// heldBuilding.XSize, heldBuilding.ZSize);
            //    }
            //    else
            //    {
            //        heldBuilding.transform.position = RawBuildingPositionToBuildingPosition(heldBuilding.Size.DiameterRoundedUp);// heldBuilding.XSize, heldBuilding.ZSize);
            //    }
            //}

            //bool collision = heldBuilding.CollisionUpdate();

            ////Place it or cancel building it
            //if (placeBuilding && ResourceController.Instance.Ore >= heldBuilding.OreCost && !collision && MapController.Instance.PositionAvailableForBuilding(heldBuilding))
            //{
            //    Vector3 spawnPos = heldBuilding.transform.position;
            //    spawnPos.y = 0.02f;
            //    PipeManager.Instance.RegisterPipeBuilding(spawnPos);
            //    spawnPos.y = 0.5f;
            //    heldBuilding.Place(spawnPos);

            //    heldBuilding = null;
            //    spawnBuilding = false;
            //    placeBuilding = false;
            //    cancelBuilding = false;
            //}
            //else if (cancelBuilding || (placeBuilding && (ResourceController.Instance.Ore < heldBuilding.OreCost || collision || !MapController.Instance.PositionAvailableForBuilding(heldBuilding))))
            //{
            //    if (placeBuilding)
            //    {
            //        if (ResourceController.Instance.Ore < heldBuilding.OreCost)
            //        {
            //            Debug.Log("You have insufficient ore to build this building.");
            //        }

            //        if (collision)
            //        {
            //            Debug.Log("You cannot place a building there; it would occupy the same space as something else.");
            //        }
            //        else if (!MapController.Instance.PositionAvailableForBuilding(heldBuilding))
            //        {
            //            Debug.Log("You cannot place a building there; it would either occupy the same space as something else, or exceed the bounds of the map.");
            //        }

            //    }

            //    BuildingFactory.Instance.DestroyBuilding(heldBuilding, false, false);
            //    heldBuilding = null;
            //    spawnBuilding = false;
            //    placeBuilding = false;
            //    cancelBuilding = false;
            //}
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

}
