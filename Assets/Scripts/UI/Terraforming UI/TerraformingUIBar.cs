using UnityEngine;
using UnityEngine.UI;

public class TerraformingUIBar : MonoBehaviour
{
	[SerializeField] private EBuilding bar;
	[SerializeField] private Image targetFill;
	[SerializeField] private Image currentFill;

    private void Awake()
    {
		TerraformingUI.updateCurrentRatio += UpdateCurrentRatio;
		TerraformingUI.updateTargetRatio += UpdateTargetRatio;
	}

	public void UpdateTargetRatio(int[] ratioArray)
	{
        if (targetFill.gameObject.activeInHierarchy)
			targetFill.fillAmount = ratioArray[(int)bar - 6] * TerraformingUI.Instance.MaxMultiplier / (float)TerraformingUI.Instance.MaxBarValue;
        //Debug.Log($"Value: {(int)bar - 6}");
        //Debug.Log($"Value: {this.gameObject.name}, {(int)bar}");
        //Debug.Log($"Max Value: {TerraformingUI.Instance.MaxMultiplier}");
        //Debug.Log($"Max Value: {TerraformingUI.Instance.MaxBarValue}");
    }

    public void UpdateCurrentRatio(int[] ratioArray)
	{
		if (currentFill.gameObject.activeInHierarchy)
			currentFill.fillAmount = ratioArray[(int)bar - 6] / (float)TerraformingUI.Instance.MaxBarValue;
        //Debug.Log($"Value: {ratioArray[(int)bar - 6]}");
        //Debug.Log($"Max Value: {TerraformingUI.Instance.MaxMultiplier}");
        //Debug.Log($"Max Value: {TerraformingUI.Instance.MaxBarValue}");
    }
}
