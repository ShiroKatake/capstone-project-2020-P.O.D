using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public enum ButtonType
{
    None,
    AirCannon,
    Generator,
    Harvester,
    Extender,
    FogRepeller,
    Destroy,
    Upgrades
}

//TODO: this shit needs cleaning up so that it's not a *check's Visual Studio* 2853-line wall of text.
//Look at the GTFO post mortem PDF and check out the proposed restructure it mentions for the tutorial controller to get an idea of how it should look.

public class StageManager : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private bool skipTutorial;
    [SerializeField] private List<btnTutorial> subscribedButtons;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Dictionary<EStage, Stage> stages;
    private Stage currentStage;
    private Stage savedStage;
    private int savedStep;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// StageManager's singleton public property.
    /// </summary>
    public static StageManager Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The current stage of the game;
    /// </summary>
    public Stage CurrentStage { get => currentStage; }

    /// <summary>
    /// Whether the player has elected to skip the tutorial or not;
    /// </summary>
    public bool SkipTutorial { get => skipTutorial; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    //Ensures singleton-ness
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more StageManagers.");
        }

        Instance = this;

        //if (GlobalVars.LoadedFromMenu)
        //{
        //    skipTutorial = GlobalVars.SkipTut;
        //}
    }

    /// <summary>
    /// Triggers the start of the game.
    /// </summary>
    public void BeginGame()
    {

    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {

    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves the chosen stage if it exists.
    /// </summary>
    /// <param name="stage">The particular stage you want to retrieve.</param>
    /// <returns>The stage you wanted to retrieve.</returns>
    public Stage GetStage(EStage stage)
    {
        if (stages.ContainsKey(stage))
        {
            return stages[stage];
        }

        return null;
    }

    /// <summary>
    /// Subscribes a btnTutorial to the StageManager's list of subscribed buttons.
    /// </summary>
    /// <param name="button">The btnTutorial being subscribed to the StageManager.</param>
    public void Subscribe(btnTutorial button)
    {
        if (!subscribedButtons.Contains(button))
        {
            subscribedButtons.Add(button);
        }
    }

    /// <summary>
    /// Removes a btnTutorial from the StageManager's list of subscribed buttons.
    /// </summary>
    /// <param name="button">The btnTutorial being unsubscribed from the StageManager.</param>
    public void Unsubscribe(btnTutorial button)
    {
        if (subscribedButtons.Contains(button))
        {
            subscribedButtons.Remove(button);
        }
    }


    public void NotifySubscribers(List<ButtonType> buttons)
    {

    }
}