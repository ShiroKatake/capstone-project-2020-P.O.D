using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GreenBar greenBar;
    private int barVal = 100;
    private int curVal = 0;

    // Start is called before the first frame update
    void Start()
    {
        greenBar.SetGreenBarValue(curVal);
        greenBar.setMaxBarValue(barVal);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)){
            ChangeGreen(10);
        }

        else if (Input.GetKeyDown(KeyCode.I)){
            ChangeGreen(-10);
        }
    }

    void ChangeGreen(int val){
        curVal += val;
        greenBar.SetGreenBarValue(curVal);
    }
}
