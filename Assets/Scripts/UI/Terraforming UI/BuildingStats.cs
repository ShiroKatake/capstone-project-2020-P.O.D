using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingStats : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI enabledText;
	[SerializeField] private TextMeshProUGUI disabledText;
	[SerializeField] private TextMeshProUGUI requiredText;

	[SerializeField] private EBuilding ratioBuilding;

	public void DisplayBuildingInfo()
	{
		enabledText.text = $"Enabled: {BuildingManager.Instance.BuiltAndOperationalBuildingsCount(ratioBuilding)}";
		disabledText.text = $"Disabled: {BuildingManager.Instance.BuiltAndNonOperationalBuildingsCount(ratioBuilding)}";
		requiredText.text = $"Required: {TerraformingUI.Instance.TargetRatio((int)ratioBuilding - 6) - BuildingManager.Instance.BuiltAndOperationalBuildingsCount(ratioBuilding)}";
	}
}
