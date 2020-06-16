using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Health health;
    private float barMax = 0;
    private float curVal = 0;
    private UIFadeControl fader;

    public float CurrentValue {get => curVal;}
    public UIFadeControl Fader {get => fader;}

    private void Awake() {
        barMax = health.CurrentHealth;
        curVal = barMax;
        
        slider.maxValue = barMax;
        slider.value = curVal;

        fader = new UIFadeControl(this.GetComponent<CanvasGroup>());
    }

    public void SetMaxBarValue(float value){
        barMax = value;
    }

    public void SetBarValue(float value){
        if (IsBarWithinBounds(value)){
            curVal = value;
            slider.value = curVal;
        }
    }

    public void ChangeValueAdd(float value){
        if (IsBarWithinBounds(curVal + value)){
            curVal += value;
            slider.value = curVal;
        }
    }

    private bool IsBarWithinBounds(float val){
        if (val > barMax || val < 0){
            return false;
        } else {
            return true;
        }
    }

    public bool isTransparent(){
        return fader.isTransparent();
    }
}