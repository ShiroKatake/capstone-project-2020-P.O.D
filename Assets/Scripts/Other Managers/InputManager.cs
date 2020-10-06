using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

/// <summary>
/// A manager class for getting the right input values from the player's current input device(s) without having to specify the device-specific input for what you're after.
/// </summary>
public class InputManager : SerializableSingleton<InputManager>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Player Selection Settings")]
    [SerializeField] private int playerID = 0;
    [SerializeField] private Rewired.Player player;

    [Header("Building Buttons")]
    [SerializeField] private Button fusionReactor;
    [SerializeField] private Button iceDrill;
    [SerializeField] private Button harvester;
    [SerializeField] private Button gasPump;
    [SerializeField] private Button boiler;
    [SerializeField] private Button greenhouse;
    [SerializeField] private Button incinerator;
    [SerializeField] private Button shotgunTurret;
    [SerializeField] private Button machineGunTurret;

    [Header("ButtonsClickEventManagers")]
    [SerializeField] private List<ButtonClickEventManager> buttonClickEventManagers;
    // needs to be renamed...
    [SerializeField] private GameObject buildingUIParent;

    //Non-Serialized Fields------------------------------------------------------------------------

    //ClickedButton
    private ButtonClickEventManager clickedButton;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        buttonClickEventManagers = new List<ButtonClickEventManager>(buildingUIParent.GetComponentsInChildren<ButtonClickEventManager>());
        player = ReInput.players.GetPlayer(playerID);
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///  Checks if the player has pressed the specified button.
    /// </summary>
    /// <param name="requestedInput">The input or button to check.</param>
    /// <returns>Was a button pressed?</returns>
    public bool ButtonPressed(string requestedInput)
    {
        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "CycleWeapon":
                return player.GetButton("SwapWeapon");
            case "Pause":
                return player.GetButton("Pause");

            case "Shoot":
                return player.GetButton("Shoot");
            case "MoveLeftRight":
                return player.GetButton("Horizontal");
            case "MoveForwardsBackwards":
                return player.GetButton("Vertical");

            //Aliased
            case "PlaceBuilding":
                return player.GetButton("Submit");
            case "CancelBuilding":
                return player.GetButton("Cancel");

            //Custom
            case "CycleBuilding":
            case "SpawnBuilding":
                return (player.GetButton("Reactor") && fusionReactor.interactable)
                    || (player.GetButton("IceDrill") && iceDrill.interactable)
                    || (player.GetButton("Harvester") && harvester.interactable)
                    || (player.GetButton("GasPump") && gasPump.interactable)
                    || (player.GetButton("Boiler") && boiler.interactable)
                    || (player.GetButton("GreenHouse") && greenhouse.interactable)
                    || (player.GetButton("Incinerator") && incinerator.interactable)
                    || (player.GetButton("ShotgunTurret") && shotgunTurret.interactable)
                    || (player.GetButton("MachineGunTurret") && machineGunTurret.interactable)
                    || CheckUIButtonClicked();      

            //Unknown input
            default:
                return false;
        }
    }

    /// <summary>
    ///  Checks if the player is holding the specified button or input down.
    /// </summary>
    /// <param name="requestedInput">The input or button to check.</param>
    /// <returns>Was a button held?</returns>
    public bool ButtonHeld(string requestedInput)
    {
        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "CycleWeapon":
                return player.GetButton("SwapWeapon");
            case "Pause":
                return player.GetButton("Pause");

            //Button for MK, axis for XB and DS
            case "Shoot":
                return player.GetButton("Shoot");
            case "MoveLeftRight":
                return player.GetButton("Horizontal");
            case "MoveForwardsBackwards":
                return player.GetButton("Vertical");

            //Aliased
            case "PlaceBuilding":
                return player.GetButton("Submit");
            case "CancelBuilding":
                return player.GetButton("Cancel");

            //Custom
            case "CycleBuilding":
            case "SpawnBuilding":
                return player.GetButton("Reactor")
                    || player.GetButton("IceDrill")
                    || player.GetButton("Harvester")
                    || player.GetButton("GasPump")
                    || player.GetButton("Boiler")
                    || player.GetButton("GreenHouse")
                    || player.GetButton("Incinerator")
                    || player.GetButton("ShotgunTurret")
                    || player.GetButton("MachineGunTurret")
                    || CheckUIButtonClicked();

            //Unknown input
            default:
                return false;
        }
    }

    
    /// <summary>
    ///  Check if player is moving or looking; if axes are button pairs, returns integer value of -1, 0 or 1; 
    ///  if axes are mouse / analog stick axes, returns float value between -1 and 1
    /// </summary>
    /// <param name="requestedInput">The input or axis to check.</param>
    /// <returns>The magnitude of any input on the requested axis.</returns>
    public float GetAxis(string requestedInput)
    {

        switch (requestedInput)
        {
            //case "MoveUpDown":
            case "Shoot":
            case "MoveForwardsBackwards":
            case "MoveLeftRight":
            case "LookUpDown":
                return player.GetAxis("Vertical");
            case "LookLeftRight":
                return player.GetAxis("Horizontal");

            //Unknown input
            default:
                return 0f;
        }
    }

    //Building Type Selection------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks the inputs for selecting a building type.
    /// </summary>
    /// <param name="currentSelection">The currently-selected building type in case no other building type has been selected.</param>
    /// <returns>The selected building type, or the current selection if none.</returns>
    public EBuilding SelectBuilding(EBuilding currentSelection)
    {
        if (clickedButton != null)
        {
            return clickedButton.GetBuildingType;
        }

        if (player.GetButton("Reactor"))
        {
            return EBuilding.FusionReactor;
        }

        if (player.GetButton("IceDrill"))
        {
            return EBuilding.IceDrill;
        }

        if (player.GetButton("Harvester"))
        {
            return EBuilding.Harvester;
        }

        if (player.GetButton("GasPump"))
        {
            return EBuilding.GasPump;
        }

        if (player.GetButton("Boiler"))
        {
            return EBuilding.Boiler;
        }

        if (player.GetButton("GreenHouse"))
        {
            return EBuilding.Greenhouse;
        }

        if (player.GetButton("Incinerator"))
        {
            return EBuilding.Incinerator;
        }

        if (player.GetButton("ShotgunTurret"))
        {
            return EBuilding.ShotgunTurret;
        }

        if (player.GetButton("MachineGunTurret"))
        {
            return EBuilding.MachineGunTurret;
        }

        return currentSelection;
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if a UI button for building was clicked by the player.
    /// </summary>
    /// <returns>Was a UI button for building clicked by the player?</returns>
    private bool CheckUIButtonClicked()
    {
        foreach (ButtonClickEventManager b in buttonClickEventManagers)
        {
            if (b.Clicked)
            {
                clickedButton = b;
                return true;
            }
        }

        clickedButton = null;
        return false;
    }
}
