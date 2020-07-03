using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class PlayUISound : MonoBehaviour
{
    public static PlayUISound Instance { get; protected set; }
    public void Negative()
    {
        AudioManager.Instance.PlaySound(AudioManager.ESound.Negative_UI);
    }
}
