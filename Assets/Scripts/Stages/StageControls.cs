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

    private DialogueBox cat; 
    private DialogueBox dog; 
    private DialogueBox console; 

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
        Debug.Log($"Stage Controls is not implemented yet.");
        yield return null;
        //char newLine = DialogueBoxManager.Instance.NewLineMarker;
        //cat = DialogueBoxManager.Instance.GetDialogueBox("CAT");
        //dog = DialogueBoxManager.Instance.GetDialogueBox("DOG");
        //console = DialogueBoxManager.Instance.GetDialogueBox("Console");

        //console.SubmitDialogue("test single", 2, false);

        //yield return new WaitForSeconds(4);

        //console.ClearDialogue();
        //console.SubmitDialogue("test multiple", 2, false);

        //yield return new WaitForSeconds(4);

        //console.SubmitErrorMessage($"Error: Test error message{newLine}Error: Test message successful", 2);

        //yield return new WaitForSeconds(4);

        //cat.SubmitDialogue("test single", 0, false);

        //while (!cat.DialogueRead)
        //{
        //    yield return null;
        //}

        //cat.SubmitDialogue("test multiple", 0, true);

        //while (!cat.DialogueRead)
        //{
        //    yield return null;
        //}

        //dog.SubmitDialogue("test multiple", 0, true);

        //while (!dog.DialogueRead)
        //{
        //    yield return null;
        //}

        //console.SubmitDialogue("finished", 0, false);
    }
}
