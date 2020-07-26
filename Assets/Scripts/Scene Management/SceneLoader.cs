using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the changing of scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private GameObject canvas;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private float fadeSpeed;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private List<Graphic> graphics;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// SceneLoader's singleton public property.
    /// </summary>
    public static SceneLoader Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one SceneLoader.");
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        graphics = new List<Graphic>() { background, textBox };
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Loads the game scene from the main menu.
    /// </summary>
    public void LoadGame()
    {
        StartCoroutine(LoadingGame());
    }

    /// <summary>
    /// Loads the game scene from the main menu.
    /// </summary>
    private IEnumerator LoadingGame()
    {
        canvas.SetActive(true);
        yield return StartCoroutine(Fade(true));
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene((int)EScene.Game);
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(Fade(false));
        canvas.SetActive(false);
    }

    /// <summary>
    /// Loads the main menu scene from the game.
    /// </summary>
    public void LoadMainMenu()
    {
        StartCoroutine(LoadingMainMenu());
    }

    /// <summary>
    /// Loads the main menu scene from the game.
    /// </summary>
    private IEnumerator LoadingMainMenu()
    {
        canvas.SetActive(true);
        yield return StartCoroutine(Fade(true));
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene((int)EScene.MainMenu);
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(Fade(false));
        canvas.SetActive(false);
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
                colour.a += fadeSpeed * Time.deltaTime * directionMultiplier;
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
