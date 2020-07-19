using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoveringDialogueManager : MonoBehaviour
{
	[SerializeField] private GameObject hoverDialogue;
	[SerializeField] private RectTransform hoverDialogueRect;

	[SerializeField] private TextMeshProUGUI dialogueName;

	[SerializeField] private TextMeshProUGUI oreCost;
	[SerializeField] private TextMeshProUGUI powerCost;
	[SerializeField] private TextMeshProUGUI waterCost;
	[SerializeField] private TextMeshProUGUI wasteCost;

	[SerializeField] private TextMeshProUGUI buildTime;

	[SerializeField] private TextMeshProUGUI description;
	[SerializeField] private TextMeshProUGUI powerProductionAmount;
	[SerializeField] private TextMeshProUGUI waterProductionAmount;
	[SerializeField] private TextMeshProUGUI wasteProductionAmount;

	[SerializeField] private TextMeshProUGUI mineralValue;

	public static HoveringDialogueManager Instance { get; protected set; }

	private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
	private List<HoveringDialogueContainer> containers = new List<HoveringDialogueContainer>();
	private List<RectTransform> rectsWithContentFitter = new List<RectTransform>();
	private RectTransform hoverDialogueRectTransform;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There should never be more than one HoveringDialogueManager.");
		}
		Instance = this;

		HoveringDialogueText[] hoveringDialogueTexts = hoverDialogue.GetComponentsInChildren<HoveringDialogueText>(true);
		foreach (HoveringDialogueText hoveringDialogueText in hoveringDialogueTexts)
		{
			texts.Add(hoveringDialogueText.GetComponent<TextMeshProUGUI>());
		}

		HoveringDialogueContainer[] containerArray = hoverDialogue.GetComponentsInChildren<HoveringDialogueContainer>(true);
		foreach (var container in containerArray)
		{
			containers.Add(container);
		}

		ContentSizeFitter[] contentSizeFitters = hoverDialogue.GetComponentsInChildren<ContentSizeFitter>();
		foreach (ContentSizeFitter contentSizeFitter in contentSizeFitters)
		{
			rectsWithContentFitter.Add(contentSizeFitter.GetComponent<RectTransform>());
		}
	}

	private void Start()
	{
		HideDialogue();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			hoverDialogue.SetActive(true);
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			HideDialogue();
		}
	}

	public void ShowDialogue(HoverDialogueBoxPreset hoveringDialogueBoxPreset)
	{
		FillDialogueBox(hoveringDialogueBoxPreset);
		hoverDialogue.transform.SetParent(hoveringDialogueBoxPreset.AnchorPoint, false);
		hoverDialogue.SetActive(true);
		CheckEmpty();
		Rebuild();
	}

	public void HideDialogue()
	{
		hoverDialogue.SetActive(false);
	}

	private void Rebuild()
	{
		foreach (RectTransform rectTransform in rectsWithContentFitter)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}
	}

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

	private void CheckEmpty()
	{
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
