using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player. Player controls the player's movement, shooting and healing. For building spawning, see BuildingSpawningController.
/// </summary>
public class POD : PublicInstanceSerializableSingleton<POD>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private PODHealthController healthController;
    private PlayerID id;
    private PODMovementController movementController;
    private PODShootingController shootingController;

    //Other
    private Player playerInputManager;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// POD's health controller component.
    /// </summary>
    public PODHealthController HealthController { get => healthController; }

    /// <summary>
    /// POD's movement controller component.
    /// </summary>
    public PODMovementController MovementController { get => movementController; }

    /// <summary>
    /// POD's shooting controller component.
    /// </summary>
    public PODShootingController ShootingController { get => shootingController; }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// POD's ID component.
    /// </summary>
    public PlayerID ID
    {
        get
        {
            if (id == null) id = GetComponent<PlayerID>();
            return id;
        }
    }

    /// <summary>
    /// POD's Rewired player input manager.
    /// </summary>
    public Player PlayerInputManager
    {
        get
        {
            if (playerInputManager == null) playerInputManager = ReInput.players.GetPlayer(ID.Value);
            return playerInputManager;
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        if (id == null) id = GetComponent<PlayerID>();
        healthController = GetComponent<PODHealthController>();
        movementController = GetComponent<PODMovementController>();
        shootingController = GetComponent<PODShootingController>();
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        if (playerInputManager == null) playerInputManager = ReInput.players.GetPlayer(GetComponent<PlayerID>().Value);
    }
}

