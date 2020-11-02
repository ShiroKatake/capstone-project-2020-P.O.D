using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the changing of scenes.
/// </summary>
public class SceneLoader : PublicInstanceSerializableSingleton<SceneLoader>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private GameObject canvas;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private float fadeSpeed;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<Graphic> graphics;
    private bool skipTutorial;
    private bool loadingScene;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Is the SceneLoader in the middle of loading a scene?
    /// </summary>
    public bool LoadingScene { get => loadingScene; }

    /// <summary>
    /// Has the player chosen to skip the tutorial?
    /// </summary>
    public bool SkipTutorial { get => skipTutorial; set => skipTutorial = value; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        graphics = new List<Graphic>() { background, textBox };
        loadingScene = false;
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Loads the game scene.
    /// </summary>
    public void LoadGame()
    {
        StartCoroutine(LoadingGame());
    }

    /// <summary>
    /// Loads the game scene.
    /// </summary>
    private IEnumerator LoadingGame()
    {
        loadingScene = true;
        canvas.SetActive(true);
        yield return StartCoroutine(Fade(true));
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene((int)EScene.Game);
        yield return new WaitForSecondsRealtime(0.3f);
        yield return StartCoroutine(Fade(false));
        canvas.SetActive(false);
        loadingScene = false;
        if (Time.timeScale != 1) Time.timeScale = 1;        
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        StartCoroutine(LoadingMainMenu());
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    private IEnumerator LoadingMainMenu()
    {
        loadingScene = true;
        canvas.SetActive(true);
        yield return StartCoroutine(Fade(true));
        yield return new WaitForSecondsRealtime(0.3f);
        SceneManager.LoadScene((int)EScene.MainMenu);
        yield return new WaitForSecondsRealtime(0.3f);
        yield return StartCoroutine(Fade(false));
        canvas.SetActive(false);
        loadingScene = false;
        if (Time.timeScale != 1) Time.timeScale = 1;
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        StartCoroutine(QuittingGame());
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    private IEnumerator QuittingGame()
    {
        Debug.Log("Quit");
        yield return null;
        Application.Quit();
    }

    /// <summary>
    /// Fades the loading screen in or out.
    /// </summary>
    /// <param name="increasing">Is the opacity of the loading screen increasing?</param>
    private IEnumerator Fade(bool increasing)
    {
        bool finished = false;
        int directionMultiplier = (increasing ? 1 : -1);
        int goal = (increasing ? 1 : 0);

        while (!finished)
        {
            finished = true;

            foreach (Graphic g in graphics)
            {
                Color colour = g.color;
                colour.a += fadeSpeed * Time.unscaledDeltaTime * directionMultiplier;
                g.color = colour;

                if (g.color.a * directionMultiplier < goal)
                {
                    finished = false;
                }
            }

            yield return null;
        }
    }
}
