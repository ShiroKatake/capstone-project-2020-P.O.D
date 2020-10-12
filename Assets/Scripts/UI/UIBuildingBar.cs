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

    void Update()
    {
        foreach(Button b in buttonList){
            b.Btn.OnInteractableChanged(
                b.AssociatedBuilding.OreCost <= ResourceController.Instance.Ore
                && b.AssociatedBuilding.PowerConsumption <= ResourceController.Instance.SurplusPower
                && b.AssociatedBuilding.PlantsConsumption <= ResourceController.Instance.SurplusPlants
                && b.AssociatedBuilding.WaterConsumption <= ResourceController.Instance.SurplusWater
            );
        }
    }
}
