using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A player script for collecting minerals.
/// </summary>
public class MineralCollectionController : PublicInstanceSerializableSingleton<MineralCollectionController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private Camera camera;
	[SerializeField] private MiningBeam miningBeam;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Player playerInputManager;
    private bool collectMinerals;
	private bool isOnMineral;
    private LayerMask mineralsLayerMask;
    private bool mining;
	private Mineral cacheMineral = null;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Is the player currently mining a mineral?
    /// </summary>
    public bool Mining { get => mining; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        mineralsLayerMask = LayerMask.GetMask("Minerals");
        mining = false;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        playerInputManager = POD.Instance.PlayerInputManager;
        //playerInputManager = ReInput.players.GetPlayer(GetComponent<PlayerID>().Value);
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
        if (!BuildingSpawnController.Instance.SpawningBuilding)
        {
            //Debug.Log("Mining");
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mineralsLayerMask))
            {
				Mineral mineral = hit.collider.GetComponentInParent<Mineral>();

				//Debug.Log($"Mineral at {mineral.transform.position} is {cacheMineral != mineral} to cache mineral at {cacheMineral?.transform.position}");
				if (cacheMineral != mineral)
				{
                    StopMining();
                    HideMineralInfo();
					DisplayMineralInfo(mineral);
				}

				if (collectMinerals && mineral != null && mineral.OreCount > 0)
                {
                    if (!mining) StartMining(mineral);                    
                    mineral.Mine();
                }
				else if (mining)
				{
                    StopMining();
                }
			}
			else
			{
                HideMineralInfo();
                if (mining) StopMining();
            }
        }
        else
        {
            HideMineralInfo();
            if (mining) StopMining();
        }
    }

    /// <summary>
    /// Starts the mining beam and mining sound, and sets mining to true.
    /// </summary>
    /// <param name="mineral">The mineral the player is mining.</param>
    private void StartMining(Mineral mineral)
    {
        mining = true;
        miningBeam.OnMineEnable(mineral.MiningPoint);
        AudioManager.Instance.PlaySound(AudioManager.ESound.Mining, this.gameObject);
    }

    /// <summary>
    /// Stops the mining beam and mining sound, and sets mining to false.
    /// </summary>
    private void StopMining()
    {
        mining = false;
        miningBeam.OnMineDisable();
        AudioManager.Instance.StopSound(AudioManager.ESound.Mining, this.gameObject);
    }

	/// <summary>
	/// Trigger hovering dialogue box if mouse hovers over the mineral deposit.
	/// </summary>
	private void DisplayMineralInfo(Mineral mineral)
	{
		if (!isOnMineral)
		{
			HoveringDialogueManager.Instance.ShowDialogue(mineral.GetComponent<HoverDialogueBoxPreset>());
			cacheMineral = mineral;
			isOnMineral = true;
		}
	}
	
	/// <summary>
	/// Hide hovering dialogue box if mouse leaves the mineral deposit.
	/// </summary>
	private void HideMineralInfo()
	{
		if (isOnMineral)
		{
			HoveringDialogueManager.Instance.HideDialogue();
			cacheMineral = null;
			isOnMineral = false;
		}
	}
}
