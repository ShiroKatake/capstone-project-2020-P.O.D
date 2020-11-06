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

	protected override void Awake()
	{
        base.Awake();
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
                && building.OreCost <= ResourceManager.Instance.Ore
                && building.PowerConsumption <= ResourceManager.Instance.SurplusPower
                && building.GasConsumption <= ResourceManager.Instance.SurplusGas
                && building.PlantsConsumption <= ResourceManager.Instance.SurplusPlants
                && building.WaterConsumption <= ResourceManager.Instance.SurplusWater;
        buttonInteract.GetComponent<UIElementStatusManager>().Interactable = interactable;
        buttonInteract.OnInteractableChanged(interactable);
    }
}
