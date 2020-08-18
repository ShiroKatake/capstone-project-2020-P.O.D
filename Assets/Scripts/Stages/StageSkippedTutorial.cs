using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is just left to do their own thing and play the game.
/// </summary>
public class StageSkippedTutorial : SerializableSingleton<StageSkippedTutorial>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("General UI")]
    [SerializeField] private UIElementStatusController uiBorder;
    [SerializeField] private UIElementStatusController console;
    [SerializeField] private UIElementStatusController buildingAndResourcesBar;
    [SerializeField] private UIElementStatusController miniMapBorder;
    [SerializeField] private UIElementStatusController miniMap;
    [SerializeField] private UIElementStatusController clock;

    [Header("Building Buttons")]
    [SerializeField] private UIElementStatusController fusionReactor;
    [SerializeField] private UIElementStatusController iceDrill;
    [SerializeField] private UIElementStatusController boiler;
    [SerializeField] private UIElementStatusController greenhouse;
    [SerializeField] private UIElementStatusController incinerator;
    [SerializeField] private UIElementStatusController shotgunTurret;
    [SerializeField] private UIElementStatusController machineGunTurret;

    [Header("Progress/Ratio Bars")]
    [SerializeField] private UIElementStatusController progressBar;
    [SerializeField] private UIElementStatusController humidityBar;
    [SerializeField] private UIElementStatusController biodiversityBar;
    [SerializeField] private UIElementStatusController atmosphereBar;

    //Non-Serialized Fields------------------------------------------------------------------------

    private DialogueBox consoleDB;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    ////Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// StageSkippedTutorial's singleton public property.
    ///// </summary>
    //public StageSkippedTutorial Instance { get; protected set; }

    /// <summary>
    /// The ID of StageSkippedTutorial. 
    /// </summary>
    public EStage GetID()
    {
        return EStage.SkippedTutorial;
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        //if (Instance != null)
        //{
        //    Debug.LogError("There should never be more than one StageFinishedTutorial.");
        //}

        //Instance = this;
        base.Awake();
    }

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
        ClockController.Instance.Paused = true;
        ClockController.Instance.SetTime(0);
        yield return new WaitForSeconds(3);
    }

    /// <summary>
    /// Brings the UI online.
    /// </summary>
    private IEnumerator EnableUI()
    {
        uiBorder.Visible = true;
        console.Visible = true;
        buildingAndResourcesBar.Visible = true;
        miniMapBorder.Visible = true;

        while (!uiBorder.FinishedFlickeringIn
            || !console.FinishedFlickeringIn
            || !buildingAndResourcesBar.FinishedFlickeringIn
            || !miniMapBorder.FinishedFlickeringIn)
        {
            yield return null;
        }

        ClockController.Instance.Paused = false;
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
        fusionReactor.Interactable = true;
        yield return new WaitForSeconds(0.15f);
        iceDrill.Visible = true;
        iceDrill.Interactable = true;
        yield return new WaitForSeconds(0.15f);
        boiler.Visible = true;
        boiler.Interactable = true;
        humidityBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        greenhouse.Visible = true;
        greenhouse.Interactable = true;
        biodiversityBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        incinerator.Visible = true;
        incinerator.Interactable = true;
        atmosphereBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        shotgunTurret.Visible = true;
        shotgunTurret.Interactable = true;
        yield return new WaitForSeconds(0.15f);
        machineGunTurret.Visible = true;
        machineGunTurret.Interactable = true;
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
