﻿using System.Collections;
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

    //Non-Serialized Fields------------------------------------------------------------------------

    private string gamepadPrefix;

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
            case "Shoot":       //TODO: check if XB/DS trigger buttons are buttons or axes
            case "CycleWeapon":
            case "Pause":
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
            case "Shoot":       //TODO: check if XB/DS trigger buttons are buttons or axes
            case "CycleWeapon":
            case "Pause":
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

    /// <summary>
    /// Checks if the player is holding a button to spawn a building.
    /// </summary>
    /// <returns></returns>
    public EBuilding SpawnBuilding()
    {
        if (gamepad == EGamepad.MouseAndKeyboard)
        {
            if (Input.GetButton("MKBuildSolarPanel"))
            {
                return EBuilding.SolarPanel;
            }

            if (Input.GetButton("MKBuildWindTurbine"))
            {
                return EBuilding.WindTurbine;
            }

            if (Input.GetButton("MKBuildWaterDrill"))
            {
                return EBuilding.WaterDrill;
            }

            if (Input.GetButton("MKBuildGasDiffuser"))
            {
                return EBuilding.GasDiffuser;
            }

            if (Input.GetButton("MKBuildHumidifier"))
            {
                return EBuilding.Humidifier;
            }

            if (Input.GetButton("MKBuildGreenhouse"))
            {
                return EBuilding.Greenhouse;
            }

            if (Input.GetButton("MKBuildTurret"))
            {
                return EBuilding.Turret;
            }
        }
        else if (gamepad == EGamepad.XboxController || gamepad == EGamepad.DualShockController)
        {
            //TODO: implement controller support
        }     

        return EBuilding.None;
    }
}