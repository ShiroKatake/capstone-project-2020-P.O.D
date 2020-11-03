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
    private bool highlighted;
	private bool breakCoroutine;
	Coroutine currentCoroutine;

	private void Awake()
    {
        interactable = true;
        highlighted = false;

		InitializeColors(border, borderNormal);
		InitializeColors(fill, fillNormal);
	}

    public void SetDefault()
    {
        SetNormal();
    }

	public void FadeToColor(Image image, Color color)
	{
		if (gameObject.activeInHierarchy)
		{
			//breakCoroutine = true;
			//breakCoroutine = false;
			if (currentCoroutine != null)
				StopCoroutine(currentCoroutine);
			IEnumerator coroutine = FadeImageToColor(image, color);
			currentCoroutine = StartCoroutine(coroutine);
		}
	}

	private IEnumerator FadeImageToColor(Image image, Color color)
	{
		float currentFade = 0f;
		while (currentFade < fadeDuration)
		{
			if (breakCoroutine)
			{
				breakCoroutine = false;
				yield break;
			}
			currentFade += Time.unscaledDeltaTime;
			image.color = Color.Lerp(image.color, color, currentFade / fadeDuration);
			yield return null;
		}

		image.color = color;
		yield break;
	}

	public void InitializeColors(Image image, Color color)
	{
		image.color = color;
	}

	public void SetNormal()
    {
        highlighted = false;

        if (interactable)
        {
            FadeToColor(border, borderNormal);
            FadeToColor(fill, fillNormal);
		}
	}

    public void SetHighlighted()
    {
        highlighted = true;

        if (interactable)
        {
            FadeToColor(border, borderHighlight);
            FadeToColor(fill, fillHighlight);
        }
    }

    private void SetInteractable()
    {
        if (icon != null)
        {
            FadeToColor(icon, iconInteractable);
        }

        if (highlighted)
        {
            SetHighlighted();
        }
        else
        {
            SetNormal();
        }

		//Debug.Log("Setting to interactable");
    }

    private void SetUninteractable()
    {
        FadeToColor(border, borderUninteractable);
        FadeToColor(fill, fillUninteractable);

        if (icon != null)
        {
            FadeToColor(icon, iconUninteractable);
        }

		//Debug.Log("Setting to UNinteractable");
	}


	public void SayYes()
    {
        print("Button has been pressed");
    }

    public void OnInteractableChanged(bool interactable)
    {
        this.interactable = interactable;

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
