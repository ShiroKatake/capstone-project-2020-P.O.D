using DG.Tweening;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Allows other classes to submit dialogue for the dialogue box to display.
/// </summary>
public class DialogueBox : MonoBehaviour
{
    /// <summary>
    /// Encapsulates all the data required of a new dialogue submission.
    /// </summary>
    private struct DialogueSubmission
    {
        public string key;
        public float delay;
        public bool tweenOut;
        public bool fadeOut;
    }

    //Private Fields---------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("ID")]
    [SerializeField] private string id;

    [Header("Components")]
    [SerializeField] private Image background;
    [SerializeField] private Image border;
    [SerializeField] private TextMeshProUGUI textBox;
    //[SerializeField] private TextMeshProUGUI debug;

    [Header("Tween Stats")]
    [SerializeField] private Vector2 offScreenPos;
    [SerializeField] private Vector2 onScreenPos;
    [SerializeField] private float tweenInDuration;
    [SerializeField] private float tweenOutDuration;
    [SerializeField] private float fadeSpeed;

    [Header("Dialogue")]
    [SerializeField] private bool dismissable;
    [SerializeField] private bool appendDialogue;
    [SerializeField] private int lerpTextInterval;

    [Header("Logging Dialogue")]
    [SerializeField] private bool logDialogue;
    [SerializeField] private Color logColour;

    [Header("Cull Overflowing Text")]
    [SerializeField] private bool cullOverflow;
    [SerializeField] private int lines;

    //Non-Serialized Fields------------------------------------------------------------------------

    //Components
    private RectTransform rectTransform;

    //Other external classes
    private Player playerInputManager;

    //Collections
    private Dictionary<string, List<string>> dialogue;
    private Queue<DialogueSubmission> dialogueQueue;
    private List<Graphic> graphics;

    //New dialogue status
    private bool acceptingSubmissions;
    private bool nextDialogueSetReady;
    private bool activated;
    private bool changing;
    private bool clickable;

    //Finished dialogue status
    private bool tweenOut;
    private bool fadeOut;
    private bool dialogueReadRegistered;
    private bool dialogueRead;
    private bool deactivationSubmitted;
    private bool deactivating;

    //Dialogue submissions
    private string currentDialogueKey;
    private string lastDialogueKey;

    //Dialogue lerping special characters
    private char newLineMarker;
    private List<ColourTag> colourTags;

    //Dialogue lerping
    private bool lerpFinished;
    private int dialogueIndex;
    private int lerpTextMaxIndex;
    private string dialogueStash;   
    private string currentText;
    private string pendingText;
    private string pendingColouredText;

    //Dialogue timer
    private float dialogueTimer;

    //Logging dialogue
    private string logColourName;

    //Test variable for players spamming LMB or Z
    //private int lastUpdate;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Is this dialogue box currently accepting dialogue set submissions?
    /// </summary>
    public bool AcceptingSubmissions { get => acceptingSubmissions; }

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
    /// Warning: should only be set if a stage needs to progress and there is a chance the progression condition can be satisfied without the dialogue being read or finishing lerping.
    /// </summary>
    public bool DialogueRead { get => dialogueRead; set => dialogueRead = value; }

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
        //Components
        rectTransform = GetComponent<RectTransform>();

        //Collections
        dialogue = new Dictionary<string, List<string>>();
        dialogueQueue = new Queue<DialogueSubmission>();
        graphics = new List<Graphic>();

        //New dialogue status
        acceptingSubmissions = true;
        nextDialogueSetReady = false;
        activated = false;
        changing = false;
        clickable = false;

        //Finished dialogue status
        tweenOut = true;
        fadeOut = false;
        dialogueRead = false;
        deactivationSubmitted = false;
        deactivating = false;

        //Dialogue submissions
        currentDialogueKey = "";
        lastDialogueKey = "";

        //Dialogue lerping special characters
        colourTags = new List<ColourTag>();

        //Dialogue lerping
        lerpFinished = true;
        dialogueIndex = 0;
        lerpTextMaxIndex = 0;
        textBox.text = "";
        currentText = "";
        pendingText = "";
        pendingColouredText = "";

        //Dialogue timer
        dialogueTimer = 0;

        //Dialogue logging
        logColourName = $"#{ColorUtility.ToHtmlStringRGB(logColour)}";

        //Test variable for players spamming LMB or Z
        //lastUpdate = 0;

        //Populating graphics
        if (background != null)
        {
            graphics.Add(background);
        }
        
        if (border != null)
        {
            graphics.Add(border);
        }

        if (textBox != null)
        {
            graphics.Add(textBox);
        }
    }

    ///// <summary>
    ///// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    ///// Start() runs after Awake().
    ///// </summary>
    private void Start()
    {
        playerInputManager = POD.Instance.PlayerInputManager;
        newLineMarker = DialogueBoxManager.Instance.NewLineMarker;

        List<string[]> dialogueData = DialogueBoxManager.Instance.GetDialogueData(id);

        if (dialogueData == null)
        {
            Debug.LogError($"{id} could not load its dialogue.");
        }
        else
        {
            foreach (string[] row in dialogueData)
            {
                if (!dialogue.ContainsKey(row[2]))
                {
                    dialogue[row[2]] = new List<string>();
                }

                dialogue[row[2]].Add(row[3]);
            }
        }
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            if (clickable)
            {
                dialogueTimer += Time.deltaTime;
                GetInput();
                CheckDialogueRead();
            }

            UpdateDialogueBoxState();
            LerpDialogue();
            //CullOverflow();
        }
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Retrieve any dialogue box-related user input that needs to be accounted for manually in-code.
    /// </summary>
    private void GetInput()
    {
        if (!dialogueReadRegistered && playerInputManager.GetButtonDown("DialogueRead"))
        {
            RegisterDialogueRead();
        }
    }

    /// <summary>
    /// Checks if the dialogue has been read and determines what should happen next.
    /// </summary>
    private void CheckDialogueRead()
    {
        if (dismissable && dialogueReadRegistered) //Requires checking clickable, but that is checked by the enclosing if statement in Update();
        {
            if (dialogueIndex < dialogue[currentDialogueKey].Count)
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
                    Deactivate();
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
        if (dialogueQueue.Count > 0 && !deactivating && lerpFinished && !changing)
        {
            if (activated)
            {
                StartCoroutine(ChangeDialogue(dialogueQueue.Dequeue()));
            }
            else
            {
                StartCoroutine(Activate(dialogueQueue.Dequeue()));
            }
        }
        else if (deactivationSubmitted && activated && clickable && (tweenOut || fadeOut))
        {
            lastDialogueKey = currentDialogueKey;
            currentDialogueKey = "";
            clickable = false;
            Deactivate();
        }

        deactivationSubmitted = false;

        //Triggers the lerping of the next line of dialogue onto the screen, or informs the classes that use the dialogue box that all of the dialogue in the current dialogue set has been read.
        if (nextDialogueSetReady && dialogue.ContainsKey(currentDialogueKey) && dialogueIndex < dialogue[currentDialogueKey].Count)
        {
            DisplayNext();
            nextDialogueSetReady = false;
        }
        else
        {
            if (nextDialogueSetReady && (dialogue.ContainsKey(currentDialogueKey) || dialogueIndex >= dialogue[currentDialogueKey].Count))
            {
                nextDialogueSetReady = false;

                if (dialogue.ContainsKey(currentDialogueKey))
                {
                    Debug.Log($"Warning: nextDialogueSetReady was true, but dialogue key {currentDialogueKey} doesn't exist in dialogueDictionary.");
                }
                else
                {
                    Debug.Log($"Warning: nextDialogueSetReady was true and dialogue key {currentDialogueKey} exists, but dialogueIndex {dialogueIndex} is an invalid index, given dialogueDictionary[{currentDialogueKey}].Count ({dialogue[currentDialogueKey].Count}).");
                }
            }
        }

        dialogueReadRegistered = false;
    }

    /// <summary>
    /// Lerps dialogue on-to the screen
    /// </summary>
    private void LerpDialogue()
    {
        if (!lerpFinished && !changing)
        {
            //Reset variables
            //colourTag = null;
            colourTags.Clear();
            pendingText = "";
            pendingColouredText = "";

            //Get string of new letters to be added
            foreach (char c in currentText.Substring(0, lerpTextMaxIndex))
            {
                pendingText += CheckForSpecialCharacters(c, "<br>");
            }

            //Add if coloured
            if (colourTags.Count > 0)
            {
                pendingText += $"</b></color>";
            }

            //Add all pending text
            textBox.text = dialogueStash + pendingText;

            //Check progress
            if (lerpTextMaxIndex < currentText.Length)// - 1)
            {
                lerpTextMaxIndex = Mathf.Min(lerpTextMaxIndex + lerpTextInterval, currentText.Length);// - 1);
            }
            else if (dialogueQueue.Count > 0 && !deactivating)
            {
                StartCoroutine(ChangeDialogue(dialogueQueue.Dequeue()));
            }
            else
            {
                lerpFinished = true;
            }
        }
    }

    /// <summary>
    /// Checks if the passed character is a special character and needs a custom string returned, or is a regular text character that should be returned as is.
    /// </summary>
    /// <param name="c">The character to be assessed.</param>
    /// <param name="newLine">What to return if the character is a new line marker.</param>
    /// <returns>The string to be appended to the text to be displayed.</returns>
    private string CheckForSpecialCharacters(char c, string newLine)
    {
        //Check for new line marker
        if (c == newLineMarker)
        {
            return newLine;
        }
        //Check for closing colour tag
        else if (colourTags.Count > 0 && c == colourTags[colourTags.Count - 1].ClosingTag)
        {
            string result = $"</b></color>";
            colourTags.RemoveAt(colourTags.Count - 1);

            if (colourTags.Count > 0)
            {
                result += $"<color={colourTags[colourTags.Count - 1].ColourName}><b>";
            }

            return result;
        }
        //Check for opening colour tag
        else if (DialogueBoxManager.Instance.ColourTags != null && DialogueBoxManager.Instance.ColourTags.Count > 0)
        {
            foreach (ColourTag t in DialogueBoxManager.Instance.ColourTags)
            {
                if (c == t.OpeningTag)
                {
                    string result = "";

                    if (colourTags.Count > 0)
                    {
                        result += $"</b></color>";
                    }

                    colourTags.Add(t);
                    result += $"<color={colourTags[colourTags.Count - 1].ColourName}><b>";
                    return result;
                }
            }

            //Else just a regular character
            return $"{c}";
        }
        //Else just a regular character
        else
        {
            return $"{c}";
        }
    }

    /// <summary>
    /// Culls any excess lines of text that poke down from off-screen.
    /// </summary>
    private void CullOverflow()
    {
        if (cullOverflow)
        {
            TMP_TextInfo info = textBox.GetTextInfo(textBox.text);
            int cullChars = info.lineInfo[0].characterCount;

            if (info.lineCount > lines)
            {
                dialogueStash = dialogueStash.Substring(cullChars);

                if (dialogueStash.StartsWith("br>"))
                {
                    dialogueStash = dialogueStash.Substring(3);
                }

                if (dialogueStash.StartsWith("r>"))
                {
                    dialogueStash = dialogueStash.Substring(2);
                }

                if (dialogueStash.StartsWith(">"))
                {
                    dialogueStash = dialogueStash.Substring(1);
                }

                textBox.text = dialogueStash + pendingText;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    //Change over the dialogue list----------------------------------------------------------------

    /// <summary>
    /// Submit a custom message to the dialogue box.
    /// </summary>
    /// <param name="message">The custom message to display.</param>
    /// <param name="error">Is the custom message a error message?</param>
    /// <param name="delay">How long the dialogue box should wait to display the message.</param>
    public void SubmitCustomMessage(string message, bool error, float delay)
    {
        if (id != "Console")
        {
            Debug.LogError($"You should not submit an error message to the dialogue box {id}. Submit it to the dialogue box Console instead.");
        }
        else
        {
            int num = IdGenerator.Instance.GetNextId();
            string id = (error ? $"error {num}" : $"message {num}");

            dialogue[id] = new List<string>() { message };
            SubmitDialogue(id, delay, false, false);
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
        //Debug.Log($"{this} received dialogue submission with key {key} during update {lastUpdate}. appendDialogue: {appendDialogue}, lerpFinished: {lerpFinished}, dialogueRead: {dialogueRead}, dialogueQueue.Count: {dialogueQueue.Count}");

        if (!appendDialogue && ((!lerpFinished && !dialogueRead) || dialogueQueue.Count > 0))
        {
            Debug.LogError($"{this} already has dialogue to display and is not set to append additional dialogue to what it already displays. Submission of the dialogue set with key {key} rejected.");
            return;
        }

        if (!dialogue.ContainsKey(key))
        {
            Debug.LogError($"Dialogue key '{key}' is invalid.");
            return;
        }

        if (dialogue[key].Count == 0)
        {
            Debug.LogError($"dialogueDictionary[{key}] contains no dialogue set for DialogueBox to display.");
            return;
        }

        DialogueSubmission ds = new DialogueSubmission();
        ds.key = key;
        ds.delay = delay;
        ds.tweenOut = tweenOut;
        ds.fadeOut = fadeOut;
        dialogueQueue.Enqueue(ds);
        dialogueRead = false;
        acceptingSubmissions = appendDialogue;
    }

    /// <summary>
    /// Activates the dialogue box, prompting it to appear on-screen.
    /// </summary>
    /// <param name="submission">The dialogue submission to have its content displayed.</param>
    private IEnumerator Activate(DialogueSubmission submission)
    {
        //Debug.Log($"{this}.Activate() called during update {lastUpdate} with dialogue set with key {submission.key}");
        acceptingSubmissions = true;
        dialogueIndex = 0;
        lastDialogueKey = currentDialogueKey == "" ? lastDialogueKey : currentDialogueKey;
        currentDialogueKey = submission.key;
        tweenOut = submission.tweenOut;
        fadeOut = submission.fadeOut;

        activated = true;
        nextDialogueSetReady = false;
        
        if (submission.delay > 0)
        {
            yield return new WaitForSeconds(submission.delay);
        }

        nextDialogueSetReady = true;
        rectTransform.DOAnchorPos(onScreenPos, tweenInDuration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(
            delegate
            {
                clickable = true;
                dialogueTimer = 0;
            });
    }

    /// <summary>
    /// Changes over the dialogue set to display when the dialogue box is already active.
    /// </summary>
    /// <param name="submission">The dialogue submission to have its content displayed.</param>
    private IEnumerator ChangeDialogue(DialogueSubmission submission)
    {
        //Debug.Log($"{this}.ChangeDialogue() called during update {lastUpdate} with dialogue set with key {submission.key}");

        if (dialogue.ContainsKey(submission.key) && dialogue[submission.key].Count > 0)
        {
            while (dialogueIndex < dialogue[currentDialogueKey].Count)
            {
                LogDialogue();
                dialogueIndex++;
            }

            acceptingSubmissions = true;
            changing = true;
            lastDialogueKey = currentDialogueKey;
            currentDialogueKey = submission.key;
            dialogueIndex = 0;
            tweenOut = submission.tweenOut;
            fadeOut = submission.fadeOut;

            if (submission.delay > 0)
            {
                yield return new WaitForSeconds(submission.delay);
            }

            LerpNext();
            changing = false;
        }
        else
        {
            Debug.LogError($"dialogueDictionary[{submission.key}] contains no dialogue set for DialogueBox to display.");
        }
    }

    /// <summary>
    /// Adds the currently displaying line of dialogue to the dialogue log (or the most recently added line if it's an appending dialogue box).
    /// </summary>
    private void LogDialogue()
    {
        string raw = dialogue[currentDialogueKey][dialogueIndex];

        if (logDialogue && raw != "" && raw != " ")
        {
            //Reset variables
            string result = $"<color={logColourName}>" + (PauseMenuManager.Instance.DialogueLog.text != "" ? $"<br>{id}: " : $"{id}: ");
            colourTags.Clear();

            //Get string of new letters to be added
            foreach (char c in raw)
            {
                result += CheckForSpecialCharacters(c, $"<br>{id}: ");
            }

            result += "</color>";
            PauseMenuManager.Instance.DialogueLog.text += $"{result}";
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

        LogDialogue();
        currentText = dialogue[currentDialogueKey][dialogueIndex];
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

        LogDialogue();
        currentText = dialogue[currentDialogueKey][dialogueIndex];
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
        //Debug.Log($"{this}.RegisterDialogueRead() called during update {lastUpdate}");
        dialogueReadRegistered = true;
    }
    
    /// <summary>
    /// Clears the contents of DialogueBox.textBox.
    /// </summary>
    public void ClearDialogue()
    {
        textBox.text = "";
        dialogueStash = "";

        if (appendDialogue)
        {
            SubmitDialogue("blank", 0, false, false);
        }
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
    private void Deactivate() 
    {
        dialogueTimer = 0;
        deactivating = true;

        if (lastDialogueKey != "")
        {
            while (dialogueIndex < dialogue[lastDialogueKey].Count)
            {
                LogDialogue();
                dialogueIndex++;
            }
        }

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
        float deltaTime;
        bool finished;

        do
        {
            deltaTime = Time.deltaTime;
            finished = true;

            foreach (Graphic g in graphics)
            {
                if (g.color.a > 0)
                {
                    finished = false;
                    Color colour = g.color;
                    colour.a -= fadeSpeed * deltaTime;
                    g.color = colour;
                }
            }
            
            yield return null;
        }
        while (!finished);

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
        rectTransform.DOAnchorPos(offScreenPos, duration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(
                delegate
                {
                    //Reset position after tweening
                    textBox.text = "";
                    deactivating = false;
                    activated = false;
                });
    }
}
