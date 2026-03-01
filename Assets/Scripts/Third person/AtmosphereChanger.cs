using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;
using Unity.Cinemachine;
using AtmosphericHeightFog;

public class AtmosphereChanger : MonoBehaviour
{
    [Header("Trigger")]
    public string playerTag = "Player";
    private bool hasTriggered = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip rockAppearSound;

    [Header("Particles")]
    public ParticleSystem particles;
    public float particleDuration = 3f;

    [Header("Post Processing")]
    public Volume volume;
    public float colorTransitionDuration = 3f;

    private LiftGammaGain liftGammaGain;
    private ShadowsMidtonesHighlights smh;

    private Vector4 originalLift, originalGamma, originalGain;
    private Vector4 originalShadows, originalMidtones, originalHighlights;

    public Color targetLift;
    public Color targetGamma;
    public Color targetGain;
    public Color targetShadows;
    public Color targetMidtones;
    public Color targetHighlights;

    [Header("Fog")]
    public HeightFogGlobal fog;
    public float targetFogIntensity = 0.8f;
    public float fogDuration = 3f;
    private float originalFog;

    [Header("Directional Light")]
    public Light directionalLight;
    public Vector3 targetLightRotation;
    public float targetLightIntensity = 0.5f;
    public float lightDuration = 4f;

    private Quaternion originalLightRotation;
    private float originalLightIntensity;

    [Header("Skybox")]
    public Material currentSkybox;
    public Material newSkybox;
    public float skyboxDuration = 3f;

    private Material originalSkyboxMaterial;
    private float originalSkyboxExposure;

    [Header("Rock")]
    public GameObject rock;
    public Animator rockAnimator;

    [Header("Cinemachine 3 Shake")]
    public CinemachineCamera cineCamera;
    public NoiseSettings strongNoiseProfile;
    public float shakeDuration = 2f;
    public float strongAmplitude = 3f;
    public float strongFrequency = 2f;

    private CinemachineBasicMultiChannelPerlin noiseComponent;
    private float originalAmplitude;
    private float originalFrequency;
    private NoiseSettings originalProfile;

    void Awake()
    {
        if (rock != null)
            rock.SetActive(false);

        if (volume.profile.TryGet(out liftGammaGain))
        {
            originalLift = liftGammaGain.lift.value;
            originalGamma = liftGammaGain.gamma.value;
            originalGain = liftGammaGain.gain.value;
        }

        if (volume.profile.TryGet(out smh))
        {
            originalShadows = smh.shadows.value;
            originalMidtones = smh.midtones.value;
            originalHighlights = smh.highlights.value;
        }

        originalFog = fog.fogIntensity;

        originalLightRotation = directionalLight.transform.rotation;
        originalLightIntensity = directionalLight.intensity;

        // Guardamos skybox original y su exposición
        originalSkyboxMaterial = RenderSettings.skybox;
        if (originalSkyboxMaterial != null && originalSkyboxMaterial.HasProperty("_Exposure"))
            originalSkyboxExposure = originalSkyboxMaterial.GetFloat("_Exposure");

        noiseComponent = cineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noiseComponent != null)
        {
            originalAmplitude = noiseComponent.AmplitudeGain;
            originalFrequency = noiseComponent.FrequencyGain;
            originalProfile = noiseComponent.NoiseProfile;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            StartCoroutine(FullSequence());
        }
    }

    IEnumerator FullSequence()
    {
        StartCoroutine(ShakeRoutine());
        StartCoroutine(ParticleRoutine());
        StartCoroutine(ColorTransition());
        StartCoroutine(FogTransition());
        StartCoroutine(RotateLightAndIntensity());

        yield return StartCoroutine(SkyboxTransition());

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(RockSequence());

        // 🔥 Restauramos el skybox original al finalizar todo
        RenderSettings.skybox = originalSkyboxMaterial;

        if (originalSkyboxMaterial != null && originalSkyboxMaterial.HasProperty("_Exposure"))
            originalSkyboxMaterial.SetFloat("_Exposure", originalSkyboxExposure);

        DynamicGI.UpdateEnvironment();
    }

    IEnumerator ParticleRoutine()
    {
        if (particles == null) yield break;

        particles.gameObject.SetActive(true);
        particles.Play();

        yield return new WaitForSeconds(particleDuration);

        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particles.gameObject.SetActive(false);
    }

    IEnumerator ShakeRoutine()
    {
        if (noiseComponent == null) yield break;

        noiseComponent.NoiseProfile = strongNoiseProfile;
        noiseComponent.AmplitudeGain = strongAmplitude;
        noiseComponent.FrequencyGain = strongFrequency;

        yield return new WaitForSeconds(shakeDuration);

        noiseComponent.NoiseProfile = originalProfile;
        noiseComponent.AmplitudeGain = originalAmplitude;
        noiseComponent.FrequencyGain = originalFrequency;
    }

    IEnumerator ColorTransition()
    {
        float time = 0f;

        Vector4 liftTarget = ToVector4(targetLift);
        Vector4 gammaTarget = ToVector4(targetGamma);
        Vector4 gainTarget = ToVector4(targetGain);

        Vector4 shadowsTarget = ToVector4(targetShadows);
        Vector4 midtonesTarget = ToVector4(targetMidtones);
        Vector4 highlightsTarget = ToVector4(targetHighlights);

        while (time < colorTransitionDuration)
        {
            float t = time / colorTransitionDuration;

            liftGammaGain.lift.value = Vector4.Lerp(originalLift, liftTarget, t);
            liftGammaGain.gamma.value = Vector4.Lerp(originalGamma, gammaTarget, t);
            liftGammaGain.gain.value = Vector4.Lerp(originalGain, gainTarget, t);

            smh.shadows.value = Vector4.Lerp(originalShadows, shadowsTarget, t);
            smh.midtones.value = Vector4.Lerp(originalMidtones, midtonesTarget, t);
            smh.highlights.value = Vector4.Lerp(originalHighlights, highlightsTarget, t);

            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FogTransition()
    {
        float time = 0f;

        while (time < fogDuration)
        {
            float t = time / fogDuration;
            fog.fogIntensity = Mathf.Lerp(originalFog, targetFogIntensity, t);

            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RotateLightAndIntensity()
    {
        float time = 0f;
        Quaternion targetRot = Quaternion.Euler(targetLightRotation);

        while (time < lightDuration)
        {
            float t = time / lightDuration;

            directionalLight.transform.rotation =
                Quaternion.Slerp(originalLightRotation, targetRot, t);

            directionalLight.intensity =
                Mathf.Lerp(originalLightIntensity, targetLightIntensity, t);

            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SkyboxTransition()
    {
        float half = skyboxDuration / 2f;
        float time = 0f;

        float originalExposure = 1f;
        if (currentSkybox != null && currentSkybox.HasProperty("_Exposure"))
            originalExposure = currentSkybox.GetFloat("_Exposure");

        // Oscurece el actual
        while (time < half)
        {
            float t = time / half;
            if (currentSkybox.HasProperty("_Exposure"))
                currentSkybox.SetFloat("_Exposure", Mathf.Lerp(originalExposure, 0f, t));

            time += Time.deltaTime;
            yield return null;
        }

        // Cambia al nuevo
        RenderSettings.skybox = newSkybox;
        DynamicGI.UpdateEnvironment();

        time = 0f;

        // Ilumina el nuevo
        while (time < half)
        {
            float t = time / half;
            if (newSkybox.HasProperty("_Exposure"))
                newSkybox.SetFloat("_Exposure", Mathf.Lerp(0f, originalExposure, t));

            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RockSequence()
    {
        rock.SetActive(true);

        if (audioSource != null && rockAppearSound != null)
            audioSource.PlayOneShot(rockAppearSound);

        if (rockAnimator != null)
            rockAnimator.SetTrigger("Break");

        yield return null;
    }

    Vector4 ToVector4(Color c)
    {
        return new Vector4(c.r, c.g, c.b, 0);
    }
}
