using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to control the day-night cycle.
/// </summary>
public class DayNightCycleController : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private float cycleDuration;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private bool daytime;
    private float time;
    private float halfCycleDuration;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// DayNightCycleController's singleton public property.
    /// </summary>
    public static DayNightCycleController Instance { get; protected set; }

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
    /// The time elapsed in seconds since the start of the current day.
    /// </summary>
    public float Time { get => time; }

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
            Debug.LogError("There should never be more than one DayNightCycleController.");
        }

        Instance = this;
        halfCycleDuration = cycleDuration * 0.5f;
        daytime = true;
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    //private void Start()
    //{

    //}

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        time += UnityEngine.Time.deltaTime;

        if (daytime)
        {
            if (time >= halfCycleDuration)
            {
                daytime = false;
            }
        }
        else
        {
            if (time >= cycleDuration)
            {
                time -= cycleDuration;
                daytime = true;
            }
        }
    }

    /// <summary>
    /// FixedUpdate() is run at a fixed interval independant of framerate.
    /// </summary>
    //private void FixedUpdate()
    //{

    //}

    //Recurring Methods (Update())------------------------------------------------------------------------------------------------------------------  



    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------



    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------



    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------



    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  


}
