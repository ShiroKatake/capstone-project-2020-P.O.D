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

    /// <summary>
    /// Triggers the main behaviour of StageControls.
    /// </summary>
    public override void StartExecution()
    {
        StartCoroutine(Execution());
    }

    /// <summary>
    /// The main behaviour of StageControls.
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    protected override IEnumerator Execution()
    {
        test1 = DialogueBoxManager.Instance.GetDialogueBox("Test 1");
        test2 = DialogueBoxManager.Instance.GetDialogueBox("Test 2");
        test1.SubmitDialogue("test single", 5, false);

        while (!test1.DialogueRead)
        {
            yield return null;
        }

        test1.SubmitDialogue("test multiple", 0, true);

        while (!test1.DialogueRead)
        {
            yield return null;
        }

        test2.SubmitDialogue("test multiple", 0, true);

        while (!test2.DialogueRead)
        {
            yield return null;
        }

        Debug.Log("Finished test");
    }
}
