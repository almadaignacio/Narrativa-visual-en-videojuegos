using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class VisualGraphicsManager : MonoBehaviour
{
    public static VisualGraphicsManager Instance;

    [Header("LUT Textures")]
    public Texture normalLUT;
    public Texture deuteranopiaLUT;
    public Texture protanopiaLUT;
    public Texture tritanopiaLUT;

    private ColorLookup colorLookup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        LoadSavedMode();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindVolumeAndApply();
    }

    void FindVolumeAndApply()
    {
        Volume volume = FindObjectOfType<Volume>();

        if (volume != null && volume.profile.TryGet(out colorLookup))
        {
            LoadSavedMode();
        }
    }

    public void SetColorblindMode(int mode)
    {
        if (colorLookup == null) return;

        switch (mode)
        {
            case 0:
                colorLookup.texture.value = normalLUT;
                break;

            case 1:
                colorLookup.texture.value = deuteranopiaLUT;
                break;

            case 2:
                colorLookup.texture.value = protanopiaLUT;
                break;

            case 3:
                colorLookup.texture.value = tritanopiaLUT;
                break;
        }

        PlayerPrefs.SetInt("ColorblindMode", mode);
    }

    void LoadSavedMode()
    {
        int savedMode = PlayerPrefs.GetInt("ColorblindMode", 0);
        SetColorblindMode(savedMode);
    }
}
