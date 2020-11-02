﻿using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image image;
    private float barMax = 1;

    public void SetMax(float val){
        barMax = val;
    }

    public void SetBarValue(float value){
        if (IsBarWithinBounds(value)){
			Debug.Log($"Value received: {value}, Max bar: {barMax}");
			image.fillAmount = value / barMax;
        } else if (value > barMax){
			value = barMax;
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
