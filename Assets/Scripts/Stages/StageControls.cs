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

    [SerializeField] private UIElementStatusController uiBorderUIEC;
    [SerializeField] private UIElementStatusController consoleUIEC;
    [SerializeField] private UIElementStatusController mineralsHighlightUIEC;


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

    //TODO: in start, if tutorial isn't skipped, set 

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
        //Switch the clock off
        ClockController.Instance.Paused = true;
        ClockController.Instance.SetTime(ClockController.Instance.HalfCycleDuration * 0.2f);

        //Get all dialogue boxes, etc.
        //Debug.Log("Starting Stage Controls");
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
        //Debug.Log($"Enabling UI");
        uiBorderUIEC.Visible = true;

        while (!uiBorderUIEC.FinishedFlickeringIn)
        {
            yield return null;
        }

        consoleUIEC.Visible = true;

        while (!consoleUIEC.FinishedFlickeringIn)
        {
            yield return null;
        }

        //Movement controls: WASD
        console.SubmitDialogue("ai online", 0, false, false);
        yield return new WaitForSeconds(2);

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

        //Assess planet livability
        if (game.Activated)
        {
            game.SubmitDeactivation();
        }

        console.SubmitDialogue("systems online", 0, false, false);
        yield return new WaitForSeconds(2);

        console.SubmitDialogue("welcome pod", 0, false, false);
        yield return new WaitForSeconds(3);

        console.ClearDialogue();
        console.SubmitDialogue("planet livability", 0, false, false);
        yield return new WaitForSeconds(2);

        console.SubmitDialogue("planet unlivable", 0, false, false);
        yield return new WaitForSeconds(2);

        //Minerals controls: LMB
        console.SubmitDialogue("launch cat", 0, false, false);
        cat.SubmitDialogue("need minerals", 2, true, false);
        
        while (!cat.DialogueRead)
        {
            yield return null;
        }

        console.SubmitDialogue("scan minerals", 0, false, false);
        yield return new WaitForSeconds(2);

        console.SubmitDialogue("minerals detected", 0, false, false);
        yield return new WaitForSeconds(2);

        console.ClearDialogue();
        console.SubmitDialogue("task gather minerals", 0, false, false);
        cat.SubmitDialogue("gather minerals", 0, true, false);
        game.SubmitDialogue("lmb", 1, true, false);
        float startingMinerals = ResourceController.Instance.Ore;

        while (ResourceController.Instance.Ore < startingMinerals + 4)
        {
            yield return null;
        }

        //Finished controls stage
        if (game.Activated)
        {
            game.SubmitDeactivation();
        }

        console.ClearDialogue();
        cat.SubmitDialogue("collected minerals", 0, false, false);
        mineralsHighlightUIEC.Visible = true;

        //Start terraforming stage
        while (!cat.DialogueRead)
        {
            yield return null;
        }

        StageManager.Instance.SetStage(EStage.Terraforming);
    }
}
