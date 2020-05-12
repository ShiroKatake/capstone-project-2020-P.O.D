using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockTest : MonoBehaviour
{
    [SerializeField] private Image clock;
    [SerializeField] private Image clockBG;

    [SerializeField] private Color day;
    [SerializeField] private Color night;

    [SerializeField] private float timmerSpeed;

    private bool isDay;

    private void Awake()
    {
        clock.fillAmount = 1;
        isDay = true;
        clock.color = day;
        clockBG.color = night;
    }

    // Update is called once per frame
    private void Update()
    {
        checkClockReset();
        updateFill();
    }

    private void updateFill(){
        clock.fillAmount -= timmerSpeed;
    }

    private void checkClockReset(){
        if (clock.fillAmount <= 0){
            clock.fillAmount = 1;
            if (isDay){
                //place holder change color here
                clock.color = night;
                clockBG.color = day;

                isDay = !isDay;
            } else {
                //place holder change color here
                clock.color = day;
                clockBG.color = night;

                isDay = !isDay;
            }
        }
    }
}
