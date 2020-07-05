using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class BackgroundMusicTest : MonoBehaviour
{
    private Rewired.Player player;
    // Start is called before the first frame update

    [SerializeField] AudioManager.ESound soundToPlay;

    private void Start() {
        player = PlayerMovementController.Instance.PlayerInputManager;
    }

    void Update()
    {
        if (player == null){
            player = PlayerMovementController.Instance.PlayerInputManager;
        } else {
            if (player.GetButton("GeneralAction")) {
                print("SWITCH!!");
                AudioManager.Instance.SwitchBackgroundTrack(AudioManager.ESound.NightTime);
            }
        }
        //AudioManager.Instance.PlayBackground(soundToPlay);
    }
}
