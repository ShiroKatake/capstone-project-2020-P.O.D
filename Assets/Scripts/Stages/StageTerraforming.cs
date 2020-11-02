using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The stage of the game where the player is introduced to the buildings and how terraforming works.
/// </summary>
public class StageTerraforming : PublicInstanceSerializableSingleton<StageTerraforming>, IStage
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("General UI")]
    [SerializeField] private UIElementStatusManager clock;

    [Header("Building Buttons")]
    [SerializeField] private UIElementStatusManager fusionReactor;
    [SerializeField] private UIElementStatusManager iceDrill;
    [SerializeField] private UIElementStatusManager harvester;
    [SerializeField] private UIElementStatusManager gasPump;
    [SerializeField] private UIElementStatusManager boiler;
    [SerializeField] private UIElementStatusManager greenhouse;
    [SerializeField] private UIElementStatusManager incinerator;

    [Header("Progress/Ratio Bars")]
    [SerializeField] private UIElementStatusManager progressBar;
    [SerializeField] private UIElementStatusManager humidityBar;
    [SerializeField] private UIElementStatusManager biodiversityBar;
    [SerializeField] private UIElementStatusManager atmosphereBar;

    [Header("Highlights")]
    [SerializeField] private UIElementStatusManager fusionReactorHighlight;
    [SerializeField] private UIElementStatusManager iceDrillHighlight;
    [SerializeField] private UIElementStatusManager boilerHighlight;
    [SerializeField] private UIElementStatusManager greenhouseHighlight;
    [SerializeField] private UIElementStatusManager incineratorHighlight;
    [SerializeField] private UIElementStatusManager humidityBarHighlight;
    [SerializeField] private UIElementStatusManager biodiversityBarHighlight;
    [SerializeField] private UIElementStatusManager atmosphereBarHighlight;
    [SerializeField] private UIElementStatusManager ratioBarsHighlight;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    DialogueBox console;
    DialogueBox cat;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The ID of StageTerraforming. 
    /// </summary>
    public EStage GetID()
    {
        return EStage.Terraforming;
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

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
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    public IEnumerator Execution()
    {
        yield return StartCoroutine(BuildFusionReactor());
        yield return StartCoroutine(BuildIceDrill());
        yield return StartCoroutine(TerraformingWalkthrough());
        yield return StartCoroutine(StageComplete());        
        StageManager.Instance.SetStage(EStage.Combat);
    }

    /// <summary>
    /// Teaches the player about the fusion reactor.
    /// </summary>
    private IEnumerator BuildFusionReactor()
    {
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

        console.ClearDialogue();
        console.SubmitDialogue("task build fusion reactor", 0, false, false);
        cat.SubmitDialogue("build fusion reactor", 0, true, false);
        fusionReactor.Visible = true;
        fusionReactor.Interactable = true;
        fusionReactorHighlight.Visible = true;

        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.FusionReactor) == 0)
        {
            bool placedFusionReactor = BuildingManager.Instance.PlacedBuildingsCount(EBuilding.FusionReactor) > 0;

            //Keep fusion reactor button interactable only while it needs to be placed
            if (placedFusionReactor)
            {
                if (fusionReactor.Interactable)
                {
                    fusionReactor.Interactable = false;
                }
            }
            else
            {
                if (!fusionReactor.Interactable)
                {
                    fusionReactor.Interactable = true;
                }
            }

            yield return null;
        }

        fusionReactor.Interactable = false;
    }

    /// <summary>
    /// Teaches the player about the ice drill.
    /// </summary>
    private IEnumerator BuildIceDrill()
    {
        console.ClearDialogue();
        console.SubmitDialogue("task build ice drill", 0, false, false);
        cat.SubmitDialogue("build ice drill", 0, true, false);
        iceDrill.Visible = true;
        iceDrill.Interactable = true;
        iceDrillHighlight.Visible = true;

        while (BuildingManager.Instance.BuiltBuildingsCount(EBuilding.IceDrill) == 0)
        {
            bool placedIceDrill = BuildingManager.Instance.PlacedBuildingsCount(EBuilding.IceDrill) > 0;

            //Keep ice drill button interactable only while it needs to be placed
            if (placedIceDrill)
            {
                if (iceDrill.Interactable)
                {
                    iceDrill.Interactable = false;
                }
            }
            else
            {
                if (!iceDrill.Interactable)
                {
                    iceDrill.Interactable = true;
                }
            }

            yield return null;
        }

        iceDrill.Interactable = false;
        console.ClearDialogue();
        cat.SubmitDialogue("got power and water", 0, false, false);

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Teaches the player about terraforming.
    /// </summary>
    private IEnumerator TerraformingWalkthrough()
    {
        cat.SubmitDialogue("boiler", 0, false, false);
        boiler.Visible = true;
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
    }

    /// <summary>
    /// Concludes the terraforming stage of the tutorial.
    /// </summary>
    private IEnumerator StageComplete()
    {
        cat.SubmitDialogue("good luck", 0, true, false);
        clock.Visible = true;
        ClockManager.Instance.Paused = false;

        while (!cat.DialogueRead || !cat.AcceptingSubmissions)
        {
            yield return null;
        }

        console.SubmitDialogue("cat closed", 1, false, false);
        harvester.Visible = true;
        gasPump.Visible = true;
        fusionReactor.Interactable = true;
        iceDrill.Interactable = true;
        harvester.Interactable = true;
        gasPump.Interactable = true;
        boiler.Interactable = true;
        greenhouse.Interactable = true;
        incinerator.Interactable = true;
    }
}
