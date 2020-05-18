using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorManager : MonoBehaviour
{
    [SerializeField] private Image UIBackground;
    [SerializeField] private Image UIForeground;

    [SerializeField] private Color alienAttackBackground;
    [SerializeField] private Color alienAttackForeground;
    [SerializeField] private Color backgroundDay;
    [SerializeField] private Color backgroundNight;
    [SerializeField] private Color foregroundDay;
    [SerializeField] private Color foregroundNight;
    
    [SerializeField] private float duration = 0;
    private float eDuration = 0.5f;

    private Color backgroundCurColor;
    private Color eBackgroundCurColor;
    private Color backgroundNewColor;
    private Color eBackgroundNewColor;
    private Color foregroundCurColor;
    private Color eForegroundCurColor;
    private Color foregroundNewColor;
    private Color eForegroundNewColor;

    private bool alienPhaseOne = false;
    private bool alienPhaseTwo = false;
    private float t = 0;
    private float et = 0;

    public static UIColorManager Instance {get; protected set;}

    private void Awake() {
        if (Instance != null)
        {
            Debug.LogError("There should never be 2 or more Color Managers.");
        }

        Instance = this;

        backgroundCurColor = backgroundDay;
        foregroundCurColor = foregroundDay;
        backgroundNewColor = backgroundDay;
        foregroundNewColor = foregroundDay;

        UIBackground.GetComponent<Image>().color = backgroundCurColor;
        UIForeground.GetComponent<Image>().color = foregroundCurColor;

    }

    public void ColorUpdate() {
        if (Player.Instance.RewiredPlayer.GetButton("EnemyAttack") && !alienPhaseOne && !alienPhaseTwo){
            eBackgroundCurColor = backgroundCurColor;
            eForegroundCurColor = foregroundCurColor;

            eBackgroundNewColor = alienAttackBackground;
            eForegroundCurColor = alienAttackForeground;

            alienPhaseOne = true;

            print("ATTTTAACKKKKK!!!!");
        }

        if (t<1){
            t += Time.deltaTime/duration;
        }

        //for smooth transition replace first cur color field with starting color field
        backgroundCurColor = Color.Lerp(backgroundCurColor, backgroundNewColor, t);
        foregroundCurColor = Color.Lerp(foregroundCurColor, foregroundNewColor, t);

        if (alienPhaseOne || alienPhaseTwo){
            if (et<1){
                et += Time.deltaTime/eDuration;
            }

            eBackgroundCurColor = Color.Lerp(eBackgroundCurColor, eBackgroundNewColor, et);
            eForegroundCurColor = Color.Lerp(eForegroundCurColor, eForegroundNewColor, et);
            
            UIBackground.GetComponent<Image>().color = eBackgroundCurColor;
            UIForeground.GetComponent<Image>().color = eForegroundCurColor;

        } else {
            UIBackground.GetComponent<Image>().color = backgroundCurColor;
            UIForeground.GetComponent<Image>().color = foregroundCurColor;
        }

        if (et >= 1 && alienPhaseOne){
            alienPhaseOne = false;
            alienPhaseTwo = true;
            et = 0;

            eBackgroundNewColor = backgroundCurColor;
            eForegroundNewColor = foregroundNewColor;
        } else if (et >= 1 && alienPhaseTwo) {
            alienPhaseTwo = false;
            et = 0;
        }

        
    }

    public void SetNight(){
        backgroundCurColor = backgroundDay;
        foregroundCurColor = foregroundDay;
        backgroundNewColor = backgroundNight;
        foregroundNewColor = foregroundNight;
        t = 0;
    }

    public void SetDay(){
        backgroundCurColor = backgroundNight;
        foregroundCurColor = foregroundNight;
        backgroundNewColor = backgroundDay;
        foregroundNewColor = foregroundDay;
        t = 0;
    }
}
