using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float fadeRate;
    private float targetAlpha;
    private Color curColor;
    private Color newColor;

    // Start is called before the first frame update
    void Start()
    {
        targetAlpha = image.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            curColor = image.color;
            newColor = UnityEngine.Random.ColorHSV(0f,1f,1f,1f,0.5f,1f);
        }
        curColor = Color.Lerp(curColor, newColor, fadeRate*Time.deltaTime);
        image.GetComponent<Image>().color = curColor;
    }
}
