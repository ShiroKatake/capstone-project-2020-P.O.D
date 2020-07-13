using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementStatusController : MonoBehaviour
{
    //Fields-----------------------------------------------------------------------------------------------------------------------------------------

    //Serialized Fields----------------------------------------------------------------------------

    [Header("Visibility Stats")]
    [SerializeField] private bool visibleOnAwake;
    [SerializeField] private bool flickersIn;
    [SerializeField] private float flickerInSpeed;
    [SerializeField] private int flickerCount;
    [SerializeField] private float finalOpacity;
    [SerializeField] private bool flickerOutOnComplete;

    [Header("Interactability Requirements - Resources")]
    [SerializeField] private bool requiresResources;
    [SerializeField] private int requiredOre;
    [SerializeField] private int requiredPower;
    [SerializeField] private int requiredWater;
    [SerializeField] private int requiredWaste;

    //Non-Serialized Fields------------------------------------------------------------------------

    private bool interactable;
    private bool visible;
    private Button button;
    private Image image;
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
    /// Does this UI element have a button component?
    /// </summary>
    public bool IsButton
    {
        get
        {
            return button != null;
        }
    }

    /// <summary>
    /// Does this UI element have an image component?
    /// </summary>
    public bool IsImage
    {
        get
        {
            return image != null;
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

            if (image != null && image.enabled != visible)
            {
                if (!image.enabled && visible && flickersIn)
                {
                    Color colour = image.color;
                    colour.a = 0;
                    image.color = colour;
                    image.enabled = true;
                    StartCoroutine(FlickerIn());
                }
                else
                {
                    image.enabled = visible;
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
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        //Debug.Log($"{this}.button is {button}, {this}.image is {image}");

        if (!visibleOnAwake)
        {
            image.enabled = false;
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
        Color colour = image.color;

        yield return null;

        while (flickers < flickerCount)
        {
            do
            {
                yield return null;
                colour.a += flickerInSpeed * Time.deltaTime;
                image.color = colour;
                //Debug.Log($"Incrementing {this}.image.color.a, alpha is {image.color.a}, finished incrementing up to {0.67f * finalOpacity} is {image.color.a >= 0.67f * finalOpacity}");
            }
            while (image.color.a < 0.67f * finalOpacity);

            do
            {
                yield return null;
                colour.a -= flickerInSpeed * Time.deltaTime;
                image.color = colour;
                //Debug.Log($"Decrementing {this}.image.color.a, alpha is {image.color.a}, finished decrementing to 0 is {image.color.a <= 0}");
            }
            while (image.color.a > 0);

            flickers++;
        }

        do
        {
            yield return null;
            colour.a += flickerInSpeed * 1.25f * Time.deltaTime;
            image.color = colour;
            //Debug.Log($"Incrementing {this}.image.color.a, alpha is {image.color.a}, finished incrementing up to {finalOpacity} is {image.color.a >= finalOpacity}");
        }
        while (image.color.a < finalOpacity);

        if (flickerOutOnComplete)
        {
            do
            {
                yield return null;
                colour.a -= flickerInSpeed * 1.25f * Time.deltaTime;
                image.color = colour;
                //Debug.Log($"Decrementing {this}.image.color.a, alpha is {image.color.a}, finished decrementing down to {0} is {image.color.a <= 0}");
            }
            while (image.color.a > 0);
        }

        finishedFlickeringIn = true;
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
