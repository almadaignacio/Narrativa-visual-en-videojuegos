using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject MenuPausa;
    bool pauseMode;

        // Start is called before the first frame update
        void Start()
        {
            pauseMode = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame && pauseMode == false)
            {
            pauseMode = true;
                Pausa();
            }

            else if (Keyboard.current.escapeKey.wasPressedThisFrame && pauseMode == true)
            {
                Renaudar();
            pauseMode = false;
            }
        }

        public void Pausa()
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            MenuPausa.SetActive(true);
            pauseMode = true;
        }

        public void Renaudar()
        {
            Time.timeScale = 1f;
            MenuPausa.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            pauseMode = false;
        }

        public void Reiniciar()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
