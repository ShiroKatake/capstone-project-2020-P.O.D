using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class BackgroundMusicTest : MonoBehaviour
{
    private Rewired.Player player;
    // Start is called before the first frame update

    [SerializeField] AudioManager.Sound soundToPlay;

    private void Start() {
        player = PlayerMovementController.Instance.RewiredPlayer;
    }

    void Update()
    {
        if (player == null){
            player = PlayerMovementController.Instance.RewiredPlayer;
        } else {
            if (player.GetButton("GeneralAction")) {
                print("SWITCH!!");
                AudioManager.Instance.SwitchBackgroundTrack(AudioManager.Sound.NightTime);
            }
        }
        //AudioManager.Instance.PlayBackground(soundToPlay);
    }
}
