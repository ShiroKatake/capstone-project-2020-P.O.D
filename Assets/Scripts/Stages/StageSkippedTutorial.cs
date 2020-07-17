using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is just left to do their own thing and play the game.
/// </summary>
public class StageSkippedTutorial : Stage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("General UI")]
    [SerializeField] private UIElementStatusController uiBorder;
    [SerializeField] private UIElementStatusController console;
    [SerializeField] private UIElementStatusController buildingAndResourcesBar;
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

    //Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// StageSkippedTutorial's singleton public property.
    /// </summary>
    public StageSkippedTutorial Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one StageFinishedTutorial.");
        }

        Instance = this;
        id = EStage.SkippedTutorial;
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
    /// The main behaviour of StageFinishedTutorial.
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
        //Switch the clock off
        ClockController.Instance.Paused = true;
        ClockController.Instance.SetTime(0);
        yield return new WaitForSeconds(3);

        //Enable UI
        uiBorder.Visible = true;

        while (!uiBorder.FinishedFlickeringIn)
        {
            yield return null;
        }

        console.Visible = true;
        consoleDB.SubmitDialogue("blank", 0, false, false);

        while (!console.FinishedFlickeringIn)
        {
            yield return null;
        }

        consoleDB.SubmitDialogue("system check", 0, false, false);
        buildingAndResourcesBar.Visible = true;
        miniMap.Visible = true;

        while (!buildingAndResourcesBar.FinishedFlickeringIn || !miniMap.FinishedFlickeringIn || consoleDB.LerpingDialogue)
        {
            yield return null;
        }

        consoleDB.SubmitDialogue("initialising functions", 0, false, false);
        ResourceTextManager.Instance.FadeIn();
        clock.Visible = true;
        //TODO: miniMapContent.Visible = true;

        while (!clock.FinishedFlickeringIn /*|| !miniMapContent.FinishedFlickeringIn*/ || consoleDB.LerpingDialogue)
        {
            yield return null;
        }

        //Enable Building Buttons
        ClockController.Instance.Paused = false;
        consoleDB.SubmitDialogue("clock gps inventory online", 0, false, false);
        fusionReactor.Visible = true;
        yield return new WaitForSeconds(0.15f);
        iceDrill.Visible = true;
        yield return new WaitForSeconds(0.15f);
        boiler.Visible = true;
        yield return new WaitForSeconds(0.15f);
        greenhouse.Visible = true;
        yield return new WaitForSeconds(0.15f);
        incinerator.Visible = true;
        yield return new WaitForSeconds(0.15f);
        shotgunTurret.Visible = true;
        yield return new WaitForSeconds(0.15f);
        machineGunTurret.Visible = true;

        while (!fusionReactor.FinishedFlickeringIn 
            || !iceDrill.FinishedFlickeringIn 
            || !boiler.FinishedFlickeringIn
            || !greenhouse.FinishedFlickeringIn
            || !incinerator.FinishedFlickeringIn
            || !shotgunTurret.FinishedFlickeringIn
            || !machineGunTurret.FinishedFlickeringIn 
            || consoleDB.LerpingDialogue
        )
        {
            yield return null;
        }

        //Enable Progress/Ratio Bars
        consoleDB.SubmitDialogue("buildings ready", 0, false, false);
        progressBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        humidityBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        biodiversityBar.Visible = true;
        yield return new WaitForSeconds(0.15f);
        atmosphereBar.Visible = true;

        while (!progressBar.FinishedFlickeringIn
            || !humidityBar.FinishedFlickeringIn
            || !biodiversityBar.FinishedFlickeringIn
            || !atmosphereBar.FinishedFlickeringIn
        )
        {
            yield return null;
        }

        //Begin Game
        consoleDB.SubmitDialogue("begin game", 0, false, false);
        fusionReactor.Interactable = true;
        iceDrill.Interactable = true;
        boiler.Interactable = true;
        greenhouse.Interactable = true;
        incinerator.Interactable = true;
        shotgunTurret.Interactable = true;
        machineGunTurret.Interactable = true;
        StageManager.Instance.SetStage(EStage.MainGame);
    }
}
