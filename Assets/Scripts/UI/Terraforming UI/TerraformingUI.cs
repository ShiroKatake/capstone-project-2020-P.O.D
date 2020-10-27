using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TerraformingUI : SerializableSingleton<TerraformingUI>
{
	private int[] targetRatioValues = new int[3];
	private int maxMultiplier;
	private int maxBarValue;

	public int MaxMultiplier
	{
		get { return maxMultiplier; }
	}

	public int MaxBarValue
	{
		get { return maxBarValue; }
	}

	public UnityAction<int[]> updateCurrentRatio;
	public UnityAction<int[]> updateTargetRatio;

	public void UpdateCurrent(int[] currentRatio)
	{
		updateCurrentRatio.Invoke(currentRatio);
	}

	public void UpdateTarget(int[] targetRatio, int[] currentRatio)
	{
		maxMultiplier = 1;
		int currentMultiplier;

		//Find the largest multiplier
		for (int i = 0; i < targetRatio.Length; i++)
		{
			currentMultiplier = Mathf.CeilToInt((float)currentRatio[i] / targetRatio[i]);
			if (currentMultiplier > maxMultiplier)
				maxMultiplier = currentMultiplier;
		}
		//Debug.Log($"Max Multiplier: {multiplier}");

		//Update the target array
		targetRatioValues[0] = targetRatio[0];
		targetRatioValues[1] = targetRatio[1];
		targetRatioValues[2] = targetRatio[2];

		//Multiply each element in the target with that multiplier
		for (int i = 0; i < targetRatioValues.Length; i++)
		{
			targetRatioValues[i] = targetRatioValues[i] * maxMultiplier;
		}
		maxBarValue = targetRatioValues.Max() + 5;

		updateTargetRatio.Invoke(targetRatio);
	}
}
