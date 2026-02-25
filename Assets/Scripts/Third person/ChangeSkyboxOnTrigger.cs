using UnityEngine;

public class ChangeSkyboxOnTrigger : MonoBehaviour
{
    [Header("Nuevo Skybox")]
    public Material newSkybox;

    [Header("Opcional - Suavizado")]
    public bool updateAmbientLighting = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeSkybox();
        }
    }

    void ChangeSkybox()
    {
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;

            if (updateAmbientLighting)
            {
                DynamicGI.UpdateEnvironment();
            }

            Debug.Log("Skybox cambiado correctamente");
        }
        else
        {
            Debug.LogWarning("No hay Skybox asignado");
        }
    }
}