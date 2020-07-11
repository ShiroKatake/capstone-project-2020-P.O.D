using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract base class for game stages.
/// </summary>
public abstract class Stage : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    protected EStage id;
    protected int step;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The EStage value denoting which stage this is
    /// </summary>
    public EStage ID { get => id; }

    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected virtual void Awake()
    {
        step = 1;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        if (id == EStage.None)
        {
            Debug.LogError($"There's a stage class that hasn't had implemented the assignment of its EStage value to Stage.id in StageSomething.Awake().");
        }
    }

    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------



    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Executes the stage's code.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Updates the current stage of the game.
    /// </summary>
    public virtual void UpdateStage(EStage stage)
    {
        StageManager.Instance.SetStage(stage);
    }

    /// <summary>
    /// Called by btnTutorial to register that that button has been clicked
    /// </summary>
    //public void RegisterMouseClicked()
    //{
    //    tileClicked = true;
    //}

    /// <summary>
    /// Called by DialogueBox to register that dialogue has all been read
    /// </summary>
    //public void RegisterDialogueRead()
    //{
    //    dialogueRead = true;
    //}

    /// <summary>
    /// Resets the dialogueRead variable
    /// </summary>
    //protected void ResetDialogueRead()
    //{
    //    dialogueRead = false;
    //}



    /// <summary>
    /// Checks if a tile is allowed to be built on.
    /// </summary>
    /// <param name="tile">The tile being assessed.</param>
    /// <returns>Whether that tile can be built on.</returns>
    //public bool TileAllowed(TileData tile)
    //{
    //    bool tileOkay;

    //    if (!cameraController.FinishedOpeningCameraPan || (stage <= TutorialStage.CollectSonar && tile == sonarLandmarkTile))
    //    {
    //        return false;
    //    }

    //    switch (stage)
    //    {
    //        case TutorialStage.BuildHarvesters:
    //            if (step < 3 || step > 6 || !lerpTargetLock)
    //            {
    //                tileOkay = tile.Resource != null && !tile.FogUnitActive;
    //            }
    //            else
    //            {
    //                tileOkay = tile == targetTile;
    //            }

    //            break;
    //        case TutorialStage.BuildExtender:
    //        case TutorialStage.BuildMortar:
    //            if (step < 3 || !lerpTargetLock)
    //            {
    //                tileOkay = tile.Resource == null && !tile.FogUnitActive;
    //            }
    //            else
    //            {
    //                tileOkay = tile == targetTile;
    //            }

    //            break;
    //        case TutorialStage.BuildHarvestersExtended:
    //            tileOkay = tile.Resource != null && !tile.FogUnitActive;
    //            break;
    //        case TutorialStage.BuildGenerator:
    //            if (step == 7 || !lerpTargetLock)
    //            {
    //                tileOkay = tile.Resource == null && !tile.FogUnitActive;
    //            }
    //            else
    //            {
    //                tileOkay = tile == targetTile;
    //            }

    //            break;
    //        case TutorialStage.BuildMoreGenerators:
    //            tileOkay = tile.Resource == null && !tile.FogUnitActive;
    //            break;
    //        case TutorialStage.CollectMinerals:
    //        case TutorialStage.DefenceActivation:
    //        case TutorialStage.BuildDefencesInRange:
    //            tileOkay = !tile.FogUnitActive || tile.Building != null;

    //            if (!tileOkay && tile.FogUnitActive && !dialogueBox.Activated)
    //            {
    //                savedStage = stage;
    //                savedSubStage = step;

    //                stage = TutorialStage.DontBuildInFog;
    //                step = 1;
    //            }

    //            break;
    //        case TutorialStage.CollectSonar:
    //            tileOkay = !tile.FogUnitActive;
    //            break;
    //        case TutorialStage.ActivateSonar:
    //            tileOkay = Vector3.Distance(tile.Position, buildingTarget.transform.position) < targetRenderer.bounds.extents.x;  //.x or .z will work perfectly fine here, they'll have the radius (orthogonal extent) of the lerp target
    //            break;
    //        case TutorialStage.BuildExtenderInFog:
    //            tileOkay = tile.Resource == null;   //TODO: add extra substages if power doesn't reach fog? Would need to be accounted for here.
    //            break;
    //        case TutorialStage.BuildPulseDefence:
    //            tileOkay = (tile.Resource == null && !tile.FogUnitActive) || tile.Building != null;

    //            if (!tileOkay && !dialogueBox.Activated)
    //            {
    //                savedStage = stage;
    //                savedSubStage = step;

    //                stage = TutorialStage.DontBuildInFog;
    //                step = 1;
    //            }

    //            break;
    //        case TutorialStage.Finished:
    //            tileOkay = true;
    //            break;
    //        default:
    //            Debug.Log("TutorialController.TileAllowed().default case");
    //            tileOkay = tile == targetTile;
    //            break;
    //    }

    //    //if (tileOkay && (saveTile /*|| !lerpTargetLock*/) && (stage != TutorialStage.BuildExtenderInFog || tile.FogUnitActive))
    //    //{
    //    //    lastTileChecked = tile;
    //    //}

    //    if (tileOkay || !lerpTargetLock)
    //    {
    //        lastTileChecked = tile;
    //    }

    //    //lastTileChecked = tile;

    //    return tileOkay;
    //}

    /// <summary>
    /// Checking if a button is allowed to be clicked.
    /// </summary>
    /// <param name="button">The button being assessed.</param>
    /// <returns>Whether the button is allowed to be clicked.</returns>
    //public bool ButtonAllowed(ButtonType button)
    //{
    //    bool buttonOkay = false;

    //    if (ButtonsNormallyAllowed(lastTileChecked).Contains(button))
    //    {
    //        switch (stage)
    //        {
    //            case TutorialStage.BuildHarvesters:
    //            case TutorialStage.BuildHarvestersExtended:
    //                buttonOkay = button == ButtonType.Harvester;
    //                break;
    //            case TutorialStage.BuildExtender:
    //                buttonOkay = button == ButtonType.Extender;
    //                break;
    //            case TutorialStage.BuildGenerator:
    //            case TutorialStage.BuildMoreGenerators:
    //                buttonOkay = button == ButtonType.Generator;
    //                //Debug.Log($"Stage BuildGenerator / BuildMoreGenerators. ButtonType: {button}; ButtonOkay: {buttonOkay}");
    //                break;
    //            case TutorialStage.CollectMinerals:
    //            case TutorialStage.CollectSonar:
    //                buttonOkay = button == ButtonType.Extender
    //                       || button == ButtonType.Harvester
    //                       || button == ButtonType.Generator
    //                       || button == ButtonType.Destroy;
    //                break;
    //            case TutorialStage.BuildExtenderInFog:
    //                buttonOkay = button == ButtonType.Extender;
    //                break;
    //            case TutorialStage.BuildMortar:
    //                buttonOkay = button == ButtonType.AirCannon;
    //                break;
    //            case TutorialStage.BuildPulseDefence:
    //                buttonOkay = (button == ButtonType.FogRepeller /*&& subStage >= 5*/)
    //                       /*|| (button == ButtonType.Extender && lastTileChecked != pulseDefenceLandmark.Location)*/;
    //                break;
    //            case TutorialStage.DefenceActivation:
    //            case TutorialStage.BuildDefencesInRange:
    //                buttonOkay = button != ButtonType.Upgrades;
    //                break;
    //            case TutorialStage.Finished:
    //                buttonOkay = true;
    //                break;
    //            default:
    //                buttonOkay = button == currentlyLerping || button == ButtonType.Destroy;
    //                break;
    //        }
    //    }

    //    return buttonOkay;
    //}

    //Getting the normally acceptable buttons for a tile
    //private List<ButtonType> ButtonsNormallyAllowed(TileData tile)
    //{
    //    List<ButtonType> result = new List<ButtonType>();

    //    if (tile.Resource != null)
    //    {
    //        result.Add(ButtonType.Harvester);
    //    }
    //    else
    //    {
    //        if (ResourceController.Instance.Generators.Count < ObjectiveController.Instance.GeneratorLimit)
    //        {
    //            result.Add(ButtonType.Generator);
    //        }

    //        result.Add(ButtonType.AirCannon);
    //        result.Add(ButtonType.Extender);
    //        result.Add(ButtonType.FogRepeller);
    //    }

    //    result.Add(ButtonType.Destroy);
    //    result.Add(ButtonType.Upgrades);
    //    return result;
    //}


    /// <summary>
    /// Tells MouseController to report clicks to StageManager
    /// </summary>
    //private void ActivateMouse()
    //{
    //    MouseController.Instance.ReportTutorialClick = true;
    //}

    /// <summary>
    /// Dismisses the mouse and increments the step
    /// </summary>
    //private void DismissMouse()
    //{
    //    MouseController.Instance.ReportTutorialClick = false;
    //    tileClicked = false;
    //    currentlyLerping = ButtonType.None;
    //    IncrementStep();
    //}

    ///// <summary>
    ///// Sends dialogue to the dialogue box and increments the step.
    ///// </summary>
    ///// <param name="dialogueBoxID">The ID of the dialogue box being sent dialogue.</param>
    ///// <param name="dialogueKey">The dialogue key of the dialogue set to be displayed.</param>
    ///// <param name="invokeDelay">How long the dialogue should be delayed before displaying.</param>
    //protected void SendDialogue(string dialogueBoxID, string dialogueKey, float invokeDelay)
    //{
    //    DialogueBoxManager.Instance.GetDialogueBox(dialogueBoxID).SubmitDialogueSet(dialogueKey, invokeDelay);
    //    IncrementStep();
    //}

    /// <summary>
    /// Dismisses the dialogue and increments the step
    /// </summary>
    //private void DismissDialogue(string dialogueBoxID)
    //{
    //    ResetDialogueRead();
    //    IncrementStep();
    //}

    /// <summary>
    /// Increments the step by 1
    /// </summary>
    protected void IncrementStep()
    {
        step++;
    }

    /// <summary>
    /// Dismisses dialogue and the mouse and advances/retreats to the specified step appropriately
    /// </summary>
    /// <param name="nextStep">The step to be advanced to.</param>
    protected void GoToStep(int nextStep)
    {
        //ResetDialogueRead();
        step = nextStep;
    }

    /// <summary>
    /// Toggles visibility of the objective window.
    /// </summary>
    //public void ToggleObjWindow()
    //{
    //    if (!objWindowVisible)
    //    {
    //        objectiveWindow.GetComponent<RectTransform>().DOAnchorPosY(205, 0.3f).SetEase(Ease.OutCubic);
    //        objectiveWindowOpenArrows.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 270), 0.3f);
    //        StageManager.Instance.ObjWindowVisible = true;
    //        ObjectiveController.Instance.ObjWindowVisible = true;
    //    }
    //    else
    //    {
    //        objectiveWindow.GetComponent<RectTransform>().DOAnchorPosY(25, 0.3f).SetEase(Ease.InCubic);
    //        objectiveWindowOpenArrows.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 90), 0.3f);
    //        StageManager.Instance.ObjWindowVisible = false;
    //        ObjectiveController.Instance.ObjWindowVisible = false;
    //    }
    //}

    //Runs a "yay, you did the thing" screen
    //Borrowed and adapted from ObjectiveController
    //IEnumerator CompleteTutorialObjective(string message)
    //{
    //    GameObject objComp = Instantiate(ObjectiveController.Instance.ObjectiveCompletePrefab, GameObject.Find("Canvas").transform);//Apparently TC's prefab is broken, but OC's isn't
    //    GameObject objCompImage = objComp.GetComponentInChildren<Image>().gameObject;
    //    TextMeshProUGUI unlocksText = objCompImage.GetComponentInChildren<TextMeshProUGUI>();

    //    unlocksText.text = message;
    //    objCompImage.GetComponent<RectTransform>().DOAnchorPosX(0, 0.3f).SetEase(Ease.OutQuad).SetUpdate(true);

    //    yield return new WaitForSecondsRealtime(3f);

    //    objCompImage.GetComponent<RectTransform>().DOAnchorPosX(1250, 0.3f).SetEase(Ease.InQuad).SetUpdate(true);

    //    yield return new WaitForSecondsRealtime(0.3f);

    //    Destroy(objComp);
    //}

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
