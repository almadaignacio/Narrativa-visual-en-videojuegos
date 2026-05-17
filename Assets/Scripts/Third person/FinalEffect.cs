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

    private float currentLens;
    private float currentContrast;
    private float currentSaturation;

    private float targetLens;
    private float targetContrast;
    private float targetSaturation;

    private bool skyboxChanged = false;

    void Awake()
    {
        volume.profile = Instantiate(volume.profile);

        originalSkybox = RenderSettings.skybox;

        if (originalSkybox.HasProperty("_Exposure"))
            originalExposure = originalSkybox.GetFloat("_Exposure");

        if (volume.profile.TryGet(out lensDistortion))
        {
            currentLens = lensDistortion.intensity.value;
            targetLens = currentLens;
        }

        if (volume.profile.TryGet(out colorAdjustments))
        {
            currentContrast = colorAdjustments.contrast.value;
            currentSaturation = colorAdjustments.saturation.value;

            targetContrast = currentContrast;
            targetSaturation = currentSaturation;
        }
    }

    void Update()
    {
        if (lensDistortion != null)
        {
            currentLens = Mathf.Lerp(currentLens, targetLens, Time.deltaTime * transitionSpeed);
            lensDistortion.intensity.value = currentLens;
        }

        if (colorAdjustments != null)
        {
            currentContrast = Mathf.Lerp(currentContrast, targetContrast, Time.deltaTime * transitionSpeed);
            currentSaturation = Mathf.Lerp(currentSaturation, targetSaturation, Time.deltaTime * transitionSpeed);

            colorAdjustments.contrast.value = currentContrast;
            colorAdjustments.saturation.value = currentSaturation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        targetLens = maxLensDistortion;
        targetContrast = maxContrast;
        targetSaturation = maxSaturation;

        if (!skyboxChanged)
            StartCoroutine(SkyboxBlend(true));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        targetLens = 0f;
        targetContrast = 0f;
        targetSaturation = 0f;

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

