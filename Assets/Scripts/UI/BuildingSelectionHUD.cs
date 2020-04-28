using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionHUD : MonoBehaviour
{
    private List<ButtonClickEventManager> buttons;
    private ButtonClickEventManager button;
    private EBuilding selectedBuildingType;

    public static BuildingSelectionHUD Instance { get; protected set; }

    void Awake(){
        if (Instance != null)
        {
            Debug.Log("There should never be 2 or more BuildingSelectionHUDs in the scene.");
        }

        Instance = this;
        selectedBuildingType = EBuilding.FusionReactor;
        buttons = new List<ButtonClickEventManager>(GetComponentsInChildren<ButtonClickEventManager>());
    }

    // Update is called once per frame
    //use array for buttons to make their calling simpler
    void Update(){
        foreach (ButtonClickEventManager btn in buttons){
            btn.AssociatedKeyPressed();
        }
    }

    //use array to quickly do every buttons cleanup function
    private void LateUpdate(){
        foreach (ButtonClickEventManager btn in buttons){
            btn.AfterUpdateCleanup();
        }
    }
}
