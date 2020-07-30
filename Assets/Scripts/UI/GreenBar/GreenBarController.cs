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

    // Start is called before the first frame update
    // thinking about putting all nessassary green bars into an array to make referencing easier
    void Start()
    {
        greenBarPrimary.SetGreenBarValue(0);
        greenBarPrimary.SetMaxBarValue(100);

        greenBarSmall1.SetGreenBarValue(0);
        greenBarSmall1.SetMaxBarValue(100);
        greenBarSmall1.gameObject.SetActive(false);

        greenBarSmall2.SetGreenBarValue(0);
        greenBarSmall2.SetMaxBarValue(100);
        greenBarSmall2.gameObject.SetActive(false);

        greenBarSmall3.SetGreenBarValue(0);
        greenBarSmall3.SetMaxBarValue(100);
        greenBarSmall3.gameObject.SetActive(false);
    }

    // Update is called once per frame
    // thinking about using a switch case statement here...
    void Update()
    {
        if (!PauseMenuManager.Paused)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ChangeGreen(10);
            }

            else if (Input.GetKeyDown(KeyCode.U))
            {
                ChangeGreen(-10);
            }

            else if (Input.GetKeyDown(KeyCode.O))
            {
                ChangeGreenSmall1(10);
            }

            else if (Input.GetKeyDown(KeyCode.P))
            {
                ChangeGreenSmall1(-10);
            }

            else if (Input.GetKeyDown(KeyCode.K))
            {
                ChangeGreenSmall2(10);
            }

            else if (Input.GetKeyDown(KeyCode.L))
            {
                ChangeGreenSmall2(-10);
            }

            else if (Input.GetKeyDown(KeyCode.N))
            {
                ChangeGreenSmall3(10);
            }

            else if (Input.GetKeyDown(KeyCode.M))
            {
                ChangeGreenSmall3(-10);
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            {
                greenBarSmall1.gameObject.SetActive(true);
                greenBarSmall2.gameObject.SetActive(true);
                greenBarSmall3.gameObject.SetActive(true);
            }

            else if (Input.GetKeyUp(KeyCode.Space))
            {
                greenBarSmall1.gameObject.SetActive(false);
                greenBarSmall2.gameObject.SetActive(false);
                greenBarSmall3.gameObject.SetActive(false);
            }
        }
    }

//might need to redesign these functions, compact it, its very bad
// works better once using an array to store all green bars, maybe...
    void ChangeGreen(int val){
        greenBarPrimary.ChangeValue(val);
    }

    void ChangeGreenSmall1(int val){
        greenBarSmall1.ChangeValue(val);
    }

    void ChangeGreenSmall2(int val){
        greenBarSmall2.ChangeValue(val);
    }

    void ChangeGreenSmall3(int val){
        greenBarSmall3.ChangeValue(val);
    }
}
