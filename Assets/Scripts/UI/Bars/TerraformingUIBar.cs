using UnityEngine;
using UnityEngine.UI;

public enum BarType
{
	Boiler,
	Greenhouse,
	Incinerator
}

public class TerraformingUIBar : TerraformingUI
{
	[SerializeField] private BarType bar;
	[SerializeField] private Image targetFill;
	[SerializeField] private Image currentFill;

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
		Debug.Log("updatingT");
	}

	private void UpdateCurrent()
	{
		currentFill.fillAmount = currentRatioFill[(int)bar] / maxBarValue;
		Debug.Log("updatingC");
	}
}
