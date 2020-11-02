using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game that is triggered if the player loses.
/// </summary>
public class StageLose : PublicInstanceSerializableSingleton<StageLose>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private GameObject uiManager;
    [SerializeField] private GOMessageManager goMessage;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageLose. 
    /// </summary>
    public EStage GetID()
    {
        return EStage.Lose;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    public IEnumerator Execution()
    {
        goMessage.SetText(false);
        uiManager.GetComponent<UIManager>().SetGameOverCanvasActive(true);
		Time.timeScale = 0;
        yield return null;
    }
}
