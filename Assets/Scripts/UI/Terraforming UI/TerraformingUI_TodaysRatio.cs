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

	//private void Start()
	//{
	//	TerraformingUI.updateTargetRatio += ChangeTexts;
	//}

	public void ChangeTexts(int[] ratioArray)
	{
        string text = $"{ratioArray[0]} : {ratioArray[1]} : {ratioArray[2]}";
        Debug.Log($"{this}.TerraformingUI_TodaysRatio.ChangeTexts(), ratio is {ratioArray[0]}:{ratioArray[1]}:{ratioArray[2]}, text is \"{text}\"");
        todaysRatio.text = text;
	}
}
