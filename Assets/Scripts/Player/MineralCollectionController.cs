using Rewired;
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

    [SerializeField] private Camera camera;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Player playerInputManager;
    private bool collectMinerals;
    private LayerMask mineralsLayerMask;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// MineralCollectionController's singleton public property.
    /// </summary>
    public static MineralCollectionController Instance { get; protected set; }

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
        mineralsLayerMask = LayerMask.GetMask("Minerals");
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        playerInputManager = ReInput.players.GetPlayer(GetComponent<PlayerID>().Value);
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
        collectMinerals = playerInputManager.GetButton("Mine");
    }

    /// <summary>
    /// Checks if the player wants to collect minerals, and collects those minerals if they do.
    /// </summary>
    private void CollectMinerals()
    {
        if (collectMinerals && !BuildingSpawningController.Instance.SpawningBuilding)
        {
            //Debug.Log("Mining");
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mineralsLayerMask))
            {
                //Debug.Log("Raycast hit minerals");
                Mineral mineral = hit.collider.GetComponentInParent<Mineral>();

                if (mineral != null)
                {
                    int mined = mineral.Mine();
                    //Debug.Log($"Raycast hit mineral node. Mined {mined} minerals");
                    ResourceController.Instance.Ore += mined;
                }
            }
        }
    }
}
