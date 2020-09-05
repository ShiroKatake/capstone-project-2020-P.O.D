using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class to control the clock in the UI and the day-night cycle.
/// </summary>
public class ClockController : SerializableSingleton<ClockController>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [Header("Time Stats")]
    [SerializeField] private float cycleDuration;
    
    [Header("UI Elements")]
    [SerializeField] private Image clockTimer;
    //[SerializeField] private Image clockBackground;

    [Header("UI Colours")]
    [SerializeField] private Color day;
    [SerializeField] private Color night;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

	[SerializeField] private float time12hr;
    [SerializeField] private float time24hr;
    private float halfCycleDuration;

    private bool daytime;
    private bool paused;

    private RectTransform rectTransform;

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

        //rectTransform = clockTimer.GetComponent<RectTransform>();

        //rectTransform.Rotate(new Vector3(0,0,0));
        //clockTimer.fillAmount = 1;
        //clockTimer.color = day;
        //clockBackground.color = day;
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

        if (daytime)
        {
            time12hr += UnityEngine.Time.deltaTime;
            time24hr += UnityEngine.Time.deltaTime;
        }
        else if (time12hr < halfCycleDuration * AlienController.Instance.AlienKillProgress)
        {
            time12hr += UnityEngine.Time.deltaTime;
            time24hr += UnityEngine.Time.deltaTime;
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
                time12hr -= halfCycleDuration;
                daytime = false;
                //clockTimer.color = night;
                //clockBackground.color = night;
                UIColorManager.Instance.SetNight();
                AudioManager.Instance.SwitchBackgroundTrack(AudioManager.ESound.NightTime);
            }
        }
        else
        {
            if (time24hr >= cycleDuration)
            {
                time12hr -= halfCycleDuration;
                time24hr -= cycleDuration;
                daytime = true;
                //clockTimer.color = day;
                //clockBackground.color = day;
                UIColorManager.Instance.SetDay();
                AudioManager.Instance.SwitchBackgroundTrack(AudioManager.ESound.DayTimeLvlOne);
            }
        }
    }

    /// <summary>
    /// Update the clock according to the time.
    /// </summary>
    private void UpdateClock()
    {
        //rectTransform.Rotate(new Vector3(0,0,360 * (time24hr / cycleDuration)));
        //rectTransform.Rotate(new Vector3(0,0,180));
        //clockTimer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (360 * (time24hr / cycleDuration))));
        clockTimer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (180 - 360 * (time24hr / cycleDuration))));
        //clockTimer.fillAmount = 1 - (time12hr / halfCycleDuration);
        UIColorManager.Instance.ColorUpdate();
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Manually sets the time of day.
    /// </summary>
    /// <param name="time">The 24-hour-equivalent time in seconds since the start of the day-night cycle that you want to set the clock to.</param>
    public void SetTime(float time)
    {
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
