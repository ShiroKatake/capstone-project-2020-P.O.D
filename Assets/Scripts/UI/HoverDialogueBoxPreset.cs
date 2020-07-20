using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverDialogueBoxPreset : MonoBehaviour
{
	[SerializeField] private Transform anchorPoint;

	[SerializeField] private string dialogueName;
	[SerializeField] private string objectClass;
	[SerializeField] private string oreCost;
	[SerializeField] private string powerCost;
	[SerializeField] private string waterCost;
	[SerializeField] private string wasteCost;
	[SerializeField] private string buildTime;
	[SerializeField] private string powerProductionAmount;
	[SerializeField] private string waterProductionAmount;
	[SerializeField] private string wasteProductionAmount;
	[TextArea]
	[SerializeField] private string description;

	public Transform AnchorPoint { get => anchorPoint; }

	public string DialogueName { get => dialogueName; }
	public string ObjectClass { get => objectClass; }

	public string OreCost { get => oreCost; }
	public string PowerCost { get => powerCost; }
	public string WaterCost { get => waterCost; }
	public string WasteCost { get => wasteCost; }

	public string BuildTime { get => buildTime; }

	public string Description { get => description; }

	public string PowerProductionAmount { get => powerProductionAmount; }
	public string WaterProductionAmount { get => waterProductionAmount; }
	public string WasteProductionAmount { get => wasteProductionAmount; }

	public string MineralValue {
		get
		{
			Mineral mineral = GetComponent<Mineral>();
			if (mineral != null)
			{
				return (mineral.OreCount * OreFactory.Instance.OreValue).ToString();
			}
			return "";
		}
	}
}
