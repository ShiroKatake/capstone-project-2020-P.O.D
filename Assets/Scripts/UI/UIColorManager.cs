using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorManager : SerializableSingleton<UIColorManager>
{
    [SerializeField] private List<Image> UIBackgrounds = new List<Image>();
    [SerializeField] private List<Image> UIBorders = new List<Image>();

    [SerializeField] private Color alienAttackBackground;
    [SerializeField] private Color alienAttackBorder;
    [SerializeField] private Color backgroundDay;
    [SerializeField] private Color backgroundNight;
    [SerializeField] private Color borderDay;
    [SerializeField] private Color borderNight;
    
    [SerializeField] private float duration = 0;
    [SerializeField] private float eDuration = 0.5f;
    [SerializeField] private float timeBetweenAttacks = 5;

    private Color backgroundCurSetColor;
    private Color eBackgroundCurSetColor;
    private Color backgroundNewColor;
    private Color eBackgroundNewColor;
    private Color borderCurSetColor;
    private Color eborderCurSetColor;
    private Color borderNewColor;
    private Color eBorderNewColor;

    private Color backgroundCurColor;
    private Color borderCurColor;

    private bool alienPhaseOne = false;
    private bool alienPhaseTwo = false;
    private bool beingAttacked = false;
    private bool canFlash = false;
    private float timeSinceLastAttack;
    private float t = 0;
    private float transistionTimeStart = 0;

    private float et = 0;
    private float eTransistionTimeStart = 0;

    protected override void Awake() {
        base.Awake();
        backgroundCurSetColor = backgroundDay;
        borderCurSetColor = borderDay;
        backgroundNewColor = backgroundDay;
        borderNewColor = borderDay;
        timeSinceLastAttack = Time.time;

        SetBackgroundColor(backgroundCurSetColor);
		SetBorderColor(borderCurSetColor);

    }

    public void ColorUpdate() {
        if (t < 1){
            t = (Time.time - transistionTimeStart)/duration;
            
            if (t >= 1){
                t = 1;
            }
        }

        if ((Time.time - timeSinceLastAttack) >= timeBetweenAttacks) {
            canFlash = true;
        }
        
        backgroundCurColor = Color.Lerp(backgroundCurSetColor, backgroundNewColor, t);
        borderCurColor = Color.Lerp(borderCurSetColor, borderNewColor, t);

        

        if (beingAttacked && canFlash && !alienPhaseOne && !alienPhaseTwo){
            eBackgroundNewColor = alienAttackBackground;
            eBorderNewColor = alienAttackBorder;

            alienPhaseOne = true;

            eTransistionTimeStart = Time.time;

            beingAttacked = false;
            canFlash = false;
            timeSinceLastAttack = Time.time;

            //print("ATTTTAACKKKKK!!!!");
        }

        if (alienPhaseOne || alienPhaseTwo){
            if (et < 1){
                //et += Time.deltaTime/eDuration;
                et = (Time.time - eTransistionTimeStart) / eDuration;
                //Debug.Log("Percent complete: " + et);
                
                if (et >= 1){
                    et = 1;
                }
            }

            if (alienPhaseOne) {
                eBackgroundCurSetColor = backgroundCurColor;
                eborderCurSetColor = borderCurColor;
            } else if (alienPhaseTwo) {
                eBackgroundCurSetColor = alienAttackBackground;
                eborderCurSetColor = alienAttackBorder;
            }
            

            //eBackgroundCurSetColor = Color.Lerp(eBackgroundCurSetColor, eBackgroundNewColor, et);
            //eborderCurSetColor = Color.Lerp(eborderCurSetColor, eBorderNewColor, et);
            
            SetBackgroundColor(Color.Lerp(eBackgroundCurSetColor, eBackgroundNewColor, et));
            SetBorderColor(Color.Lerp(eborderCurSetColor, eBorderNewColor, et));

        } else {
			SetBackgroundColor(backgroundCurColor);
			SetBorderColor(borderCurColor);
        }

        if (et >= 1 && alienPhaseOne){
            alienPhaseOne = false;
            alienPhaseTwo = true;
            eTransistionTimeStart = Time.time;
            et = 0;

            //eBackgroundCurSetColor = eBackgroundNewColor;
            //eborderCurSetColor = eBorderNewColor;

            eBackgroundNewColor = backgroundCurColor;
            eBorderNewColor = borderCurColor;
        } else if (et >= 1 && alienPhaseTwo) {
            alienPhaseTwo = false;
            et = 0;
        }

        
    }

    public void UITriggerAttackFlash(){
        beingAttacked = true;
    }

	public void SetBackgroundColor(Color color)
	{
		foreach (Image background in UIBackgrounds)
		{
            if (background.color.a < 0)
            {
                color.a = background.color.a;
            }

            background.color = color;
        }
	}

	public void SetBorderColor(Color color)
	{
		foreach (Image border in UIBorders)
		{
            if (border.color.a < 0)
            {
                color.a = border.color.a;
            }

			border.color = color;
		}
	}

	public void SetNight(){
        backgroundCurSetColor = backgroundDay;
        borderCurSetColor = borderDay;
        backgroundNewColor = backgroundNight;
        borderNewColor = borderNight;
        t = 0;
        transistionTimeStart = Time.time;
    }

    public void SetDay(){
        backgroundCurSetColor = backgroundNight;
        borderCurSetColor = borderNight;
        backgroundNewColor = backgroundDay;
        borderNewColor = borderDay;
        t = 0;
        transistionTimeStart = Time.time;
    }
}
