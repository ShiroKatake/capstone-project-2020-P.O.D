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



/// <summary>
/// A manager class for the current stage of the game
/// </summary>
public class StageManager : SerializableSingleton<StageManager>
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private bool skipTutorial;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Dictionary<EStage, Stage> stages;
    private Stage currentStage;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The current stage of the game.
    /// </summary>
    public Stage CurrentStage { get => currentStage; }

    /// <summary>
    /// Whether the player has elected to skip the tutorial or not.
    /// </summary>
    public bool SkipTutorial { get => skipTutorial; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        if (SceneLoaderListener.Instance.SceneLoaderInstantiatedOnAwake)
        {
            skipTutorial = SceneLoader.Instance.SkipTutorial;
        }

        GetStages();
        BeginGame();
    }

    /// <summary>
    /// Gets all stage classes attached to the StageManager's game object.
    /// </summary>
    private void GetStages()
    {
        stages = new Dictionary<EStage, Stage>();
        Stage[] stageList = GetComponents<Stage>();

        foreach (Stage s in stageList)
        {
            if (!stages.ContainsKey(s.ID))
            {
                stages[s.ID] = s;
            }
            else
            {
                Debug.Log($"There there is more than one stage being processed by StageManager with the ID {s.ID}. Each stage should have a unique ID. Go back and check their Awake() methods.");
            }
        }
    }

    /// <summary>
    /// Sets up the start of the game.
    /// </summary>
    public void BeginGame()
    {
        currentStage = stages[(skipTutorial ? EStage.SkippedTutorial : EStage.Controls)];
        currentStage.StartExecution();
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
    /// Sets the selected stage as the current stage of the game.
    /// </summary>
    /// <param name="stage">The stage that will become the current stage of the game.</param>
    public void SetStage(EStage stage)
    {
        if (stages.ContainsKey(stage))
        {
            currentStage = stages[stage];
            currentStage.StartExecution();
        }
        else
        {
            Debug.LogError($"StageManager does not have a stage {stage}");
        }
    }
}