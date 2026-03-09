using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject menuPausa;

    [Header("Audio UI")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private GameObject panelSettings;

    private bool isPaused = false;

    private void Awake()
    {
        Resume(); // arranca sin pausa
    }

    private void Start()
    { 
        StartCoroutine(ForceCursorLock());
    }

    IEnumerator ForceCursorLock()
    {
        yield return null; // espera 1 frame para que Unity inicialice todo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PlayButtonSound();
            TogglePause();
            panelSettings.SetActive(false);
        }

        // 🔹 Si el juego no está pausado, aseguramos que el cursor esté bloqueado
        if (!isPaused)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;

        Time.timeScale = 0f;
        menuPausa.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;

        Time.timeScale = 1f;
        menuPausa.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(ForceCursorLock());
    }

    public void Restart()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu(int menuIndex)
    {
        PlayButtonSound();
        Resume(); 
        StartCoroutine(ForceCursorLock());
        SceneManager.LoadScene(menuIndex);
    }

    void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }

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
