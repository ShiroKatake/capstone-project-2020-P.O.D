using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is introduced to the resource collection buildings.
/// </summary>
public class StageResourceBuildings : PublicInstanceSerializableSingleton<StageResourceBuildings>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Building Buttons")]
    [SerializeField] private UIElementStatusManager fusionReactor;
    [SerializeField] private UIElementStatusManager iceDrill;
    [SerializeField] private UIElementStatusManager harvester;
    [SerializeField] private UIElementStatusManager gasPump;

    [Header("Highlights")]
    [SerializeField] private UIElementStatusManager fusionReactorHighlight;
    [SerializeField] private UIElementStatusManager iceDrillHighlight;
    [SerializeField] private UIElementStatusManager gasPumpHighlight;

    [Header("Building Prefabs")]
    [SerializeField] private Building fusionReactorPrefab;
    [SerializeField] private Building iceDrillPrefab;
    [SerializeField] private Building gasPumpPrefab;

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
        return EStage.ResourceBuildings;
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
        Debug.Log($"StageResourceBuildings.Execution()");
        yield return StartCoroutine(BuildingsIntroduction());
        yield return StartCoroutine(BuildFusionReactor());
        yield return StartCoroutine(IntroduceResourceCollectors());
        yield return StartCoroutine(BuildIceDrill());
        yield return StartCoroutine(BuildGasPump());
        StageManager.Instance.SetStage(EStage.Terraforming);
    }

    /// <summary>
    /// Informs the player of the existence of buildings.
    /// </summary>
    private IEnumerator BuildingsIntroduction()
    {
        cat.SubmitDialogue("enough for building", 0, false, false);

        do
        {
            yield return null;
        }
        while (!cat.DialogueRead || !cat.AcceptingSubmissions);

        cat.SubmitDialogue("building blueprints", 0, false, false);

        do
        {
            yield return null;
        }
        while (!cat.DialogueRead || !cat.AcceptingSubmissions);
    }

    /// <summary>
    /// Teaches the player about the fusion reactor.
    /// </summary>
    private IEnumerator BuildFusionReactor()
    {
        console.ClearDialogue();
        console.SubmitDialogue("task build fusion reactor", 0, false, false);
        cat.SubmitDialogue("build fusion reactor", 0, true, false);
        fusionReactor.Visible = true;
        fusionReactorHighlight.Visible = true;

        do
        {
            //Keep fusion reactor button interactable only while it needs to be placed
            if (BuildingManager.Instance.PlacedBuildingsCount(EBuilding.FusionReactor) > 0)
            {
                if (fusionReactor.ButtonInteract.InInteractableGameStage) fusionReactor.ButtonInteract.InInteractableGameStage = false;
                if (fusionReactor.Interactable) fusionReactor.Interactable = false;
            }
            else
            {
                if (!fusionReactor.ButtonInteract.InInteractableGameStage)
                {
                    fusionReactor.ButtonInteract.InInteractableGameStage = true;
                    UIBuildingBar.Instance.UpdateButton(fusionReactorPrefab, fusionReactor.ButtonInteract);
                }

                if (MineralCollectionController.Instance.CanMine)
                {
                    if (ResourceManager.Instance.Ore >= fusionReactorPrefab.OreCost) MineralCollectionController.Instance.CanMine = false;
                }
                else
                {
                    if (ResourceManager.Instance.Ore < fusionReactorPrefab.OreCost) MineralCollectionController.Instance.CanMine = true;
                } 
            }

            yield return null;
        }
        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.FusionReactor) == 0) ;

        fusionReactor.ButtonInteract.InInteractableGameStage = false;
        fusionReactor.Interactable = false;
    }

    /// <summary>
    /// Introduces the player to the resource buildings.
    /// </summary>
    private IEnumerator IntroduceResourceCollectors()
    {
        cat.SubmitDialogue("resource buildings", 0, true, false);
        iceDrill.ButtonInteract.InInteractableGameStage = false;
        iceDrill.Interactable = false;
        iceDrill.Visible = true;
        harvester.ButtonInteract.InInteractableGameStage = false;
        harvester.Interactable = false;
        harvester.Visible = true;
        gasPump.ButtonInteract.InInteractableGameStage = false;
        gasPump.Interactable = false;
        gasPump.Visible = true;

        do
        {
            yield return null;
        }
        while (!cat.DialogueRead || !cat.AcceptingSubmissions);
    }

    /// <summary>
    /// Teaches the player about the ice drill.
    /// </summary>
    private IEnumerator BuildIceDrill()
    {
        console.ClearDialogue();
        console.SubmitDialogue("task build ice drill", 0, false, false);
        cat.SubmitDialogue("build ice drill", 0, true, false);
        iceDrillHighlight.Visible = true;

        do
        {
            //Keep ice drill button interactable only while it needs to be placed
            if (BuildingManager.Instance.PlacedBuildingsCount(EBuilding.IceDrill) > 0)
            {
                if (fusionReactor.ButtonInteract.InInteractableGameStage) fusionReactor.ButtonInteract.InInteractableGameStage = false;
                if (fusionReactor.Interactable) fusionReactor.Interactable = false;
                if (iceDrill.ButtonInteract.InInteractableGameStage) iceDrill.ButtonInteract.InInteractableGameStage = false;
                if (iceDrill.Interactable) iceDrill.Interactable = false;
                if (MineralCollectionController.Instance.CanMine) MineralCollectionController.Instance.CanMine = false;
            }
            else
            {
                if (!iceDrill.ButtonInteract.InInteractableGameStage)
                {
                    iceDrill.ButtonInteract.InInteractableGameStage = true;
                    UIBuildingBar.Instance.UpdateButton(iceDrillPrefab, iceDrill.ButtonInteract);
                }

                UpdateResourceBuildingButtonInteractability(ResourceManager.Instance.SurplusPower, iceDrillPrefab.PowerConsumption, fusionReactorPrefab, fusionReactor);

                if (MineralCollectionController.Instance.CanMine)
                {
                    if (ResourceManager.Instance.Ore >= iceDrillPrefab.OreCost
                        && (!fusionReactor.ButtonInteract.InInteractableGameStage || ResourceManager.Instance.Ore >= fusionReactorPrefab.OreCost))
                    {
                        MineralCollectionController.Instance.CanMine = false;
                    }
                }
                else
                {
                    if (ResourceManager.Instance.Ore < iceDrillPrefab.OreCost
                        || (fusionReactor.ButtonInteract.InInteractableGameStage && ResourceManager.Instance.Ore < fusionReactorPrefab.OreCost))
                    {
                        MineralCollectionController.Instance.CanMine = true;
                    }
                }
            }

            yield return null;
        }
        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.IceDrill) == 0);

        fusionReactor.ButtonInteract.InInteractableGameStage = false;
        fusionReactor.Interactable = false;
        iceDrill.ButtonInteract.InInteractableGameStage = false;
        iceDrill.Interactable = false;
        MineralCollectionController.Instance.CanMine = false;
    }

    /// <summary>
    /// Teaches the player about the gas pump.
    /// </summary>
    private IEnumerator BuildGasPump()
    {
        console.ClearDialogue();
        console.SubmitDialogue("task build gas pump", 0, false, false);
        cat.SubmitDialogue("build gas pump", 0, true, false);
        gasPumpHighlight.Visible = true;

        do
        {
            //Keep gas pump button interactable only while it needs to be placed
            if (BuildingManager.Instance.PlacedBuildingsCount(EBuilding.GasPump) > 0)
            {
                if (fusionReactor.ButtonInteract.InInteractableGameStage) fusionReactor.ButtonInteract.InInteractableGameStage = false;
                if (fusionReactor.Interactable) fusionReactor.Interactable = false;
                if (gasPump.ButtonInteract.InInteractableGameStage) gasPump.ButtonInteract.InInteractableGameStage = false;
                if (gasPump.Interactable) gasPump.Interactable = false;
                if (MineralCollectionController.Instance.CanMine) MineralCollectionController.Instance.CanMine = false;
            }
            else
            {
                if (!gasPump.ButtonInteract.InInteractableGameStage)
                {
                    gasPump.ButtonInteract.InInteractableGameStage = true;
                    UIBuildingBar.Instance.UpdateButton(gasPumpPrefab, gasPump.ButtonInteract);
                }

                UpdateResourceBuildingButtonInteractability(ResourceManager.Instance.SurplusPower, gasPumpPrefab.PowerConsumption, fusionReactorPrefab, fusionReactor);

                if (MineralCollectionController.Instance.CanMine)
                {
                    if (ResourceManager.Instance.Ore >= gasPumpPrefab.OreCost 
                        && (!fusionReactor.ButtonInteract.InInteractableGameStage || ResourceManager.Instance.Ore >= fusionReactorPrefab.OreCost))
                    {
                        MineralCollectionController.Instance.CanMine = false;
                    }
                }
                else
                {
                    if (ResourceManager.Instance.Ore < gasPumpPrefab.OreCost
                        || (fusionReactor.ButtonInteract.InInteractableGameStage && ResourceManager.Instance.Ore < fusionReactorPrefab.OreCost))
                    {
                        MineralCollectionController.Instance.CanMine = true;
                    }
                }
            }

            yield return null;
        }
        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.GasPump) == 0);

        fusionReactor.ButtonInteract.InInteractableGameStage = false;
        fusionReactor.Interactable = false;
        gasPump.ButtonInteract.InInteractableGameStage = false;
        gasPump.Interactable = false;
        MineralCollectionController.Instance.CanMine = false;
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
