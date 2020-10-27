using UnityEngine;
using UnityEngine.Events;

public class TerraformingUI : SerializableSingleton<TerraformingUI>
{
	private int[] targetRatioFill = new int[3];
	private string todaysRatio = "";

	public UnityAction<int[]> updateCurrentRatio;
	public UnityAction<int[]> updateTargetRatio;

	public string TodaysRatio
	{
		get { return todaysRatio; }
	}

	public void UpdateCurrent(int[] currentRatio)
	{
		updateCurrentRatio.Invoke(currentRatio);
	}

	public void UpdateTarget(int[] targetRatio, int[] currentRatio)
	{
		int multiplier = 1;
		int currentMultiplier;

		//Find the largest multiplier
		for (int i = 0; i < targetRatio.Length; i++)
		{
			currentMultiplier = Mathf.CeilToInt((float)currentRatio[i] / targetRatio[i]);
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

		todaysRatio = $"{targetRatio[0]} : {targetRatio[1]} : {targetRatio[2]}";
		updateTargetRatio.Invoke(targetRatioFill);
	}
}
