using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [SerializeField] private EGamepad gamepad;

    //Non-Serialized Fields

    private string gamepadPrefix;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property

    public static InputController Instance { get; protected set; }

    //Basic Public Properties

    public EGamepad Gamepad { get => gamepad; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------
    
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
    }

    //Input Methods----------------------------------------------------------------------------------------------------------------------------------

    //Checks if the player has pressed the specified button
    public bool ButtonPressed(string requestedInput)
    {
        if (gamepadPrefix == "")
        {
            return false;
        }

        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "Shoot":       //TODO: check if XB/DS trigger buttons are buttons or axes
            case "CycleWeapon":
            case "Pause":
            case "HoldTerraformer":
            //case "MoveUpDown":
                return Input.GetButtonDown(gamepadPrefix + requestedInput);

            //Button for MK, axis for XB and DS
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
            //case "LookUpDown":
            //case "LookLeftRight":
            //    return GetAxis(requestedInput) != 0;

            //Unknown input
            default:
                return false;
        }
    }

    //Checks if the player is holding the specified button down
    public bool ButtonHeld(string requestedInput)
    {
        if (gamepadPrefix == "")
        {
            return false;
        }

        switch (requestedInput)
        {
            //Always a button for MK, XB and DS
            case "Shoot":       //TODO: check if XB/DS trigger buttons are buttons or axes
            case "CycleWeapon":
            case "Pause":
            case "HoldTerraformer":
            //case "MoveUpDown":
                return Input.GetButton(gamepadPrefix + requestedInput);

            //Button for MK, axis for XB and DS
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
            //case "LookUpDown":
            //case "LookLeftRight":
            //    return GetAxis(requestedInput) != 0;

            //Unknown input
            default:
                return false;
        }
    }

    //Check if player is moving or looking; if axes are button pairs, returns integer value of -1, 0 or 1; 
    //if axes are mouse / analog stick axes, returns float value between -1 and 1
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
            case "MoveLeftRight":
            case "MoveForwardsBackwards":
            //case "LookUpDown":
            //case "LookLeftRight":
                return Input.GetAxis(gamepadPrefix + requestedInput);

            //Unknown input
            default:
                return 0f;
        }
    }
}
