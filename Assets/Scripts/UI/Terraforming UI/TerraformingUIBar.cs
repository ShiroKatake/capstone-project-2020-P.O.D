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

	private float maxBarValue = 30f;

    private void Awake()
    {
		TerraformingUI.Instance.updateCurrentRatio += UpdateCurrentRatio;
		TerraformingUI.Instance.updateTargetRatio += UpdateTargetRatio;
	}

	public void UpdateTargetRatio(int[] ratioArray)
	{
		targetFill.fillAmount = ratioArray[(int)bar] / maxBarValue;
	}

	public void UpdateCurrentRatio(int[] ratioArray)
	{
		currentFill.fillAmount = ratioArray[(int)bar] / maxBarValue;
	}
}
