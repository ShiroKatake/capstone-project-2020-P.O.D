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

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Triggers the main behaviour of the stage.
    /// </summary>
    public abstract void StartExecution();

    /// <summary>
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    protected abstract IEnumerator Execution();

    /// <summary>
    /// Updates the current stage of the game.
    /// </summary>
    public virtual void UpdateStage(EStage stage)
    {
        StageManager.Instance.SetStage(stage);
    }

    ///// <summary>
    ///// Increments the step by 1
    ///// </summary>
    //protected void IncrementStep()
    //{
    //    step++;
    //}

    ///// <summary>
    ///// Dismisses dialogue and the mouse and advances/retreats to the specified step appropriately
    ///// </summary>
    ///// <param name="nextStep">The step to be advanced to.</param>
    //protected void GoToStep(int nextStep)
    //{
    //    //ResetDialogueRead();
    //    step = nextStep;
    //}

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
}
