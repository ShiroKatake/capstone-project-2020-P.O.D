using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransparencyFader
{
    private CanvasGroup cg;

    private float timeStartedLerping;
    private float timeSinceStarted;
    private float percentageComplete;
    private float lerpTime;
    private float start;
    private float end;
    private bool fading;

    public UITransparencyFader(CanvasGroup canvasGroup) {
        timeSinceStarted = 0;
        timeStartedLerping = 0;
        percentageComplete = 0;
        lerpTime = 0;
        start = 0;
        end = 0;
        fading = false;
        cg = canvasGroup;
    }

    public void StartFade(float initVal, float finVal, float dur){
        timeStartedLerping = Time.time;
        timeSinceStarted = Time.time - timeStartedLerping;
        percentageComplete = 0;
        lerpTime = dur;
        start = initVal;
        end = finVal;
        fading = true;
    }

    public void Fade(){
        if (fading){
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            cg.alpha = currentValue;
            if (percentageComplete >= 1){
                fading = false;
            }
        }
    }

    public bool isTransparent(){
        if (cg.alpha == 0){
            return true;
        } else {
            return false;
        }
    }

    public bool IsFading(){
        return fading;
    }

    public void SetInvisible(){
        cg.alpha = 0;
    }

    public void SetVisible(){
        cg.alpha = 1;
    }

/*
    public IEnumerator FadeCanvaseGround(CanvasGroup cg, float start, float end, float lerpTime){
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;
        while (true){
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            cg.alpha = currentValue;
            yield return new WaitForEndOfFrame();
        }

        print("Done");
    }
*/
}
