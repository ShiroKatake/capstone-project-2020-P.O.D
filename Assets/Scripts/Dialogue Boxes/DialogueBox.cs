using DG.Tweening;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// An enum for the expressions the AI should display.
/// </summary>
public enum AIExpression
{
    None,
    Happy,
    Neutral,
    Sad,
    Excited,
    Shocked
}

/// <summary>
/// A serializable container for an AI expression and a line of dialogue.
/// </summary>
[Serializable]
public class ExpressionDialoguePair
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField, TextArea(15, 20)] private string dialogue;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The line of dialogue to be displayed.
    /// </summary>
    public string Dialogue { get => dialogue; set => dialogue = value; }
}

/// <summary>
/// A serializable key-value pair of a dialogue key and a set of ExpressionDialoguePairs, because Unity doesn't serialize dictionaries.
/// </summary>
[Serializable]
public class DialogueSet
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [SerializeField] private string key;
    [SerializeField] private List<ExpressionDialoguePair> expressionDialoguePairs;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// The key of the dialogue set.
    /// </summary>
    public string Key { get => key; set => key = value; }

    /// <summary>
    /// The list of expression-dialogue pairs that comprise the dialogue set.
    /// </summary>
    public List<ExpressionDialoguePair> ExpressionDialoguePairs { get => expressionDialoguePairs; set => expressionDialoguePairs = value; }
    //TODO: tidy this up because if images aren't being used, we don't need a class to encapsulate a list I don't think. Don't need the inspector-serializable verificational stuff either
}

/// <summary>
/// Allows other classes to submit dialogue for the dialogue box to display.
/// </summary>
public class DialogueBox : MonoBehaviour
{
    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("ID")]
    [SerializeField] private string id;

    [Header("Components")]
    [SerializeField] private Image background;
    [SerializeField] private Image border;
    [SerializeField] private TextMeshProUGUI textBox;

    [Header("Tween Stats")]
    [SerializeField] private Vector2 offScreenPos;
    [SerializeField] private Vector2 onScreenPos;
    [SerializeField] private float tweenInDuration;
    [SerializeField] private bool tweenOut;
    [SerializeField] private float tweenOutDuration;
    [SerializeField] private bool fadeOut;
    [SerializeField] private float fadeSpeed;

    [Header("Dialogue")]
    [SerializeField] private bool dismissable;
    [SerializeField] private bool appendDialogue;
    [SerializeField] private int lerpTextInterval;

    //Non-Serialized Fields------------------------------------------------------------------------

    private Vector2 originalRectTransformPosition;
    private RectTransform dialogueRectTransform;
    private Vector2 arrowInitialPosition;
    private bool activated;
    private bool clickable;

    private List<DialogueSet> dialogue;
    private Dictionary<string, List<ExpressionDialoguePair>> dialogueDictionary;
    private string currentDialogueKey;
    private string lastDialogueKey;
    private int dialogueIndex;
    private string currentText;
    private string pendingText;
    private string pendingColouredText;
    private int lerpTextMaxIndex;
    private string dialogueStash;

    private char newLineMarker;
    private ColourTag colourTag;
    private bool lerpFinished;

    private string nextDialogueKey;
    private float nextInvokeDelay;
    private bool tweenOutNextDialogueSet;
    private bool fadeOutNextDialogueSet;
    private bool deactivationSubmitted;
    private bool nextDialogueSetReady;

    private bool dialogueReadRegistered;
    private bool dialogueRead;
    private bool deactivating;

    private Player playerInputManager;
    private float dialogueTimer = 0;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Has dialogue been submitted to be displayed and is the dialogue box either moving on-screen or on-screen displaying the submitted dialogue?
    /// </summary>
    public bool Activated { get => activated; }

    /// <summary>
    /// Is the dialogue box on the screen and interactable?
    /// </summary>
    public bool Clickable { get => clickable; }

    /// <summary>
    /// Gets the key of the dialogue set that is currently being displayed.
    /// </summary>
    public string CurrentDialogueSet { get => currentDialogueKey; }

    /// <summary>
    /// Is the dialogue box in the process of moving off-screen?
    /// </summary>
    public bool Deactivating { get => deactivating; }

    /// <summary>
    /// The index of the currently displayed line of dialogue within the current dialogue set.
    /// </summary>
    public int DialogueIndex { get => dialogueIndex; }

    /// <summary>
    /// Whether the dialogue set currently being displayed by this dialogue box has been read.
    /// </summary>
    public bool DialogueRead { get => dialogueRead; }

    /// <summary>
    /// Gets the amount of time the current dialogue set has been displayed.
    /// </summary>
    public float DialogueTimer { get => dialogueTimer; }

    /// <summary>
    /// The ID of this dialogue box to differentiate which dialogue box it is.
    /// </summary>
    public string ID { get => id; }

    /// <summary>
    /// Gets the key of the dialogue set submitted before the currently displayed dialogue set.
    /// </summary>
    public string LastDialogueSet { get => lastDialogueKey; }

    /// <summary>
    /// Is text currently scrolling onto the dialogue box?
    /// </summary>
    public bool LerpingDialogue { get => !lerpFinished; }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        dialogueRectTransform = GetComponent<RectTransform>();
        originalRectTransformPosition = GetComponent<RectTransform>().anchoredPosition;
        lerpFinished = true;
        tweenOut = true;
        fadeOut = false;
        tweenOutNextDialogueSet = true;
        fadeOutNextDialogueSet = false;

        currentDialogueKey = "";
        lastDialogueKey = "";
        dialogueIndex = 0;
        currentText = "";
        pendingText = "";
        pendingColouredText = "";
        lerpTextMaxIndex = 0;
        colourTag = null;
        nextDialogueKey = "";
        nextInvokeDelay = 0;
        deactivationSubmitted = false;
        nextDialogueSetReady = false;
        dialogueRead = false;
        deactivating = false;
        textBox.text = "";
        clickable = false;
        activated = false;
    }

    ///// <summary>
    ///// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    ///// Start() runs after Awake().
    ///// </summary>
    private void Start()
    {
        playerInputManager = PlayerMovementController.Instance.PlayerInputManager;
        newLineMarker = DialogueBoxManager.Instance.NewLineMarker;

        List<string[]> dialogueData = DialogueBoxManager.Instance.GetDialogueData(id);
        dialogueDictionary = new Dictionary<string, List<ExpressionDialoguePair>>();
        dialogue = new List<DialogueSet>(); //For in-editor verification during testing

        if (dialogueData == null)
        {
            Debug.LogError($"{id} could not load its dialogue.");
        }
        else
        {
            foreach (string[] row in dialogueData)
            {
                if (!dialogueDictionary.ContainsKey(row[2]))
                {
                    dialogueDictionary[row[2]] = new List<ExpressionDialoguePair>();
                }

                ExpressionDialoguePair edp = new ExpressionDialoguePair();
                edp.Dialogue = row[3];
                dialogueDictionary[row[2]].Add(edp);

                //For in-editor verification during testing
                DialogueSet ds = null;

                foreach (DialogueSet set in dialogue)
                {
                    if (set.Key == row[2])
                    {
                        ds = set;
                        break;
                    }
                }

                if (ds == null)
                {
                    ds = new DialogueSet();
                    ds.Key = row[2];
                    ds.ExpressionDialoguePairs = new List<ExpressionDialoguePair>();
                    dialogue.Add(ds);
                }

                ds.ExpressionDialoguePairs.Add(edp);
            }
        }
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (clickable)
        {
            dialogueTimer += Time.deltaTime;
            GetInput();
            CheckDialogueRead();
        }
        
        UpdateDialogueBoxState();
        LerpDialogue();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieve any dialogue box-related user input that needs to be accounted for manually in-code.
    /// </summary>
    private void GetInput()
    {
        if (!dialogueReadRegistered)
        {
            dialogueReadRegistered = playerInputManager.GetButtonDown("DialogueRead");
        }
    }

    /// <summary>
    /// Checks if the dialogue has been read and determines what should happen next.
    /// </summary>
    private void CheckDialogueRead()
    {
        if (dismissable && dialogueReadRegistered) //Should check clickable, but that is checked by the enclosing if statement in Update();
        {
            if (dialogueIndex < dialogueDictionary[currentDialogueKey].Count)
            {
                LerpNext();
            }
            else if (activated)
            {
                dialogueRead = true;

                if (tweenOut)
                {
                    lastDialogueKey = currentDialogueKey;
                    currentDialogueKey = "";
                    clickable = false;
                    DeactivateDialogueBox();
                }
            }
        }
    }

    /// <summary>
    /// Progresses the dialogue box's state depending on user input, dialogue submissions, etc. and reports when all of the dialogue has been read.
    /// </summary>
    private void UpdateDialogueBoxState()
    {
        //Activates the dialogue box if dialogue has been submitted to be displayed.
        if (nextDialogueKey != "" && !deactivating)
        {
            if (activated)
            {
                ChangeDialogue(nextDialogueKey);
            }
            else
            {
                StartCoroutine(ActivateDialogueBox(nextDialogueKey, nextInvokeDelay));
            }

            nextDialogueKey = "";
            nextInvokeDelay = 0f;
        }
        else if (deactivationSubmitted && activated && (tweenOut || fadeOut))
        {
            lastDialogueKey = currentDialogueKey;
            currentDialogueKey = "";
            clickable = false;
            DeactivateDialogueBox();
        }

        deactivationSubmitted = false;

        //Triggers the lerping of the next line of dialogue onto the screen, or informs the classes that use the dialogue box that all of the dialogue in the current dialogue set has been read.
        if (nextDialogueSetReady && dialogueDictionary.ContainsKey(currentDialogueKey) && dialogueIndex < dialogueDictionary[currentDialogueKey].Count)
        {
            DisplayNext();
            nextDialogueSetReady = false;
        }
        else
        {
            if (nextDialogueSetReady && (dialogueDictionary.ContainsKey(currentDialogueKey) || dialogueIndex >= dialogueDictionary[currentDialogueKey].Count))
            {
                nextDialogueSetReady = false;

                if (dialogueDictionary.ContainsKey(currentDialogueKey))
                {
                    Debug.Log($"Warning: nextDialogueSetReady was true, but dialogue key {currentDialogueKey} doesn't exist in dialogueDictionary.");
                }
                else
                {
                    Debug.Log($"Warning: nextDialogueSetReady was true and dialogue key {currentDialogueKey} exists, but dialogueIndex {dialogueIndex} is an invalid index, given dialogueDictionary[{currentDialogueKey}].Count ({dialogueDictionary[currentDialogueKey].Count}).");
                }
            }

            if (clickable && dialogueRead)
            {
                RegisterDialogueRead();
            }
        }

        dialogueReadRegistered = false;
    }

    /// <summary>
    /// Lerps dialogue on-to the screen
    /// </summary>
    private void LerpDialogue()
    {
        if (!lerpFinished)
        {
            //Reset variables
            colourTag = null;
            pendingText = "";
            pendingColouredText = "";

            //Get string of new letters to be added
            foreach (char c in currentText.Substring(0, lerpTextMaxIndex))
            {
                if (colourTag == null)
                {
                    //Check for opening colour tag
                    if (DialogueBoxManager.Instance.ColourTags != null && DialogueBoxManager.Instance.ColourTags.Count > 0)
                    {
                        foreach (ColourTag t in DialogueBoxManager.Instance.ColourTags)
                        {
                            if (c == t.OpeningTag)
                            {
                                colourTag = t;
                                break;
                            }
                        }
                    }

                    //Add if not coloured
                    if (colourTag == null)
                    {
                        if (c == newLineMarker)
                        {
                            pendingText += "<br>";
                        }
                        else
                        {
                            pendingText += c;
                        }
                    }
                }
                else
                {
                    //Check for closing colour tag
                    if (c == newLineMarker)
                    {
                        pendingColouredText += "<br>";
                    }
                    else if (c == colourTag.ClosingTag)
                    {
                        pendingText += $"<color={colourTag.ColourName}><b>{pendingColouredText}</b></color>";
                        pendingColouredText = "";
                        colourTag = null;
                    }
                    else
                    {
                        pendingColouredText += c;
                    }
                }            
            }

            //Add if coloured
            if (colourTag != null)
            {
                pendingText += $"<color={colourTag.ColourName}><b>{pendingColouredText}</b></color>";
            }

            //Add all pending text
            textBox.text = dialogueStash + pendingText;

            //Check progress
            if (lerpTextMaxIndex < currentText.Length)// - 1)
            {
                lerpTextMaxIndex = Mathf.Min(lerpTextMaxIndex + lerpTextInterval, currentText.Length);// - 1);
            }
            else
            {
                lerpFinished = true;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Change over the dialogue list----------------------------------------------------------------

    /// <summary>
    /// Submit a custom error message to the dialogue box.
    /// </summary>
    /// <param name="message">The custom error message to display</param>
    /// <param name="delay">How long the dialogue box should wait to display the message.</param>
    public void SubmitErrorMessage(string message, float delay)
    {
        if (id != "Console")
        {
            Debug.LogError($"You should not submit an error message to the dialogue box {id}. Submit it to the dialogue box Console instead.");
        }
        else
        {
            dialogueDictionary["error"] = new List<ExpressionDialoguePair>();
            ExpressionDialoguePair errorMessage = new ExpressionDialoguePair();
            errorMessage.Dialogue = $"<{message}>";
            dialogueDictionary["error"].Add(errorMessage);
            SubmitDialogue("error", delay, false, false);
        }
    }

    /// <summary>
    /// Submit a dialogue set for the dialogue box to display during the next update.
    /// </summary>
    /// <param name="key">The key of the dialogue set to be displayed.</param>
    /// <param name="delay">How long the dialogue box should wait to display the new dialogue set.</param>
    /// <param name="tweenOut">Should the dialogue box tween out on completion of the dialogue set?</param>
    /// <param name="fadeOut">Should the dialogue box fade out on completion of the dialogue set?</param>
    public void SubmitDialogue(string key, float delay, bool tweenOut, bool fadeOut)
    {
        if (dialogueDictionary.ContainsKey(key))
        {
            if (dialogueDictionary[key].Count > 0)
            {
                nextDialogueKey = key;
                nextInvokeDelay = delay;
                tweenOutNextDialogueSet = tweenOut;
                fadeOutNextDialogueSet = fadeOut;
                dialogueRead = false;
            }
            else
            {
                Debug.LogError($"dialogueDictionary[{key}] contains no dialogue set for DialogueBox to display.");
            }
        }
        else
        {
            Debug.LogError($"Dialogue key '{key}' is invalid.");
        }
    }

    /// <summary>
    /// Activates the dialogue box, prompting it to appear on-screen.
    /// </summary>
    /// <param name="key">The key of the dialogue set to be displayed.</param>
    /// <param name="delay">How long the dialogue box should wait to display the new dialogue set.</param>
    IEnumerator ActivateDialogueBox(string key, float delay)
    {
        dialogueIndex = 0;
        lastDialogueKey = currentDialogueKey == "" ? lastDialogueKey : currentDialogueKey;
        currentDialogueKey = key;
        tweenOut = tweenOutNextDialogueSet;
        fadeOut = fadeOutNextDialogueSet;

        activated = true;
        nextDialogueSetReady = false;
        
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        nextDialogueSetReady = true;
        dialogueRectTransform.DOAnchorPos(onScreenPos, tweenInDuration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(
            delegate
            {
                clickable = true;
                dialogueTimer = 0;
            });
    }

    /// <summary>
    /// Changes over the dialogue set to display when the dialogue box is already active.
    /// </summary>
    /// <param name="key">The key of the dialogue set to be displayed.</param>
    private void ChangeDialogue(string key)
    {
        if (dialogueDictionary.ContainsKey(key) && dialogueDictionary[key].Count > 0)
        {
            lastDialogueKey = currentDialogueKey;
            currentDialogueKey = key;
            dialogueIndex = 0;
            tweenOut = tweenOutNextDialogueSet;
            LerpNext();
        }
        else
        {
            Debug.LogError($"dialogueDictionary[{key}] contains no dialogue set for DialogueBox to display.");
        }
    }

    /// <summary>
    /// Displays the next line of dialogue in one hit.
    /// </summary>
    private void DisplayNext()
    {
        lerpFinished = false;

        if (appendDialogue)
        {
            dialogueStash = textBox.text + "<br>";
        }
        else
        {
            textBox.text = "";
        }

        currentText = dialogueDictionary[currentDialogueKey][dialogueIndex].Dialogue;
        dialogueIndex++;
        lerpTextMaxIndex = currentText.Length - 1;
        dialogueTimer = 0;
    }

    /// <summary>
    /// Displays the next line of dialogue by lerping it into the dialogue box
    /// </summary>
    private void LerpNext()
    {
        lerpFinished = false;

        if (appendDialogue)
        {
            dialogueStash = textBox.text + "<br>";
        }
        else
        {
            textBox.text = "";
        }

        currentText = dialogueDictionary[currentDialogueKey][dialogueIndex].Dialogue;
        dialogueIndex++;
        lerpTextMaxIndex = 0;
        dialogueTimer = 0;
    }

    //Progress / Finish Dialogue-------------------------------------------------------------------

    /// <summary>
    /// Called by OnClick or pressing z to register that the player has read the currently displayed dialogue.
    /// </summary>
    public void RegisterDialogueRead()
    {
        dialogueReadRegistered = true;
    }
    
    /// <summary>
    /// Clears the contents of DialogueBox.textBox.
    /// </summary>
    public void ClearDialogue()
    {
        textBox.text = "";
        dialogueStash = "";
    }

    /// <summary>
    /// Lets other classes call for the dialogue box to be deactivated.
    /// </summary>
    public void SubmitDeactivation()
    {
        deactivationSubmitted = true;
    }

    /// <summary>
    /// Tweens the dialogue box off the screen.
    /// </summary>
    private void DeactivateDialogueBox() 
    {
        dialogueTimer = 0;
        deactivating = true;

        if (fadeOut)
        {
            StartCoroutine(FadeOut());
        }

        if (tweenOut)
        {
            TweenOut(tweenOutDuration);
        }
    }

    /// <summary>
    /// Updates the opacity of the dialogue box as it tweens in or out.
    /// </summary>
    /// <param name="fadeIn">Is the dialogue box tweening and fading in?</param>
    private IEnumerator FadeOut()
    {
        bool backgroundFinished = false;
        bool borderFinished = false;
        bool textFinished = false;
        Color backgroundColour = Color.white;
        Color borderColour = Color.white;
        Color textColour = Color.white;
        float deltaTime;

        if (background != null)
        {
            backgroundColour = background.color;
        }
        else
        {
            backgroundFinished = true;
        }

        if (border != null)
        {
            borderColour = border.color;
        }
        else
        {
            borderFinished = true;
        }

        if (textBox != null)
        {
            textColour = textBox.color;
        }
        else
        {
            textFinished = true;
        }

        do
        {
            deltaTime = Time.deltaTime;

            if (!backgroundFinished)
            {
                backgroundColour.a -= fadeSpeed * deltaTime;
                background.color = backgroundColour;

                if (background.color.a <= 0)
                {
                    backgroundFinished = true;
                }
            }

            if (!borderFinished)
            {
                borderColour.a -= fadeSpeed * deltaTime;
                border.color = borderColour;

                if (border.color.a <= 0)
                {
                    borderFinished = true;
                }
            }

            if (!textFinished)
            {
                textColour.a -= fadeSpeed * deltaTime;
                textBox.color = textColour;

                if (textBox.color.a <= 0)
                {
                    textFinished = true;
                }
            }

            yield return null;
        }
        while (!backgroundFinished || !borderFinished || !textFinished);

        if (!tweenOut)
        {
            TweenOut(0);
        }
    }

    /// <summary>
    /// Tweens the dialogue box off the screen.
    /// </summary>
    private void TweenOut(float duration)
    {
        dialogueRectTransform.DOAnchorPos(offScreenPos, duration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(
                delegate
                {
                    //Reset position after tweening
                    textBox.text = "";
                    deactivating = false;
                    activated = false;
                });
    }
}
