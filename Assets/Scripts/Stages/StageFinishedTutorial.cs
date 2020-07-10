using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is just left to do their own thing and play the game.
/// </summary>
public class StageFinishedTutorial : Stage
{
    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// StageFinishedTutorial's singleton public property.
    /// </summary>
    public StageFinishedTutorial Instance { get; protected set; }

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
        id = EStage.FinishedTutorial;
        base.Awake();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of StageFinishedTutorial.
    /// </summary>
    public override void Execute()
    {
        //Nothing
    }
}
