using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is taught the game's controls
/// </summary>
public class StageControls : SerializableSingleton<StageControls>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private UIElementStatusController uiBorderUIEC;
    [SerializeField] private UIElementStatusController consoleUIEC;
    [SerializeField] private UIElementStatusController buildingAndResourcesBarUIEC;
    [SerializeField] private UIElementStatusController miniMapBorderUIEC;
    [SerializeField] private UIElementStatusController miniMapUIEC;
    [SerializeField] private UIElementStatusController mineralsHighlightUIEC;

    //Non-Serialized Fields------------------------------------------------------------------------

    DialogueBox console;
    DialogueBox game;
    DialogueBox w;
    DialogueBox a;
    DialogueBox s;
    DialogueBox d;
    DialogueBox cat;
    Player playerInputManager;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageControls.
    /// </summary>
    public EStage GetID()
    {
        return EStage.Controls;
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        //Get all dialogue boxes, etc.
        console = DialogueBoxManager.Instance.GetDialogueBox("Console");
        game = DialogueBoxManager.Instance.GetDialogueBox("Game");
        w = DialogueBoxManager.Instance.GetDialogueBox("W");
        a = DialogueBoxManager.Instance.GetDialogueBox("A");
        s = DialogueBoxManager.Instance.GetDialogueBox("S");
        d = DialogueBoxManager.Instance.GetDialogueBox("D");
        cat = DialogueBoxManager.Instance.GetDialogueBox("CAT");
        playerInputManager = ReInput.players.GetPlayer(PlayerController.Instance.GetComponent<PlayerID>().Value);
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of StageControls.
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    public IEnumerator Execution()
    {
        yield return StartCoroutine(Setup());
        yield return StartCoroutine(EnableUI());
        yield return StartCoroutine(MovementControls());
        yield return StartCoroutine(MiningControls());
        StageManager.Instance.SetStage(EStage.Terraforming);
    }

    /// <summary>
    /// Sets up the player's health and the time.
    /// </summary>
    private IEnumerator Setup()
    {
        PlayerController.Instance.GetComponent<Health>().CurrentHealth = PlayerController.Instance.GetComponent<Health>().MaxHealth * 0.25f;  //Set the player's health ready for the healing section of the tutorial

        ClockController.Instance.Paused = true;
        ClockController.Instance.SetTime(ClockController.Instance.HalfCycleDuration * 0.2f);
        yield return new WaitForSeconds(3);
    }

    /// <summary>
    /// Brings the UI online
    /// </summary>
    private IEnumerator EnableUI()
    {
        uiBorderUIEC.Visible = true;
        consoleUIEC.Visible = true;
        console.SubmitDialogue("blank", 0, false, false);

        while (!uiBorderUIEC.FinishedFlickeringIn || !consoleUIEC.FinishedFlickeringIn)
        {
            yield return null;
        }

        console.SubmitDialogue("ai online", 0, false, false);
    }

    /// <summary>
    /// Walks the player through the movement controls.
    /// </summary>
    private IEnumerator MovementControls()
    {
        console.SubmitDialogue("calibrate movement", 0, false, false);
        game.SubmitDialogue("wasd", 1, true, false);
        w.SubmitDialogue("w", 1, false, true);
        a.SubmitDialogue("a", 1, false, true);
        s.SubmitDialogue("s", 1, false, true);
        d.SubmitDialogue("d", 1, false, true);

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
            moveHorizontal = playerInputManager.GetAxis("Horizontal");

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

        console.SubmitDialogue("systems online", 0, false, false);

        while (console.LerpingDialogue)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2);
    }

    /// <summary>
    /// Walks the player through mining.
    /// </summary>
    private IEnumerator MiningControls()
    {
        console.ClearDialogue();
        console.SubmitDialogue("planet livability", 0, false, false);

        while (console.LerpingDialogue)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        console.SubmitDialogue("launch cat", 0, false, false);
        cat.SubmitDialogue("need minerals", 2, true, false);

        while (!cat.DialogueRead)
        {
            yield return null;
        }

        console.ClearDialogue();
        console.SubmitDialogue("detect minerals", 0, false, false);
        buildingAndResourcesBarUIEC.Visible = true;
        miniMapBorderUIEC.Visible = true;

        while (!miniMapBorderUIEC.FinishedFlickeringIn)
        {
            yield return null;
        }

        console.SubmitDialogue("task gather minerals", 0, false, false);
        cat.SubmitDialogue("gather minerals", 0, true, false);
        game.SubmitDialogue("lmb", 1, true, false);
        ResourceTextManager.Instance.FadeIn();
        miniMapUIEC.Visible = true;
        float startingMinerals = ResourceController.Instance.Ore;

        while (ResourceController.Instance.Ore < startingMinerals + 4)
        {
            yield return null;
        }

        if (game.Activated)
        {
            game.SubmitDeactivation();
        }

        console.ClearDialogue();
        cat.SubmitDialogue("collected minerals", 0, false, false);
        mineralsHighlightUIEC.Visible = true;

        //Start terraforming stage
        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }
    }
}
