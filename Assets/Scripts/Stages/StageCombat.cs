using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game stage that takes the player through the combat mechanics.
/// </summary>
public class StageCombat : PublicInstanceSerializableSingleton<StageCombat>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Uninteractable Building Buttons")]
    [SerializeField] private UIElementStatusManager fusionReactor;
    [SerializeField] private UIElementStatusManager iceDrill;
    [SerializeField] private UIElementStatusManager harvester;
    [SerializeField] private UIElementStatusManager gasPump;
    [SerializeField] private UIElementStatusManager boiler;
    [SerializeField] private UIElementStatusManager greenhouse;
    [SerializeField] private UIElementStatusManager incinerator;

    [Header("Interactable Building Buttons")]
    [SerializeField] private UIElementStatusManager shotgunTurret;
    [SerializeField] private UIElementStatusManager machineGunTurret;
    
    [Header("Building Prefabs")]
    [SerializeField] private Building fusionReactorPrefab;
    [SerializeField] private Building iceDrillPrefab;
    [SerializeField] private Building harvesterPrefab;
    [SerializeField] private Building gasPumpPrefab;
    [SerializeField] private Building boilerPrefab;
    [SerializeField] private Building greenhousePrefab;
    [SerializeField] private Building incineratorPrefab;
    [SerializeField] private Building shotgunTurretPrefab;
    [SerializeField] private Building machineGunTurretPrefab;

    [Header("Other Variables")]
    [Tooltip("How long should the tutorial delay the spawning of the next wave of aliens so that the player has a chance to build some turrets?")]
    [SerializeField] private float nextAlienWaveDelay;

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
        playerInputManager = POD.Instance.PlayerInputManager;
    }

    //Recurring Methods------------------------------------------------------------------------------------------------------------------------------

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
        yield return StartCoroutine(StoreDisperse());
        yield return StartCoroutine(CompleteStage());
        StageManager.Instance.SetStage(EStage.MainGame);
    }

    /// <summary>
    /// Stalls the tutorial until the sun goes down.
    /// </summary>
    private IEnumerator WaitForNightTime()
    {
        do
        {
            yield return null;
        }
        while (ClockManager.Instance.Daytime);

        MineralCollectionController.Instance.CanMine = false;
        BuildingDemolitionController.Instance.CanDemolish = false;
        fusionReactor.ButtonInteract.InInteractableGameStage = false;
        fusionReactor.Interactable = false;
        iceDrill.ButtonInteract.InInteractableGameStage = false;
        iceDrill.Interactable = false;
        harvester.ButtonInteract.InInteractableGameStage = false;
        harvester.Interactable = false;
        gasPump.ButtonInteract.InInteractableGameStage = false;
        gasPump.Interactable = false;
        boiler.ButtonInteract.InInteractableGameStage = false;
        boiler.Interactable = false;
        greenhouse.ButtonInteract.InInteractableGameStage = false;
        greenhouse.Interactable = false;
        incinerator.ButtonInteract.InInteractableGameStage = false;
        incinerator.Interactable = false;
        shotgunTurret.ButtonInteract.InInteractableGameStage = false;
        shotgunTurret.Interactable = false;
        machineGunTurret.ButtonInteract.InInteractableGameStage = false;
        machineGunTurret.Interactable = false;
    }

    /// <summary>
    /// Introduces the player to aliens.
    /// </summary>
    private IEnumerator AlienWalkthrough()
    {
        ClockManager.Instance.Paused = true;
        AlienManager.Instance.CanSpawnAliens = true;
        console.SubmitDialogue("launch dog", 0, false, false);

        do
        {
            yield return null;
        }
        while (console.LerpingDialogue);

        dog.SubmitDialogue("dog launched", 0, false, false);

        do
        {
            yield return null;
        }
        while (!dog.DialogueRead || !dog.AcceptingSubmissions);
        
        dog.SubmitDialogue("aliens spawned", 0, false, false);

        do
        {
            yield return null;
        }
        while (!dog.DialogueRead || !dog.AcceptingSubmissions);
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

        do
        {
            //Keep ice drill button interactable only while it needs to be placed
            if (BuildingManager.Instance.PlacedBuildingsCount(EBuilding.ShotgunTurret) > 0 || BuildingManager.Instance.PlacedBuildingsCount(EBuilding.MachineGunTurret) > 0)
            {
                if (fusionReactor.ButtonInteract.InInteractableGameStage) fusionReactor.ButtonInteract.InInteractableGameStage = false;
                if (fusionReactor.Interactable) fusionReactor.Interactable = false;
                if (shotgunTurret.ButtonInteract.InInteractableGameStage) shotgunTurret.ButtonInteract.InInteractableGameStage = false;
                if (shotgunTurret.Interactable) shotgunTurret.Interactable = false;
                if (machineGunTurret.ButtonInteract.InInteractableGameStage) machineGunTurret.ButtonInteract.InInteractableGameStage = false;
                if (machineGunTurret.Interactable) machineGunTurret.Interactable = false;
                if (MineralCollectionController.Instance.CanMine) MineralCollectionController.Instance.CanMine = false;
            }
            else
            {
                if (!shotgunTurret.ButtonInteract.InInteractableGameStage)
                {
                    shotgunTurret.ButtonInteract.InInteractableGameStage = true;
                    UIBuildingBar.Instance.UpdateButton(shotgunTurretPrefab, shotgunTurret.ButtonInteract);
                }

                if (!machineGunTurret.ButtonInteract.InInteractableGameStage)
                {
                    machineGunTurret.ButtonInteract.InInteractableGameStage = true;
                    UIBuildingBar.Instance.UpdateButton(machineGunTurretPrefab, machineGunTurret.ButtonInteract);
                }

                float maxPowerRequired = Mathf.Max(shotgunTurretPrefab.PowerConsumption, machineGunTurretPrefab.PowerConsumption);
                UpdateResourceBuildingButtonInteractability(ResourceManager.Instance.SurplusPower, maxPowerRequired, fusionReactorPrefab, fusionReactor);

                if (MineralCollectionController.Instance.CanMine)
                {
                    if (ResourceManager.Instance.Ore >= shotgunTurretPrefab.OreCost
                        && ResourceManager.Instance.Ore >= machineGunTurretPrefab.OreCost
                        && (!fusionReactor.ButtonInteract.InInteractableGameStage || ResourceManager.Instance.Ore >= fusionReactorPrefab.OreCost))
                    {
                        MineralCollectionController.Instance.CanMine = false;
                    }
                }
                else
                {
                    if (ResourceManager.Instance.Ore < shotgunTurretPrefab.OreCost
                        || ResourceManager.Instance.Ore < machineGunTurretPrefab.OreCost
                        || (fusionReactor.ButtonInteract.InInteractableGameStage && ResourceManager.Instance.Ore < fusionReactorPrefab.OreCost))
                    {
                        MineralCollectionController.Instance.CanMine = true;
                    }
                }
            }

            yield return null;
        }
        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.ShotgunTurret) == 0 && BuildingManager.Instance.BuiltBuildingsCount(EBuilding.MachineGunTurret) == 0);

        fusionReactor.ButtonInteract.InInteractableGameStage = false;
        fusionReactor.Interactable = false;
        shotgunTurret.ButtonInteract.InInteractableGameStage = false;
        shotgunTurret.Interactable = false;
        machineGunTurret.ButtonInteract.InInteractableGameStage = false;
        machineGunTurret.Interactable = false;
        MineralCollectionController.Instance.CanMine = false;
        AlienManager.Instance.CanSpawnAliens = false;

        dog.SubmitDialogue("protect tower", 0, false, false);
        //TODO: highlight tower

        do
        {
            yield return null;
        }
        while (!dog.DialogueRead || !dog.AcceptingSubmissions);
    }

    /// <summary>
    /// Teaches the player how to shoot.
    /// </summary>
    private IEnumerator Shooting()
    {        
        console.ClearDialogue();
        console.SubmitDialogue("task shoot", 0, false, false);
        dog.SubmitDialogue("shoot", 0, true, false);
        game.SubmitDialogue("shoot", 0, true, false);
        POD.Instance.ShootingController.CanShoot = true;

        do
        {
            yield return null;
        }
        while (!ProjectileManager.Instance.HasProjectileWithOwner(POD.Instance.transform) || !dog.AcceptingSubmissions || AlienManager.Instance.Aliens.Count > 0);

        yield return new WaitForSeconds(2f);

        POD.Instance.ShootingController.CanShoot = false;
        if (!dog.DialogueRead) dog.DialogueRead = true;
    }

    /// <summary>
    /// Teaches the player how to heal themselves.
    /// </summary>
    private IEnumerator Healing()
    {
        console.ClearDialogue();
        console.SubmitDialogue("task heal", 0, false, false);
        dog.SubmitDialogue("heal at cryo egg", 0, true, false);
        game.SubmitDialogue("heal", 0, true, false);
        POD.Instance.HealthController.CanHeal = true;
        POD.Instance.HealthController.Health.CurrentHealth *= 0.5f;

        do
        {
            yield return null;
        }
        while (!playerInputManager.GetButtonDown("Heal") || Vector3.Distance(POD.Instance.transform.position, Tower.Instance.transform.position) >= POD.Instance.HealthController.HealingRange || !dog.AcceptingSubmissions);
        
        yield return new WaitForSeconds(2f);

        POD.Instance.HealthController.CanHeal = false;
        if (!dog.DialogueRead) dog.DialogueRead = true;
    }

    /// <summary>
    /// Introduces the player to the store disperse menu, the final part of the terraforming mechanics.
    /// </summary>
    private IEnumerator StoreDisperse()
    {
        StoreDisperseUI.Instance.CanShowMenu = true;
        RatioManager.Instance.updateStoreDisperse?.Invoke(true);
        dog.SubmitDialogue("store disperse", 0, true, false);

        do
        {
            yield return null;
        }
        while (!StoreDisperseUI.Instance.IsVisible);

        do
        {
            yield return null;
        }
        while (StoreDisperseUI.Instance.IsVisible);

        StoreDisperseUI.Instance.CanShowMenu = false;
        if (!dog.DialogueRead) dog.DialogueRead = true;
    }

    /// <summary>
    /// Concludes the tutorial.
    /// </summary>
    private IEnumerator CompleteStage()
    {
        if (game.Activated) game.SubmitDeactivation();
        console.ClearDialogue();
        dog.SubmitDialogue("good luck", 0, true, false);

        do
        {
            yield return null;
        }
        while (!dog.DialogueRead);

        console.SubmitDialogue("dog closed", 0, false, false);
        game.SubmitDialogue("finished tutorial", 0, true, false);
        ClockManager.Instance.Paused = false;

        MineralCollectionController.Instance.CanMine = true;
        BuildingDemolitionController.Instance.CanDemolish = true;
        POD.Instance.HealthController.CanHeal = true;
        POD.Instance.ShootingController.CanShoot = true;
        StoreDisperseUI.Instance.CanShowMenu = true;

        fusionReactor.ButtonInteract.InInteractableGameStage = true;
        iceDrill.ButtonInteract.InInteractableGameStage = true;
        harvester.ButtonInteract.InInteractableGameStage = true;
        gasPump.ButtonInteract.InInteractableGameStage = true;
        boiler.ButtonInteract.InInteractableGameStage = true;
        greenhouse.ButtonInteract.InInteractableGameStage = true;
        incinerator.ButtonInteract.InInteractableGameStage = true;
        shotgunTurret.ButtonInteract.InInteractableGameStage = true;
        machineGunTurret.ButtonInteract.InInteractableGameStage = true;

        UIBuildingBar.Instance.UpdateButton(fusionReactorPrefab, fusionReactor.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(iceDrillPrefab, iceDrill.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(harvesterPrefab, harvester.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(gasPumpPrefab, gasPump.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(boilerPrefab, boiler.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(greenhousePrefab, greenhouse.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(incineratorPrefab, incinerator.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(shotgunTurretPrefab, shotgunTurret.ButtonInteract);
        UIBuildingBar.Instance.UpdateButton(machineGunTurretPrefab, machineGunTurret.ButtonInteract);

        yield return new WaitForSeconds(nextAlienWaveDelay);
        AlienManager.Instance.CanSpawnAliens = true;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Checks if a building that provides resources for the building the tutorial currently wants built still needs to be built or if there is enough of the resource it produces.
    /// </summary>
    /// <param name="resourceSurplus">The current surplus of the resource the building produces.</param>
    /// <param name="requiredQty">The ammount that the currently required building needs in order to be built.</param>
    /// <param name="buildingPrefab">The prefab of the resource building under consideration.</param>
    /// <param name="buildingButton">The UI button for the resource building under consideration.</param>
    private void UpdateResourceBuildingButtonInteractability(float resourceSurplus, float requiredQty, Building buildingPrefab, UIElementStatusManager buildingButton)
    {
        if (resourceSurplus < requiredQty)
        {
            if (!buildingButton.ButtonInteract.InInteractableGameStage)
            {
                buildingButton.ButtonInteract.InInteractableGameStage = true;
                UIBuildingBar.Instance.UpdateButton(buildingPrefab, buildingButton.ButtonInteract);
            }
        }
        else
        {
            if (buildingButton.ButtonInteract.InInteractableGameStage)
            {
                buildingButton.ButtonInteract.InInteractableGameStage = false;
                buildingButton.Interactable = false;
            }
        }
    }
}
