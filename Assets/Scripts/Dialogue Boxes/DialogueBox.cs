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

    private Dictionary<string, List<string>> dialogue;
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

    private bool deactivationSubmitted;
    private bool nextDialogueSetReady;
    private Queue<DialogueSubmission> dialogueQueue;

    private bool dialogueReadRegistered;
    private bool dialogueRead;
    private bool deactivating;
    private bool changing;

    private Player playerInputManager;
    private float dialogueTimer = 0;

    private List<Graphic> graphics;

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

        currentDialogueKey = "";
        lastDialogueKey = "";
        dialogueIndex = 0;
        currentText = "";
        pendingText = "";
        pendingColouredText = "";
        lerpTextMaxIndex = 0;
        colourTag = null;
        deactivationSubmitted = false;
        nextDialogueSetReady = false;
        dialogueRead = false;
        deactivating = false;
        changing = false;
        textBox.text = "";
        clickable = false;
        activated = false;

        dialogue = new Dictionary<string, List<string>>();
        dialogueQueue = new Queue<DialogueSubmission>();

        graphics = new List<Graphic>();

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
        playerInputManager = PlayerMovementController.Instance.PlayerInputManager;
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
        if (dialogueQueue.Count > 0 && !deactivating && lerpFinished)
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
        else if (deactivationSubmitted && activated && (tweenOut || fadeOut))
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
            else if (dialogueQueue.Count > 0 && !deactivating && !changing)
            {
                StartCoroutine(ChangeDialogue(dialogueQueue.Dequeue()));
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
            int id = IdGenerator.Instance.GetNextId();
            dialogue[$"error {id}"] = new List<string>() { $"<{message}>" };
            SubmitDialogue($"error {id}", delay, false, false);
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
        if (!appendDialogue && (!lerpFinished || dialogueQueue.Count > 0))
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
    }

    /// <summary>
    /// Activates the dialogue box, prompting it to appear on-screen.
    /// </summary>
    /// <param name="submission">The dialogue submission to have its content displayed.</param>
    private IEnumerator Activate(DialogueSubmission submission)
    {
        Debug.Log($"Activating dialogue box, key is {submission.key}");
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
    /// <param name="submission">The dialogue submission to have its content displayed.</param>
    private IEnumerator ChangeDialogue(DialogueSubmission submission)
    {
        Debug.Log($"Changing dialogue, key is {submission.key}");
        if (dialogue.ContainsKey(submission.key) && dialogue[submission.key].Count > 0)
        {
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
    private void Deactivate() 
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
