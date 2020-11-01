using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIBuildingBar : MonoBehaviour
{
    [System.Serializable]
    public struct Button {
        [SerializeField] public ButtonInteract Btn;
        [SerializeField] public Building AssociatedBuilding;
    }

    [SerializeField] private List<Button> buttonList;

	private void Awake()
	{
		ResourceManager.Instance.resourcesUpdated += UpdateButton;
		UpdateButton();
	}

	private void UpdateButton()
	{
		foreach (Button b in buttonList)
		{
			bool interactable =
				b.AssociatedBuilding.OreCost <= ResourceManager.Instance.Ore && ResourceManager.Instance.Ore > 0
				&& b.AssociatedBuilding.PowerConsumption <= ResourceManager.Instance.SurplusPower && ResourceManager.Instance.SurplusPower > 0
				&& b.AssociatedBuilding.GasConsumption <= ResourceManager.Instance.SurplusGas && ResourceManager.Instance.SurplusGas > 0
				&& b.AssociatedBuilding.PlantsConsumption <= ResourceManager.Instance.SurplusPlants && ResourceManager.Instance.SurplusPlants > 0
				&& b.AssociatedBuilding.WaterConsumption <= ResourceManager.Instance.SurplusWater && ResourceManager.Instance.SurplusWater > 0;
			b.Btn.GetComponent<UIElementStatusManager>().Interactable = interactable;
			b.Btn.OnInteractableChanged(interactable);
		}
	}
}
