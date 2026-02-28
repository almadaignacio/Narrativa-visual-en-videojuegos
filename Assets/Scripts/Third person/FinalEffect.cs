using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class FinalEffect : MonoBehaviour
{
    [Header("Referencias")]
    public Volume volume;

    [Header("Lens Distortion")]
    [Range(-1f, 1f)]
    public float maxLensDistortion = -0.5f;

    [Header("Color Adjustments")]
    [Range(-100f, 100f)]
    public float maxContrastReduction = -40f;

    [Range(-100f, 100f)]
    public float maxSaturation = -50f; // Negativo = desaturado

    [Header("Velocidad transición")]
    public float transitionSpeed = 2f;

    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;

    private float targetWeight = 0f;
    private float currentWeight = 0f;

    void Start()
    {
        if (volume.profile.TryGet(out lensDistortion))
            lensDistortion.intensity.value = 0f;

        if (volume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.contrast.value = 0f;
            colorAdjustments.saturation.value = 0f;
        }

        volume.weight = 0f;
    }

    void Update()
    {
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);
        volume.weight = currentWeight;

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(0f, maxLensDistortion, currentWeight);

        if (colorAdjustments != null)
        {
            colorAdjustments.contrast.value = Mathf.Lerp(0f, maxContrastReduction, currentWeight);
            colorAdjustments.saturation.value = Mathf.Lerp(0f, maxSaturation, currentWeight);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetWeight = 1f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            targetWeight = 0f;
    }
}
