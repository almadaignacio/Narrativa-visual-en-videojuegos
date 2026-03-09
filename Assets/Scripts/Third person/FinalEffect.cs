using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;

public class FinalEffect : MonoBehaviour
{
    [Header("Trigger")]
    public string playerTag = "Player";

    [Header("Volume")]
    public Volume volume;

    [Header("Lens Distortion")]
    [Range(-1f, 1f)]
    public float maxLensDistortion = -0.5f;

    [Header("Color Adjustments")]
    [Range(-100f, 100f)]
    public float maxContrast = -40f;

    [Range(-100f, 100f)]
    public float maxSaturation = -50f;

    [Header("Transition Speed")]
    public float transitionSpeed = 2f;

    [Header("Skybox")]
    public Material newSkybox;
    public float skyboxTransitionDuration = 3f;

    private Material originalSkybox;
    private float originalExposure;

    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;

    private float originalLens;
    private float originalContrast;
    private float originalSaturation;
    private float originalVolumeWeight;

    private float targetWeight;
    private float currentWeight;

    private bool playerInside = false;
    private bool skyboxChanged = false;

    void Awake()
    {
        volume.profile = Instantiate(volume.profile);

        // Guardar skybox original
        originalSkybox = RenderSettings.skybox;

        if (originalSkybox.HasProperty("_Exposure"))
            originalExposure = originalSkybox.GetFloat("_Exposure");

        // Obtener overrides
        if (volume.profile.TryGet(out lensDistortion))
        {
            originalLens = lensDistortion.intensity.value;
            lensDistortion.intensity.value = 0f; // empezar sin efecto
        }

        if (volume.profile.TryGet(out colorAdjustments))
        {
            originalContrast = colorAdjustments.contrast.value;
            originalSaturation = colorAdjustments.saturation.value;

            colorAdjustments.contrast.value = 0f; // empezar normal
            colorAdjustments.saturation.value = 0f;
        }

        // Guardar weight original del volume
        originalVolumeWeight = volume.weight;

        currentWeight = volume.weight;
        targetWeight = volume.weight; 
    }

    void Update()
    {
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);
        volume.weight = currentWeight;

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(originalLens, maxLensDistortion, currentWeight);

        if (colorAdjustments != null)
        {
            colorAdjustments.contrast.value = Mathf.Lerp(originalContrast, maxContrast, currentWeight);
            colorAdjustments.saturation.value = Mathf.Lerp(originalSaturation, maxSaturation, currentWeight);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = true;
        targetWeight = 1f;

        if (!skyboxChanged)
            StartCoroutine(SkyboxBlend(true));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = false;
        targetWeight = 0f;

        if (skyboxChanged)
            StartCoroutine(SkyboxBlend(false));
    }

    IEnumerator SkyboxBlend(bool entering)
    {
        skyboxChanged = true;

        float half = skyboxTransitionDuration / 2f;
        float time = 0f;

        Material fromMat = entering ? originalSkybox : newSkybox;
        Material toMat = entering ? newSkybox : originalSkybox;

        float fromExposure = fromMat.HasProperty("_Exposure") ? fromMat.GetFloat("_Exposure") : 1f;

        while (time < half)
        {
            float t = time / half;
            fromMat.SetFloat("_Exposure", Mathf.Lerp(fromExposure, 0f, t));
            time += Time.deltaTime;
            yield return null;
        }

        RenderSettings.skybox = toMat;
        DynamicGI.UpdateEnvironment();

        time = 0f;

        while (time < half)
        {
            float t = time / half;
            toMat.SetFloat("_Exposure", Mathf.Lerp(0f, originalExposure, t));
            time += Time.deltaTime;
            yield return null;
        }

        if (!entering)
        {
            originalSkybox.SetFloat("_Exposure", originalExposure);
            skyboxChanged = false;
        }
    }
}
