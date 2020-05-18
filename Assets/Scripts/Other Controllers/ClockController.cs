using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class to control the clock in the UI and the day-night cycle.
/// </summary>
public class ClockController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Time Stats")]
    [SerializeField] private float cycleDuration;
    
    [Header("UI Elements")]
    [SerializeField] private Image clockTimer;
    [SerializeField] private Image clockBackground;

    [Header("UI Colours")]
    [SerializeField] private Color day;
    [SerializeField] private Color night;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool daytime;
    private float time12hr;
    private float time24hr;
    private float halfCycleDuration;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// DayNightCycleController's singleton public property.
    /// </summary>
    public static ClockController Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The duration of one day-night cycle in seconds.
    /// </summary>
    public float CycleDuration { get => cycleDuration; }

    /// <summary>
    /// Is it currently daytime?
    /// </summary>
    public bool Daytime { get => daytime; }

    /// <summary>
    /// The duration of a day or night in seconds; i.e. half of one day-night cycle.
    /// </summary>
    public float HalfCycleDuration { get => halfCycleDuration; }

    /// <summary>
    /// The time elapsed in seconds since the start of the current day or night. Equivalent to 12-hour time.
    /// </summary>
    public float Time12hr { get => time12hr; }

    /// <summary>
    /// The time elapsed in seconds since the start of the current day. Equivalent to 24-hour time.
    /// </summary>
    public float Time24hr { get => time24hr; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one DayNightCycleController.");
        }

        Instance = this;
        halfCycleDuration = cycleDuration * 0.5f;
        daytime = true;
        clockTimer.fillAmount = 1;
        clockTimer.color = day;
        clockBackground.color = night;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        UpdateTime();
        CheckDayNight();
        UpdateClock();
    }

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Increment the time by Time.deltaTime;
    /// </summary>
    private void UpdateTime()
    {
        time12hr += UnityEngine.Time.deltaTime;
        time24hr += UnityEngine.Time.deltaTime;
    }

    /// <summary>
    /// Check if it should be daytime or nighttime.
    /// </summary>
    private void CheckDayNight()
    {
        if (daytime)
        {
            if (time24hr >= halfCycleDuration)
            {
                time12hr -= halfCycleDuration;
                daytime = false;
                clockTimer.color = night;
                clockBackground.color = day;
                UIColorManager.Instance.SetNight();
            }
        }
        else
        {
            if (time24hr >= cycleDuration)
            {
                time12hr -= halfCycleDuration;
                time24hr -= cycleDuration;
                daytime = true;
                clockTimer.color = day;
                clockBackground.color = night;
                UIColorManager.Instance.SetDay();
            }
        }
    }

    /// <summary>
    /// Update the clock according to the time.
    /// </summary>
    private void UpdateClock()
    {
        clockTimer.fillAmount = 1 - (time12hr / halfCycleDuration);
        UIColorManager.Instance.ColorUpdate();
    }
}
