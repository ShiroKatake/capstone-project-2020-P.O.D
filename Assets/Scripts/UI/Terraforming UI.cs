using UnityEngine;
using UnityEngine.Events;

public class TerraformingUI : MonoBehaviour
{
	protected int[] targetRatioFill;
	protected int[] currentRatioFill;

	protected UnityAction updateCurrentRatio;
	protected UnityAction updateTargetRatio;

	private void Start()
	{
		targetRatioFill = new int[3];
	}

	public void UpdateCurrent(int[] currentRatio)
	{
		currentRatioFill = currentRatio;
		updateCurrentRatio?.Invoke();
	}

	public void UpdateTarget(int[] targetRatio, int[] currentRatio)
	{
		int multiplier = 1;
		int currentMultiplier;

		//Find the largest multiplier
		for (int i = 0; i < targetRatio.Length; i++)
		{
			currentMultiplier = Mathf.CeilToInt(targetRatio[i] / currentRatio[i]);
			if (currentMultiplier > multiplier)
				multiplier = currentMultiplier;
		}
		
		//Multiply each element in the target with that multiplier
		for (int i = 0; i < targetRatioFill.Length; i++)
		{
			targetRatioFill[i] *= multiplier;
		}

		updateTargetRatio?.Invoke();
	}
}
