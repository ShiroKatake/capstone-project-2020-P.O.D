using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game stage that takes the player through the combat mechanics.
/// </summary>
public class StageCombat : Stage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------
    
    [SerializeField] private UIElementStatusController shotgunTurret;
    [SerializeField] private UIElementStatusController machineGunTurret;
    [SerializeField] private UIElementStatusController turretsHighlight;

    //Non-Serialized Fields------------------------------------------------------------------------

    DialogueBox console;
    DialogueBox game;
    DialogueBox dog;
    Player playerInputManager;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// StageCombat's singleton public property.
    /// </summary>
    public StageCombat Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("TThere should never be more than one StageCombat.");
        }

        Instance = this;
        id = EStage.Combat;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        //Get all dialogue boxes, etc.
        console = DialogueBoxManager.Instance.GetDialogueBox("Console");
        game = DialogueBoxManager.Instance.GetDialogueBox("Game");
        dog = DialogueBoxManager.Instance.GetDialogueBox("DOG");
        playerInputManager = ReInput.players.GetPlayer(PlayerMovementController.Instance.GetComponent<PlayerID>().Value);
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of StageCombat.
    /// </summary>
    public override void StartExecution()
    {
        StartCoroutine(Execution());
    }

    /// <summary>
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    protected override IEnumerator Execution()
    {
        //Wait for aliens to spawn
        while (ClockController.Instance.Daytime)
        {
            yield return null;
        }

        //Help from DOG, aliens bad
        ClockController.Instance.Paused = true;
        console.SubmitDialogue("launch dog", 0, false, false);
        dog.SubmitDialogue("dog launched", 1, false, false);

        while (!dog.DialogueRead || !dog.AcceptingSubmissions)
        {
            yield return null;
        }

        dog.SubmitDialogue("aliens spawned", 0, false, false);

        while (!dog.DialogueRead || !dog.AcceptingSubmissions)
        {
            yield return null;
        }

        //Build Turrets
        console.ClearDialogue();
        console.SubmitDialogue("task build turret", 0, false, false);
        dog.SubmitDialogue("build turret", 0, true, false);
        shotgunTurret.Visible = true;
        machineGunTurret.Visible = true;
        shotgunTurret.Interactable = true;
        machineGunTurret.Interactable = true;
        turretsHighlight.Visible = true;

        while (!BuildingController.Instance.HasBuiltBuilding(EBuilding.ShortRangeTurret) || !BuildingController.Instance.HasBuiltBuilding(EBuilding.ShortRangeTurret))
        {
            yield return null;
        }

        //If player doesn't know, here's how to shoot
        if (!ProjectileManager.Instance.HasProjectileWithOwner(PlayerMovementController.Instance.transform))
        {
            console.ClearDialogue();
            console.SubmitDialogue("task shoot", 0, false, false);
            dog.SubmitDialogue("shoot", 0, true, false);
            game.SubmitDialogue("shoot", 0, true, false);

            while (!ProjectileManager.Instance.HasProjectileWithOwner(PlayerMovementController.Instance.transform) || !dog.AcceptingSubmissions)
            {
                yield return null;
            }

            if (!dog.DialogueRead)
            {
                dog.DialogueRead = true;
            }
        }

        ////If player doesn't know how, here's how to heal
        //if (!playerInputManager.GetButtonDown("Heal") || Vector3.Distance(PlayerMovementController.Instance.transform.position, CryoEgg.Instance.transform.position) > healingDistance)
        //{
        //    console.ClearDialogue();
        //    console.SubmitDialogue("task heal", 0, false, false);
        //    dog.SubmitDialogue("heal at cryo egg", 0, true, false);
        //    game.SubmitDialogue("heal", 0, true, false);

        //    while (!playerInputManager.GetButtonDown("Heal") || Vector3.Distance(PlayerMovementController.Instance.transform.position, CryoEgg.Instance.transform.position) > healingDistance || !dog.AcceptingSubmissions) //TODO: get reference to thing that says what the healing distance is
        //    {
        //        yield return null;
        //    }
        //
        //    if (!dog.DialogueRead)
        //    {
        //        dog.DialogueRead = true;
        //    }
        //}

        console.ClearDialogue();
        dog.SubmitDialogue("good luck", 0, true, false);

        while (!dog.DialogueRead)
        {
            yield return null;
        }

        console.SubmitDialogue("dog closed", 0, false, false);
        game.SubmitDialogue("finished tutorial", 0, true, false);
        ClockController.Instance.Paused = false;
        StageManager.Instance.SetStage(EStage.MainGame);
    }
}
