using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; protected set;}

    //gonna break these up later
    public enum ESound
    {
        DayTimeLvlOne,
        NightTime,
        Player_Hover,
        Laser_POD,
        Collected_Minerals,
        Mining,
        Damage_To_Player,
        Damage_To_Building,
        Explosion,
        Negative_UI,
        Building_Materialises,
        Building_Completes,
        ShotGun_Shoot,
        MachineGun_Shoot,
        WaterDrill_Idle,
        Reactor_Idle,
        Greenhouse_Idle,
        Incinorator_Idle,
        Boiler_Idle,
        Turret_Idle,
        Alien_Moves,
        Alien_Takes_Damage,
        Alien_Dies,
    }

    [System.Serializable]
    public class SoundClip {
        [SerializeField] private string name;
        [SerializeField] private AudioManager.ESound sound;
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool loop;

        [SerializeField] [Range(0f, 1f)] private float volume;
        [SerializeField] [Range(0.3f, 3f)] private float pitch;
        [SerializeField] [Range(0f, 500f)] private float minDistance;
        [SerializeField] [Range(0f, 500f)] private float maxDistance;


        [HideInInspector] private AudioSource source;

        public string Name { get => name; }
        public AudioManager.ESound Sound { get => sound; }
        public AudioClip Clip { get => clip; }
        public float Volume { get => volume; }
        public float Pitch { get => pitch; }
        public bool Loop { get => loop; }
        public float MinDistance {get => minDistance;}
        public float MaxDistance {get => maxDistance;}
        public AudioSource Source { get => source; set => source = value; }
    }

    [SerializeField] private SoundClip[] OneShotSounds;
    [SerializeField] private SoundClip[] BackGroundSounds;

    private Dictionary<ESound, float> soundTimerDictionary;
    private Dictionary<ESound, AudioSource> audioSourceReferenceDictionary;
    private GameObject oneShotGameObject;
    private AudioSource oneShotAudioSource;
    private AudioSource currentBackgroundTrack;
    private float timeStamp;
    private float volumeControlBackground;
    private float volumeControlTimer;
    private bool bgSwitching, bgDown, bgUp, bgSwitchControl;
    private ESound bgSoundSwitch;

    private void Awake() {
        if (Instance != null){
            Debug.LogError("There should never be 2 or more Audio Managers.");
        }
        Instance = this;
        audioSourceReferenceDictionary = new Dictionary<ESound, AudioSource>();

        foreach (SoundClip s in BackGroundSounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
        }

        currentBackgroundTrack = Array.Find(BackGroundSounds, SoundClip => SoundClip.Sound == ESound.DayTimeLvlOne).Source;
        currentBackgroundTrack.Play();

        soundTimerDictionary = new Dictionary<ESound, float>();
        soundTimerDictionary[ESound.Player_Hover] = 0f;
        soundTimerDictionary[ESound.Mining] = 0f;
        soundTimerDictionary[ESound.Damage_To_Building] = 0f;

        bgSwitching = false;
        bgDown = false;
        bgUp =false;
        bgSwitchControl = false;
    }

    private void Update() {
        if (bgSwitching){
            CheckBackgroundSound();
        }
    }

    public void PlayBackground(ESound sound){
        SoundClip s = Array.Find(BackGroundSounds, SoundClip => SoundClip.Sound == sound);
        if (s == null) {
            Debug.LogWarning("Sound: " + sound + " not found.");
            return;
        }
        s.Source.Play();
    }

    // legacy code
    /*public void PlaySound(ESound sound, Vector3 position){
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
    }*/

    public void PlaySound(ESound sound, GameObject obj){
        if (CanPlaySound(sound)){
            //GameObject soundGameObject = new GameObject("Sound");
            //soundGameObject.transform.position = obj.transform.position;
            bool tmp = true;
            AudioSource[] sources = obj.GetComponents<AudioSource>();
            foreach(AudioSource source in sources){
                oneShotAudioSource = source;
                if (source.clip == GetAudio(sound).Clip){
                    tmp = false;
                    break;
                }
            }
            if (tmp) {
                AudioSource audioSource = obj.AddComponent<AudioSource>();
                SoundClip s = GetAudio(sound);
                audioSource.clip = s.Clip;
                audioSource.volume = s.Volume;
                audioSource.pitch = s.Pitch;
                audioSource.loop = s.Loop;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.minDistance = s.MinDistance;
                audioSource.maxDistance = s.MaxDistance;
            
                audioSource.Play();
            } else {
                oneShotAudioSource.Play();
            }
            /*
            if (!audioSource.loop){
                UnityEngine.Object.Destroy(soundGameObject, audioSource.clip.length);
            }
            */
        }
    }


    public void PlaySound(ESound sound){
        if (CanPlaySound(sound)){
            if (audioSourceReferenceDictionary[sound] == null){
                //oneShotGameObject = this.gameObject.AddComponent<AudioSource>();//new GameObject("Sound");
                //oneShotAudioSource = this.gameObject.AddComponent<AudioSource>();
                audioSourceReferenceDictionary[sound] = this.gameObject.AddComponent<AudioSource>();
            }
            //oneShotAudioSource.PlayOneShot(GetAudio(sound).Clip);
            audioSourceReferenceDictionary[sound].PlayOneShot(GetAudio(sound).Clip);
        }
    }

    public void StopSound(ESound sound){
        AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
        if (sources.Length != 0){
            foreach(AudioSource source in sources) {
                if (source.clip == GetAudio(sound).Clip){
                    source.Stop();
                    break;
                }
            }
        } else {
            Debug.LogError("The object has no audio sources!");
        }
    }

    public void StopSound(ESound sound, GameObject obj){
        AudioSource[] sources = obj.GetComponents<AudioSource>();
        if (sources.Length != 0){
            foreach(AudioSource source in sources) {
                if (source.clip == GetAudio(sound).Clip){
                    source.Stop();
                    break;
                }
            }
        } else {
            Debug.LogError("The object has no audio sources!");
        }
    }


    private SoundClip GetAudio(ESound sound){
        SoundClip s = Array.Find(OneShotSounds, SoundClip => SoundClip.Sound == sound);
        if (s != null){
            return s;
        } else {
            return null;
        }
    }

    private bool CanPlaySound(ESound sound){
        if (Array.Find(OneShotSounds, SoundClip => SoundClip.Sound == sound) != null){
            switch (sound){
                default:
                    return true;
                case ESound.Player_Hover:
                    if (soundTimerDictionary.ContainsKey(sound))
                    {
                        if (Time.time == 0f)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                case ESound.Mining:
                    if (soundTimerDictionary.ContainsKey(sound))
                    {
                        Debug.Log("Has found Mining");
                        if (Time.time == 0f)
                        {
                            Debug.Log("Is playing Mining");
                            return true;
                        }
                        else
                        {
                            Debug.Log("Is not playing Mining");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
            }

        } else {
            Debug.Log("Sound: " + sound.ToString() + " cannot be found");
            return false;
        }
    }

    public void SwitchBackgroundTrack(ESound sound){
        bgSoundSwitch = sound;
        volumeControlTimer = 0;
        timeStamp = Time.time;
        volumeControlBackground = currentBackgroundTrack.volume;

        bgDown = true;
        bgSwitching = true;
    }

    private void CheckBackgroundSound(){
        if (Time.time >= timeStamp + 1f && !bgSwitchControl){
            bgSwitchControl = true;
            currentBackgroundTrack.Stop();
            currentBackgroundTrack.volume = volumeControlBackground;

            currentBackgroundTrack = Array.Find(BackGroundSounds, SoundClip => SoundClip.Sound == bgSoundSwitch).Source;

            bgDown = false;
            bgUp = true;
            volumeControlTimer = 0;
            volumeControlBackground = currentBackgroundTrack.volume;
            currentBackgroundTrack.volume = 0;

            currentBackgroundTrack.Play();
        } else if (Time.time >= timeStamp + 2f){
            timeStamp = 0f;
            bgSwitchControl = false;
            bgUp = false;
            bgSwitching = false;
        }

        if (bgDown) {
            VolumeControlDown();
        } else if (bgUp) {
            VolumeControlUp();
        }
    }

    private void VolumeControlDown(){
        volumeControlTimer += Time.deltaTime;
        currentBackgroundTrack.volume = Mathf.Lerp(volumeControlBackground, 0f, volumeControlTimer);
    }

    private void VolumeControlUp(){
        volumeControlTimer += Time.deltaTime;
        currentBackgroundTrack.volume = Mathf.Lerp(0f, volumeControlBackground, volumeControlTimer);
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