using System.Collections.Generic;
using UnityEngine;

public class TriggerEmissionZone : MonoBehaviour
{
    [Header("Renderers que deben brillar")]
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();

    [Header("Luces que deben encenderse")]
    [SerializeField] private List<Light> lights = new List<Light>();

    [Header("Emission Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color emissionColor = Color.white;

    [SerializeField] private float maxEmissionIntensity = 2f;

    [Header("Light Target Intensity")]
    [SerializeField] private float targetLightIntensity = 5f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 3f;

    private float currentValue = 0f;
    private float targetValue = 0f;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        GetComponent<Collider>().isTrigger = true;

        // Activar emisión en materiales
        foreach (var r in renderers)
        {
            if (r == null) continue;

            foreach (var mat in r.sharedMaterials)
            {
                if (mat != null)
                    mat.EnableKeyword("_EMISSION");
            }
        }

        // Aseguramos que las luces empiecen apagadas
        foreach (var l in lights)
        {
            if (l == null) continue;
            l.intensity = 0f;
        }

        ApplyEffects(0f);
    }

    private void Update()
    {
        if (Mathf.Approximately(currentValue, targetValue))
            return;

        currentValue = Mathf.MoveTowards(
            currentValue,
            targetValue,
            fadeSpeed * Time.deltaTime
        );

        ApplyEffects(currentValue);
    }

    private void ApplyEffects(float value)
    {
        // -------- EMISSION --------
        float emissionIntensity = value * maxEmissionIntensity;
        Color finalColor = emissionColor * emissionIntensity;

        foreach (var r in renderers)
        {
            if (r == null) continue;

            r.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(EmissionID, finalColor);
            r.SetPropertyBlock(propertyBlock);
        }

        // -------- LIGHTS --------
        float newLightIntensity = Mathf.Lerp(0f, targetLightIntensity, value);

        foreach (var l in lights)
        {
            if (l == null) continue;
            l.intensity = newLightIntensity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetValue = 1f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            targetValue = 0f;
    }
}
