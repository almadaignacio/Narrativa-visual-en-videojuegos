using System.Collections.Generic;
using UnityEngine;

public class TriggerEmissionZone : MonoBehaviour
{
    [Header("Renderers que deben brillar")]
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();

    [Header("Emission Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color emissionColor = Color.white;

    [SerializeField] private float maxIntensity = 2f;
    [SerializeField] private float fadeSpeed = 3f;

    private float currentIntensity = 0f;
    private float targetIntensity = 0f;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        // Seguridad: aseguramos que sea trigger
        GetComponent<Collider>().isTrigger = true;

        // Activamos keyword de emission en materiales compartidos
        foreach (var r in renderers)
        {
            if (r == null) continue;

            foreach (var mat in r.sharedMaterials)
            {
                if (mat != null)
                    mat.EnableKeyword("_EMISSION");
            }
        }

        ApplyEmission(0f);
    }

    private void Update()
    {
        if (Mathf.Approximately(currentIntensity, targetIntensity))
            return;

        currentIntensity = Mathf.MoveTowards(
            currentIntensity,
            targetIntensity,
            fadeSpeed * Time.deltaTime
        );

        ApplyEmission(currentIntensity);
    }

    private void ApplyEmission(float intensity)
    {
        Color finalColor = emissionColor * intensity;

        foreach (var r in renderers)
        {
            if (r == null) continue;

            r.GetPropertyBlock(propertyBlock);

            // Si el renderer tiene varios materiales
            for (int i = 0; i < r.sharedMaterials.Length; i++)
            {
                propertyBlock.SetColor(EmissionID, finalColor);
            }

            r.SetPropertyBlock(propertyBlock);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetIntensity = maxIntensity;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            targetIntensity = 0f;
    }
}
