using UnityEngine;
using UnityEngine.UI;

public class GraphicsController : MonoBehaviour
{
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vSyncToggle;

    private Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", currentResolutionIndex);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 1) == 1;

        ApplySettings();
    }

    public void ApplySettings()
    {
        Resolution res = resolutions[resolutionDropdown.value];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;

        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("VSync", vSyncToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
