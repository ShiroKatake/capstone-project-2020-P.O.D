using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is taught the game's controls
/// </summary>
public class StageControls : Stage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private GameObject uiBorder;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

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
        //Get all dialogue boxes, etc.
        DialogueBox console = DialogueBoxManager.Instance.GetDialogueBox("Console");
        DialogueBox game = DialogueBoxManager.Instance.GetDialogueBox("Game");
        DialogueBox w = DialogueBoxManager.Instance.GetDialogueBox("W");
        DialogueBox a = DialogueBoxManager.Instance.GetDialogueBox("A");
        DialogueBox s = DialogueBoxManager.Instance.GetDialogueBox("S");
        DialogueBox d = DialogueBoxManager.Instance.GetDialogueBox("D");
        DialogueBox cat = DialogueBoxManager.Instance.GetDialogueBox("CAT");
        Player playerInputManager = ReInput.players.GetPlayer(PlayerMovementController.Instance.GetComponent<PlayerID>().Value);
        char newLine = DialogueBoxManager.Instance.NewLineMarker;

        yield return new WaitForSeconds(3);

        //Enable UI
        console.SubmitDialogue("ai online", 0, false, false);
        //TODO: UI border becomes visible

        yield return new WaitForSeconds(2);

        //Movement controls: WASD
        console.SubmitDialogue("calibrate movement", 0, false, false);
        w.SubmitDialogue("w", 1, false, true);
        a.SubmitDialogue("a", 1, false, true);
        s.SubmitDialogue("s", 1, false, true);
        d.SubmitDialogue("d", 1, false, true);
        game.SubmitDialogue("wasd", 1, true, false);

        while (!w.Clickable || !a.Clickable || !s.Clickable || !d.Clickable || !game.Clickable)
        {
            yield return null;
        }

        float moveVertical;
        float moveHorizontal;

        while (w.Activated || a.Activated || s.Activated || d.Activated)
        {
            yield return null;

            moveVertical = -playerInputManager.GetAxis("Vertical");
            moveHorizontal  = playerInputManager.GetAxis("Horizontal");

            if (moveVertical != 0)
            {
                if (moveVertical > 0 && w.Activated)
                {
                    w.SubmitDeactivation();
                }

                if (moveVertical < 0 && s.Activated)
                {
                    s.SubmitDeactivation();
                }
            }

            if (moveHorizontal != 0)
            {
                if (moveHorizontal < 0 && a.Activated)
                {
                    a.SubmitDeactivation();
                }
                
                if (moveHorizontal > 0 && d.Activated)
                {
                    d.SubmitDeactivation();
                }
            }            
        }

        if (game.Activated)
        {
            game.SubmitDeactivation();
        }
    }

    //private void Test()
    //{
    //    char newLine = DialogueBoxManager.Instance.NewLineMarker;
    //    cat = DialogueBoxManager.Instance.GetDialogueBox("CAT");
    //    dog = DialogueBoxManager.Instance.GetDialogueBox("DOG");
    //    console = DialogueBoxManager.Instance.GetDialogueBox("Console");

    //    console.SubmitDialogue("test single", 2, false);

    //    yield return new WaitForSeconds(4);

    //    console.ClearDialogue();
    //    console.SubmitDialogue("test multiple", 2, false);

    //    yield return new WaitForSeconds(4);

    //    console.SubmitErrorMessage($"Error: Test error message{newLine}Error: Test message successful", 2);

    //    yield return new WaitForSeconds(4);

    //    cat.SubmitDialogue("test single", 0, false);

    //    while (!cat.DialogueRead)
    //    {
    //        yield return null;
    //    }

    //    cat.SubmitDialogue("test multiple", 0, true);

    //    while (!cat.DialogueRead)
    //    {
    //        yield return null;
    //    }

    //    dog.SubmitDialogue("test multiple", 0, true);

    //    while (!dog.DialogueRead)
    //    {
    //        yield return null;
    //    }

    //    console.SubmitDialogue("finished", 0, false);
    //}
}
