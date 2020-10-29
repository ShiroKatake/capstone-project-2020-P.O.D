using UnityEngine;
using UnityEngine.UI;

public class TerraformingUIBar : MonoBehaviour
{
	[SerializeField] private EBuilding bar;
	[SerializeField] private Image targetFill;
	[SerializeField] private Image currentFill;

    private void Start()
    {
		TerraformingUI.Instance.updateCurrentRatio += UpdateCurrentRatio;
		TerraformingUI.Instance.updateTargetRatio += UpdateTargetRatio;
	}

	public void UpdateTargetRatio(int[] ratioArray)
	{
		//Debug.Log($"Value: {(int)bar - 6}");
		//Debug.Log($"Value: {this.gameObject.name}, {(int)bar}");
		targetFill.fillAmount = ratioArray[(int)bar - 6] * TerraformingUI.Instance.MaxMultiplier / (float)TerraformingUI.Instance.MaxBarValue;
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxMultiplier}");
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxBarValue}");
	}

	public void UpdateCurrentRatio(int[] ratioArray)
	{
		currentFill.fillAmount = ratioArray[(int)bar - 6] / (float)TerraformingUI.Instance.MaxBarValue;
		//Debug.Log($"Value: {ratioArray[(int)bar]}");
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxMultiplier}");
		//Debug.Log($"Max Value: {TerraformingUI.Instance.MaxBarValue}");
	}
}
