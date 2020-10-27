using UnityEngine;
using UnityEngine.Events;

public class TerraformingUI : MonoBehaviour
{
	public int[] targetRatioFill = new int[3];
	public int[] currentRatioFill = new int[3];

	public UnityAction updateCurrentRatio;
	public UnityAction updateTargetRatio;

	[SerializeField] TerraformingUIBar bar1;
	[SerializeField] TerraformingUIBar bar2;
	[SerializeField] TerraformingUIBar bar3;


	public void UpdateCurrent(int[] currentRatio)
	{
		currentRatioFill = currentRatio;
		updateCurrentRatio.Invoke();
	}

	public void UpdateTarget(int[] targetRatio, int[] currentRatio)
	{
		int multiplier = 1;
		int currentMultiplier;
		//Find the largest multiplier
		for (int i = 0; i < targetRatio.Length; i++)
		{
			currentMultiplier = Mathf.CeilToInt(currentRatio[i] / targetRatio[i]);
			if (currentMultiplier > multiplier)
				multiplier = currentMultiplier;
		}
		//Debug.Log($"Max Multiplier: {multiplier}");

		//Update the target array
		targetRatioFill[0] = targetRatio[0];
		targetRatioFill[1] = targetRatio[1];
		targetRatioFill[2] = targetRatio[2];

		//Multiply each element in the target with that multiplier
		for (int i = 0; i < targetRatioFill.Length; i++)
		{
			targetRatioFill[i] = targetRatioFill[i] * multiplier;
		}

		updateTargetRatio.Invoke();
	}
}
