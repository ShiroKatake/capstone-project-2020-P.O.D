using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls what to put in the hovering dialogue box and handling showing/hiding.
/// </summary>
public class HoveringDialogueManager : MonoBehaviour
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[Header("Hovering Dialogue")]
	[SerializeField] private GameObject hoverDialogueObject;
	[SerializeField] private RectTransform hoverDialogueRect;

	[Header("Dialogue Contents")]
	[SerializeField] private TextMeshProUGUI dialogueName;

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
	[SerializeField] private TextMeshProUGUI mineralValue;

	//Non-Serialized Fields------------------------------------------------------------------------

	private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
	private List<HoveringDialogueContainer> containers = new List<HoveringDialogueContainer>();
	private List<RectTransform> rectsWithContentFitter = new List<RectTransform>();
	private RectTransform hoverDialogueRectTransform;

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

		Instance = this;

		//Get all "editable" text fields
		HoveringDialogueText[] hoveringDialogueTexts = hoverDialogueObject.GetComponentsInChildren<HoveringDialogueText>(true);
		foreach (HoveringDialogueText hoveringDialogueText in hoveringDialogueTexts)
		{
			texts.Add(hoveringDialogueText.GetComponent<TextMeshProUGUI>());
		}

		//Get all game objects with multiple editable text fields
		HoveringDialogueContainer[] containerArray = hoverDialogueObject.GetComponentsInChildren<HoveringDialogueContainer>(true);
		foreach (var container in containerArray)
		{
			containers.Add(container);
		}

		ContentSizeFitter[] contentSizeFitters = hoverDialogueObject.GetComponentsInChildren<ContentSizeFitter>();
		foreach (ContentSizeFitter contentSizeFitter in contentSizeFitters)
		{
			rectsWithContentFitter.Add(contentSizeFitter.GetComponent<RectTransform>());
		}
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
		if (Input.GetKeyDown(KeyCode.G))
		{
			hoverDialogueObject.SetActive(true);
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			HideDialogue();
		}
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Fill in the dialogue's contents, enable the dialogue object, disable the empty text fields, and update the sizing.
	/// <param name="hoveringDialogueBoxPreset">The preset that the mouse hovered over.</param>
	/// </summary>
	public void ShowDialogue(HoverDialogueBoxPreset hoveringDialogueBoxPreset)
	{
		FillDialogueBox(hoveringDialogueBoxPreset);
		hoverDialogueObject.transform.SetParent(hoveringDialogueBoxPreset.AnchorPoint, false);
		hoverDialogueObject.SetActive(true);
		CheckEmpty(); //This being after enabling the dialogue object is necessary in order to execute Rebuild() correctly.
		Rebuild();
	}

	/// <summary>
	/// Disable the dialogue object.
	/// </summary>
	public void HideDialogue()
	{
		hoverDialogueObject.SetActive(false);
	}
	
	/// <summary>
	/// Update every object that has a ContentSizeFitter component because they require explicit commands to update.
	/// </summary>
	private void Rebuild()
	{
		foreach (RectTransform rectTransform in rectsWithContentFitter)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}
	}

	/// <summary>
	/// Fill the dialogue's content with everything from the preset (including empty strings).
	/// </summary>
	/// <param name="hoveringDialogueBoxPreset">The preset that the mouse hovered over.</param>
	private void FillDialogueBox(HoverDialogueBoxPreset hoveringDialogueBoxPreset)
	{
		dialogueName.text = hoveringDialogueBoxPreset.DialogueName;
		oreCost.text = hoveringDialogueBoxPreset.OreCost;
		powerCost.text = hoveringDialogueBoxPreset.PowerCost;
		waterCost.text = hoveringDialogueBoxPreset.WaterCost;
		wasteCost.text = hoveringDialogueBoxPreset.WasteCost;
		buildTime.text = hoveringDialogueBoxPreset.BuildTime;
		description.text = hoveringDialogueBoxPreset.Description;
		powerProductionAmount.text = hoveringDialogueBoxPreset.PowerProductionAmount;
		waterProductionAmount.text = hoveringDialogueBoxPreset.WaterProductionAmount;
		wasteProductionAmount.text = hoveringDialogueBoxPreset.WasteProductionAmount;
		mineralValue.text = hoveringDialogueBoxPreset.MineralValue;
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
		foreach (HoveringDialogueContainer container in containers)
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
