using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIBuildingBar : PublicInstanceSerializableSingleton<UIBuildingBar>
{
    [System.Serializable]
    public struct Button {
        [SerializeField] public ButtonInteract Btn;
        [SerializeField] public Building AssociatedBuilding;
    }

    [SerializeField] private List<Button> buttonList;

	private void Start()
	{
		ResourceManager.Instance.resourcesUpdated += UpdateButtons;
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		foreach (Button b in buttonList)
		{
            UpdateButton(b.AssociatedBuilding, b.Btn);
		}
	}

    public void UpdateButton(Building building, ButtonInteract buttonInteract)
    {
        bool interactable =
            buttonInteract.InInteractableGameStage
            && (building.OreCost == 0 || ResourceManager.Instance.Ore >= building.OreCost)
            && (building.PowerConsumption == 0 || ResourceManager.Instance.SurplusPower >= building.PowerConsumption)
            && (building.WaterConsumption == 0 || ResourceManager.Instance.SurplusWater >= building.WaterConsumption)
            && (building.PlantsConsumption == 0 || ResourceManager.Instance.SurplusPlants >= building.PlantsConsumption)
            && (building.GasConsumption == 0 || ResourceManager.Instance.SurplusGas >= building.GasConsumption);
        buttonInteract.GetComponent<UIElementStatusManager>().Interactable = interactable;
        buttonInteract.OnInteractableChanged(interactable);
    }
}
