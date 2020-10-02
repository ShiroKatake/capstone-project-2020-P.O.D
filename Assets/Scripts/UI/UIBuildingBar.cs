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
            if (b.AssociatedBuilding.OreCost > ResourceController.Instance.Ore){
                b.Btn.OnInteractableChanged(false);
            } else if (b.AssociatedBuilding.PowerConsumption > ResourceController.Instance.SurplusPower){
                b.Btn.OnInteractableChanged(false);
            } else if (b.AssociatedBuilding.WasteConsumption > ResourceController.Instance.SurplusWaste){
                b.Btn.OnInteractableChanged(false);
            } else if (b.AssociatedBuilding.WaterConsumption > ResourceController.Instance.SurplusWater){
                b.Btn.OnInteractableChanged(false);
            } else {
                b.Btn.OnInteractableChanged(true);
            }
        }
    }
}
