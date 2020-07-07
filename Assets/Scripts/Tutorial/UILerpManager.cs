using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the lerping of UI elements
/// </summary>
public class UILerpManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    



    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Stage currentStage;
    private Image uiLerpTarget;
    private float lerpMultiplier;
    private bool lerpUIScalingTarget;
    private Image currentUILerpFocus;
    private float uiMinLerp;
    private Color minLerpColour;
    private Color maxLerpColour;
    private float uiTargetLerpProgress;
    private bool lerpPaused;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// UILerpManager's singleton public property.
    /// </summary>
    public static UILerpManager Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    public Stage CurrentStage { get => currentStage; set => currentStage = value; }
    public Image UILerpTarget { get => uiLerpTarget; }

    //Complex Public Properties--------------------------------------------------------------------                                                    



    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one [CLASSNAME].");
        }

        Instance = this;
    }

    private void Configure()
    {

    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {

    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    private void FixedUpdate()
    {

    }

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  



    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------



    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------



    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Activates the UI lerp target
    private void ActivateUIColourLerpTarget(Image button, Color minLerpColour, Color maxLerpColour)
    {
        lerpUIColourTarget = true;
        uiColourLerpTarget = button;
        uiColourLerpTarget.gameObject.SetActive(true);

        uiTargetLerpForward = true;
        uiTargetLerpProgress = 0;

        this.minLerpColour = minLerpColour;
        this.maxLerpColour = maxLerpColour;
        uiColourLerpTarget.color = minLerpColour;
    }

    //Lerps the UI lerp target
    private void LerpUIColourTarget()
    {
        uiColourLerpTarget.color = Color.Lerp(minLerpColour, maxLerpColour, uiTargetLerpProgress);
        UpdateUITargetLerpValues();
    }

    //Called by other functionality to update the colour of the lerp target on mouse enter and pause lerping
    public void PauseColourLerp(Image calledBy)
    {
        if (lerpUIColourTarget && uiColourLerpTarget == calledBy)
        {
            uiColourLerpTarget.color = maxLerpColour;
            lerpPaused = true;
        }
    }

    //Called by other functionality to update the colour of the lerp target on mouse exit and unpause lerping
    public void UnpauseColourLerp(Image calledBy)
    {
        if (lerpUIColourTarget && uiColourLerpTarget == calledBy)
        {
            lerpPaused = false;
            uiTargetLerpProgress = 1;
            uiTargetLerpForward = false;
            uiColourLerpTarget.gameObject.SetActive(true);
        }
    }

    //Deactivates the UI lerp target
    private void DeactivateUIColourLerpTarget()
    {
        lerpUIColourTarget = false;
        lerpPaused = false;
        uiTargetLerpForward = true;
        uiTargetLerpProgress = 0;

        minLerpColour = Color.clear;
        maxLerpColour = Color.clear;
        uiLerpTarget.color = Color.clear;

        uiColourLerpTarget.gameObject.SetActive(false);
        uiColourLerpTarget = null;
    }

    //Activates the UI lerp target, also assigning mouse enter and mouse exit colours
    private void ActivateUIScalingLerpTarget(Image newUILerpTarget, float minLerp, Color mouseEnterColour, Color mouseExitColour)
    {
        maxLerpColour = mouseEnterColour;
        minLerpColour = mouseExitColour;
        ActivateUIScalingLerpTarget(newUILerpTarget, minLerp, mouseExitColour);
    }

    //Activates the UI lerp target
    private void ActivateUIScalingLerpTarget(Image newUILerpTarget, float minLerp, Color colour)
    {
        lerpUIScalingTarget = true;
        uiLerpTarget.transform.position = newUILerpTarget.transform.position;
        uiTargetLerpForward = true;
        uiTargetLerpProgress = 0;
        uiMinLerp = minLerp;
        currentUILerpFocus = newUILerpTarget;
        UpdateUIScalingLerpTargetColour(colour);
    }

    //Updates the colour of the ui lerp target
    private void UpdateUIScalingLerpTargetColour(Color colour)
    {
        uiLerpTarget.color = colour;

        if (multipleLerpRingsForBattery && stage == TutorialStage.MouseOverPowerDiagram)
        {
            foreach (Image i in uiLerpTarget.GetComponentsInChildren<Image>())
            {
                i.color = colour;
            }
        }
    }

    //Lerps the UI lerp target
    private void LerpUIScalingTarget()
    {
        float lerp = Mathf.Lerp(uiMinLerp, uiMinLerp * 1.5f, uiTargetLerpProgress);
        uiLerpTarget.transform.localScale = new Vector3(lerp, lerp, uiLerpTarget.transform.localScale.z);

        if (uiLerpTarget.transform.position != currentUILerpFocus.transform.position)
        {
            uiLerpTarget.transform.position = currentUILerpFocus.transform.position;
        }

        if (stage == TutorialStage.MouseOverPowerDiagram)
        {
            SynchroniseToBatteryColour();
        }

        UpdateUITargetLerpValues();
    }

    //Called to update the battery colour according to the current amount of power stored
    private void SynchroniseToBatteryColour()
    {
        float power = UIController.instance.CurrentPowerValDisplayed;

        if (power == 0)
        {
            if (uiLerpTarget.color != batteryEmptyColour)
            {
                UpdateUIScalingLerpTargetColour(batteryEmptyColour);
            }
        }
        else if (power <= 25)
        {
            if (uiLerpTarget.color != batteryLowColour)
            {
                UpdateUIScalingLerpTargetColour(batteryLowColour);
            }
        }
        else if (power <= 50)
        {
            if (uiLerpTarget.color != batteryHalfColour)
            {
                UpdateUIScalingLerpTargetColour(batteryHalfColour);
            }
        }
        else if (power <= 75)
        {
            if (uiLerpTarget.color != batteryHighColour)
            {
                UpdateUIScalingLerpTargetColour(batteryHighColour);
            }
        }
        else if (power > 75)
        {
            if (uiLerpTarget.color != batteryFullColour)
            {
                UpdateUIScalingLerpTargetColour(batteryFullColour);
            }
        }
    }

    //Deactivates the UI lerp target
    private void DeactivateUIScalingLerpTarget()
    {
        lerpUIScalingTarget = false;
        uiLerpTarget.transform.localScale = new Vector3(uiMinLerp, uiMinLerp, uiLerpTarget.transform.localScale.z);
        UpdateUIScalingLerpTargetColour(Color.clear);

        uiMinLerp = 1;
        //uiMaxLerp = 1;
    }


    //Update UI lerp progress
    private void UpdateUITargetLerpValues()
    {
        if (uiTargetLerpForward)
        {
            uiTargetLerpProgress += Time.deltaTime * lerpMultiplier;
        }
        else
        {
            uiTargetLerpProgress -= Time.deltaTime * lerpMultiplier;
        }

        if (uiTargetLerpProgress > 1)
        {
            uiTargetLerpProgress = 1;
            uiTargetLerpForward = false;
        }
        else if (uiTargetLerpProgress < 0)
        {
            uiTargetLerpProgress = 0;
            uiTargetLerpForward = true;
        }
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
