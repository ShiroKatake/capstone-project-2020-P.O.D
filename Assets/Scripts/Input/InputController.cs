using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

/// <summary>
/// A manager class for getting the right input values from the player's current input device(s) without having to specify the device-specific input for what you're after.
/// </summary>
public class InputController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Settings")]
    //dont think these fields are necessary anymore
    [SerializeField] private EGamepad gamepad;
    [SerializeField] private EOperatingSystem operatingSystem;

    [Header("Player Selection Settings")]
    [SerializeField] private int playerID = 0;
    [SerializeField] private Rewired.Player player;

    [Header("Building Buttons")]
    [SerializeField] private Button fusionReactor;
    [SerializeField] private Button iceDrill;
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

    // probably not needed with the new rewired system
    //Prefixes
    private string gamepadPrefix;
    private string osPrefix;

    //ClickedButton
    private ButtonClickEventManager clickedButton;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// InputController's singleton public property.
    /// </summary>
    public static InputController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    // probably not needed with rewired...
    /// <summary>
    /// The input device(s) the player is using.
    /// </summary>
    public EGamepad Gamepad { get => gamepad; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more InputControllers.");
        }

        Instance = this;

        switch (gamepad)
        {
            case EGamepad.XboxController:
                gamepadPrefix = "XB";
                break;
            case EGamepad.DualShockController:
                gamepadPrefix = "DS";
                break;
            case EGamepad.MouseAndKeyboard:
            default:
                gamepadPrefix = "MK";
                break;
        }

        switch (operatingSystem)
        {
            case EOperatingSystem.Mac:
                osPrefix = "M";
                break;
            case EOperatingSystem.Windows:
            default:
                osPrefix = "W";
                break;
        }

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
        /*if (gamepadPrefix == "")
        {
            return false;
        }*/

        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "CycleWeapon":
                return player.GetButton("SwapWeapon");
            case "Pause":
                return player.GetButton("Pause");
                //return Input.GetButtonDown(gamepadPrefix + requestedInput);

            //Button for MK, axis for XB and DS
            case "Shoot":
                return player.GetButton("Shoot");
            case "MoveLeftRight":
                return player.GetButton("Horizontal");
            case "MoveForwardsBackwards":
                return player.GetButton("Vertical");
                /*if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButtonDown($"MK{requestedInput}");
                }
                else
                {
                    return GetAxis(requestedInput) != 0;
                }*/

            //Always an axis for MK, XB and DS
            //case "LookUpDown":
            //case "LookLeftRight":
            //    return GetAxis(requestedInput) != 0;

            //Aliased
            case "PlaceBuilding":
                return player.GetButton("Submit");
                //return Input.GetButtonDown(gamepadPrefix + "Submit");
            case "CancelBuilding":
                return player.GetButton("Cancel");
                //return Input.GetButtonDown(gamepadPrefix + "Cancel");

            //Custom
            case "CycleBuilding":
                return (player.GetButton("Reactor") && fusionReactor.interactable)
                    || (player.GetButton("IceDrill") && iceDrill.interactable)
                    || (player.GetButton("Boiler") && boiler.interactable)
                    || (player.GetButton("GreenHouse") && greenhouse.interactable)
                    || (player.GetButton("Incinerator") && incinerator.interactable)
                    || (player.GetButton("Turret1") && shotgunTurret.interactable)
                    || (player.GetButton("Turret2") && machineGunTurret.interactable)
                    || CheckUIButtonClicked();
                /*if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButtonDown("MKSpawnFusionReactor")
                        || Input.GetButtonDown("MKSpawnIceDrill")
                        || Input.GetButtonDown("MKSpawnBoiler")
                        || Input.GetButtonDown("MKSpawnGreenhouse")
                        || Input.GetButtonDown("MKSpawnIncinerator")
                        || Input.GetButtonDown("MKSpawnShortRangeTurret")
                        || Input.GetButtonDown("MKSpawnLongRangeTurret")
                        || CheckUIButtonClicked();
                }
                else
                {
                    return Input.GetButtonDown(gamepadPrefix + "CycleBuildingLeft")
                        || Input.GetButtonDown(gamepadPrefix + "CycleBuildingRight");
                }*/

            case "SpawnBuilding":
                return (player.GetButton("Reactor") && fusionReactor.interactable)
                    || (player.GetButton("IceDrill") && iceDrill.interactable)
                    || (player.GetButton("Boiler") && boiler.interactable)
                    || (player.GetButton("GreenHouse") && greenhouse.interactable)
                    || (player.GetButton("Incinerator") && incinerator.interactable)
                    || (player.GetButton("Turret1") && shotgunTurret.interactable)
                    || (player.GetButton("Turret2") && machineGunTurret.interactable)
                    || CheckUIButtonClicked();
                /*if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButtonDown("MKSpawnFusionReactor")
                        || Input.GetButtonDown("MKSpawnIceDrill")
                        || Input.GetButtonDown("MKSpawnBoiler")
                        || Input.GetButtonDown("MKSpawnGreenhouse")
                        || Input.GetButtonDown("MKSpawnIncinerator")
                        || Input.GetButtonDown("MKSpawnShortRangeTurret")
                        || Input.GetButtonDown("MKSpawnLongRangeTurret")
                        || CheckUIButtonClicked();
                }
                else
                {
                    return Input.GetButtonDown(gamepadPrefix + "SpawnBuilding");
                }    */        

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
        /*if (gamepadPrefix == "")
        {
            return false;
        }*/

        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "CycleWeapon":
                return player.GetButton("SwapWeapon");
            case "Pause":
                return player.GetButton("Pause");
            //case "MoveUpDown":

            //Button for MK, axis for XB and DS
            case "Shoot":
                return player.GetButton("Shoot");
            case "MoveLeftRight":
                return player.GetButton("Horizontal");
            case "MoveForwardsBackwards":
                return player.GetButton("Vertical");
                /*if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButton($"MK{requestedInput}");
                }
                else
                {
                    return GetAxis(requestedInput) != 0;
                }*/

            //Always an axis for MK, XB and DS
            //case "LookUpDown":
            //case "LookLeftRight":
            //    return GetAxis(requestedInput) != 0;

            //Aliased
            case "PlaceBuilding":
                return player.GetButton("Submit");
                //return Input.GetButton(gamepadPrefix + "Submit");
            case "CancelBuilding":
                return player.GetButton("Cancel");
                //return Input.GetButton(gamepadPrefix + "Cancel");

            //Custom
            case "CycleBuilding":
                return player.GetButton("IceDrill")
                    || player.GetButton("Reactor")
                    || player.GetButton("Incinerator")
                    || player.GetButton("Boiler")
                    || player.GetButton("GreenHouse")
                    || player.GetButton("Turret1")
                    || player.GetButton("Turret2")
                    || CheckUIButtonClicked();
                /*if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButton("MKSpawnFusionReactor")
                        || Input.GetButton("MKSpawnIceDrill")
                        || Input.GetButton("MKSpawnBoiler")
                        || Input.GetButton("MKSpawnGreenhouse")
                        || Input.GetButton("MKSpawnIncinerator")
                        || Input.GetButton("MKSpawnShortRangeTurret")
                        || Input.GetButton("MKSpawnLongRangeTurret");
                }
                else
                {
                    return Input.GetButton(gamepadPrefix + "CycleBuildingLeft")
                        || Input.GetButton(gamepadPrefix + "CycleBuildingRight");
                }*/

            case "SpawnBuilding":
                return player.GetButton("IceDrill")
                    || player.GetButton("Reactor")
                    || player.GetButton("Incinerator")
                    || player.GetButton("Boiler")
                    || player.GetButton("GreenHouse")
                    || player.GetButton("Turret1")
                    || player.GetButton("Turret2")
                    || CheckUIButtonClicked();
                /*if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButton("MKSpawnFusionReactor")
                        || Input.GetButton("MKSpawnIceDrill")
                        || Input.GetButton("MKSpawnBoiler")
                        || Input.GetButton("MKSpawnGreenhouse")
                        || Input.GetButton("MKSpawnIncinerator")
                        || Input.GetButton("MKSpawnShortRangeTurret")
                        || Input.GetButton("MKSpawnLongRangeTurret");
                }
                else
                {
                    return Input.GetButton(gamepadPrefix + "SpawnBuilding");
                }*/

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
        /*if (gamepadPrefix == "")
        {
            Debug.Log("No Gamepad Prefix Yet");
            return 0f;
        }*/

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

        if (player.GetButton("Turret1"))
        {
            return EBuilding.ShortRangeTurret;
        }

        if (player.GetButton("Turret2"))
        {
            return EBuilding.LongRangeTurret;
        }

        return currentSelection;
        /*switch (gamepad)
        {
            case EGamepad.XboxController:
            case EGamepad.DualShockController:
                return ControllerSelectBuilding(currentSelection);
            case EGamepad.MouseAndKeyboard:
                return MKSelectBuilding(currentSelection);
            default:
                return EBuilding.None;
        }*/
    }

    /*/// <summary>
    /// Check the inputs for selecting a building type when the player is using a mouse and keyboard.
    /// </summary>
    /// <param name="currentSelection"> The currently-selected building type in case no other building type has been selected.</param>
    /// <returns>The selected building type, or the current selection if none.</returns>
    private EBuilding MKSelectBuilding(EBuilding currentSelection)
    {
        if (clickedButton != null)
        {
            return clickedButton.GetBuildingType;
        }

        if (Input.GetButton("MKSpawnFusionReactor"))
        {
            return EBuilding.FusionReactor;
        }

        if (Input.GetButton("MKSpawnIceDrill"))
        {
            return EBuilding.IceDrill;
        }

        if (Input.GetButton("MKSpawnBoiler"))
        {
            return EBuilding.Boiler;
        }

        if (Input.GetButton("MKSpawnGreenhouse"))
        {
            return EBuilding.Greenhouse;
        }

        if (Input.GetButton("MKSpawnIncinerator"))
        {
            return EBuilding.Incinerator;
        }

        if (Input.GetButton("MKSpawnShortRangeTurret"))
        {
            return EBuilding.ShortRangeTurret;
        }

        if (Input.GetButton("MKSpawnLongRangeTurret"))
        {
            return EBuilding.LongRangeTurret;
        }

        return currentSelection;
    }

    /// <summary>
    /// Checks the inputs for selecting a building type when the player is using an Xbox or DualShock controller.
    /// </summary>
    /// <param name="currentSelection">The current-currently-selected building type in case no other building type has been selected.</param>
    /// <returns>The selected building type, or the current selection if none.</returns>
    private EBuilding ControllerSelectBuilding(EBuilding currentSelection)
    {
        int cycle = 0;

        if (Input.GetButtonDown(gamepadPrefix + "CycleBuildingRight"))
        {
            cycle++;
        }

        if (Input.GetButtonDown(gamepadPrefix + "CycleBuildingLeft"))
        {
            cycle--;
        }

        if (cycle != 0)
        {
            int result = (int)currentSelection + cycle;

            if (result > (int)EBuilding.LongRangeTurret)
            {
                result = 2;
            }
            else if (result < (int)EBuilding.FusionReactor)
            {
                result = 8;
            }

            return (EBuilding)result;
        }

        return currentSelection;
    }*/

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
