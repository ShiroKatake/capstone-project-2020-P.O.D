using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteract : MonoBehaviour
{
    [Header("Button Options")]
    [SerializeField] private float fadeDuration;

    [Header("Button Images")]
    [SerializeField] private Image border;
    [SerializeField] private Image fill;
    [SerializeField] private Image icon;

    [Header("Border Colors")]
    [SerializeField] private Color borderNormal;
    [SerializeField] private Color borderHighlight;
    [SerializeField] private Color borderUninteractable;

    [Header("Fill Colors")]
    [SerializeField] private Color fillNormal;
    [SerializeField] private Color fillHighlight;
    [SerializeField] private Color fillUninteractable;

    [Header("Image Colors")]
    [SerializeField] private Color iconInteractable;
    [SerializeField] private Color iconUninteractable;

    private bool interactable;
    private bool inInteractableGameStage;
    private bool highlighted;

	Coroutine currentFillCoroutine;
	Coroutine currentBorderCoroutine;
    Coroutine currentIconCoroutine;


    public bool InInteractableGameStage
    {
        get
        {
            return inInteractableGameStage;
        }

        set
        {
            inInteractableGameStage = value;
        }
    }

    private void Awake()
    {
        interactable = true;
        inInteractableGameStage = true;
        highlighted = false;

		InitializeColors(border, borderNormal);
		InitializeColors(fill, fillNormal);
	}

    public void SetDefault()
    {
        SetNormal();
    }

	public void StartCoroutineFadeToColour(Image image, Color colour)
	{
		if (gameObject.activeInHierarchy)
		{
            if (image == fill)
            {
                if (currentFillCoroutine != null) StopCoroutine(currentFillCoroutine);
                IEnumerator coroutine = FadeToColour(image, colour);
                currentFillCoroutine = StartCoroutine(coroutine);
            }
            else if (image == border)
            {
                if (currentBorderCoroutine != null) StopCoroutine(currentBorderCoroutine);
                IEnumerator coroutine = FadeToColour(image, colour);
                currentBorderCoroutine = StartCoroutine(coroutine);
            }
            else if (image == icon)
            {
                if (currentIconCoroutine != null) StopCoroutine(currentIconCoroutine);
                IEnumerator coroutine = FadeToColour(image, colour);
                currentIconCoroutine = StartCoroutine(coroutine);
            }
        }
	}

	private IEnumerator FadeToColour(Image image, Color colour)
	{
		float currentFade = 0f;
        Color startColour = image.color;

        while (currentFade < fadeDuration)
		{
			currentFade += Time.unscaledDeltaTime;
			image.color = Color.Lerp(startColour, colour, currentFade / fadeDuration);
			yield return null;
		}

		image.color = colour;

        if (image == fill)
        {
            if (currentFillCoroutine != null) currentFillCoroutine = null;
        }
        else if (image == border)
        {
            if (currentBorderCoroutine != null) currentBorderCoroutine = null;
        }
        else if (image == icon)
        {
            if (currentIconCoroutine != null) currentIconCoroutine = null;
        }

        yield break;
	}

	public void InitializeColors(Image image, Color color)
	{
		image.color = color;
	}

	public void SetNormal()
    {
        //Debug.Log($"{this}.ButtonInteract.SetNormal()");
        highlighted = false;

        if (interactable)
        {
            StartCoroutineFadeToColour(border, borderNormal);
            StartCoroutineFadeToColour(fill, fillNormal);
		}
	}

    public void SetHighlighted()
    {
        //Debug.Log($"{this}.ButtonInteract.SetHighlighted()");
        highlighted = true;

        if (interactable)
        {
            StartCoroutineFadeToColour(border, borderHighlight);
            StartCoroutineFadeToColour(fill, fillHighlight);
        }
    }

    private void SetInteractable()
    {
        //Debug.Log($"{this}.ButtonInteract.SetInteractable()");

        if (icon != null)
        {
            StartCoroutineFadeToColour(icon, iconInteractable);
        }

        if (highlighted)
        {
            SetHighlighted();
        }
        else
        {
            SetNormal();
        }
    }

    private void SetUninteractable()
    {
        //Debug.Log($"{this}.ButtonInteract.SetUnInteractable()");

        StartCoroutineFadeToColour(border, borderUninteractable);
        StartCoroutineFadeToColour(fill, fillUninteractable);

        if (icon != null)
        {
            StartCoroutineFadeToColour(icon, iconUninteractable);
        }
	}

    public void OnInteractableChanged(bool interactable)
    {
        this.interactable = interactable && inInteractableGameStage;

        if (interactable)
        {
            SetInteractable();
        }
        else
        {
            SetUninteractable();
        }
    }
}
