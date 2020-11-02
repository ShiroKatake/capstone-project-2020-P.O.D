using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is just left to do their own thing and play the game.
/// </summary>
public class StageSkippedTutorial : PublicInstanceSerializableSingleton<StageSkippedTutorial>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("General UI")]
    [SerializeField] private UIElementStatusManager uiBorder;
    [SerializeField] private UIElementStatusManager console;
    [SerializeField] private UIElementStatusManager buildingBar;
    [SerializeField] private UIElementStatusManager resourcesBar;
    [SerializeField] private UIElementStatusManager miniMapBorder;
    [SerializeField] private UIElementStatusManager miniMap;
    [SerializeField] private UIElementStatusManager clock;

    [Header("Building Buttons")]
    [SerializeField] private UIElementStatusManager fusionReactor;
    [SerializeField] private UIElementStatusManager iceDrill;
    [SerializeField] private UIElementStatusManager harvester;
    [SerializeField] private UIElementStatusManager gasPump;
    [SerializeField] private UIElementStatusManager boiler;
    [SerializeField] private UIElementStatusManager greenhouse;
    [SerializeField] private UIElementStatusManager incinerator;
    [SerializeField] private UIElementStatusManager shotgunTurret;
    [SerializeField] private UIElementStatusManager machineGunTurret;

    [Header("Progress/Ratio Bars")]
    [SerializeField] private UIElementStatusManager progressBar;
    [SerializeField] private UIElementStatusManager humidityBar;
    [SerializeField] private UIElementStatusManager biodiversityBar;
    [SerializeField] private UIElementStatusManager atmosphereBar;

    //Non-Serialized Fields------------------------------------------------------------------------

    private DialogueBox consoleDB;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageSkippedTutorial. 
    /// </summary>
    public EStage GetID()
    {
        return EStage.SkippedTutorial;
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        //Get all dialogue boxes, etc.
        consoleDB = DialogueBoxManager.Instance.GetDialogueBox("Console");
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
        yield return StartCoroutine(Setup());
        yield return StartCoroutine(EnableUI());
        yield return StartCoroutine(EnableBuildingBarContent());
        yield return StartCoroutine(BeginGame());        
        StageManager.Instance.SetStage(EStage.MainGame);
    }

    /// <summary>
    /// Sets up the clock.
    /// </summary>
    private IEnumerator Setup()
    {
        ClockManager.Instance.Paused = true;
        ClockManager.Instance.SetTime(0);
        yield return new WaitForSeconds(3);
    }

    /// <summary>
    /// Brings the UI online.
    /// </summary>
    private IEnumerator EnableUI()
    {
        uiBorder.Visible = true;
        console.Visible = true;
        buildingBar.Visible = true;
		resourcesBar.Visible = true;
		miniMapBorder.Visible = true;

        while (!uiBorder.FinishedFlickeringIn
            || !console.FinishedFlickeringIn
            || !buildingBar.FinishedFlickeringIn
			|| !resourcesBar.FinishedFlickeringIn
			|| !miniMapBorder.FinishedFlickeringIn)
        {
            yield return null;
        }

        ClockManager.Instance.Paused = false;
        consoleDB.SubmitDialogue("blank", 0, false, false);
        consoleDB.SubmitDialogue("system check", 0, false, false);
        ResourceTextManager.Instance.FadeIn();
        miniMap.Visible = true;
        clock.Visible = true;
    }

    /// <summary>
    /// Brings the contents of the building bar online.
    /// </summary>
    private IEnumerator EnableBuildingBarContent()
    {
        yield return new WaitForSeconds(0.15f);
        progressBar.Visible = true;
        fusionReactor.Visible = true;
        fusionReactor.Interactable = false;
        yield return new WaitForSeconds(0.15f);
        iceDrill.Visible = true;
        iceDrill.Interactable = false;
        yield return new WaitForSeconds(0.15f);
        harvester.Visible = true;
        harvester.Interactable = false;
        yield return new WaitForSeconds(0.15f);
        gasPump.Visible = true;
        gasPump.Interactable = false;
        yield return new WaitForSeconds(0.15f);
        boiler.Visible = true;
        boiler.Interactable = false;
        humidityBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        greenhouse.Visible = true;
        greenhouse.Interactable = false;
        biodiversityBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        incinerator.Visible = true;
        incinerator.Interactable = false;
        atmosphereBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        shotgunTurret.Visible = true;
        shotgunTurret.Interactable = false;
        yield return new WaitForSeconds(0.15f);
        machineGunTurret.Visible = true;
        machineGunTurret.Interactable = false;
        yield return new WaitForSeconds(0.15f);
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    private IEnumerator BeginGame()
    {
        consoleDB.SubmitDialogue("begin game", 0, false, false);
        yield return null;
    }
}
