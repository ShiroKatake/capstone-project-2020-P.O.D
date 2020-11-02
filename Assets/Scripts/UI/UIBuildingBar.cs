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
				b.AssociatedBuilding.OreCost <= ResourceManager.Instance.Ore
				&& b.AssociatedBuilding.PowerConsumption <= ResourceManager.Instance.SurplusPower
				&& b.AssociatedBuilding.GasConsumption <= ResourceManager.Instance.SurplusGas
				&& b.AssociatedBuilding.PlantsConsumption <= ResourceManager.Instance.SurplusPlants
				&& b.AssociatedBuilding.WaterConsumption <= ResourceManager.Instance.SurplusWater;
			b.Btn.GetComponent<UIElementStatusManager>().Interactable = interactable;
			b.Btn.OnInteractableChanged(interactable);
		}
	}
}
