using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class Menucontroller : MonoBehaviour
{
    public bool level;
    public bool panelChange;
    public int indexlevel;
    public GameObject panel;
    void Update()
    {
        if (level)
        {
            ChangeLevel(indexlevel);
        }
       
    }

    public void ChangeLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void Exit()
    {
        Application.Quit();
    }

    // Forzar activar
    public void ShowPanel(GameObject panelUI)
    {
            panelUI.SetActive(true);
    }

    // Forzar desactivar
    public void HidePanel(GameObject panelUI)
    {
            panelUI.SetActive(false);
    }
}
