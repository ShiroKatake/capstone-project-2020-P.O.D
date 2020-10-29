using UnityEngine;
using UnityEngine.UI;

public class TerraformingUIBar : MonoBehaviour
{
	[SerializeField] private BarType bar;
	[SerializeField] private Image targetFill;
	[SerializeField] private Image currentFill;

    private void Start()
    {
		TerraformingUI.Instance.updateCurrentRatio += UpdateCurrentRatio;
		TerraformingUI.Instance.updateTargetRatio += UpdateTargetRatio;
	}

	public void UpdateTargetRatio(int[] ratioArray)
	{
		targetFill.fillAmount = ratioArray[(int)bar] * TerraformingUI.Instance.MaxMultiplier / (float)TerraformingUI.Instance.MaxBarValue;
		//Debug.Log($"Value: {ratioArray[(int)bar]}");
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxMultiplier}");
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxBarValue}");
	}

	public void UpdateCurrentRatio(int[] ratioArray)
	{
		currentFill.fillAmount = ratioArray[(int)bar] / (float)TerraformingUI.Instance.MaxBarValue;
		//Debug.Log($"Value: {ratioArray[(int)bar]}");
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxMultiplier}");
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxBarValue}");
	}
}
