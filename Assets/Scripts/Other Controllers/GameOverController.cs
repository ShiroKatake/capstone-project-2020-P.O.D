using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject uiManager;
    [SerializeField] private GOMessageController goMessage;

    private bool gameOver = false;

    // Update is called once per frame
    void Update()
    {
        /*if (PlayerMovementController.Instance.PlayerInputManager.GetButton("GeneralAction")){
            goMessage.SetText(false);
            uiManager.GetComponent<UIAppearScript>().ToggleVisibility();
        }*/
        if (!gameOver){
            if (EnvironmentalController.Instance.Win) {
                goMessage.SetText(true);
                uiManager.GetComponent<UIAppearScript>().ToggleVisibility();
                gameOver = true;
            } else if (CryoEgg.Instance.Health.IsDead()){
                goMessage.SetText(false);
                uiManager.GetComponent<UIAppearScript>().ToggleVisibility();
                gameOver = true;
            }
        }
    }
}
