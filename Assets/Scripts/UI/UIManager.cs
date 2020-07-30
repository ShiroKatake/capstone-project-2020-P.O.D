using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Canvases")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    //Non-Serialized Fields------------------------------------------------------------------------

    //[Header("Testing")]
    //[SerializeField] private bool isShowingUICanvas;
    //[SerializeField] private bool isShowingPauseMenuCanvas;
    //[SerializeField] private bool isShowingGameOverCanvas;

    private UIButtonInitialise pauseInitialiser;
    private UIButtonInitialise gameOverInitialiser;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    /// <summary>
    /// UIManager's singleton public property.
    /// </summary>
    public static UIManager Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The game over canvas.
    /// </summary>
    public GameObject GameOverCanvas {get => gameOverCanvas;}   
    
    /// <summary>
    /// The pause menu canvas.
    /// </summary>
    public GameObject PauseMenuCanvas {get => pauseMenuCanvas;}  
    
    /// <summary>
    /// The main UI canvas.
    /// </summary>
    public GameObject UICanvas {get => uiCanvas;}

    //Initialisation Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more UIManagers.");
        }

        Instance = this;

        pauseInitialiser = pauseMenuCanvas.GetComponent<UIButtonInitialise>();
        gameOverInitialiser = gameOverCanvas.GetComponent<UIButtonInitialise>();
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    void Start()
    {
        uiCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
    }

    //Triggered Methods----------------------------------------------------------------------------

    /// <summary>
    /// Activates or deactivates the game over canvas.
    /// </summary>
    /// <param name="active">Should the game over canvas be active or not?</param>
    public void SetGameOverCanvasActive(bool active)
    {
        if (!SceneLoader.Instance.LoadingScene)
        {
            gameOverCanvas.SetActive(active);

            if (active)
            {
                gameOverInitialiser.Initialise();
            }
        }
    }

    /// <summary>
    /// Activates or deactivates the pause menu canvas.
    /// </summary>
    /// <param name="active">Should the pause menu canvas be active or not?</param>
    public void SetPauseMenuCanvasActive(bool active)
    {
        if (!SceneLoader.Instance.LoadingScene)
        {
            pauseMenuCanvas.SetActive(active);

            if (active)
            {
                pauseInitialiser.Initialise();
            }
        }
    }
}
