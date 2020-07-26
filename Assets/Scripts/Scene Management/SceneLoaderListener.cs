using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates SceneLoader if it's not instantiated and triggers its scene changing methods.
/// </summary>
public class SceneLoaderListener : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private SceneLoader sceneLoaderPrefab;

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Start()
    {
        bool sceneLoaderInstantiated = false;

        try
        {
            if (SceneLoader.Instance != null)
            {
                sceneLoaderInstantiated = true;
            }
        }
        catch
        {

        }

        if (!sceneLoaderInstantiated)
        {
            Instantiate(sceneLoaderPrefab);
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Triggers the loading of the game from the main menu.
    /// </summary>
    public void LoadGame()
    {
        SceneLoader.Instance.LoadGame();
    }

    /// <summary>
    /// Triggers the loading of the main menu from the game.
    /// </summary>
    public void LoadMainMenu()
    {
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
