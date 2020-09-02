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

    private float opacityValue;


    private void Awake() {
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;
        if (slider.maxValue != slider.value){
            opacityValue = 1f;
            SetVisible();
        } else {
            opacityValue = 0f;
            SetInvisible();
        }
    }

    private void Update() {
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;
        
        if (slider.value >= slider.maxValue && opacityValue != 0){
            FadeOut();
        } else if (slider.value < slider.maxValue && opacityValue != 1){
            Debug.Log("Starting to Fade In");
            FadeIn();
        }
    }

    private void FadeOut(){
        opacityValue = Mathf.Lerp(opacityValue, 0f, 2f * Time.deltaTime);
        Color tmpClr1 = fill.color;
        Color tmpClr2 = border.color;
        tmpClr1.a = opacityValue;
        tmpClr2.a = opacityValue;
        fill.color = tmpClr1;
        border.color = tmpClr2;
    }

    private void FadeIn(){
        opacityValue = Mathf.Lerp(opacityValue, 1f, 3f * Time.deltaTime);
        Color tmpClr1 = fill.color;
        Color tmpClr2 = border.color;
        tmpClr1.a = opacityValue;
        tmpClr2.a = opacityValue;
        fill.color = tmpClr1;
        border.color = tmpClr2;
    }

    private void SetInvisible(){
        Color tmpClr1 = fill.color;
        Color tmpClr2 = border.color;
        tmpClr1.a = 0f;
        tmpClr2.a = 0f;
        fill.color = tmpClr1;
        border.color = tmpClr2;
    }

    private void SetVisible(){
        Color tmpClr1 = fill.color;
        Color tmpClr2 = border.color;
        tmpClr1.a = 1f;
        tmpClr2.a = 1f;
        fill.color = tmpClr1;
        border.color = tmpClr2;
    }
}