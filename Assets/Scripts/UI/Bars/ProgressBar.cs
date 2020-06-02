using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image image;
    private float barMax = 1;
    [SerializeField] private float curVal = 0;

    private void Awake() {
        image.fillAmount = curVal;
    }

    private void Update() {
        image.fillAmount = curVal / barMax;
    }

    public void SetMax(float val){
        barMax = val;
    }

    public void ChangeValueAdd(float value){
        if (IsBarWithinBounds(curVal + value)){
            curVal += value;
        } else if (value > barMax){
            curVal = barMax;
        }
    }

    public void SetBarValue(float value){
        if (IsBarWithinBounds(value)){
            curVal = value;
        } else if (value > barMax){
            curVal = barMax;
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
