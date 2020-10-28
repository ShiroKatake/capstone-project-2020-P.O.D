using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionHUD : SerializableSingleton<BuildingSelectionHUD>
{
    private List<ButtonClickEventManager> buttons;
    private ButtonClickEventManager button;
    private EBuilding selectedBuildingType;

    protected override void Awake(){
        base.Awake();
        selectedBuildingType = EBuilding.FusionReactor;
        buttons = new List<ButtonClickEventManager>(GetComponentsInChildren<ButtonClickEventManager>());
    }

    // Update is called once per frame
    //use array for buttons to make their calling simpler
    void Update(){
        /*foreach (ButtonClickEventManager btn in buttons){
            btn.AssociatedKeyPressed();
        }*/
    }

    //use array to quickly do every buttons cleanup function
    private void LateUpdate(){
        foreach (ButtonClickEventManager btn in buttons){
            btn.AfterUpdateCleanup();
        }
    }
}
