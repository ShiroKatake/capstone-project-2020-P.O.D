using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TerraformingUI_TodaysRatio : MonoBehaviour
{
	TextMeshProUGUI todaysRatio;

	private void Awake()
	{
		todaysRatio = GetComponent<TextMeshProUGUI>();
	}

	private void Start()
	{
		TerraformingUI.Instance.updateTargetRatio += ChangeTexts;
	}

	private void ChangeTexts(int[] ratioArray)
	{
		todaysRatio.text = $"{ratioArray[0]} : {ratioArray[1]} : {ratioArray[2]}";
	}
}
