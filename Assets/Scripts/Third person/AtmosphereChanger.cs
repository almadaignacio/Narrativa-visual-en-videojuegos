using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;

public class AtmosphereChanger : MonoBehaviour
{
    [Header("Volume Global")]
    public Volume volume;

    [Header("Skybox")]
    public Material newSkybox;

    [Header("Partículas")]
    public ParticleSystem particlePrefab;
    public Transform particleSpawnPoint;

    [Header("Cinemachine Shake")]
    public ShakeCinemachine shakeController;

    [Header("Duración transición color")]
    public float colorTransitionDuration = 2f;

    private LiftGammaGain liftGammaGain;
    private ShadowsMidtonesHighlights smh;

    private Vector4 originalLift, originalGamma, originalGain;
    private Vector4 originalShadows, originalMidtones, originalHighlights;

    [Header("Target - Lift Gamma Gain")]
    [ColorUsage(false, true)]
    public Color targetLift;
    [ColorUsage(false, true)]
    public Color targetGamma;
    [ColorUsage(false, true)]
    public Color targetGain;

    [Header("Target - Shadows Midtones Highlights")]
    [ColorUsage(false, true)]
    public Color targetShadows;
    [ColorUsage(false, true)]
    public Color targetMidtones;
    [ColorUsage(false, true)]
    public Color targetHighlights;

    private bool triggered = false;

    void Start()
    {
        // Clonar profile para no modificar el original
        volume.profile = Instantiate(volume.profile);

        volume.profile.TryGet(out liftGammaGain);
        volume.profile.TryGet(out smh);

        // Guardar valores originales
        originalLift = liftGammaGain.lift.value;
        originalGamma = liftGammaGain.gamma.value;
        originalGain = liftGammaGain.gain.value;

        originalShadows = smh.shadows.value;
        originalMidtones = smh.midtones.value;
        originalHighlights = smh.highlights.value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(AtmosphereSequence());
        }
    }

    IEnumerator AtmosphereSequence()
    {
        // 1 Shake Cinemachine
        if (shakeController != null)
        {
            shakeController.PlayStrongShake();
            yield return new WaitForSeconds(shakeController.fadeInTime + shakeController.holdTime + shakeController.fadeOutTime);
        }

        // 2 Partículas
        if (particlePrefab != null && particleSpawnPoint != null)
        {
            Instantiate(particlePrefab, particleSpawnPoint.position, Quaternion.identity);
        }

        // 3️⃣ Transición Color Grading
        yield return StartCoroutine(ColorTransition());

        // 4️⃣ Cambio de Skybox (ahora va al final)
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment();
            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator ColorTransition()
    {
        float time = 0f;
        bool skyboxChanged = false;

        float skyboxTriggerPoint = 0.6f; // 60% de la transición

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

            // 🔥 Cambiar skybox cuando alcanza el porcentaje deseado
            if (!skyboxChanged && t >= skyboxTriggerPoint)
            {
                if (newSkybox != null)
                {
                    RenderSettings.skybox = newSkybox;
                    DynamicGI.UpdateEnvironment();
                }

                skyboxChanged = true;
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Asegurar valores finales exactos
        liftGammaGain.lift.value = liftTarget;
        liftGammaGain.gamma.value = gammaTarget;
        liftGammaGain.gain.value = gainTarget;

        smh.shadows.value = shadowsTarget;
        smh.midtones.value = midtonesTarget;
        smh.highlights.value = highlightsTarget;
    }

    Vector4 ToVector4(Color c)
    {
        return new Vector4(c.r, c.g, c.b, 0f);
    }
}
