using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Instantiates SceneLoader if it's not instantiated and triggers its scene changing methods.
/// </summary>
public class SceneLoaderListener : SerializableSingleton<SceneLoaderListener>
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private SceneLoader sceneLoaderPrefab;
    [SerializeField] private Toggle skipTutorialToggle;

    //Non-Serialized Fields------------------------------------------------------------------------

    private bool sceneLoaderInstantiatedOnAwake;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------

    ///// <summary>
    ///// SceneLoaderListener's singleton public property.
    ///// </summary>
    //public static SceneLoaderListener Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Was SceneLoader already instantiated when SceneLoaderListener.Awake() ran?
    /// </summary>
    public bool SceneLoaderInstantiatedOnAwake { get => sceneLoaderInstantiatedOnAwake; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    protected override void Awake()
    {
        //if (Instance != null)
        //{
        //    Debug.LogError("There should never be 2 or more SceneLoaderListeners at once.");
        //}

        //Instance = this;
        base.Awake();
        sceneLoaderInstantiatedOnAwake = false;

        try
        {
            if (SceneLoader.Instance != null)
            {
                sceneLoaderInstantiatedOnAwake = true;
            }
        }
        catch
        {

        }

        if (!sceneLoaderInstantiatedOnAwake)
        {
            Instantiate(sceneLoaderPrefab);
        }
        else if (skipTutorialToggle != null)
        {
            skipTutorialToggle.isOn = SceneLoader.Instance.SkipTutorial;
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Triggers the loading of the game.
    /// </summary>
    public void LoadGame()
    {
        if (skipTutorialToggle != null)
        {
            SceneLoader.Instance.SkipTutorial = skipTutorialToggle.isOn;
        }
        else
        {
            try
            {
                if (StageManager.Instance != null)
                {
                    SceneLoader.Instance.SkipTutorial = StageManager.Instance.SkipTutorial;
                }
            }
            catch
            {

            }
        }

        SceneLoader.Instance.LoadGame();
    }

    /// <summary>
    /// Triggers the loading of the main menu.
    /// </summary>
    public void LoadMainMenu()
    {
        try
        {
            if (StageManager.Instance != null)
            {
                SceneLoader.Instance.SkipTutorial = StageManager.Instance.SkipTutorial;
            }
        }
        catch
        {

        }

        SceneLoader.Instance.LoadMainMenu();
    }

    /// <summary>
    /// Triggers the quitting of the game.
    /// </summary>
    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
