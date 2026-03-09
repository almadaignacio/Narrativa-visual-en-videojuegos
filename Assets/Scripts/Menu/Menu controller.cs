using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;


public class Menucontroller : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;// Imagen negra fullscreen
    public float fadeDuration = 1f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip buttonSound;

    //public static Menucontroller Instance;

    private bool isTransitioning = false;

    void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        // Forzar que empiece completamente negro
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }
    }

    void Start()
    {
        // Mostrar cursor en menús
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Fade OUT (de negro a transparente)
        if (fadeImage != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    // ================================
    // CAMBIAR ESCENA CON FADE
    // ================================

    public void ChangeLevel(int index)
    {
        if (!isTransitioning)
        {
            Time.timeScale = 1f;
            StartCoroutine(LoadSceneRoutine(index));
        }
    }

    IEnumerator LoadSceneRoutine(int index)
    {
        isTransitioning = true;

        PlayButtonSound();

        yield return StartCoroutine(FadeIn());

        SceneManager.LoadScene(index);
    }

    // ================================
    // FADE IN (a negro)
    // ================================

    IEnumerator FadeIn()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);

            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }

        // asegurar negro total
        Color finalColor = fadeImage.color;
        finalColor.a = 1f;
        fadeImage.color = finalColor;
    }

    // ================================
    // FADE OUT (desde negro)
    // ================================

    IEnumerator FadeOut()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }

        // asegurar transparente total
        Color finalColor = fadeImage.color;
        finalColor.a = 0f;
        fadeImage.color = finalColor;
    }

    // ================================
    // SONIDO BOTÓN
    // ================================

    public void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }

    // ================================
    // SALIR DEL JUEGO
    // ================================

    public void Exit()
    {
        PlayButtonSound();
        Application.Quit();
    }

    // ================================
    // MOSTRAR / OCULTAR PANEL
    // ================================

    public void ShowPanel(GameObject panelUI)
    {
        PlayButtonSound();
        panelUI.SetActive(true);
    }

    public void HidePanel(GameObject panelUI)
    {
        PlayButtonSound();
        panelUI.SetActive(false);
    }
}
