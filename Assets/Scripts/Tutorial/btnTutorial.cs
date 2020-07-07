using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class btnTutorial : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields
    [SerializeField] private ButtonType buildingType;

    //Non-Serialized Fields
    private StageManager stageManager;
    private Button button;

    //Public Properties
    public Button Button { get => button; }
    public ButtonType ButtonType { get => buildingType; }

    //Setup Methods----------------------------------------------------------------------------------------------------------------------------------

    //Gets some necessary values
    private void Start()
    {
        button = GetComponent<Button>();
        stageManager = StageManager.Instance;
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    //Controls when lerping happens based on outside variables
    private void Update()
    {
        if (stageManager.Stage == TutorialStage.Finished)
        {
            button.interactable = true;
            Destroy(this);
        }
        else if ((UIController.instance.buildingSelector.Visible || buildingType == ButtonType.Upgrades || buildingType == ButtonType.Destroy) && stageManager.ButtonAllowed(buildingType))
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    private void Notify(List<ButtonType> buttons)
    {

    }
}
