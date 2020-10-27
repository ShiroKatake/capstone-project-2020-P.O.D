using UnityEngine;
using UnityEngine.UI;

public enum BarType
{
	Boiler,
	Greenhouse,
	Incinerator
}

public class TerraformingUIBar : MonoBehaviour
{
	[SerializeField] private BarType bar;
	[SerializeField] private Image targetFill;
	[SerializeField] private Image currentFill;
	[SerializeField] private TerraformingUI terraformingUI;

	private float maxBarValue = 30f;

    private void Awake()
    {
		terraformingUI.updateCurrentRatio += UpdateCurrentRatio;
		terraformingUI.updateTargetRatio += UpdateTargetRatio;
	}

	public void UpdateTargetRatio()
	{
		targetFill.fillAmount = terraformingUI.targetRatioFill[(int)bar] / maxBarValue;
	}

	public void UpdateCurrentRatio()
	{
		currentFill.fillAmount = terraformingUI.currentRatioFill[(int)bar] / maxBarValue;
	}
}
