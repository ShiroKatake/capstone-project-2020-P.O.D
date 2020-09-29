using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBars : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private Health health;

    [SerializeField] private Image fill;
    [SerializeField] private Image border;

    private UITransparencyFader fader;

    public UITransparencyFader Fader {get => fader;}


    private void Awake() {
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;

        fader = new UITransparencyFader(this.GetComponent<CanvasGroup>());
        if (slider.maxValue != slider.value){
            fader.SetVisible();
        } else {
            fader.SetInvisible();
        }
    }

    private void Update() {
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;
        
        if (fader.IsFading()){
            fader.Fade();
        } else if (slider.value >= slider.maxValue && !fader.isTransparent()){
            fader.StartFade(1, 0, 1f);
        } else if (slider.value < slider.maxValue && fader.isTransparent()){
            fader.StartFade(0, 1, 0.3f);
        }
    }
}