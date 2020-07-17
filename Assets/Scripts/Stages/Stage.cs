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

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The EStage value denoting which stage this is
    /// </summary>
    public EStage ID { get => id; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

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
}
