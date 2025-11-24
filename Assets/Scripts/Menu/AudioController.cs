using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;
    [SerializeField] private Toggle muteToggle;

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        muteToggle.isOn = PlayerPrefs.GetInt("Muted", 0) == 1;
        ApplySettings();
    }

    public void ApplySettings()
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(masterSlider.value) * 20);
        mixer.SetFloat("MusicVol", Mathf.Log10(musicSlider.value) * 20);
        mixer.SetFloat("SFXVol", Mathf.Log10(sfxSlider.value) * 20);
        AudioListener.pause = muteToggle.isOn;

        PlayerPrefs.SetFloat("MasterVol", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
        PlayerPrefs.SetInt("Muted", muteToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

}
