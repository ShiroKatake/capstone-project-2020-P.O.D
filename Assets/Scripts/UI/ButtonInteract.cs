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

    private void Awake()
    {
        interactable = true;
        highlighted = false;
        SetNormal();
    }

    public void SetDefault()
    {
        SetNormal();
    }

    private void FadeToColor(Image image, Color color)
    {
        Graphic graphic = image.GetComponent<Graphic>();
        graphic.CrossFadeColor(color, fadeDuration, true, false);
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
    }

    private void SetUninteractable()
    {
        FadeToColor(border, borderUninteractable);
        FadeToColor(fill, fillUninteractable);

        if (icon != null)
        {
            FadeToColor(icon, iconUninteractable);
        }
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
