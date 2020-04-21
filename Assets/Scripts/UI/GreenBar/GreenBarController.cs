using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBarController : MonoBehaviour
{
    public GreenBar greenBarPrimary;
    public GreenBar greenBarSmall1;
    public GreenBar greenBarSmall2;
    public GreenBar greenBarSmall3;
    private int barVal = 100;
    private int curValPrimary = 0;
    private int curValSmall1 = 0;
    private int curValSmall2 = 0;
    private int curValSmall3 = 0;

    // Start is called before the first frame update
    void Start()
    {
        greenBarPrimary.setGreenBarValue(curValPrimary);
        greenBarPrimary.setMaxBarValue(barVal);

        greenBarSmall1.setGreenBarValue(curValSmall1);
        greenBarSmall1.setMaxBarValue(barVal);
        greenBarSmall1.gameObject.SetActive(false);

        greenBarSmall2.setGreenBarValue(curValSmall2);
        greenBarSmall2.setMaxBarValue(barVal);
        greenBarSmall2.gameObject.SetActive(false);

        greenBarSmall3.setGreenBarValue(curValSmall3);
        greenBarSmall3.setMaxBarValue(barVal);
        greenBarSmall3.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)){
            ChangeGreen(10);
        }

        else if (Input.GetKeyDown(KeyCode.U)){
            ChangeGreen(-10);
        }

        else if (Input.GetKeyDown(KeyCode.O)){
            ChangeGreenSmall1(10);
        }

        else if (Input.GetKeyDown(KeyCode.P)){
            ChangeGreenSmall1(-10);
        }

        else if (Input.GetKeyDown(KeyCode.K)){
            ChangeGreenSmall2(10);
        }

        else if (Input.GetKeyDown(KeyCode.L)){
            ChangeGreenSmall2(-10);
        }

        else if (Input.GetKeyDown(KeyCode.N)){
            ChangeGreenSmall3(10);
        }

        else if (Input.GetKeyDown(KeyCode.M)){
            ChangeGreenSmall3(-10);
        }

        else if (Input.GetKeyDown(KeyCode.Space)){
            greenBarSmall1.gameObject.SetActive(true);
            greenBarSmall2.gameObject.SetActive(true);
            greenBarSmall3.gameObject.SetActive(true);
        }

        else if (Input.GetKeyUp(KeyCode.Space)){
            greenBarSmall1.gameObject.SetActive(false);
            greenBarSmall2.gameObject.SetActive(false);
            greenBarSmall3.gameObject.SetActive(false);
        }
    }

//might need to redesign these functions, compact it, its very bad
    void ChangeGreen(int val){
        curValPrimary += val;
        greenBarPrimary.setGreenBarValue(curValPrimary);
    }

    void ChangeGreenSmall1(int val){
        curValSmall1 += val;
        greenBarSmall1.setGreenBarValue(curValSmall1);
    }

    void ChangeGreenSmall2(int val){
        curValSmall2 += val;
        greenBarSmall2.setGreenBarValue(curValSmall2);
    }

    void ChangeGreenSmall3(int val){
        curValSmall3 += val;
        greenBarSmall3.setGreenBarValue(curValSmall3);
    }
}
