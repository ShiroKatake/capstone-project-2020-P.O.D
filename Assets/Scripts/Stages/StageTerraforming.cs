using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is introduced to the buildings and how terraforming works.
/// </summary>
public class StageTerraforming : Stage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("General UI")]
    [SerializeField] private UIElementStatusController clock;

    [Header("Building Buttons")]
    [SerializeField] private UIElementStatusController fusionReactor;
    [SerializeField] private UIElementStatusController iceDrill;
    [SerializeField] private UIElementStatusController boiler;
    [SerializeField] private UIElementStatusController greenhouse;
    [SerializeField] private UIElementStatusController incinerator;

    [Header("Progress/Ratio Bars")]
    [SerializeField] private UIElementStatusController progressBar;
    [SerializeField] private UIElementStatusController humidityBar;
    [SerializeField] private UIElementStatusController biodiversityBar;
    [SerializeField] private UIElementStatusController atmosphereBar;

    [Header("Highlights")]
    [SerializeField] private UIElementStatusController fusionReactorHighlight;
    [SerializeField] private UIElementStatusController iceDrillHighlight;
    [SerializeField] private UIElementStatusController boilerHighlight;
    [SerializeField] private UIElementStatusController greenhouseHighlight;
    [SerializeField] private UIElementStatusController incineratorHighlight;
    [SerializeField] private UIElementStatusController humidityBarHighlight;
    [SerializeField] private UIElementStatusController biodiversityBarHighlight;
    [SerializeField] private UIElementStatusController atmosphereBarHighlight;
    [SerializeField] private UIElementStatusController ratioBarsHighlight;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    DialogueBox console;
    DialogueBox cat;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// StageTerraforming's singleton public property.
    /// </summary>
    public StageTerraforming Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one StageTerraforming.");
        }

        Instance = this;
        id = EStage.Terraforming;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        console = DialogueBoxManager.Instance.GetDialogueBox("Console");
        cat = DialogueBoxManager.Instance.GetDialogueBox("CAT");
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of StageTerraforming.
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
        //Yay minerals for building
        cat.SubmitDialogue("enough for building", 0, false, false);

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        cat.SubmitDialogue("build buildings", 0, false, false);

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        //Build fusion reactor
        console.ClearDialogue();
        console.SubmitDialogue("task build fusion reactor", 0, false, false);
        cat.SubmitDialogue("build fusion reactor", 0, true, false);
        fusionReactor.Visible = true;
        fusionReactor.Interactable = true;
        fusionReactorHighlight.Visible = true;

        while(!BuildingController.Instance.HasBuiltBuilding(EBuilding.FusionReactor))
        {
            yield return null;
        }

        //Build ice drill
        fusionReactor.Interactable = false;
        console.ClearDialogue();
        console.SubmitDialogue("task build ice drill", 0, false, false);
        cat.SubmitDialogue("build ice drill", 0, true, false);
        iceDrill.Visible = true;
        iceDrill.Interactable = true;
        iceDrillHighlight.Visible = true;

        while (!BuildingController.Instance.HasBuiltBuilding(EBuilding.IceDrill))
        {
            yield return null;
        }

        //Yay power and water
        console.ClearDialogue();
        cat.SubmitDialogue("got power and water", 0, false, false);
        fusionReactor.Interactable = true;

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        //Here's the terraformers
        cat.SubmitDialogue("boiler", 0, false, false);
        boiler.Visible = true;
        boiler.Interactable = true;
        boilerHighlight.Visible = true;
        humidityBar.Visible = true;
        humidityBarHighlight.Visible = true;

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        boilerHighlight.Visible = false;
        humidityBarHighlight.Visible = false;
        cat.SubmitDialogue("greenhouse", 0, false, false);
        greenhouse.Visible = true;
        greenhouse.Interactable = true;
        greenhouseHighlight.Visible = true;
        biodiversityBar.Visible = true;
        biodiversityBarHighlight.Visible = true;

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        greenhouseHighlight.Visible = false;
        biodiversityBarHighlight.Visible = false;
        cat.SubmitDialogue("incinerator", 0, false, false);
        incinerator.Visible = true;
        incinerator.Interactable = true;
        incineratorHighlight.Visible = true;
        atmosphereBar.Visible = true;
        atmosphereBarHighlight.Visible = true;

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        incineratorHighlight.Visible = false;
        atmosphereBarHighlight.Visible = false;
        cat.SubmitDialogue("buildings important", 0, false, false);

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        cat.SubmitDialogue("ratios important", 0, false, false);
        progressBar.Visible = true;
        ratioBarsHighlight.Visible = true;

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        cat.SubmitDialogue("good luck", 0, true, false);
        clock.Visible = true;
        ClockController.Instance.Paused = false;

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        console.SubmitDialogue("cat closed", 1, false, false);
        StageManager.Instance.SetStage(EStage.Combat);
    }
}
