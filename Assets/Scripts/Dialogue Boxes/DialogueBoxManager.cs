using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class for the dialogue boxes.
/// </summary>
public class DialogueBoxManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    [SerializeField] private List<DialogueBox> dialogueBoxList;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    protected Dictionary<string, DialogueBox> dialogueBoxes;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// DialogueBoxManager's singleton public property.
    /// </summary>
    public static DialogueBoxManager Instance { get; protected set; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be more than one DialogueBoxManager.");
        }

        Instance = this;

        foreach (DialogueBox d in dialogueBoxList)
        {
            dialogueBoxes[d.ID] = d;
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves the specified dialogue box if it exists.
    /// </summary>
    /// <param name="id">The ID of the dialogue box to be retrieved.</param>
    /// <returns>The dialogue box that you wanted to retrieve.</returns>
    public DialogueBox GetDialogueBox(string id)
    {
        if (dialogueBoxes.ContainsKey(id))
        {
            return dialogueBoxes[id];
        }
        else
        {
            Debug.LogError($"The dialogue box {id} doesn't exist.");
        }

        return null;
    }
}
