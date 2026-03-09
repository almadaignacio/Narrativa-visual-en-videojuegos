using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsUIBinder : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    [Header("Graphics")]
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    public TMP_Dropdown resolutionDropdown;

    [Header("Accessibility")]
    public TMP_Dropdown colorblindDropdown;

    private void Awake()
    {
        ApplySavedSettings();
    }

    void Start()
    {
        ConnectAudio();
        ConnectGraphics();
        ConnectAccessibility();
    }

    // =========================
    // AUDIO
    // =========================

    void ConnectAudio()
    {
        if (AudioSettingsManager.Instance == null) return;

        if (masterSlider != null)
        {
            masterSlider.value = AudioSettingsManager.Instance.GetMasterVolume();
            masterSlider.onValueChanged.AddListener(AudioSettingsManager.Instance.SetMasterVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.value = AudioSettingsManager.Instance.GetMusicVolume();
            musicSlider.onValueChanged.AddListener(AudioSettingsManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = AudioSettingsManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(AudioSettingsManager.Instance.SetSFXVolume);
        }

        if (muteToggle != null)
        {
            muteToggle.onValueChanged.AddListener(AudioSettingsManager.Instance.MuteAll);
        }
    }

    // =========================
    // GRAFICOS
    // =========================

    void ConnectGraphics()
    {
        if (GraphicsSettingsManager.Instance == null) return;

        // FULLSCREEN
        if (fullscreenToggle != null)
        {
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.isOn = fullscreen;

            fullscreenToggle.onValueChanged.AddListener(
                GraphicsSettingsManager.Instance.SetFullscreen
            );
        }

        // VSYNC
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;

            vsyncToggle.onValueChanged.AddListener(
                GraphicsSettingsManager.Instance.SetVSync
            );
        }

        // RESOLUTION
        if (resolutionDropdown != null)
        {
            GraphicsSettingsManager.Instance.InitializeResolutionDropdown(resolutionDropdown);
        }
    }

    void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    void SetVsync(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }

    void SetResolution(int index)
    {
        Resolution[] resolutions = Screen.resolutions;
        Resolution res = resolutions[index];

        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    // =========================
    // ACCESIBILIDAD
    // =========================

    void ConnectAccessibility()
    {
        if (VisualGraphicsManager.Instance == null) return;

        if (colorblindDropdown != null)
        {
            int savedMode = PlayerPrefs.GetInt("ColorblindMode", 0);

            colorblindDropdown.value = savedMode;

            colorblindDropdown.onValueChanged.AddListener(
                VisualGraphicsManager.Instance.SetColorblindMode
            );
        }
    }

    void ApplySavedSettings()
    {
        Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 1) == 1 ? 1 : 0;
    }
}

