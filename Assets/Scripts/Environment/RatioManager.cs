using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RatioManager : PublicInstanceSerializableSingleton<RatioManager>
{
	[SerializeField] TerraformingUI terraformingUI;
	[SerializeField] ProgressBar progressBar;

	[SerializeField] private float winAmount;

	[SerializeField] private int[] currentRatio = { 0, 0, 0 };
	[SerializeField] private int[] targetRatio = { 0, 0, 0 };

	[SerializeField] private float pointsPerRatio;
	[SerializeField] private float multiplierIncreasePerWave;
	[SerializeField] private int maxRatioValue;

	private int[] waveStartRatio;

	float currentMultiplier = 1;
	float storedPoints = 0;
	float pointsThisWave;

	public UnityAction<bool> updateStoreDisperse;

	public float WinAmount { get => winAmount; }
	public float PointsStored { get => storedPoints; }
	public float PointsGained { get => pointsThisWave; }
	public float PointsPerRatio { get => pointsPerRatio; }
	public float DisperseBonus { get => Mathf.Round((currentMultiplier - 1f) * 100f); }

	protected override void Awake()
	{
		base.Awake();
		progressBar.SetMax(winAmount);
		progressBar.SetBarValue(0f);
	}

	private void Start()
	{
		UpdateTargetRatio();
		terraformingUI.UpdateTarget(targetRatio, currentRatio);
	}

	public float ScoreRatioAlignment()
	{
		int gatingIndex = 0;
		int maxIndex = 0;
		
		float[] factors = new float[3];
		factors[0] = (float)currentRatio[0] / (float)targetRatio[0];
		factors[1] = (float)currentRatio[1] / (float)targetRatio[1];
		factors[2] = (float)currentRatio[2] / (float)targetRatio[2];

		//Debug.Log($"factors array: {factors[0]}, {factors[1]}, {factors[2]}");

		for (int i = 0; i < factors.Length; i++)
		{
			if (factors[i] < factors[gatingIndex])
				gatingIndex = i;
			if (factors[i] > factors[maxIndex])
				maxIndex = i;
		}

		int multiplier = (int)Mathf.Floor(factors[gatingIndex]);
		int maxTier = (int)Mathf.Ceil(factors[maxIndex]);
		int leftoverTier = maxTier - multiplier;

		int[] leftoverRatio = new int[3];
		leftoverRatio[0] = currentRatio[0] - multiplier * targetRatio[0];
		leftoverRatio[1] = currentRatio[1] - multiplier * targetRatio[1];
		leftoverRatio[2] = currentRatio[2] - multiplier * targetRatio[2];

		//Debug.Log($"leftoverRatio array: {leftoverRatio[0]}, {leftoverRatio[1]}, {leftoverRatio[2]}");

		float[] ratioDiviationAccuracy = new float[3];
		ratioDiviationAccuracy[0] = (float)leftoverRatio[0] / (float)(targetRatio[0] * leftoverTier);
		ratioDiviationAccuracy[1] = (float)leftoverRatio[1] / (float)(targetRatio[1] * leftoverTier);
		ratioDiviationAccuracy[2] = (float)leftoverRatio[2] / (float)(targetRatio[2] * leftoverTier);

		for (int i = 0; i < ratioDiviationAccuracy.Length; i++)
		{
			if (float.IsNaN(ratioDiviationAccuracy[i]))
				ratioDiviationAccuracy[i] = 0;
		}

		//Debug.Log($"ratioAccuracy array: {ratioDiviationAccuracy[0]}, {ratioDiviationAccuracy[1]}, {ratioDiviationAccuracy[2]}");

		float averageAccuracy = (ratioDiviationAccuracy[0] + ratioDiviationAccuracy[1] + ratioDiviationAccuracy[2]) / 3;
		float maxPoints = pointsPerRatio * maxTier;

		float scoredPoints = multiplier * pointsPerRatio + averageAccuracy * pointsPerRatio;

		//Debug.Log(scoredPoints);

		return Mathf.Round(scoredPoints);
	}

	private void IncreaseMultiplier()
	{
		currentMultiplier += multiplierIncreasePerWave;
	}

	private void ClearMultiplier()
	{
		currentMultiplier = 1;
	}

	/// <summary>
	/// Updates the target ratio for the player
	/// </summary>
	public void UpdateTargetRatio()
	{
		targetRatio[0] = Random.Range(1, maxRatioValue + 1);
		targetRatio[1] = Random.Range(1, maxRatioValue + 1);
		targetRatio[2] = Random.Range(1, maxRatioValue + 1);

		if (targetRatio[0] == targetRatio[1] && targetRatio[1] == targetRatio[2])
		{
			targetRatio[0] = 1;
			targetRatio[1] = 1;
			targetRatio[2] = 1;
		}

		terraformingUI.UpdateTarget(targetRatio, currentRatio);
	}

	/// <summary>
	/// FOR TESTING: Updates the current ratio for the player
	/// </summary>
	public void TestCurrentRatio()
	{
		currentRatio[0] = Random.Range(1, 20);
		currentRatio[1] = Random.Range(1, 20);
		currentRatio[2] = Random.Range(1, 20);

		terraformingUI.UpdateTarget(targetRatio, currentRatio);
		terraformingUI.UpdateCurrent(currentRatio);
	}

	/// <summary>
	/// Updates the current ratio for the player
	/// </summary>
	public void UpdateCurrentRatio()
	{
		currentRatio[0] = BuildingManager.Instance.BuiltAndOperationalBuildingsCount(EBuilding.Boiler);
		currentRatio[1] = BuildingManager.Instance.BuiltAndOperationalBuildingsCount(EBuilding.Greenhouse);
		currentRatio[2] = BuildingManager.Instance.BuiltAndOperationalBuildingsCount(EBuilding.Incinerator);

		terraformingUI.UpdateTarget(targetRatio, currentRatio);
		terraformingUI.UpdateCurrent(currentRatio);
	}

	/// <summary>
	/// Stores the ratio at the start of the wave for comparison at the end
	/// </summary>
	/// <param name="ratios"></param>
	public void StartWave(/*int[] ratios*/)
	{
		waveStartRatio = currentRatio/*ratios*/;
	}

	/// <summary>
	/// Calculates changes in the multiplier based on wave performance
	/// </summary>
	/// <param name="ratios"></param>
	public void EndWave(/*int[] ratios*/)
	{
		pointsThisWave = ScoreRatioAlignment();
		// Are any building counts lower than at the start of the wave
		bool isCountLower = (waveStartRatio[0] > currentRatio[0] ||
							 waveStartRatio[1] > currentRatio[1] ||
							 waveStartRatio[2] > currentRatio[2]);
		if (isCountLower)
			ClearMultiplier();
		else
			IncreaseMultiplier();

		updateStoreDisperse?.Invoke(true);
	}

	/// <summary>
	/// Returns the total number of points the player receives for dispersing on this wave.
	/// Must be called after EndWave
	/// </summary>
	/// <returns></returns>
	public void DispersePoints()
	{
		float returnVal = (pointsThisWave + storedPoints) * currentMultiplier;
		storedPoints = 0;
		ClearMultiplier();

		progressBar.SetBarValue(returnVal);
		//Debug.Log("Disperse");
	}

	/// <summary>
	/// Stores the points earned this wave in the ratio manager
	/// </summary>
	public void StorePoints()
	{
		storedPoints += ScoreRatioAlignment();
		//Debug.Log("Store");
	}

}
