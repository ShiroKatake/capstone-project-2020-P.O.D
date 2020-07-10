using UnityEngine;

/// <summary>
/// Just an off-switch for a light that ought to be on while in the editor but off in play-mode.
/// </summary>
public class InEditorLight : MonoBehaviour
{
    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        Debug.Log("You left the in-editor light on before going into play mode. Make sure not to do that when it's build time.");
        gameObject.SetActive(false);
    }
}
