using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for getting the right input values from the player's current input device(s) without having to specify the device-specific input for what you're after.
/// </summary>
public class InputController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private EGamepad gamepad;
    [SerializeField] private EOperatingSystem operatingSystem;

    //Non-Serialized Fields------------------------------------------------------------------------

    private string gamepadPrefix;
    private string osPrefix;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// InputController's singleton public property.
    /// </summary>
    public static InputController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

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
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///  Checks if the player has pressed the specified button.
    ///  <param name="requestedInput">The input or button to check.</param>
    /// </summary>
    public bool ButtonPressed(string requestedInput)
    {
        if (gamepadPrefix == "")
        {
            return false;
        }

        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "CycleWeapon":
            case "Pause":
                return Input.GetButtonDown(gamepadPrefix + requestedInput);

            //Button for MK, axis for XB and DS
            case "Shoot":
            case "MoveLeftRight":
            case "MoveForwardsBackwards":
                if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButtonDown($"MK{requestedInput}");
                }
                else
                {
                    return GetAxis(requestedInput) != 0;
                }

            //Always an axis for MK, XB and DS
            case "LookUpDown":
            case "LookLeftRight":
                return GetAxis(requestedInput) != 0;

            //Aliased
            case "PlaceBuilding":
                return Input.GetButtonDown(gamepadPrefix + "Submit");
            case "CancelBuilding":
                return Input.GetButtonDown(gamepadPrefix + "Cancel");

            //Custom
            case "CycleBuilding":
                if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButtonDown("MKSpawnFusionReactor")
                        || Input.GetButtonDown("MKSpawnIceDrill")
                        || Input.GetButtonDown("MKSpawnBoiler")
                        || Input.GetButtonDown("MKSpawnGreenhouse")
                        || Input.GetButtonDown("MKSpawnIncinerator")
                        || Input.GetButtonDown("MKSpawnShortRangeTurret")
                        || Input.GetButtonDown("MKSpawnLongRangeTurret");
                }
                else
                {
                    return Input.GetButtonDown(gamepadPrefix + "CycleBuildingLeft")
                        || Input.GetButtonDown(gamepadPrefix + "CycleBuildingRight");
                }

            case "SpawnBuilding":
                if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButtonDown("MKSpawnFusionReactor")
                        || Input.GetButtonDown("MKSpawnIceDrill")
                        || Input.GetButtonDown("MKSpawnBoiler")
                        || Input.GetButtonDown("MKSpawnGreenhouse")
                        || Input.GetButtonDown("MKSpawnIncinerator")
                        || Input.GetButtonDown("MKSpawnShortRangeTurret")
                        || Input.GetButtonDown("MKSpawnLongRangeTurret");
                }
                else
                {
                    return Input.GetButtonDown(gamepadPrefix + "SpawnBuilding");
                }            

            //Unknown input
            default:
                return false;
        }
    }

    /// <summary>
    ///  Checks if the player is holding the specified button or input down.
    ///  <param name="requestedInput">The input or button to check.</param>
    /// </summary>
    public bool ButtonHeld(string requestedInput)
    {
        if (gamepadPrefix == "")
        {
            return false;
        }

        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "CycleWeapon":
            case "Pause":
            //case "MoveUpDown":
                return Input.GetButton(gamepadPrefix + requestedInput);

            //Button for MK, axis for XB and DS
            case "Shoot":
            case "MoveLeftRight":
            case "MoveForwardsBackwards":
                if (gamepad == EGamepad.MouseAndKeyboard)
                {
                    return Input.GetButton($"MK{requestedInput}");
                }
                else
                {
                    return GetAxis(requestedInput) != 0;
                }

            //Always an axis for MK, XB and DS
            case "LookUpDown":
            case "LookLeftRight":
                return GetAxis(requestedInput) != 0;

            //Aliased
            case "PlaceBuilding":
                return Input.GetButton(gamepadPrefix + "Submit");
            case "CancelBuilding":
                return Input.GetButton(gamepadPrefix + "Cancel");

            //Custom
            case "CycleBuilding":
                if (gamepad == EGamepad.MouseAndKeyboard)
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
                }

            case "SpawnBuilding":
                if (gamepad == EGamepad.MouseAndKeyboard)
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
                }

            //Unknown input
            default:
                return false;
        }
    }

    /// <summary>
    ///  Check if player is moving or looking; if axes are button pairs, returns integer value of -1, 0 or 1; 
    ///  if axes are mouse / analog stick axes, returns float value between -1 and 1
    ///  <param name="requestedInput">The input or axis to check.</param>
    /// </summary>
    public float GetAxis(string requestedInput)
    {
        if (gamepadPrefix == "")
        {
            Debug.Log("No Gamepad Prefix Yet");
            return 0f;
        }

        switch (requestedInput)
        {
            //case "MoveUpDown":
            case "Shoot":
            case "MoveForwardsBackwards":
            case "MoveLeftRight":
            case "LookUpDown":
            case "LookLeftRight":
                return Input.GetAxis(gamepadPrefix + requestedInput);

            //Unknown input
            default:
                return 0f;
        }
    }

    //Building Type Selection------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks the inputs for selecting a building type.
    /// </summary>
    /// <returns></returns>
    public EBuilding SelectBuilding(EBuilding currentSelection)
    {
        switch (gamepad)
        {
            case EGamepad.XboxController:
            case EGamepad.DualShockController:
                return ControllerSelectBuilding(currentSelection);
            case EGamepad.MouseAndKeyboard:
                return MKSelectBuilding(currentSelection);
            default:
                return EBuilding.None;
        }
    }

    /// <summary>
    /// Check the inputs for selecting a building type when the player is using a mouse and keyboard.
    /// </summary>
    /// <returns></returns>
    private EBuilding MKSelectBuilding(EBuilding currentSelection)
    {
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
    /// <returns></returns>
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
    }
}
