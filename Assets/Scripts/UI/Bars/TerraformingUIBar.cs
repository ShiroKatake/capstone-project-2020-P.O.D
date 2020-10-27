using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum BarType
{
	Boiler,
	Greenhouse,
	Incinerator
}

public class TerraformingUIBar : TerraformingUI
{
	[SerializeField] private BarType bar;
	[SerializeField] private Image currentFill;
	[SerializeField] private Image targetFill;

	private int maxBarValue;

    // Start is called before the first frame update
    void Start()
    {
		updateCurrentRatio += UpdateCurrent;
		updateTargetRatio += UpdateCurrent;
	}

	private void UpdateTarget(int target, int current)
	{
		targetFill.fillAmount = targetRatioFill[(int)bar];
	}

	private void UpdateCurrent()
	{
		currentFill.fillAmount = currentRatioFill[(int)bar] / maxBarValue;
	}
}
