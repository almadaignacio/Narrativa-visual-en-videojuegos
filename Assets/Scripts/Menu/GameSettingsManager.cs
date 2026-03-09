using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;

    [Header("Audio")]
    public Slider volumeSlider;
    public AudioSource musicSource;

    [Header("Graphics")]
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;

    [Header("Colorblind")]
    public Dropdown colorblindDropdown;
    public Volume postProcessVolume;

    private ColorAdjustments colorAdjust;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out colorAdjust);
        }
    }

    // AUDIO
    public void SetVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        PlayerPrefs.SetFloat("volume", value);
    }

    // FULLSCREEN
    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("fullscreen", value ? 1 : 0);
    }

    // QUALITY
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("quality", index);
    }

    // COLORBLIND
    public void SetColorblind(int mode)
    {
        if (colorAdjust == null) return;

        switch (mode)
        {
            case 0: // Normal
                colorAdjust.saturation.value = 0;
                break;

            case 1: // Protanopia
                colorAdjust.saturation.value = -50;
                break;

            case 2: // Deuteranopia
                colorAdjust.saturation.value = -30;
                break;

            case 3: // Tritanopia
                colorAdjust.saturation.value = -70;
                break;
        }

        PlayerPrefs.SetInt("colorblind", mode);
    }

    void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        int fullscreen = PlayerPrefs.GetInt("fullscreen", 1);
        int quality = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());
        int colorblind = PlayerPrefs.GetInt("colorblind", 0);

        if (musicSource != null)
            musicSource.volume = volume;

        Screen.fullScreen = fullscreen == 1;

        QualitySettings.SetQualityLevel(quality);

        SetColorblind(colorblind);
    }

    // ESTE MÉTODO CONECTA AUTOMÁTICAMENTE LA UI
    public void ConnectUI(
        Slider vol,
        Toggle full,
        Dropdown quality,
        Dropdown colorblind
    )
    {
        volumeSlider = vol;
        fullscreenToggle = full;
        qualityDropdown = quality;
        colorblindDropdown = colorblind;

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (fullscreenToggle != null)
        {
            bool value = PlayerPrefs.GetInt("fullscreen", 1) == 1;
            fullscreenToggle.isOn = value;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        if (qualityDropdown != null)
        {
            int value = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());
            qualityDropdown.value = value;
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }

        if (colorblindDropdown != null)
        {
            int value = PlayerPrefs.GetInt("colorblind", 0);
            colorblindDropdown.value = value;
            colorblindDropdown.onValueChanged.AddListener(SetColorblind);
        }
    }
}

