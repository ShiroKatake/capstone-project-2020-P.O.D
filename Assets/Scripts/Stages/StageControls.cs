using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is taught the game's controls
/// </summary>
public class StageControls : Stage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Non-Serialized Fields--------------------------------------------------------------------------------------------------------------------------

    private DialogueBox test1; 
    private DialogueBox test2; 

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// StageControls' singleton public property.
    /// </summary>
    public StageControls Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one StageControls.");
        }

        Instance = this;
        id = EStage.Controls;
        base.Awake();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //TODO: turn into coroutine, or method that triggers a coroutine and the coroutine itself
    /// <summary>
    /// The main behaviour of StageControls.
    /// </summary>
    public override void Execute()
    {
        switch (step)
        {
            case 1:
                test1 = DialogueBoxManager.Instance.GetDialogueBox("Test 1");
                test2 = DialogueBoxManager.Instance.GetDialogueBox("Test 2");
                test1.SubmitDialogue("test single", 5, false);
                IncrementStep();
                break;
            case 2:
                if (test1.DialogueRead)
                {
                    test1.SubmitDialogue("test multiple", 0, true);
                    IncrementStep();
                }

                break;
            case 3:
                if (test1.DialogueRead)
                {
                    test2.SubmitDialogue("test multiple", 0, true);
                    IncrementStep();
                }

                break;
            case 4:
                if (test2.DialogueRead)
                {
                    Debug.Log("Finished test");
                    IncrementStep();
                }

                break;
            default:

                break;
        }
    }
}
