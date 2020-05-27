using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; protected set;}
    public enum Sound{
        Test,
        Player_Hover
    }

    [System.Serializable]
    public class SoundClip {
        [SerializeField] private string name;
        [SerializeField] private AudioManager.Sound sound;
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool loop;

        [SerializeField] [Range(0f, 1f)] private float volume;
        [SerializeField] [Range(0.3f, 3f)] private float pitch;

        [HideInInspector] private AudioSource source;

        public string Name { get => name; }
        public AudioManager.Sound Sound { get => sound; }
        public AudioClip Clip { get => clip; }
        public float Volume { get => volume; }
        public float Pitch { get => pitch; }
        public bool Loop { get => loop; }
        public AudioSource Source { get => source; set => source = value; }
    }

    [SerializeField] private SoundClip[] OneShotSounds;
    [SerializeField] private SoundClip[] BackGroundSounds;

    private Dictionary<Sound, float> soundTimerDictionary;
    private GameObject oneShotGameObject;
    private AudioSource oneShotAudioSource;

    private void Awake() {
        if (Instance != null){
            Debug.LogError("There should never be 2 or more Audio Managers.");
        }
        Instance = this;

        foreach (SoundClip s in BackGroundSounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
        }

        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.Player_Hover] = 0f;
    }

    public void PlayBackground(Sound sound){
        SoundClip s = Array.Find(BackGroundSounds, SoundClip => SoundClip.Sound == sound);
        if (s == null) {
            Debug.LogWarning("Sound: " + sound + " not found.");
            return;
        }
        s.Source.Play();
    }

    public void PlaySound(Sound sound, Vector3 position){
        if (CanPlaySound(sound)){
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            SoundClip s = GetAudio(sound);
            audioSource.clip = s.Clip;
            audioSource.volume = s.Volume;
            audioSource.pitch = s.Pitch;
            audioSource.loop = s.Loop;
            
            audioSource.Play();

            if (!audioSource.loop){
                UnityEngine.Object.Destroy(soundGameObject, audioSource.clip.length);
            }
        }
    }


    public void PlaySound(Sound sound){
        if (CanPlaySound(sound)){
            if (oneShotGameObject == null){
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(GetAudio(sound).Clip);
        }
    }

    private SoundClip GetAudio(Sound sound){
        SoundClip s = Array.Find(OneShotSounds, SoundClip => SoundClip.Sound == sound);
        if (s != null){
            return s;
        } else {
            return null;
        }
    }

    private bool CanPlaySound(Sound sound){
        switch (sound){
            default:
                return true;
            case Sound.Player_Hover:
                if (soundTimerDictionary.ContainsKey(sound)){
                    if (Time.time == 0f){
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return true;
                }
        }
    }

}


/*
if (soundTimerDictionary.ContainsKey(sound)){
    float lastTimePLayed = soundTimerDictionary[sound];
    float timeMax = 7f;
    if (Time.time == 0f){
        return true;
    } else if (lastTimePLayed + timeMax < Time.time){
        soundTimerDictionary[sound] = Time.time;
        return true;
    } else {
        return false;
    }
} else {
    return true;
}
*/