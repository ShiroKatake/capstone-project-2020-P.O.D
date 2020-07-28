using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorManager : MonoBehaviour
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
    private float eDuration = 0.5f;

    private Color backgroundCurColor;
    private Color eBackgroundCurColor;
    private Color backgroundNewColor;
    private Color eBackgroundNewColor;
    private Color borderCurColor;
    private Color eBorderCurColor;
    private Color borderNewColor;
    private Color eBorderNewColor;

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
        borderCurColor = borderDay;
        backgroundNewColor = backgroundDay;
        borderNewColor = borderDay;

		SetBackgroundColor(backgroundCurColor);
		SetBorderColor(borderCurColor);

    }

    public void ColorUpdate() {
        if (PlayerController.Instance.PlayerInputManager.GetButton("EnemyAttack") && !alienPhaseOne && !alienPhaseTwo){
            eBackgroundCurColor = backgroundCurColor;
            eBorderCurColor = borderCurColor;

            eBackgroundNewColor = alienAttackBackground;
            eBorderCurColor = alienAttackBorder;

            alienPhaseOne = true;

            print("ATTTTAACKKKKK!!!!");
        }

        if (t<1){
            t += Time.deltaTime/duration;
        }

        //for smooth transition replace first cur color field with starting color field
        backgroundCurColor = Color.Lerp(backgroundCurColor, backgroundNewColor, t);
        borderCurColor = Color.Lerp(borderCurColor, borderNewColor, t);

        if (alienPhaseOne || alienPhaseTwo){
            if (et<1){
                et += Time.deltaTime/eDuration;
            }

            eBackgroundCurColor = Color.Lerp(eBackgroundCurColor, eBackgroundNewColor, et);
            eBorderCurColor = Color.Lerp(eBorderCurColor, eBorderNewColor, et);
            
            SetBackgroundColor(eBackgroundCurColor);
            SetBorderColor(eBorderCurColor);

        } else {
			SetBackgroundColor(backgroundCurColor);
			SetBorderColor(borderCurColor);
        }

        if (et >= 1 && alienPhaseOne){
            alienPhaseOne = false;
            alienPhaseTwo = true;
            et = 0;

            eBackgroundNewColor = backgroundCurColor;
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
			background.GetComponent<Image>().color = color;
		}
	}

	public void SetBorderColor(Color color)
	{
		foreach (Image border in UIBorders)
		{
			border.GetComponent<Image>().color = color;
		}
	}

	public void SetNight(){
        backgroundCurColor = backgroundDay;
        borderCurColor = borderDay;
        backgroundNewColor = backgroundNight;
        borderNewColor = borderNight;
        t = 0;
    }

    public void SetDay(){
        backgroundCurColor = backgroundNight;
        borderCurColor = borderNight;
        backgroundNewColor = backgroundDay;
        borderNewColor = borderDay;
        t = 0;
    }
}
