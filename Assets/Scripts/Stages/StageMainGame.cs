﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is just left to do their own thing and play the game.
/// </summary>
public class StageMainGame: SerializableSingleton<StageMainGame>, IStage
{
    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    ////Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// StageMAinGame's singleton public property.
    ///// </summary>
    //public StageMainGame Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageMainGame. 
    /// </summary>
    public EStage GetID()
    {
        return EStage.MainGame;
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
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    public IEnumerator Execution()
    {
        while (true)
        {
            if (EnvironmentalController.Instance.Win)
            {
                StageManager.Instance.SetStage(EStage.Win);
                AudioManager.Instance.PlaySound(AudioManager.ESound.Win);
                AudioManager.Instance.StopBackGroundMusic();
                break;
            }
            else if (CryoEgg.Instance.Health.IsDead())
            {
                StageManager.Instance.SetStage(EStage.Lose);
                AudioManager.Instance.PlaySound(AudioManager.ESound.Lose);
                AudioManager.Instance.StopBackGroundMusic();
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
}
