using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatioBars : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private float barMax = 0;
    [SerializeField] private float curVal = 0;

    private float renderBarMax = 0;

    public float CurrentValue {get => curVal;}

    private void Awake() {
        slider.maxValue = barMax;
        slider.value = curVal;
        renderBarMax = barMax;
    }
    public void SetMaxBarValue(float value){
        barMax = value;
        renderBarMax = barMax;
        slider.maxValue = barMax;
    }

    public void SetMaxRenderBarValue(float value){
        renderBarMax = value;
        slider.maxValue = renderBarMax;
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
}