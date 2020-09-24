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

    private Color backgroundCurSetColor;
    private Color ebackgroundCurSetColor;
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
    private float t = 0;
    private float transistionTimeStart = 0;

    private float et = 0;
    private float etransistionTimeStart = 0;

    protected override void Awake() {
        base.Awake();
        backgroundCurSetColor = backgroundDay;
        borderCurSetColor = borderDay;
        backgroundNewColor = backgroundDay;
        borderNewColor = borderDay;

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
        
        backgroundCurColor = Color.Lerp(backgroundCurSetColor, backgroundNewColor, t);
        borderCurColor = Color.Lerp(borderCurSetColor, borderNewColor, t);

        ebackgroundCurSetColor = backgroundCurColor;
        eborderCurSetColor = borderCurColor;

        if (PlayerController.Instance.PlayerInputManager.GetButton("EnemyAttack") && !alienPhaseOne && !alienPhaseTwo){
            eBackgroundNewColor = alienAttackBackground;
            eBorderNewColor = alienAttackBorder;

            alienPhaseOne = true;

            etransistionTimeStart = Time.time;

            print("ATTTTAACKKKKK!!!!");
        }

        if (alienPhaseOne || alienPhaseTwo){
            if (et < 1){
                //et += Time.deltaTime/eDuration;
                et = (Time.time - etransistionTimeStart) / eDuration;
                Debug.Log("Percent complete: " + et);
                
                if (et >= 1){
                    et = 1;
                }
            }
            

            //ebackgroundCurSetColor = Color.Lerp(ebackgroundCurSetColor, eBackgroundNewColor, et);
            //eborderCurSetColor = Color.Lerp(eborderCurSetColor, eBorderNewColor, et);
            
            SetBackgroundColor(Color.Lerp(ebackgroundCurSetColor, eBackgroundNewColor, et));
            SetBorderColor(Color.Lerp(eborderCurSetColor, eBorderNewColor, et));

        } else {
			SetBackgroundColor(backgroundCurColor);
			SetBorderColor(borderCurColor);
        }

        if (et >= 1 && alienPhaseOne){
            alienPhaseOne = false;
            alienPhaseTwo = true;
            etransistionTimeStart = Time.time;
            et = 0;

            //ebackgroundCurSetColor = eBackgroundNewColor;
            //eborderCurSetColor = eBorderNewColor;

            eBackgroundNewColor = backgroundCurSetColor;
            eBorderNewColor = borderNewColor;
        } else if (et >= 1 && alienPhaseTwo) {
            alienPhaseTwo = false;
            et = 0;
        }

        
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
