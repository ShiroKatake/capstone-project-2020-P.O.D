using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The definition of tags for colouring lines of dialogue differently to the standard dialogue box text colour.
/// </summary>
[Serializable]
public class ColourTag
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private char openingTag;
    [SerializeField] private char closingTag;
    [SerializeField] private Color colour;

    //Non-Serialized Fields------------------------------------------------------------------------

    private string colourName;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The character of the tag opening character, equivalent to XML's "<".
    /// </summary>
    public char OpeningTag { get => openingTag; }

    /// <summary>
    /// The character of the tag closing character, equivalent to XML's ">".
    /// </summary>
    public char ClosingTag { get => closingTag; }

    /// <summary>
    /// The colour that the text between the opening and closing tag characters should be.
    /// </summary>
    public Color Colour { get => colour; }

    /// <summary>
    /// The name of the colour that the text should be.
    /// </summary>
    public string ColourName { get => colourName; set => colourName = value; }
}

/// <summary>
/// A manager class for the dialogue boxes.
/// </summary>
public class DialogueBoxManager : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  

    //Serialized Fields----------------------------------------------------------------------------                                                    

    //[SerializeField] private string dialogueFilename;
    [SerializeField] private TextAsset dialogue;
    [SerializeField] private char newLineMarker;
    [SerializeField] private List<ColourTag> colourTags;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    

    private Dictionary<string, DialogueBox> dialogueBoxes;
    private Dictionary<string, List<string[]>> dialogueData;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Singleton Public Property--------------------------------------------------------------------                                                    

    /// <summary>
    /// DialogueBoxManager's singleton public property.
    /// </summary>
    public static DialogueBoxManager Instance { get; protected set; }

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The list of colour tags used by dialogue boxes.
    /// </summary>
    public List<ColourTag> ColourTags { get => colourTags; }

    /// <summary>
    /// The character that denotes a new line.
    /// </summary>
    public char NewLineMarker { get => newLineMarker; }

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

        GetDialogueBoxes();
        LoadDialogueData();
        NameColourTags();
    }

    /// <summary>
    /// Gets all dialogue boxes' DialogueBox components and stores them in a dictionary.
    /// </summary>
    private void GetDialogueBoxes()
    {
        dialogueBoxes = new Dictionary<string, DialogueBox>();

        foreach (DialogueBox d in GetComponentsInChildren<DialogueBox>())
        {
            dialogueBoxes[d.ID] = d;
        }
    }

    /// <summary>
    /// Loads dialogue data from file and groups it by the ID of the dialogue box it belongs to.
    /// </summary>
    private void LoadDialogueData()
    {
        string[] data = dialogue.text.Split(new char[] { '\n' });
        dialogueData = new Dictionary<string, List<string[]>>();

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { '\t' });
            //Debug.Log($"Read row {row} with length {row.Length}");

            if (row.Length == 3)
            {
                if (!dialogueData.ContainsKey(row[0]))
                {
                    dialogueData[row[0]] = new List<string[]>();
                }

                dialogueData[row[0]].Add(row);
            }
        }
    }

    /// <summary>
    /// Sets all colour tag objects' names based on the hexadecimal value of their colours. 
    /// </summary>
    private void NameColourTags()
    {
        foreach (ColourTag c in colourTags)
        {
            c.ColourName = $"#{ColorUtility.ToHtmlStringRGB(c.Colour)}";

            if (c.OpeningTag == newLineMarker || c.ClosingTag == newLineMarker)
            {
                Debug.LogError($"The character {newLineMarker} is reserved for denoting a new line. It cannot be the opening or closing tag of the colour tag for {c.ColourName} (hex: {ColorUtility.ToHtmlStringRGB(c.Colour)}).");
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves the raw data of a dialogue box's dialogue.
    /// </summary>
    /// <param name="id">The ID of the dialogue box retrieving its data.</param>
    /// <returns>The dialogue box's raw dialogue data.</returns>
    public List<string[]> GetDialogueData(string id)
    {
        if (dialogueData.ContainsKey(id))
        {
            return dialogueData[id];
        }
        else
        {
            Debug.LogError($"DialogueBoxManager.dialogueData does not contain the key {id}. Double check the key passed and the contents of {dialogue.name}.");
            return null;
        }
    }

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
