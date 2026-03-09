using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =============================
    // SETTERS (para conectar sliders)
    // =============================

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("EnvironmentVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    // =============================
    // GETTERS (para actualizar sliders)
    // =============================

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_KEY, 1f);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    // =============================
    // MUTE GLOBAL
    // =============================

    public void MuteAll(bool mute)
    {
        if (mute)
        {
            mixer.SetFloat("MasterVolume", -80f);
        }
        else
        {
            SetMasterVolume(GetMasterVolume());
        }
    }

    // =============================
    // CARGA INICIAL
    // =============================

    private void LoadVolumes()
    {
        SetMasterVolume(GetMasterVolume());
        SetMusicVolume(GetMusicVolume());
        SetSFXVolume(GetSFXVolume());
    }
}
