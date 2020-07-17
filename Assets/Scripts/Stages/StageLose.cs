using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game that is triggered if the player loses.
/// </summary>
public class StageLose : Stage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private GameObject uiManager;
    [SerializeField] private GOMessageController goMessage;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// StageLose's singleton public property.
    /// </summary>
    public StageLose Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one StageFinishedTutorial.");
        }

        Instance = this;
        id = EStage.Lose;
        base.Awake();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of StageFinishedTutorial.
    /// </summary>
    public override void StartExecution()
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
    protected override IEnumerator Execution()
    {
        goMessage.SetText(false);
        uiManager.GetComponent<UIAppearScript>().ToggleVisibility();
        yield return null;
    }
}
