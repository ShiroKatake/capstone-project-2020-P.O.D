using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is introduced to the terraforming buildings and how terraforming works.
/// </summary>
public class StageTerraforming : PublicInstanceSerializableSingleton<StageTerraforming>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("General UI")]
    [SerializeField] private UIElementStatusManager clock;

    [Header("Building Buttons")]
    [SerializeField] private UIElementStatusManager fusionReactor;
    [SerializeField] private UIElementStatusManager iceDrill;
    [SerializeField] private UIElementStatusManager harvester;
    [SerializeField] private UIElementStatusManager gasPump;
    [SerializeField] private UIElementStatusManager boiler;
    [SerializeField] private UIElementStatusManager greenhouse;
    [SerializeField] private UIElementStatusManager incinerator;

    [Header("Progress/Ratio Bars")]
    [SerializeField] private UIElementStatusManager progressBar;
    [SerializeField] private UIElementStatusManager humidityBar;
    [SerializeField] private UIElementStatusManager biodiversityBar;
    [SerializeField] private UIElementStatusManager atmosphereBar;

    [Header("Highlights")]
    //[SerializeField] private UIElementStatusManager harvesterHighlight;
    [SerializeField] private UIElementStatusManager boilerHighlight;
    //[SerializeField] private UIElementStatusManager greenhouseHighlight;
    //[SerializeField] private UIElementStatusManager incineratorHighlight;
    //[SerializeField] private UIElementStatusManager humidityBarHighlight;
    //[SerializeField] private UIElementStatusManager biodiversityBarHighlight;
    //[SerializeField] private UIElementStatusManager atmosphereBarHighlight;
    [SerializeField] private UIElementStatusManager ratioBarsHighlight;

    [Header("Building Prefabs")]
    [SerializeField] private Building fusionReactorPrefab;
    [SerializeField] private Building iceDrillPrefab;
    [SerializeField] private Building harvesterPrefab;
    [SerializeField] private Building gasPumpPrefab;
    [SerializeField] private Building boilerPrefab;
    [SerializeField] private Building greenhousePrefab;
    [SerializeField] private Building incineratorPrefab;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private DialogueBox console;
    private DialogueBox cat;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageTerraforming. 
    /// </summary>
    public EStage GetID()
    {
        return EStage.Terraforming;
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        console = DialogueBoxManager.Instance.GetDialogueBox("Console");
        cat = DialogueBoxManager.Instance.GetDialogueBox("CAT");
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    public IEnumerator Execution()
    {
        yield return StartCoroutine(IntroduceTerraformingBuildings());
        yield return StartCoroutine(BuildBoiler());
        yield return StartCoroutine(TerraformingWalkthrough());
        yield return StartCoroutine(DemolitionMenu());
        yield return StartCoroutine(StageComplete());        
        StageManager.Instance.SetStage(EStage.Combat);
    }

    /// <summary>
    /// Introduces the player to the terraforming buildings.
    /// </summary>
    private IEnumerator IntroduceTerraformingBuildings()
    {
        cat.SubmitDialogue("got resources", 0, false, false);
        boiler.ButtonInteract.InInteractableGameStage = false;
        boiler.Interactable = false;
        boiler.Visible = true;
        greenhouse.ButtonInteract.InInteractableGameStage = false;
        greenhouse.Interactable = false;
        greenhouse.Visible = true;
        incinerator.ButtonInteract.InInteractableGameStage = false;
        incinerator.Interactable = false;
        incinerator.Visible = true;

        do
        {
            yield return null;
        }
        while (!cat.DialogueRead || !cat.AcceptingSubmissions);
    }

    /// <summary>
    /// Introduces the player to the boiler.
    /// </summary>
    private IEnumerator BuildBoiler()
    {
        console.ClearDialogue();
        console.SubmitDialogue("task build boiler", 0, false, false);
        cat.SubmitDialogue("build boiler", 0, true, false);
        boilerHighlight.Visible = true;

        do
        {
            //Keep gas pump button interactable only while it needs to be placed
            if (BuildingManager.Instance.PlacedBuildingsCount(EBuilding.Boiler) > 0)
            {
                if (fusionReactor.ButtonInteract.InInteractableGameStage) fusionReactor.ButtonInteract.InInteractableGameStage = false;
                if (fusionReactor.Interactable) fusionReactor.Interactable = false;
                if (iceDrill.ButtonInteract.InInteractableGameStage) iceDrill.ButtonInteract.InInteractableGameStage = false;
                if (iceDrill.Interactable) iceDrill.Interactable = false;
                if (gasPump.ButtonInteract.InInteractableGameStage) gasPump.ButtonInteract.InInteractableGameStage = false;
                if (gasPump.Interactable) gasPump.Interactable = false;
                if (boiler.ButtonInteract.InInteractableGameStage) boiler.ButtonInteract.InInteractableGameStage = false;
                if (boiler.Interactable) boiler.Interactable = false;
                if (MineralCollectionController.Instance.CanMine) MineralCollectionController.Instance.CanMine = false;
            }
            else
            {
                //Boiler must be interactable
                if (!boiler.ButtonInteract.InInteractableGameStage)
                {
                    boiler.ButtonInteract.InInteractableGameStage = true;
                    UIBuildingBar.Instance.UpdateButton(boilerPrefab, boiler.ButtonInteract);
                }

                UpdateResourceBuildingButtonInteractability(ResourceManager.Instance.SurplusPower, boilerPrefab.PowerConsumption, fusionReactorPrefab, fusionReactor);
                UpdateResourceBuildingButtonInteractability(ResourceManager.Instance.SurplusWater, boilerPrefab.WaterConsumption, iceDrillPrefab, iceDrill);
                UpdateResourceBuildingButtonInteractability(ResourceManager.Instance.SurplusGas, boilerPrefab.GasConsumption, gasPumpPrefab, gasPump);

                if (MineralCollectionController.Instance.CanMine)
                {
                    if (ResourceManager.Instance.Ore >= boilerPrefab.OreCost 
                        && (!fusionReactor.ButtonInteract.InInteractableGameStage || ResourceManager.Instance.Ore >= fusionReactorPrefab.OreCost)
                        && (!iceDrill.ButtonInteract.InInteractableGameStage || ResourceManager.Instance.Ore >= iceDrillPrefab.OreCost)
                        && (!gasPump.ButtonInteract.InInteractableGameStage || ResourceManager.Instance.Ore >= gasPumpPrefab.OreCost)
                    )
                    {
                        MineralCollectionController.Instance.CanMine = false;
                    }
                }
                else
                {
                    if (ResourceManager.Instance.Ore < boilerPrefab.OreCost 
                        || (fusionReactor.ButtonInteract.InInteractableGameStage && ResourceManager.Instance.Ore < fusionReactorPrefab.OreCost)
                        || (iceDrill.ButtonInteract.InInteractableGameStage && ResourceManager.Instance.Ore < iceDrillPrefab.OreCost)
                        || (gasPump.ButtonInteract.InInteractableGameStage && ResourceManager.Instance.Ore < gasPumpPrefab.OreCost)
                    )
                    {
                        MineralCollectionController.Instance.CanMine = true;
                    }
                } 
            }

            yield return null;
        }
        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.Boiler) == 0);

        fusionReactor.ButtonInteract.InInteractableGameStage = false;
        fusionReactor.Interactable = false;
        gasPump.ButtonInteract.InInteractableGameStage = false;
        gasPump.Interactable = false;
        MineralCollectionController.Instance.CanMine = false;
    }

    /// <summary>
    /// Brings the ratio bars to the player's attention and their relationship to the progress of the terraforming of the planet.
    /// </summary>
    private IEnumerator TerraformingWalkthrough()
    {
        cat.SubmitDialogue("ratio bars", 0, false, false);
        progressBar.Visible = true;
        humidityBar.Visible = true;
        biodiversityBar.Visible = true;
        atmosphereBar.Visible = true;
        ratioBarsHighlight.Visible = true;

        do
        {
            yield return null;
        }
        while (!cat.DialogueRead || !cat.AcceptingSubmissions);

        console.ClearDialogue();
        console.SubmitDialogue("task press p", 0, false, false);
        cat.SubmitDialogue("press p", 0, true, false);

        do
        {
            yield return null;
        }
        while (!TerraformingUI.Instance.IsEnabled);

        if (!cat.DialogueRead) cat.DialogueRead = true;
        cat.SubmitDialogue("terraforming progress", 0, true, false);

        do
        {
            yield return null;
        }
        while (TerraformingUI.Instance.IsEnabled);

        if (!cat.DialogueRead) cat.DialogueRead = true;
    }

    /// <summary>
    /// Introduces the player to the demolition menu.
    /// </summary>
    private IEnumerator DemolitionMenu()
    {
        BuildingDemolitionController.Instance.CanDemolish = true;
        console.ClearDialogue();
        console.SubmitDialogue("task click building", 0, false, false);
        cat.SubmitDialogue("demolition menu", 0, true, false);

        do
        {
            yield return null;
        }
        while (!BuildingDemolitionController.Instance.ShowingDemolitionMenu);

        if (!cat.DialogueRead) cat.DialogueRead = true;
        cat.SubmitDialogue("enabling buildings", 0, true, false);

        do
        {
            yield return null;
        }
        while (BuildingDemolitionController.Instance.ShowingDemolitionMenu);

        if (!cat.DialogueRead) cat.DialogueRead = true;
        BuildingDemolitionController.Instance.CanDemolish = false;
    }

    /// <summary>
    /// Concludes the terraforming stage of the tutorial.
    /// </summary>
    private IEnumerator StageComplete()
    {
        cat.SubmitDialogue("good luck", 0, true, false);
        clock.Visible = true;
        ClockManager.Instance.Paused = false;

        do
        {
            yield return null;
        }
        while (!cat.DialogueRead || !cat.AcceptingSubmissions);

        MineralCollectionController.Instance.CanMine = true;
        BuildingDemolitionController.Instance.CanDemolish = true;
        console.SubmitDialogue("cat closed", 1, false, false);

        fusionReactor.ButtonInteract.InInteractableGameStage = true;
        iceDrill.ButtonInteract.InInteractableGameStage = true;
        harvester.ButtonInteract.InInteractableGameStage = true;
        gasPump.ButtonInteract.InInteractableGameStage = true;
        boiler.ButtonInteract.InInteractableGameStage = true;
        greenhouse.ButtonInteract.InInteractableGameStage = true;
        incinerator.ButtonInteract.InInteractableGameStage = true;

        UIBuildingBar.Instance.UpdateButton(fusionReactorPrefab, fusionReactor.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(iceDrillPrefab, iceDrill.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(harvesterPrefab, harvester.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(gasPumpPrefab, gasPump.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(boilerPrefab, boiler.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(greenhousePrefab, greenhouse.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(incineratorPrefab, incinerator.ButtonInteract);
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if a building that provides resources for the building the tutorial currently wants built still needs to be built or if there is enough of the resource it produces.
    /// </summary>
    /// <param name="resourceSurplus">The current surplus of the resource the building produces.</param>
    /// <param name="requiredQty">The ammount that the currently required building needs in order to be built.</param>
    /// <param name="buildingPrefab">The prefab of the resource building under consideration.</param>
    /// <param name="buildingButton">The UI button for the resource building under consideration.</param>
    private void UpdateResourceBuildingButtonInteractability(float resourceSurplus, float requiredQty, Building buildingPrefab, UIElementStatusManager buildingButton)
    {
        if (resourceSurplus < requiredQty)
        {
            if (!buildingButton.ButtonInteract.InInteractableGameStage)
            {
                buildingButton.ButtonInteract.InInteractableGameStage = true;
                UIBuildingBar.Instance.UpdateButton(buildingPrefab, buildingButton.ButtonInteract);
            }
        }
        else
        {
            if (buildingButton.ButtonInteract.InInteractableGameStage)
            {
                buildingButton.ButtonInteract.InInteractableGameStage = false;
                buildingButton.Interactable = false;
            }
        }
    }
}
