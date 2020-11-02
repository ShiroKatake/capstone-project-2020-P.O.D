﻿using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TerraformingUI : PublicInstanceSerializableSingleton<TerraformingUI>
{
	[SerializeField] private GameObject mainPanel;
	[SerializeField] private GameObject greyPanel;
	private int maxMultiplier;
	private int maxBarValue;
	private int[] currentRatio = new int[3] { 0, 0 ,0 };
	private int[] targetRatio = new int[3] { 0, 0, 0 };
	private int[] buildingsNeeded = new int[3] { 0, 0, 0 };

    private Rewired.Player playerInputManager;

    protected override void Awake()
    {
        base.Awake();
        playerInputManager = POD.Instance.PlayerInputManager;
    }

	public bool IsEnabled
	{
		get { return mainPanel.activeInHierarchy; }
		set
		{
			DisplayUI(value);
		}
	}

	public int MaxMultiplier
	{
		get { return maxMultiplier; }
	}

	public int MaxBarValue
	{
		get { return maxBarValue; }
	}

	public int BuildingsNeeded(int index)
	{
		return buildingsNeeded[index];
	}

	public UnityAction<int[]> updateCurrentRatio;
	public UnityAction<int[]> updateTargetRatio;

	private void Start()
	{
		DisplayUI(false);
	}

    private void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            bool productionUI = playerInputManager.GetButtonDown("ProductionUI");

            if (productionUI)
            {
                //Debug.Log($"ProductionUI is {productionUI}, displaying UI");
                IsEnabled = !IsEnabled;
            }
        }        
    }

	public void UpdateCurrent(int[] currentRatioArray)
	{
		currentRatio = currentRatioArray;
		updateCurrentRatio?.Invoke(currentRatioArray);
	}

	public void UpdateTarget(int[] targetRatioArray, int[] currentRatioArray)
	{
		targetRatio = targetRatioArray;
		maxMultiplier = 1;
		int currentMultiplier;

		//Find the largest multiplier
		for (int i = 0; i < targetRatioArray.Length; i++)
		{
			currentMultiplier = Mathf.CeilToInt((float)currentRatioArray[i] / targetRatioArray[i]);
			if (currentMultiplier > maxMultiplier)
				maxMultiplier = currentMultiplier;
		}
		//Debug.Log($"Max Multiplier: {multiplier}");

		//Update the target array
		buildingsNeeded[0] = targetRatioArray[0];
		buildingsNeeded[1] = targetRatioArray[1];
		buildingsNeeded[2] = targetRatioArray[2];

		//Multiply each element in the target with that multiplier
		for (int i = 0; i < buildingsNeeded.Length; i++)
		{
			buildingsNeeded[i] = buildingsNeeded[i] * maxMultiplier;
		}
		maxBarValue = buildingsNeeded.Max() + 5;

		updateTargetRatio?.Invoke(targetRatioArray);
	}

	public void DisplayUI(bool state)
	{
		mainPanel.SetActive(state);
		greyPanel.SetActive(state);

		if (state == true)
		{
			updateCurrentRatio?.Invoke(currentRatio);
			updateTargetRatio?.Invoke(targetRatio);
		}

		Time.timeScale = state ? 0 : 1;
	}
}