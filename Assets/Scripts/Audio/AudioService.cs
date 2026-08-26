using System;
using UnityEngine;
using UnityEngine.Audio;

namespace RehmanMughal.PostmarkProtocol
{
    [Serializable]
    public sealed class PostalSound
    {
        public string eventName;
        public AudioClip clip;
        public bool ui;
        [Range(0, 1)] public float gain = 0.65f;
    }

    /// <summary>Authored Bootstrap audio sources and mixer persist across scene changes.</summary>
    public sealed class AudioService : MonoBehaviour
    {
        public AudioMixer mixer;
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource uiSource;
        public PostalSound[] sounds;
        private static AudioService instance;
        private static float lastHaptic = -1;
#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void PostmarkLightHaptic();
#endif
        public static bool Available => instance != null;

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            if (instance != this) return;
            ApplySettings();
            if (musicSource != null && musicSource.clip != null) musicSource.Play();
        }
        public static void ApplySettings()
        {
            if (instance == null || instance.mixer == null) return;
            instance.mixer.SetFloat("MasterVolume", 0);
            instance.mixer.SetFloat("MusicVolume", Decibels(SaveService.Current.musicVolume));
            instance.mixer.SetFloat("SFXVolume", Decibels(SaveService.Current.soundVolume));
            instance.mixer.SetFloat("UIVolume", Decibels(SaveService.Current.soundVolume));
        }
        public static float Decibels(float linear) => linear <= 0.0001f ? -80f : Mathf.Log10(Mathf.Clamp01(linear)) * 20f;

        public static void Play(string eventName)
        {
            if (instance == null || instance.sounds == null) return;
            foreach (var sound in instance.sounds)
            {
                if (sound.eventName != eventName || sound.clip == null) continue;
                var source = sound.ui ? instance.uiSource : instance.sfxSource;
                if (source != null) source.PlayOneShot(sound.clip, sound.gain);
                if (eventName == "stamp" || eventName == "dispatch" || eventName == "success" || eventName == "failure") Haptic();
                return;
            }
        }

        public static void Haptic()
        {
            if (!SaveService.Current.haptics || Time.realtimeSinceStartup - lastHaptic < 0.18f) return;
            lastHaptic = Time.realtimeSinceStartup;
#if UNITY_IOS && !UNITY_EDITOR
            PostmarkLightHaptic();
#elif UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
        private void OnApplicationPause(bool paused) { if (paused) SaveService.Save(); }
        private void OnApplicationQuit() { SaveService.Save(); }
        private void OnDestroy() { if (instance == this) instance = null; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() { instance = null; lastHaptic = -1; }
    }
}
