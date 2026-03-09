using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GraphicsSettingsManager : MonoBehaviour
{
    public static GraphicsSettingsManager Instance;

    private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private ColorAdjustments colorAdjustments;

    private const string RESOLUTION_KEY = "ResolutionIndex";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string VSYNC_KEY = "VSync";
    private const string BRIGHTNESS_KEY = "Brightness";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FindResolutionDropdown();
        SetupResolutions();
        LoadSettings();
    }

    // =============================
    // DETECTAR DROPDOWN EN ESCENA
    // =============================

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindResolutionDropdown();
    }

    void FindResolutionDropdown()
    {
        TMP_Dropdown[] dropdowns = FindObjectsOfType<TMP_Dropdown>();

        foreach (TMP_Dropdown d in dropdowns)
        {
            if (d.name == "ResolutionDropdown")
            {
                resolutionDropdown = d;
                resolutionDropdown.onValueChanged.AddListener(SetResolution);

                SetupResolutions();
                break;
            }
        }
    }

    // =============================
    // RESOLUTION
    // =============================

    void SetupResolutions()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();

        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedIndex = PlayerPrefs.GetInt(RESOLUTION_KEY, currentIndex);

        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        if (resolutions == null || resolutions.Length == 0) return;

        Resolution resolution = resolutions[index];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt(RESOLUTION_KEY, index);
    }

    // =============================
    // FULLSCREEN
    // =============================

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
    }

    // =============================
    // VSYNC
    // =============================

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;

        PlayerPrefs.SetInt(VSYNC_KEY, enabled ? 1 : 0);
    }

    // =============================
    // LOAD SETTINGS
    // =============================

    void LoadSettings()
    {
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        Screen.fullScreen = fullscreen;

        bool vsync = PlayerPrefs.GetInt(VSYNC_KEY, 1) == 1;
        QualitySettings.vSyncCount = vsync ? 1 : 0;
    }

    public void InitializeResolutionDropdown(TMP_Dropdown dropdown)
    {
        resolutionDropdown = dropdown;

        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        SetupResolutions();
    }
}
