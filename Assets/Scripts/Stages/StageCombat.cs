using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game stage that takes the player through the combat mechanics.
/// </summary>
public class StageCombat : SerializableSingleton<StageCombat>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Uninteractable Building Buttons")]
    [SerializeField] private UIElementStatusManager fusionReactor;
    [SerializeField] private UIElementStatusManager iceDrill;
    [SerializeField] private UIElementStatusManager boiler;
    [SerializeField] private UIElementStatusManager greenhouse;
    [SerializeField] private UIElementStatusManager incinerator;

    [Header("Interactable Building Buttons")]
    [SerializeField] private UIElementStatusManager shotgunTurret;
    [SerializeField] private UIElementStatusManager machineGunTurret;

    [Header("Highlights")]
    [SerializeField] private UIElementStatusManager shotgunTurretHighlight;
    [SerializeField] private UIElementStatusManager machineGunTurretHighlight;

    [Header("Building Prefabs")]
    [SerializeField] private ResourceCollector fusionReactorPrefab;
    [SerializeField] private Building shotgunTurretPrefab;
    [SerializeField] private Building machineGunTurretPrefab;

    //Non-Serialized Fields------------------------------------------------------------------------

    DialogueBox console;
    DialogueBox game;
    DialogueBox dog;
    Player playerInputManager;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageCombat. 
    /// </summary>
    /// <returns></returns>
    public EStage GetID()
    {
        return EStage.Combat;
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
        dog = DialogueBoxManager.Instance.GetDialogueBox("DOG");
        playerInputManager = ReInput.players.GetPlayer(PODController.Instance.GetComponent<PlayerID>().Value);
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    public IEnumerator Execution()
    {
        yield return StartCoroutine(WaitForNightTime());
        yield return StartCoroutine(AlienWalkthrough());
        yield return StartCoroutine(BuildTurrets());
        yield return StartCoroutine(Shooting());
        yield return StartCoroutine(Healing());
        yield return StartCoroutine(CompleteStage());
        StageManager.Instance.SetStage(EStage.MainGame);
    }

    /// <summary>
    /// Stalls the tutorial until the sun goes down.
    /// </summary>
    private IEnumerator WaitForNightTime()
    {
        while (ClockManager.Instance.Daytime)
        {
            yield return null;
        }

        fusionReactor.Interactable = false;
        iceDrill.Interactable = false;
        boiler.Interactable = false;
        greenhouse.Interactable = false;
        incinerator.Interactable = false;
    }

    /// <summary>
    /// Introduces the player to aliens.
    /// </summary>
    private IEnumerator AlienWalkthrough()
    {
        ClockManager.Instance.Paused = true;
        console.SubmitDialogue("launch dog", 0, false, false);

        do
        {
            yield return null;
        }
        while (console.LerpingDialogue);

        dog.SubmitDialogue("dog launched", 0, false, false);

        while (!dog.DialogueRead || !dog.AcceptingSubmissions)
        {
            yield return null;
        }

        dog.SubmitDialogue("aliens spawned", 0, false, false);

        while (!dog.DialogueRead || !dog.AcceptingSubmissions)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Teaches the player about turrets.
    /// </summary>
    private IEnumerator BuildTurrets()
    {
        console.ClearDialogue();
        console.SubmitDialogue("task build turret", 0, false, false);
        dog.SubmitDialogue("build turret", 0, true, false);
        shotgunTurret.Visible = true;
        machineGunTurret.Visible = true;
        shotgunTurret.Interactable = true;
        machineGunTurret.Interactable = true;
        shotgunTurretHighlight.Visible = true;
        machineGunTurretHighlight.Visible = true;

        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.ShotgunTurret) == 0 || BuildingManager.Instance.BuiltBuildingsCount(EBuilding.MachineGunTurret) == 0)
        {
            bool placedShotgunTurret = BuildingManager.Instance.PlacedBuildingsCount(EBuilding.ShotgunTurret) > 0;
            bool placedMachineGunTurret = BuildingManager.Instance.PlacedBuildingsCount(EBuilding.MachineGunTurret) > 0;
            int pendingPowerSupply = fusionReactorPrefab.CollectionRate * (BuildingManager.Instance.PlacedBuildingsCount(EBuilding.FusionReactor) - BuildingManager.Instance.BuiltBuildingsCount(EBuilding.FusionReactor));

            //Keep shotgun turret button interactable only while it needs to be placed
            if (placedShotgunTurret)
            {
                if (shotgunTurret.Interactable)
                {
                    shotgunTurret.Interactable = false;
                }
            }
            else
            {
                if (!shotgunTurret.Interactable)
                {
                    shotgunTurret.Interactable = true;
                }
            }

            //Keep machine gun turret button interactable only while it needs to be placed
            if (placedMachineGunTurret)
            {
                if (machineGunTurret.Interactable)
                {
                    machineGunTurret.Interactable = false;
                }
            }
            else
            {
                if (!machineGunTurret.Interactable)
                {
                    machineGunTurret.Interactable = true;
                }
            }

            //Keep fusion reactor button interactable only while there's insufficient power
            if ((!placedShotgunTurret && ResourceManager.Instance.SurplusPower + pendingPowerSupply < shotgunTurretPrefab.PowerConsumption) 
                || (!placedMachineGunTurret && ResourceManager.Instance.SurplusPower + pendingPowerSupply < machineGunTurretPrefab.PowerConsumption))
            {
                if (!fusionReactor.Interactable)
                {
                    fusionReactor.Interactable = true;
                }
            }
            else
            {
                if (fusionReactor.Interactable)
                {
                    fusionReactor.Interactable = false;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// Teaches the player how to shoot.
    /// </summary>
    private IEnumerator Shooting()
    {
        if (!ProjectileManager.Instance.HasProjectileWithOwner(PODController.Instance.transform))
        {
            console.ClearDialogue();
            console.SubmitDialogue("task shoot", 0, false, false);
            dog.SubmitDialogue("shoot", 0, true, false);
            game.SubmitDialogue("shoot", 0, true, false);

            while (!ProjectileManager.Instance.HasProjectileWithOwner(PODController.Instance.transform) || !dog.AcceptingSubmissions)
            {
                yield return null;
            }

            if (!dog.DialogueRead)
            {
                dog.DialogueRead = true;
            }
        }
    }

    /// <summary>
    /// Teaches the player how to heal themselves.
    /// </summary>
    private IEnumerator Healing()
    {
        if (!playerInputManager.GetButtonDown("Heal") || Vector3.Distance(PODController.Instance.transform.position, Tower.Instance.transform.position) >= PODController.Instance.HealingRange)
        {
            console.ClearDialogue();
            console.SubmitDialogue("task heal", 0, false, false);
            dog.SubmitDialogue("heal at cryo egg", 0, true, false);
            game.SubmitDialogue("heal", 0, true, false);

            while (!playerInputManager.GetButtonDown("Heal") || Vector3.Distance(PODController.Instance.transform.position, Tower.Instance.transform.position) >= PODController.Instance.HealingRange || !dog.AcceptingSubmissions)
            {
                yield return null;
            }

            if (!dog.DialogueRead)
            {
                dog.DialogueRead = true;
            }
        }
    }

    /// <summary>
    /// Concludes the tutorial.
    /// </summary>
    private IEnumerator CompleteStage()
    {
        if (game.Activated)
        {
            game.SubmitDeactivation();
        }

        console.ClearDialogue();
        dog.SubmitDialogue("good luck", 0, true, false);

        while (!dog.DialogueRead)
        {
            yield return null;
        }

        console.SubmitDialogue("dog closed", 0, false, false);
        game.SubmitDialogue("finished tutorial", 0, true, false);
        ClockManager.Instance.Paused = false;
        fusionReactor.Interactable = true;
        iceDrill.Interactable = true;
        boiler.Interactable = true;
        greenhouse.Interactable = true;
        incinerator.Interactable = true;
        shotgunTurret.Interactable = true;
        machineGunTurret.Interactable = true;
    }
}
