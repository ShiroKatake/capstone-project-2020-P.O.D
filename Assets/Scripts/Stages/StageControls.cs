using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is taught the game's controls
/// </summary>
public class StageControls : PublicInstanceSerializableSingleton<StageControls>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("UI Element Prefabs")]
    [SerializeField] private UIElementStatusManager uiBorderUIEC;
    [SerializeField] private UIElementStatusManager consoleUIEC;
    [SerializeField] private UIElementStatusManager buildingAndResourcesBarUIEC;
    [SerializeField] private UIElementStatusManager resourceBar;
    [SerializeField] private UIElementStatusManager miniMapBorderUIEC;
    [SerializeField] private UIElementStatusManager miniMapUIEC;
    [SerializeField] private UIElementStatusManager mineralsHighlightUIEC;

    [Header("Building Prefabs")]
    [SerializeField] private Building fusionReactorPrefab;

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
        //playerInputManager = ReInput.players.GetPlayer(POD.Instance.GetComponent<PlayerID>().Value);
        playerInputManager = POD.Instance.PlayerInputManager;
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

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
        StageManager.Instance.SetStage(EStage.ResourceBuildings);
    }

    /// <summary>
    /// Sets up the player's health and the time.
    /// </summary>
    private IEnumerator Setup()
    {
        MineralCollectionController.Instance.CanMine = false;
        BuildingDemolitionController.Instance.CanDemolish = false;
        POD.Instance.GetComponent<Health>().CurrentHealth = POD.Instance.GetComponent<Health>().MaxHealth * 0.25f;  //Set the player's health ready for the healing section of the tutorial

        ClockManager.Instance.Paused = true;
        ClockManager.Instance.SetTime(ClockManager.Instance.HalfCycleDuration * 0.2f);
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

        do
        {
            yield return null;
        }
        while (!uiBorderUIEC.FinishedFlickeringIn || !consoleUIEC.FinishedFlickeringIn);

        console.SubmitDialogue("ai online", 0, false, false);
    }

    /// <summary>
    /// Walks the player through the movement controls.
    /// </summary>
    private IEnumerator MovementControls()
    {
        console.SubmitDialogue("calibrate movement", 0, false, false);

        do
        {
            yield return null;
        }
        while (console.LerpingDialogue);

        game.SubmitDialogue("wasd", 1, true, false);
        w.SubmitDialogue("w", 1, false, true);
        a.SubmitDialogue("a", 1, false, true);
        s.SubmitDialogue("s", 1, false, true);
        d.SubmitDialogue("d", 1, false, true);

        do
        {
            yield return null;
        }
        while (!w.Clickable || !a.Clickable || !s.Clickable || !d.Clickable || !game.Clickable);

        float moveVertical;
        float moveHorizontal;

        do
        {
            yield return null;

            moveVertical = -playerInputManager.GetAxis("Vertical");
            moveHorizontal = playerInputManager.GetAxis("Horizontal");

            if (moveVertical != 0)
            {
                if (moveVertical > 0 && w.Activated) w.SubmitDeactivation();
                if (moveVertical < 0 && s.Activated) s.SubmitDeactivation();
            }

            if (moveHorizontal != 0)
            {
                if (moveHorizontal < 0 && a.Activated) a.SubmitDeactivation();
                if (moveHorizontal > 0 && d.Activated) d.SubmitDeactivation();
            }
        }
        while (w.Activated || a.Activated || s.Activated || d.Activated);

        if (game.Activated)  game.SubmitDeactivation();
        console.SubmitDialogue("systems online", 0, false, false);

        do
        {
            yield return null;
        }
        while (console.LerpingDialogue);

        yield return new WaitForSeconds(2);
    }

    /// <summary>
    /// Walks the player through mining.
    /// </summary>
    private IEnumerator MiningControls()
    {
        console.ClearDialogue();
        console.SubmitDialogue("planet livability", 0, false, false);

        yield return new WaitForSeconds(0.5f);

        console.SubmitDialogue("launch cat", 0, false, false);

        do
        {
            yield return null;
        }
        while (console.LerpingDialogue) ;

        cat.SubmitDialogue("need minerals", 2, true, false);

        do
        {
            yield return null;
        }
        while (!cat.DialogueRead);

        console.SubmitDialogue("scan minerals", 0, false, false);
        buildingAndResourcesBarUIEC.Visible = true;
        resourceBar.Visible = true;
        miniMapBorderUIEC.Visible = true;

        do
        {
            yield return null;
        }
        while (!miniMapBorderUIEC.FinishedFlickeringIn);

        console.SubmitDialogue("minerals detected", 0, false, false);
        ResourceTextManager.Instance.FadeIn();
        miniMapUIEC.Visible = true;

        do
        {
            yield return null;
        }
        while (!miniMapUIEC.FinishedFlickeringIn);

        console.ClearDialogue();
        console.SubmitDialogue("task gather minerals", 0, false, false);
        cat.SubmitDialogue("gather minerals", 0, true, false);
        game.SubmitDialogue("lmb", 1, true, false);
        
        float startingMinerals = ResourceManager.Instance.Ore;
        MineralCollectionController.Instance.CanMine = true;

        do
        {
            yield return null;
        }
        while (ResourceManager.Instance.Ore < fusionReactorPrefab.OreCost);

        MineralCollectionController.Instance.CanMine = false;
        if (game.Activated) game.SubmitDeactivation();
        console.ClearDialogue();
        cat.SubmitDialogue("collected minerals", 0, false, false);
        mineralsHighlightUIEC.Visible = true;
        
        do
        {
            yield return null;
        }
        while (!cat.DialogueRead || !cat.AcceptingSubmissions);
    }
}
