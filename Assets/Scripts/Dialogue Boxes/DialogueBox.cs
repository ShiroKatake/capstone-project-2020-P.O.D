using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

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

    //[SerializeField] private AIExpression aiExpression = AIExpression.Neutral;
    [SerializeField, TextArea(15, 20)] private string dialogue;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    ///// <summary>
    ///// The expression that the AI should have when its matching line of dialogue is displayed.
    ///// </summary>
    //public AIExpression AIExpression { get => aiExpression; }

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
    public string Key { get => key; }

    /// <summary>
    /// The list of expression-dialogue pairs that comprise the dialogue set.
    /// </summary>
    public List<ExpressionDialoguePair> ExpressionDialoguePairs { get => expressionDialoguePairs; }
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

    [Header("Text Box")]
    [SerializeField] private TextMeshProUGUI textBox;
    //[SerializeField] private Image aiImage;

    [Header("Tween Stats")]
    [SerializeField] private Vector2 offScreenPos;
    [SerializeField] private Vector2 onScreenPos;
    [SerializeField] private float tweenSpeed;

    //[Header("Available Expressions")]
    //[SerializeField] private AIExpression currentExpression;
    //[SerializeField] private Sprite aiHappy;
    //[SerializeField] private Sprite aiNeutral;
    //[SerializeField] private Sprite aiSad;
    //[SerializeField] private Sprite aiExcited;
    //[SerializeField] private Sprite aiShocked;

    //[Header("Images")]
    //[SerializeField] private Image completeArrow;
    //[SerializeField] private Image continueArrow;

    [Header("Dialogue")]
    [SerializeField] private bool appendDialogue;
    [SerializeField] private int lerpTextInterval;
    [SerializeField] private List<DialogueSet> dialogue;

    //[Header("Objective Buttons")]
    //[SerializeField] private Image countdown;
    //[SerializeField] private Image objButton;

    //Non-Serialized Fields------------------------------------------------------------------------

    [Header("Testing")]

    private Vector2 originalRectTransformPosition;
    private RectTransform dialogueRectTransform;
    private Vector2 arrowInitialPosition;
    [SerializeField] private bool activated;
    [SerializeField] private bool clickable;

    private Dictionary<string, List<ExpressionDialoguePair>> dialogueDictionary;
    private string currentDialogueKey;
    private string lastDialogueKey;
    private int dialogueIndex;
    private string currentText;
    private string pendingText;
    private string pendingColouredText;
    //private int lerpTextMinIndex;
    private int lerpTextMaxIndex;
    private string dialogueStash;

    private char newLineMarker;
    private ColourTag colourTag;
    private bool lerpFinished;

    private string nextDialogueKey;
    private float nextInvokeDelay;
    [SerializeField] private bool tweenOut;
    [SerializeField] private bool tweenOutNextDialogueSet;
    private bool deactivationSubmitted;
    private bool nextDialogueSetReady;

    private bool dialogueRead;
    private bool deactivating;

    //private RectTransform continueArrowTransform;
    //private RectTransform completeArrowTransform;

    private float dialogueTimer = 0;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------

    /// <summary>
    /// Has dialogue been submitted to be displayed and is the dialogue box either moving on-screen or on-screen displaying the submitted dialogue?
    /// </summary>
    public bool Activated { get => activated; }

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

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        //aiImage.sprite = aiNeutral;
        //currentExpression = AIExpression.Neutral;
        //arrowInitialPosition = completeArrow.GetComponent<RectTransform>().anchoredPosition;
        dialogueRectTransform = GetComponent<RectTransform>();
        originalRectTransformPosition = GetComponent<RectTransform>().anchoredPosition;
        dialogueDictionary = new Dictionary<string, List<ExpressionDialoguePair>>();
        lerpFinished = true;
        tweenOut = true;
        tweenOutNextDialogueSet = true;

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
        newLineMarker = DialogueBoxManager.Instance.NewLineMarker;

        foreach (DialogueSet ds in dialogue)
        {
            if (dialogueDictionary.ContainsKey(ds.Key))
            {
                Debug.Log($"DialogueBox has multiple dialogue sets with the dialogue key {ds.Key}. Each dialogue key should be unique.");
            }
            else
            {
                dialogueDictionary[ds.Key] = ds.ExpressionDialoguePairs;
            }
        }
    }

    ///// <summary>
    ///// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    ///// Start() runs after Awake().
    ///// </summary>
    //private void Start()
    //{
    //    //continueArrowTransform = continueArrow.GetComponent<RectTransform>();
    //    //completeArrowTransform = completeArrow.GetComponent<RectTransform>();
    //}

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update() is run every frame.
    /// </summary>
    private void Update()
    {
        if (clickable)
        {
            dialogueTimer += Time.deltaTime;
            //UpdateArrow();
        }

        UpdateDialogueBoxState();
        LerpDialogue();
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------

    ///// <summary>
    ///// Updates the arrow prompting the player to continue / finish reading the dialogue.
    ///// </summary>
    //private void UpdateArrow()
    //{
    //    if (dialogueDictionary.ContainsKey(currentDialogueKey))
    //    {
    //        if (!continueArrow.enabled && dialogueIndex < dialogueDictionary[currentDialogueKey].Count)
    //        {
    //            if (completeArrow.enabled)
    //            {
    //                DOTween.Kill(completeArrowTransform);
    //                completeArrowTransform.anchoredPosition = arrowInitialPosition;
    //                completeArrow.enabled = false;
    //            }

    //            continueArrow.enabled = true;
    //            continueArrowTransform.DOAnchorPosX(arrowInitialPosition.x + 5, 0.3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    //        }
    //        else if (!completeArrow.enabled && dialogueIndex == dialogueDictionary[currentDialogueKey].Count)
    //        {
    //            if (continueArrow.enabled)
    //            {
    //                DOTween.Kill(continueArrowTransform);
    //                continueArrowTransform.anchoredPosition = arrowInitialPosition;
    //                continueArrow.enabled = false;
    //            }

    //            completeArrow.enabled = true;
    //            completeArrowTransform.DOAnchorPosY(arrowInitialPosition.y - 5, 0.3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log($"Cannot display dialogue set {currentDialogueKey}; Dialogue Key {currentDialogueKey} doesn't exist.");
    //    }
    //}

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
        else if (deactivationSubmitted && activated && tweenOut)
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
    /// Submit a dialogue set for the dialogue box to display during the next update.
    /// </summary>
    /// <param name="key">The key of the dialogue set to be displayed.</param>
    /// <param name="delay">How long the dialogue box should wait to display the new dialogue set.</param>
    /// <param name="delay">Should the dialogue box tween out on completion of the dialogue set?</param>
    public void SubmitDialogue(string key, float delay, bool tweenOut)
    {
        //Debug.Log($"{this} received a dialogue submission. TweenOut is {this.tweenOut} but will become {tweenOut}");

        if (dialogueDictionary.ContainsKey(key))
        {
            if (dialogueDictionary[key].Count > 0)
            {
                nextDialogueKey = key;
                nextInvokeDelay = delay;
                tweenOutNextDialogueSet = tweenOut;
                dialogueRead = false;
            }
            else
            {
                Debug.LogError($"dialogueDictionary[{key}] contains no dialogue set for DialogueBox to display.");
            }
        }
        else
        {
            Debug.Log($"Dialogue key '{key}' is invalid.");
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
        //Debug.Log($"Activating {this}, tween out is now {tweenOut}");

        activated = true;
        nextDialogueSetReady = false;
        
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        nextDialogueSetReady = true;

        //countdown.rectTransform.DOAnchorPosY(-18 + 150, tweenSpeed).SetEase(Ease.OutBack);
        //objButton.rectTransform.DOAnchorPosY(-18 + 150, tweenSpeed).SetEase(Ease.OutBack);
        dialogueRectTransform.DOAnchorPos(onScreenPos, tweenSpeed).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(
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
            //Debug.Log($"Changing dialogue of {this}, tween out is now {tweenOut}");
            LerpNext();
        }
        else
        {
            Debug.LogError($"dialogueDictionary[{key}] contains no dialogue set for DialogueBox to display.");
        }
    }

    //Display the next set of content--------------------------------------------------------------

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

        //if (dialogueDictionary[currentDialogueKey][dialogueIndex].AIExpression != currentExpression)
        //{
        //    ChangeAIExpression(dialogueDictionary[currentDialogueKey][dialogueIndex].AIExpression);
        //}
        ////else
        ////{
        ////    Debug.Log($"Keeping dialogue expression as {currentExpression}");
        ////}

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

        //if (dialogueDictionary[currentDialogueKey][dialogueIndex].AIExpression != currentExpression)
        //{
        //    ChangeAIExpression(dialogueDictionary[currentDialogueKey][dialogueIndex].AIExpression);
        //}
        ////else
        ////{
        ////    Debug.Log($"Keeping dialogue expression as {currentExpression}");
        ////}

        dialogueIndex++;
        lerpTextMaxIndex = 0;
        dialogueTimer = 0;
    }

    ///// <summary>
    ///// Updates the AI sprite.
    ///// </summary>
    ///// <param name="expression">The expression that the AI should have. The enum value corresponds to a matching sprite.</param>
    //private void ChangeAIExpression(AIExpression expression)
    //{
    //    //Debug.Log($"Changing AIExpression from {currentExpression} to {expression}");
    //    currentExpression = expression;

    //    switch (expression)
    //    {
    //        case AIExpression.Happy:
    //            aiImage.sprite = aiHappy;
    //            break;
    //        case AIExpression.Neutral:
    //            aiImage.sprite = aiNeutral;
    //            break;
    //        case AIExpression.Sad:
    //            aiImage.sprite = aiSad;
    //            break;
    //        case AIExpression.Excited:
    //            aiImage.sprite = aiExcited;
    //            break;
    //        case AIExpression.Shocked:
    //            aiImage.sprite = aiShocked;
    //            break;
    //    }

    //    //Debug.Log($"AIExpression is now {currentExpression}");
    //}

    //Progress / Finish Dialogue-------------------------------------------------------------------

    /// <summary>
    /// Called by OnClick to register that the player has read the currently displayed dialogue
    /// </summary>
    public void RegisterDialogueRead()
    {
        if (clickable)
        {
            //DOTween.Kill(continueArrowTransform);
            //continueArrowTransform.anchoredPosition = arrowInitialPosition;
            //continueArrow.enabled = false;

            //DOTween.Kill(completeArrowTransform);
            //completeArrowTransform.anchoredPosition = arrowInitialPosition;
            //completeArrow.enabled = false;

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
    /// Tweens the dialogue box off the screen.
    /// </summary>
    private void DeactivateDialogueBox()
    {
        //Debug.Log($"Deactivating {this}");
        dialogueTimer = 0;
        deactivating = true;
        //countdown.rectTransform.DOAnchorPosY(20, tweenSpeed).SetEase(Ease.InBack);
        //objButton.rectTransform.DOAnchorPosY(20, tweenSpeed).SetEase(Ease.InBack);
        dialogueRectTransform.DOAnchorPos(offScreenPos, tweenSpeed).SetEase(Ease.InBack).SetUpdate(true).OnComplete(
            delegate
            {
                //Reset position after tweening
                textBox.text = "";

                deactivating = false;
                activated = false;
            });
    }

    /// <summary>
    /// Lets other classes call for the dialogue box to be deactivated.
    /// </summary>
    public void SubmitDeactivation()
    {
        deactivationSubmitted = true;
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
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="delay"></param>
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
            SubmitDialogue("error", delay, false);
        }
    }
}
