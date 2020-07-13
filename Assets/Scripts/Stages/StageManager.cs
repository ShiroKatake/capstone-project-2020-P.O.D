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
public class StageManager : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private bool skipTutorial;
    [SerializeField] private EStage firstStage;

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
    /// The current stage of the game.
    /// </summary>
    public Stage CurrentStage { get => currentStage; }

    /// <summary>
    /// Whether the player has elected to skip the tutorial or not.
    /// </summary>
    public bool SkipTutorial { get => skipTutorial; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
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
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
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
        if (skipTutorial)
        {
            currentStage = stages[EStage.FinishedTutorial];
            //TODO: swap for a stage for skipping the tutorial
        }
        else
        {
            currentStage = stages[firstStage];
            //TODO: Set the clock to 20% of daytime and turn its ticking off.
        }

        currentStage.StartExecution();
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// Update() is run every frame.
    ///// </summary>
    //private void Update()
    //{
    //    currentStage.Execute();
    //}

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