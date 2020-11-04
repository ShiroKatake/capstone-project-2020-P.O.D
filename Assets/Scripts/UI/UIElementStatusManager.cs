using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the status (visibile, interactable, fading in/out, tweening) of a UI element and its component pieces.
/// </summary>
public class UIElementStatusManager : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Game Objects")]
    [SerializeField] private Button button;
    [SerializeField] private Image border;
    [SerializeField] private Image fill;
    [SerializeField] private Image image;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Text textBox;

    [Header("Visibility Stats")]
    [SerializeField] private bool visibleOnAwake;

    [Header("Fading / Flickering Stats")]
    [SerializeField] private bool flickersIn;
    [SerializeField] private float flickerInSpeed;
    [SerializeField] private int flickerCount;
    [SerializeField] private bool flickerOutOnComplete;

    [Header("Tweening Stats")]
    [SerializeField] private bool tweensIn;
    [SerializeField] private float tweenDuration;
    [SerializeField] private TweenAnchorManager tutorialAnchorManager;
    [SerializeField] private TweenAnchorManager skipTutorialAnchorManager;
    [SerializeField] private RectTransform finishedAnchor;

    [Header("Interactability")]
    [SerializeField] private bool interactableOnAwake;

    //Non-Serialized Fields------------------------------------------------------------------------

    private float finalBorderOpacity;
    private float finalFillOpacity;
    private float finalImageOpacity;
    private float finalRawImageOpacity;
    private float finalTextOpacity;

    private bool interactable;
    private bool visible;
    private bool finishedFlickeringIn;

    private List<Graphic> graphics;
    private ButtonInteract buttonInteract;

    private RectTransform rectTransform;
    private TweenAnchorManager tweenAnchorManager;
    private AnchorSet tweenAnchorSet;

    //Public Properties------------------------------------------------------------------------------------------------------------

    //Simple Public Properties---------------------------------------------------------------------

    /// <summary>
    /// The button's ButtonInteract component.
    /// </summary>
    public ButtonInteract ButtonInteract { get => buttonInteract; }

    /// <summary>
    /// Has this UI element finished flickering in?
    /// </summary>
    public bool FinishedFlickeringIn { get => finishedFlickeringIn; }

    //Complex Public Properties--------------------------------------------------------------------

    /// <summary>
    /// Is this UI element's button currently interactable?
    /// </summary>
    public bool Interactable
    {
        get
        {
            return interactable;
        }

        set
        {
            interactable = (buttonInteract == null || buttonInteract.InInteractableGameStage) && value;

            if (/*!requiresResources && */button != null /*&& button.interactable != interactable*/)
            {
                button.interactable = interactable;
            }

            if (buttonInteract != null)
            {
                buttonInteract.OnInteractableChanged(button.interactable);
            }
        }
    }

    /// <summary>
    /// Does this UI element have an image component for a border?
    /// </summary>
    public bool HasBorder
    {
        get
        {
            return border != null;
        }
    }

    /// <summary>
    /// Does this UI element have a button component?
    /// </summary>
    public bool HasButton
    {
        get
        {
            return button != null;
        }
    }

    /// <summary>
    /// Does this UI element have an image component for background fill?
    /// </summary>
    public bool HasFill
    {
        get
        {
            return fill != null;
        }
    }

    /// <summary>
    /// Does this UI element have an image component for images?
    /// </summary>
    public bool HasImage
    {
        get
        {
            return image != null;
        }
    }
    
    /// <summary>
    /// Does this UI element have a raw image component for images?
    /// </summary>
    public bool HasRawImage
    {
        get
        {
            return rawImage != null;
        }
    }

    /// <summary>
    /// Does this UI element have a text mesh pro component?
    /// </summary>
    public bool HasTextBox
    {
        get
        {
            return textBox != null;
        }
    }

    /// <summary>
    /// Is this UI element's image currently visible?
    /// </summary>
    public bool Visible
    {
        get
        {
            return visible;
        }

        set
        {
            //Debug.Log($"{this}.Visible accessed");
            visible = value;

            if (!flickersIn)
            {
                finishedFlickeringIn = true;
            }

            bool readyToFlickerIn = false;

            foreach (Graphic g in graphics)
            {
                if (!g.enabled && visible && flickersIn)
                {
                    Color colour = g.color;
                    colour.a = 0;
                    g.color = colour;
                    g.enabled = true;
                    readyToFlickerIn = true;
                }
                else
                {
                    g.enabled = visible;
                }
            }

            if (readyToFlickerIn)
            {
                StartCoroutine(FlickerIn());
            }
            else if (!visible)
            {
                StopCoroutine(FlickerIn());
            }
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Awake() is run when the script instance is being loaded, regardless of whether or not the script is enabled. 
    /// Awake() runs before Start().
    /// </summary>
    private void Awake()
    {
        buttonInteract = GetComponent<ButtonInteract>();
        rectTransform = GetComponent<RectTransform>();

        if (button != null)
        {
            interactable = interactableOnAwake;
            button.interactable = interactableOnAwake;

            buttonInteract = button.GetComponent<ButtonInteract>();

            if (buttonInteract != null)
            {
                buttonInteract.OnInteractableChanged(button.interactable);
            }
        }

        graphics = new List<Graphic>();

        if (border != null)
        {
            graphics.Add(border);

            if (!visibleOnAwake)
            {
                finalBorderOpacity = border.color.a;
                border.enabled = false;
            }            
        }

        if (fill != null)
        {
            graphics.Add(fill);

            if (!visibleOnAwake)
            {
                finalFillOpacity = fill.color.a;
                fill.enabled = false;
            }
        }

        if (image != null)
        {
            graphics.Add(image);

            if (!visibleOnAwake)
            {
                finalImageOpacity = image.color.a;
                image.enabled = false;
            }
        }

        if (rawImage != null)
        {
            graphics.Add(rawImage);

            if (!visibleOnAwake)
            {
                finalRawImageOpacity = rawImage.color.a;
                rawImage.enabled = false;
            }
        }

        if (textBox != null)
        {
            graphics.Add(textBox);

            if (!visibleOnAwake)
            {
                finalTextOpacity = textBox.color.a;
                textBox.enabled = false;
            }
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Makes the UI element flicker on, or simply fades in if flickerCount == 0.
    /// </summary>
    private IEnumerator FlickerIn()
    {
        int flickers = 0;
        bool borderFinished = (border == null);
        bool fillFinished = (fill == null);
        bool imageFinished = (image == null);
        bool rawImageFinished = (rawImage == null);
        bool textBoxFinished = (textBox == null);

        if (tweensIn)
        {
            tweenAnchorManager = (StageManager.Instance.SkipTutorial ? skipTutorialAnchorManager : tutorialAnchorManager);

            if (tweenAnchorManager == null)
            {
                tweensIn = false;
            }
            else
            {
                tweenAnchorSet = tweenAnchorManager.RegisterButton(rectTransform);

                if (tweenAnchorSet != null)
                {
                    rectTransform.parent = tweenAnchorSet.anchor;
                    rectTransform.localPosition = Vector2.zero;
                }
                else
                {
                    Debug.LogError($"{this} needs a not null tween start anchor.");
                }
            }
        }

        yield return null;

        while (flickers < flickerCount)
        {
            do
            {
                yield return null;
                if (!borderFinished) { borderFinished = UpdateOpacityOfGraphic(border, true, 1, 0.67f * finalBorderOpacity); }
                if (!fillFinished) { fillFinished = UpdateOpacityOfGraphic(fill, true, 1, 0.67f * finalFillOpacity); } 
                if (!imageFinished) { imageFinished = UpdateOpacityOfGraphic(image, true, 1, 0.67f * finalImageOpacity); } 
                if (!rawImageFinished) { rawImageFinished = UpdateOpacityOfGraphic(rawImage, true, 1, 0.67f * finalRawImageOpacity); } 
                if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfGraphic(textBox, true, 1, 0.67f * finalTextOpacity); }
            }
            while (!borderFinished || !fillFinished || !imageFinished || !rawImageFinished || !textBoxFinished);

            borderFinished = (border == null);
            fillFinished = (fill == null);
            imageFinished = (image == null);
            rawImageFinished = (rawImage == null);
            textBoxFinished = (textBox == null);

            do
            {
                yield return null;
                if (!borderFinished) { borderFinished = UpdateOpacityOfGraphic(border, false, 1, 0); }
                if (!fillFinished) { fillFinished = UpdateOpacityOfGraphic(fill, false, 1, 0); }
                if (!imageFinished) { imageFinished = UpdateOpacityOfGraphic(image, false, 1, 0); }
                if (!rawImageFinished) { rawImageFinished = UpdateOpacityOfGraphic(rawImage, false, 1, 0); }
                if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfGraphic(textBox, false, 1, 0); }
            }
            while (!borderFinished || !fillFinished || !imageFinished || !rawImageFinished || !textBoxFinished);

            flickers++;
        }

        borderFinished = (border == null);
        fillFinished = (fill == null);
        imageFinished = (image == null);
        rawImageFinished = (rawImage == null);
        textBoxFinished = (textBox == null);

        do
        {
            yield return null;
            if (!borderFinished) { borderFinished = UpdateOpacityOfGraphic(border, true, 1.25f, finalBorderOpacity); }
            if (!fillFinished) { fillFinished = UpdateOpacityOfGraphic(fill, true, 1.25f, finalFillOpacity); }
            if (!imageFinished) { imageFinished = UpdateOpacityOfGraphic(image, true, 1.25f, finalImageOpacity); } 
            if (!rawImageFinished) { rawImageFinished = UpdateOpacityOfGraphic(rawImage, true, 1.25f, finalRawImageOpacity); } 
            if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfGraphic(textBox, true, 1.25f, finalTextOpacity); }
        }
        while (!borderFinished || !fillFinished || !imageFinished || !rawImageFinished || !textBoxFinished);

        if (tweensIn)
        {
            yield return StartCoroutine(TweenIn());
        }

        borderFinished = (border == null);
        fillFinished = (fill == null);
        imageFinished = (image == null);
        rawImageFinished = (rawImage == null);
        textBoxFinished = (textBox == null);

        if (flickerOutOnComplete)
        {
            do
            {
                yield return null; 
                if (!borderFinished) { borderFinished = UpdateOpacityOfGraphic(border, false, 1.25f, 0); } 
                if (!fillFinished) { fillFinished = UpdateOpacityOfGraphic(fill, false, 1.25f, 0); } 
                if (!imageFinished) { imageFinished = UpdateOpacityOfGraphic(image, false, 1.25f, 0); }
                if (!rawImageFinished) { rawImageFinished = UpdateOpacityOfGraphic(rawImage, false, 1.25f, 0); }
                if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfGraphic(textBox, false, 1.25f, 0); }
            }
            while (!borderFinished || !fillFinished || !imageFinished || !rawImageFinished || !textBoxFinished);
        }

        finishedFlickeringIn = true;
    }

    /// <summary>
    /// Updates the opacity of the passed image.
    /// </summary>
    /// <param name="g">The graphic whose opacity is being updated</param>
    /// <param name="increasing">Should its opacity increase?</param>
    /// <param name="speedMultiplier">Multiplies the speed at which the graphic's opacity updates.</param>
    /// <param name="targetOpacity">The opacity the graphic should be updating towards</param>
    /// <returns>Whether or not the graphic has reached the target opacity.</returns>
    private bool UpdateOpacityOfGraphic(Graphic g, bool increasing, float speedMultiplier, float targetOpacity)
    {
        float directionMultiplier = (increasing ? 1 : -1);

        if (g.color.a * directionMultiplier < targetOpacity)
        {
            Color colour = g.color;
            colour.a += flickerInSpeed * directionMultiplier * speedMultiplier * Time.deltaTime;
            g.color = colour;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Tweens the UI element from the chosen anchor position back to its normal position.
    /// </summary>
    private IEnumerator TweenIn()
    {
        while (tweenAnchorManager.SlideAnchors && tweenAnchorSet.anchor.localPosition != tweenAnchorSet.targetLocalPosition)
        {
            yield return null;
        }

        bool tweening = true;
        rectTransform.parent = finishedAnchor;
        tweenAnchorManager.DeRegisterButton(rectTransform);

        rectTransform.DOAnchorPos(Vector2.zero, tweenDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(
                delegate
                {
                    tweening = false;
                });

        while (tweening)
        {
            yield return null;
        }
    }
}
