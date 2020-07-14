using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementStatusController : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Game Objects")]
    [SerializeField] private Button button;
    [SerializeField] private Image border;
    [SerializeField] private Image fill;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textBox;

    [Header("Visibility Stats")]
    [SerializeField] private bool visibleOnAwake;
    [SerializeField] private bool flickersIn;
    [SerializeField] private float flickerInSpeed;
    [SerializeField] private int flickerCount;
    [SerializeField] private bool flickerOutOnComplete;

    [Header("Interactability Requirements - Resources")]
    [SerializeField] private bool requiresResources;
    [SerializeField] private int requiredOre;
    [SerializeField] private int requiredPower;
    [SerializeField] private int requiredWater;
    [SerializeField] private int requiredWaste;

    //Non-Serialized Fields------------------------------------------------------------------------

    private float finalBorderOpacity;
    private float finalFillOpacity;
    private float finalImageOpacity;
    private float finalTextOpacity;

    private bool interactable;
    private bool visible;
    private bool finishedFlickeringIn;

    //Public Properties------------------------------------------------------------------------------------------------------------

    //Simple Public Properties---------------------------------------------------------------------

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
            interactable = value;

            if (!requiresResources && button != null && button.interactable != interactable)
            {
                button.interactable = interactable;
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

            if (
                   (border != null && border.enabled != visible)
                || (fill != null && fill.enabled != visible)
                || (image != null && image.enabled != visible)
                || (textBox != null && textBox.enabled != visible)
            )
            {
                bool readyToFlickerIn = false;

                if (border != null)
                {
                    if (!border.enabled && visible && flickersIn)
                    {
                        Color colour = border.color;
                        colour.a = 0;
                        border.color = colour;
                        border.enabled = true;
                        readyToFlickerIn = true;
                    }
                    else
                    {
                        border.enabled = visible;
                    }
                }

                if (fill != null)
                {
                    if (!fill.enabled && visible && flickersIn)
                    {
                        Color colour = fill.color;
                        colour.a = 0;
                        fill.color = colour;
                        fill.enabled = true;
                        readyToFlickerIn = true;
                    }
                    else
                    {
                        fill.enabled = visible;
                    }
                }

                if (image != null)
                {
                    if (!image.enabled && visible && flickersIn)
                    {
                        Color colour = image.color;
                        colour.a = 0;
                        image.color = colour;
                        image.enabled = true;
                        readyToFlickerIn = true;
                    }
                    else
                    {
                        image.enabled = visible;
                    }
                }

                if (textBox != null)
                {
                    if (!textBox.enabled && visible && flickersIn)
                    {
                        Color colour = textBox.color;
                        colour.a = 0;
                        textBox.color = colour;
                        textBox.enabled = true;
                        readyToFlickerIn = true;
                    }
                    else
                    {
                        textBox.enabled = visible;
                    }
                }

                if (readyToFlickerIn)
                {
                    StartCoroutine(FlickerIn());
                }
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
        //Debug.Log($"{this}.button is {button}, {this}.image is {image}");

        if (!visibleOnAwake)
        {
            if (border != null)
            {
                finalBorderOpacity = border.color.a;
                border.enabled = false;
            }

            if (fill != null)
            {
                finalFillOpacity = fill.color.a;
                fill.enabled = false;
            }

            if (image != null)
            {
                finalImageOpacity = image.color.a;
                image.enabled = false;
            }

            if (textBox != null)
            {
                finalTextOpacity = textBox.color.a;
                textBox.enabled = false;
            }
        }
    }

    /// <summary>
    /// Start() is run on the frame when a script is enabled just before any of the Update methods are called for the first time. 
    /// Start() runs after Awake().
    /// </summary>
    private void Start()
    {
        if (button != null && requiresResources)
        {
            StartCoroutine(CheckInteractable());
        }
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Makes the UI element flicker on, or simply fades in if flickerCount == 0.
    /// </summary>
    private IEnumerator FlickerIn()
    {
        //Debug.Log($"{this} has started flickering in");
        int flickers = 0;
        bool borderFinished = (border == null);
        bool fillFinished = (fill == null);
        bool imageFinished = (image == null);
        bool textBoxFinished = (textBox == null);

        yield return null;

        while (flickers < flickerCount)
        {
            do
            {
                yield return null; 
                if (!borderFinished) { borderFinished = UpdateOpacityOfImage(border, true, 1, 0.67f * finalBorderOpacity); }
                if (!fillFinished) { fillFinished = UpdateOpacityOfImage(fill, true, 1, 0.67f * finalFillOpacity); } 
                if (!imageFinished) { imageFinished = UpdateOpacityOfImage(image, true, 1, 0.67f * finalImageOpacity); } 
                if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfTextBox(textBox, true, 1, 0.67f * finalTextOpacity); }
                //Debug.Log($"Incrementing {this}.image.color.a, alpha is {image.color.a}, finished incrementing up to {0.67f * finalOpacity} is {image.color.a >= 0.67f * finalOpacity}");
            }
            while (!borderFinished || !fillFinished || !imageFinished || !textBoxFinished);

            borderFinished = (border == null);
            fillFinished = (fill == null);
            imageFinished = (image == null);
            textBoxFinished = (textBox == null);

            do
            {
                yield return null;
                if (!borderFinished) { borderFinished = UpdateOpacityOfImage(border, false, 1, 0); }
                if (!fillFinished) { fillFinished = UpdateOpacityOfImage(fill, false, 1, 0); }
                if (!imageFinished) { imageFinished = UpdateOpacityOfImage(image, false, 1, 0); }
                if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfTextBox(textBox, false, 1, 0); }

                //Debug.Log($"Decrementing {this}.image.color.a, alpha is {image.color.a}, finished decrementing to 0 is {image.color.a <= 0}");
            }
            while (!borderFinished || !fillFinished || !imageFinished || !textBoxFinished);

            flickers++;
        }

        borderFinished = (border == null);
        fillFinished = (fill == null);
        imageFinished = (image == null);
        textBoxFinished = (textBox == null);

        do
        {
            yield return null;
            if (!borderFinished) { borderFinished = UpdateOpacityOfImage(border, true, 1.25f, finalBorderOpacity); }
            if (!fillFinished) { fillFinished = UpdateOpacityOfImage(fill, true, 1.25f, finalFillOpacity); }
            if (!imageFinished) { imageFinished = UpdateOpacityOfImage(image, true, 1.25f, finalImageOpacity); } 
            if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfTextBox(textBox, true, 1.25f, finalTextOpacity); }
            //Debug.Log($"Incrementing {this}.image.color.a, alpha is {image.color.a}, finished incrementing up to {finalOpacity} is {image.color.a >= finalOpacity}");
        }
        while (!borderFinished || !fillFinished || !imageFinished || !textBoxFinished);

        borderFinished = (border == null);
        fillFinished = (fill == null);
        imageFinished = (image == null);
        textBoxFinished = (textBox == null);

        if (flickerOutOnComplete)
        {
            do
            {
                yield return null; 
                if (!borderFinished) { borderFinished = UpdateOpacityOfImage(border, false, 1.25f, 0); } 
                if (!fillFinished) { fillFinished = UpdateOpacityOfImage(fill, false, 1.25f, 0); } 
                if (!imageFinished) { imageFinished = UpdateOpacityOfImage(image, false, 1.25f, 0); }
                if (!textBoxFinished) { textBoxFinished = UpdateOpacityOfTextBox(textBox, false, 1.25f, 0); }
                //Debug.Log($"Decrementing {this}.image.color.a, alpha is {image.color.a}, finished decrementing down to {0} is {image.color.a <= 0}");
            }
            while (!borderFinished || !fillFinished || !imageFinished || !textBoxFinished);
        }

        finishedFlickeringIn = true;
    }

    /// <summary>
    /// Updates the opacity of the passed image.
    /// </summary>
    /// <param name="i">The image whose opacity is being updated</param>
    /// <param name="increasing">Should its opacity increase?</param>
    /// <param name="speedMultiplier">Multiplies the speed at which the image's opacity updates.</param>
    /// <param name="targetOpacity">The opacity the image should be updating towards</param>
    /// <returns>Whether or not the image has reached the target opacity.</returns>
    private bool UpdateOpacityOfImage(Image i, bool increasing, float speedMultiplier, float targetOpacity)
    {
        float directionMultiplier = (increasing ? 1 : -1);

        if (i.color.a * directionMultiplier < targetOpacity)
        {
            Color colour = i.color;
            colour.a += flickerInSpeed * directionMultiplier * speedMultiplier * Time.deltaTime;
            i.color = colour;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Updates the opacity of the passed text box.
    /// </summary>
    /// <param name="t">The text box whose opacity is being updated</param>
    /// <param name="increasing">Should its opacity increase?</param>
    /// <param name="speedMultiplier">Multiplies the speed at which the text box's opacity updates.</param>
    /// <param name="targetOpacity">The opacity the text box should be updating towards</param>
    /// <returns>Whether or not the text box has reached the target opacity.</returns>
    private bool UpdateOpacityOfTextBox(TextMeshProUGUI t, bool increasing, float speedMultiplier, float targetOpacity)
    {
        float directionMultiplier = (increasing ? 1 : -1);

        if (t.color.a * directionMultiplier < targetOpacity)
        {
            Color colour = t.color;
            colour.a += flickerInSpeed * directionMultiplier * speedMultiplier * Time.deltaTime;
            t.color = colour;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Checks if the button should be interactable.
    /// </summary>
    private IEnumerator CheckInteractable()
    {
        while (true)
        {
            button.interactable = interactable && (!requiresResources ||
                    (
                           requiredOre <= ResourceController.Instance.Ore
                        && requiredPower <= ResourceController.Instance.SurplusPower
                        && requiredWaste <= ResourceController.Instance.SurplusWaste
                        && requiredWater <= ResourceController.Instance.SurplusWater
                    )
                );

            yield return null;
        }
    }
}
