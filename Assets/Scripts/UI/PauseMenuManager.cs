using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the pause menu.
/// </summary>
public class PauseMenuManager : SerializableSingleton<PauseMenuManager>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI dialogueLog;
    [SerializeField] private Scrollbar scrollbar;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    //Components
    private UIButtonInitialise buttonInitialiser;

    //Other Objects
    private Player playerInputManager;

    //Variables
    private static bool paused;
    private bool changePause;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The log of all dialogue in the pause menu.
    /// </summary>
    public TextMeshProUGUI DialogueLog { get => dialogueLog; }

    /// <summary>
    /// Is the game currently paused?
    /// </summary>
    public static bool Paused { get => paused; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        canvas.SetActive(true);
        buttonInitialiser = GetComponentInChildren<UIButtonInitialise>();
        canvas.SetActive(false);
        paused = false;
        changePause = false;
        dialogueLog.text = "";
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        playerInputManager = PODController.Instance.PlayerInputManager;
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (!UIManager.Instance.GameOverCanvas.activeInHierarchy && !SceneLoader.Instance.LoadingScene)
        {
            GetInput();
            CheckPaused();
        }
    }

    //Core Reucrring Methods (Update)----------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Gets the player's input for pausing or unpausing.
    /// </summary>
    private void GetInput()
    {
        changePause = playerInputManager.GetButtonDown("Pause");
    }

    /// <summary>
    /// Checks if the game should pause or unpause.
    /// </summary>
    private void CheckPaused()
    {
        if (changePause)
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause()
    {
        //Set timeScale and fixedTimeScale to 0
        Time.timeScale = 0f;
        canvas.SetActive(true);
        buttonInitialiser.Initialise();
        scrollbar.value = 0;
        paused = true;
    }

    /// <summary>
    /// Unpauses the game and resumes gameplay.
    /// </summary>
    public void Resume()
    {
        //Set timeScale and fixedTimeScale to 1
        canvas.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }
}
