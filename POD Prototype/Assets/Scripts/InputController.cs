using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [SerializeField] private EGamepad gamepad;

    //Non-Serialized Fields

    private static InputController instance;
    private string gamepadPrefix;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Properties

    public EGamepad Gamepad { get => gamepad; }

    //Complex Properties

    public static InputController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputController();
            }

            return instance;
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    private InputController()
    {
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

        Debug.Log($"Gamepad Prefix: {gamepadPrefix}");
    }

    //Input Methods----------------------------------------------------------------------------------------------------------------------------------

    //Checks if the player has clicked the specified button
    public bool GetButtonDown(string requestedInput)
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
            case "MoveUpDown":
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
            case "LookUpDown":
            case "LookLeftRight":
                return GetAxis(requestedInput) != 0;

            //Unknown input
            default:
                return false;
        }
    }

    //Checks if the player is holding the specified button down
    public bool GetButton(string requestedInput)
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
            case "MoveUpDown":
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
            case "LookUpDown":
            case "LookLeftRight":
                return GetAxis(requestedInput) != 0;

            //Unknown input
            default:
                return false;
        }
    }

    //Check if player is moving or looking; if axes are button pairs, returns integer value of -1, 0 or 1; 
    //if axes are mouse / analog stick axes, returns float value between -1 and 1
    public float GetAxis(string requestedInput)
    {
        Debug.Log($"Gamepad Prefix: {gamepadPrefix}, requestedInput: {requestedInput}, Final Requested Input: {gamepadPrefix}{requestedInput}");

        if (gamepadPrefix == "")
        {
            return 0f;
        }

        switch (requestedInput)
        {
            case "MoveUpDown":
            case "MoveLeftRight":
            case "MoveForwardsBackwards":
            case "LookUpDown":
            case "LookLeftRight":
                return Input.GetAxis(gamepadPrefix + requestedInput);

            //Unknown input
            default:
                return 0f;
        }
    }
}
