using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image image;
    private int barMax = 1;
    [SerializeField] private float curVal = 0;

    private void Awake() {
        image.fillAmount = curVal;
    }

    public void ChangeValue(float value){
        if (IsBarWithinBounds(curVal + value)){
            curVal += value;
        }
    }

    public void SetBarValue(int value){
        if (IsBarWithinBounds(curVal + value)){
            curVal = value;
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
