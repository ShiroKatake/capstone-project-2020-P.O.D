using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls what to put in the hovering dialogue box and handling showing/hiding.
/// </summary>
public class HoveringDialogueManager : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[Header("Hovering Dialogue")]
	[SerializeField] private GameObject hoverDialogueObject;
	[SerializeField] private RectTransform hoverDialogueRectTransform;

	[Header("Dialogue Contents")]
	[SerializeField] private TextMeshProUGUI dialogueName;
	[SerializeField] private TextMeshProUGUI objectClass;

	[Space(10)]
	[SerializeField] private TextMeshProUGUI oreCost;
	[SerializeField] private TextMeshProUGUI powerCost;
	[SerializeField] private TextMeshProUGUI waterCost;
	[SerializeField] private TextMeshProUGUI wasteCost;

	[Space(10)]
	[SerializeField] private TextMeshProUGUI buildTime;

	[Space(10)]
	[SerializeField] private TextMeshProUGUI description;
	[SerializeField] private TextMeshProUGUI powerProductionAmount;
	[SerializeField] private TextMeshProUGUI waterProductionAmount;
	[SerializeField] private TextMeshProUGUI wasteProductionAmount;

	[Space(10)]
	[SerializeField] private Image mineralFill;
	[SerializeField] private TextMeshProUGUI mineralValue;

	//Non-Serialized Fields------------------------------------------------------------------------

	private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
	private List<HoveringDialogueDemi_Container> containers = new List<HoveringDialogueDemi_Container>();
	private List<RectTransform> rectsToRefresh = new List<RectTransform>();
	private Camera playerCamera;
	private HoverDialogueBoxPreset presetWithMineral;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Singleton Public Property--------------------------------------------------------------------   

	public static HoveringDialogueManager Instance { get; protected set; }

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There should never be more than one HoveringDialogueManager.");
		}

		playerCamera = Camera.main;

		Instance = this;

		//Get all "editable" text fields
		HoveringDialogueDemi_TextElement[] hoveringDialogueTexts = hoverDialogueObject.GetComponentsInChildren<HoveringDialogueDemi_TextElement>(true);
		foreach (HoveringDialogueDemi_TextElement hoveringDialogueText in hoveringDialogueTexts)
		{
			texts.Add(hoveringDialogueText.GetComponent<TextMeshProUGUI>());
		}

		//Get all game objects with multiple editable text fields
		HoveringDialogueDemi_Container[] containerArray = hoverDialogueObject.GetComponentsInChildren<HoveringDialogueDemi_Container>(true);
		foreach (var container in containerArray)
		{
			containers.Add(container);
		}

		HoveringDialogueDemi_RefreshElement[] refreshElements = hoverDialogueObject.GetComponentsInChildren<HoveringDialogueDemi_RefreshElement>();
		foreach (HoveringDialogueDemi_RefreshElement refreshElement in refreshElements)
		{
			rectsToRefresh.Add(refreshElement.GetComponent<RectTransform>());
		}
		rectsToRefresh.Reverse();
	}

	/// <summary>
	/// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
	/// Start() runs after Awake().
	/// </summary>
	private void Start()
	{
		HideDialogue();
	}

	//Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Update() is run every frame.
	/// </summary>
	private void Update()
	{
		if (presetWithMineral != null && !string.IsNullOrEmpty(presetWithMineral.MineralValue))
		{
			UpdateMiningLive();
			if (presetWithMineral.WorldAnchorPoint != null)
			{
				hoverDialogueObject.transform.position = new Vector3(presetWithMineral.WorldAnchorPoint.x, presetWithMineral.WorldAnchorPoint.y, 0);
			}
		}
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Fill in the dialogue's contents, enable the dialogue object, disable the empty text fields, and update the sizing.
	/// <param name="preset">The preset that the mouse hovered over.</param>
	/// </summary>
	public void ShowDialogue(HoverDialogueBoxPreset preset)
	{
		presetWithMineral = preset;
		FillDialogueBox(preset);
		
		//Position the dialogue
		RectTransform rectTransform = preset.UIAnchorPoint.GetComponent<RectTransform>();
		if (rectTransform != null)
		{
			hoverDialogueObject.transform.position = rectTransform.position;
		}
		else if (presetWithMineral.WorldAnchorPoint != null)
		{
			hoverDialogueObject.transform.position = new Vector3(presetWithMineral.WorldAnchorPoint.x, presetWithMineral.WorldAnchorPoint.y, 0);
		}
		hoverDialogueObject.SetActive(true);
		CheckEmpty(); //This being after enabling the dialogue object is necessary in order to execute Rebuild() correctly.
		Rebuild();
	}

	/// <summary>
	/// Disable the dialogue object.
	/// </summary>
	public void HideDialogue()
	{
		presetWithMineral = null;
		hoverDialogueObject.SetActive(false);
	}
	
	/// <summary>
	/// Update every object that has a ContentSizeFitter component because they require explicit commands to update for some reason . . .
	/// </summary>
	private void Rebuild()
	{
		foreach (var rect in rectsToRefresh)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
		}
		//Refresh the biggest container last
		LayoutRebuilder.ForceRebuildLayoutImmediate(hoverDialogueRectTransform);
	}

	/// <summary>
	/// Fill the dialogue's content with everything from the preset (including empty strings).
	/// </summary>
	/// <param name="preset">The preset that the mouse hovered over.</param>
	private void FillDialogueBox(HoverDialogueBoxPreset preset)
	{
		dialogueName.text = preset.DialogueName;
		objectClass.text = preset.ObjectClass;
		oreCost.text = preset.OreCost;
		powerCost.text = preset.PowerCost;
		waterCost.text = preset.WaterCost;
		wasteCost.text = preset.WasteCost;
		buildTime.text = preset.BuildTime;
		description.text = preset.Description;
		powerProductionAmount.text = preset.PowerProductionAmount;
		waterProductionAmount.text = preset.WaterProductionAmount;
		wasteProductionAmount.text = preset.WasteProductionAmount;
		mineralValue.text = preset.MineralValue;
	}

	/// <summary>
	/// Update mining related data if necessary components exist.
	/// </summary>
	public void UpdateMiningLive()
	{
		mineralValue.text = presetWithMineral.MineralValue;
		mineralFill.fillAmount = presetWithMineral.MineralFillValue;
	}

	/// <summary>
	/// Enable/disable the container of the text fields depending on whether they're empty or not.
	/// </summary>
	private void CheckEmpty()
	{
		//Disable the individual container if their text field is empty (ie. power cost, build time, etc.)
		foreach (TextMeshProUGUI text in texts)
		{
			if (string.IsNullOrEmpty(text.text))
			{
				text.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				text.transform.parent.gameObject.SetActive(true);
			}
		}

		//Disable the big container if all of their children text fields are empty (ie. production has a potential to provide more than 1 type of resources, so only disable if ALL are empty.)
		foreach (HoveringDialogueDemi_Container container in containers)
		{
			if (container.IsAllEmpty)
			{
				container.gameObject.SetActive(false);
			}
			else
			{
				container.gameObject.SetActive(true);
			}
		}
	}
}
