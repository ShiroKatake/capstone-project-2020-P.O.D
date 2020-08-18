using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game that is triggered if the player wins.
/// </summary>
public class StageWin : SerializableSingleton<StageWin>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private GameObject uiManager;
    [SerializeField] private GOMessageController goMessage;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    ////Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// StageWin's singleton public property.
    ///// </summary>
    //public StageWin Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageWin.
    /// </summary>
    public EStage GetID()
    {
        return EStage.Win;
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        //if (Instance != null)
        //{
        //    Debug.LogError("There should never be more than one StageFinishedTutorial.");
        //}

        //Instance = this;
        base.Awake();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of StageFinishedTutorial.
    /// </summary>
    public void StartExecution()
    {
        StartCoroutine(Execution());
    }

    /// <summary>
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    public IEnumerator Execution()
    {
        goMessage.SetText(true);
        uiManager.GetComponent<UIManager>().SetGameOverCanvasActive(true);
        yield return null;
    }
}
