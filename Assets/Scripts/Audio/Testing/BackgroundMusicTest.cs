using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayBackground(AudioManager.Sound.Test);
    }
}
