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

    [Header("Border Colors")]
    [SerializeField] private Color borderNormal;
    [SerializeField] private Color borderHighlight;
    [SerializeField] private Color borderPressed;
    [SerializeField] private Color borderSelect;
    [SerializeField] private Color borderDiabled;

    [Header("Fill Colors")]
    [SerializeField] private Color fillNormal;
    [SerializeField] private Color fillHighlight;
    [SerializeField] private Color fillPressed;
    [SerializeField] private Color fillSelect;
    [SerializeField] private Color fillDiabled;

    private void Awake() {
        BorderNormalise();
        FillNormalise();
    }

    private void FadeToColor(Image image, Color color){
        Graphic graphic = image.GetComponent<Graphic>();
        graphic.CrossFadeColor(color, fadeDuration, true, true);
    }

    public void BorderNormalise(){
        FadeToColor(border, borderNormal);
    }
    public void BorderHighlighted(){
        FadeToColor(border, borderHighlight);
    }

    public void FillNormalise(){
        FadeToColor(fill, fillNormal);
    }
    public void FillHighlighted(){
        FadeToColor(fill, fillHighlight);
    }

    public void SayYes(){
        print("Button has been pressed");
    }
}
