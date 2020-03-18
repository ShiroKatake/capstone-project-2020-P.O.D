using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields

    [SerializeField] private EGamepad gamepad = EGamepad.XboxController;

    //Non-Serialized Fields

    private string gamepadPrefix;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    public EGamepad Gamepad { get => gamepad; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        switch (PlayerPrefs.GetString("gamepad"))
        {
            case "xbox":
                gamepad = EGamepad.XboxController;
                gamepadPrefix = "XB";
                break;
            case "dualshock":
                gamepad = EGamepad.DualShockController;
                gamepadPrefix = "DS";
                break;
            default:
                gamepad = EGamepad.MouseAndKeyboard;
                gamepadPrefix = "MK";
                break;
        }
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    //private void Update()
    //{
        

    //}

    //Input Methods----------------------------------------------------------------------------------------------------------------------------------

    //Checks if the player has clicked the specified button
    public bool GetButtonDown(string requestedInput)
    {
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
            case "LookHorizontal":
            case "LookVertical":
                return GetAxis(requestedInput) != 0;

            //Unknown input
            default:
                return false;
        }
    }

    //Checks if the player is holding the specified button down
    public bool GetButton(string requestedInput)
    {
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
            case "LookHorizontal":
            case "LookVertical":
                return GetAxis(requestedInput) != 0;

            //Unknown input
            default:
                return false;
        }
    }

    //Check if player is moving the mouse / analog stick in specified direction
    public float GetAxis(string requestedInput)
    {
        switch (requestedInput)
        {
            case "MoveLeftRight":
            case "MoveForwardsBackwards":
            case "LookHorizontal":
            case "LookVertical":
                return Input.GetAxis(gamepadPrefix + requestedInput);

            //Unknown input
            default:
                return 0f;
        }
    }
}
