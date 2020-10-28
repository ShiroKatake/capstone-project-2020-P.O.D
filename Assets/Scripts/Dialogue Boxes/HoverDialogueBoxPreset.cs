using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A preset used for filling in information for the hovering dialogue box.
/// </summary>
public class HoverDialogueBoxPreset : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	//Private Fields---------------------------------------------------------------------------------------------------------------------------------

	//Serialized Fields----------------------------------------------------------------------------

	[Header("Anchor")]
	[SerializeField] private Transform anchorPoint;
	
	[Header("Dialogue Contents")]
	[SerializeField] private string dialogueName;
	[SerializeField] private string objectClass;

	[Space(10)]
	[SerializeField] private string oreCost;
	[SerializeField] private string powerCost;
	[SerializeField] private string waterCost;
	[SerializeField] private string plantsCost;
	[SerializeField] private string gasCost;

	[Space(10)]
	[SerializeField] private string buildTime;

	[Space(10)]
	[SerializeField] private string powerProductionAmount;
	[SerializeField] private string waterProductionAmount;
	[SerializeField] private string plantsProductionAmount;
	[SerializeField] private string gasProductionAmount;

	[Space(10)]	[TextArea]
	[SerializeField] private string description;

	//Non-Serialized Fields------------------------------------------------------------------------

	private Mineral mineral;
	private int maxOreCount;
	private GetBoundsToScreenSpace bounds;

	//Public Properties------------------------------------------------------------------------------------------------------------------------------

	//Basic Public Properties----------------------------------------------------------------------

	public Transform UIAnchorPoint { get => anchorPoint; }
	public Vector2 WorldAnchorPoint { get => bounds.GetAnchorPosition(); }

	public string DialogueName { get => dialogueName; }
	public string ObjectClass { get => objectClass; }

	public string OreCost { get => oreCost; }
	public string PowerCost { get => powerCost; }
	public string WaterCost { get => waterCost; }
	public string PlantsCost { get => plantsCost; }
	public string GasCost { get => gasCost; }

	public string BuildTime { get => buildTime; }

	public string Description { get => description; }

	public string PowerProductionAmount { get => powerProductionAmount; }
	public string WaterProductionAmount { get => waterProductionAmount; }
	public string PlantsProductionAmount { get => plantsProductionAmount; }
	public string GasProductionAmount { get => gasProductionAmount; }

	//Complex Public Properties--------------------------------------------------------------------

	public string MineralValue {
		get
		{
			if (mineral != null)
			{
				return (mineral.OreCount * OreFactory.Instance.OreValue).ToString();
			}
			return "";
		}
	}

	public float MineralFillValue
	{
		get
		{
			if (mineral != null)
			{
				return (float)mineral.OreCount / maxOreCount;
			}
			return 0f;
		}
	}

	//Initialization Methods-------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
	/// Awake() runs before Start().
	/// </summary>
	private void Awake()
	{
		bounds = GetComponent<GetBoundsToScreenSpace>();
		mineral = GetComponent<Mineral>();
	}

	/// <summary>
	/// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
	/// Start() runs after Awake().
	/// </summary>
	private void Start()
	{
		maxOreCount = MineralFactory.Instance.OreCount;
	}

	//Triggered Methods------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Unity calls OnPointerEnter when the cursor enters the rect area of a selectable UI object.
	/// </summary>
	/// <param name="eventData">Event payload associated with pointer (mouse / touch) events.</param>
	public void OnPointerEnter(PointerEventData eventData)
	{
		HoveringDialogueManager.Instance.ShowDialogue(this);
	}

	/// <summary>
	/// Unity calls OnPointerExit when the cursor exits the rect area of a selectable UI object.
	/// </summary>
	/// <param name="eventData">Event payload associated with pointer (mouse / touch) events.</param>
	public void OnPointerExit(PointerEventData eventData)
	{
		HoveringDialogueManager.Instance.HideDialogue();
	}
}
