using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class to control the clock in the UI and the day-night cycle.
/// </summary>
public class ClockManager : PublicInstanceSerializableSingleton<ClockManager>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Time Stats")]
    [SerializeField] private float cycleDuration;
    
    [Header("UI Elements")]
    [SerializeField] private Image clockTimer;

    [Header("UI Colours")]
    [SerializeField] private Color day;
    [SerializeField] private Color night;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    [Header("For Testing")]
	[SerializeField] private float time12hr;
    [SerializeField] private float time24hr;
    [SerializeField] private float set24hrTo;
    [SerializeField] private bool debugClockProgressAtNight;
    private float halfCycleDuration;

    private bool daytime;
    private bool paused;

    private RectTransform rectTransform;
    private float lastSet24hrTo;


    //Public Properties------------------------------------------------------------------------------------------------------------------------------

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
    /// Is the day night cycle paused?
    /// </summary>
    public bool Paused { get => paused; set => paused = value; }

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
    protected override void Awake()
    {
        base.Awake();
        halfCycleDuration = cycleDuration * 0.5f;
        daytime = true;
        set24hrTo = 0;
        lastSet24hrTo = 0;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            if (!paused)
            {
                UpdateTime();
                CheckDayNight();
                UpdateClock();
            }
        }
    }

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  

    /// <summary>
    /// Increment the time by Time.deltaTime;
    /// </summary>
    private void UpdateTime()
    {
        if (set24hrTo != lastSet24hrTo)
        {
            lastSet24hrTo = set24hrTo;
            time24hr = set24hrTo;
            time12hr = time24hr;
            
            if (time12hr > halfCycleDuration)
            {
                time12hr -= halfCycleDuration;
            }
        }

        if (daytime)
        {
            time12hr += UnityEngine.Time.deltaTime;
            time24hr += UnityEngine.Time.deltaTime;
            //Debug.Log($"ClockController.UpdateTime(), is daytime, time12hr is now {time12hr}");
        }
        else if (time12hr < halfCycleDuration * AlienManager.Instance.AlienKillProgress)
        {
            time12hr += UnityEngine.Time.deltaTime;
            time24hr += UnityEngine.Time.deltaTime;

            if (debugClockProgressAtNight)
            {
                Debug.Log($"ClockController.UpdateTime(), is nighttime and clock is catching up to alien kill progress, time12hr is now {time12hr}, progress is {AlienManager.Instance.AlienKillProgress}, target time is {halfCycleDuration * AlienManager.Instance.AlienKillProgress}");
            }
        }
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
                //Debug.Log($"ClockController.CheckDayNight(), day to night, time12hr is {time12hr}, halfCycleDuration is {halfCycleDuration}");
                time12hr -= halfCycleDuration;
                //Debug.Log($"ClockController.CheckDayNight(), day to night, time12hr has now been set to {time12hr}");
                daytime = false;
                UIColorManager.Instance.SetNight();
                AudioManager.Instance.SwitchBackgroundTrack(AudioManager.ESound.NightTime);
            }
        }
        else
        {
            if (time24hr >= cycleDuration)
            {
                //Debug.Log("ClockController.CheckDayNight(), night to day");
                time12hr -= halfCycleDuration;
                time24hr -= cycleDuration;
                daytime = true;
                UIColorManager.Instance.SetDay();
                AudioManager.Instance.SwitchBackgroundTrack(AudioManager.ESound.DayTimeLvlOne);
            }
        }
    }

    /// <summary>
    /// Update the clock UI element according to the time.
    /// </summary>
    private void UpdateClock()
    {
        clockTimer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (180 - 360 * (time24hr / cycleDuration))));
        UIColorManager.Instance.ColorUpdate();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Manually sets the time of day.
    /// </summary>
    /// <param name="time">The 24-hour-equivalent time in seconds since the start of the day-night cycle that you want to set the clock to.</param>
    public void SetTime(float time)
    {
        //Debug.Log($"ClockController.SetTime() to {time}/{cycleDuration}");
        while (time >= cycleDuration)
        {
            time -= cycleDuration;
        }

        time24hr = time;

        if (time >= halfCycleDuration)
        {
            time -= halfCycleDuration;
        }

        time12hr = time;

        CheckDayNight();
        UpdateClock();
    }
}
