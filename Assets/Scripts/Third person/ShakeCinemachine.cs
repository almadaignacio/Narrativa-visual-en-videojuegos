using Unity.Cinemachine;
using UnityEngine;
using System.Collections;


public class ShakeCinemachine : MonoBehaviour
{
    public CinemachineCamera cineCam;

    [Header("Shake fuerte temporal")]
    public NoiseSettings strongShakeProfile;
    public float targetAmplitude = 3f;
    public float targetFrequency = 2f;

    [Header("Tiempos")]
    public float fadeInTime = 0.5f;
    public float holdTime = 1.5f;
    public float fadeOutTime = 0.5f;

    private CinemachineBasicMultiChannelPerlin noise;

    private NoiseSettings originalProfile;
    private float originalAmplitude;
    private float originalFrequency;

    void Awake()
    {
        noise = cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void PlayStrongShake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        // Guardar estado actual
        originalProfile = noise.NoiseProfile;
        originalAmplitude = noise.AmplitudeGain;
        originalFrequency = noise.FrequencyGain;

        // Cambiar perfil
        noise.NoiseProfile = strongShakeProfile;

        // FADE IN
        float time = 0f;
        while (time < fadeInTime)
        {
            float t = time / fadeInTime;

            noise.AmplitudeGain = Mathf.Lerp(originalAmplitude, targetAmplitude, t);
            noise.FrequencyGain = Mathf.Lerp(originalFrequency, targetFrequency, t);

            time += Time.deltaTime;
            yield return null;
        }

        noise.AmplitudeGain = targetAmplitude;
        noise.FrequencyGain = targetFrequency;

        // HOLD
        yield return new WaitForSeconds(holdTime);

        // FADE OUT
        time = 0f;
        while (time < fadeOutTime)
        {
            float t = time / fadeOutTime;

            noise.AmplitudeGain = Mathf.Lerp(targetAmplitude, originalAmplitude, t);
            noise.FrequencyGain = Mathf.Lerp(targetFrequency, originalFrequency, t);

            time += Time.deltaTime;
            yield return null;
        }

        // Restaurar
        noise.AmplitudeGain = originalAmplitude;
        noise.FrequencyGain = originalFrequency;
        noise.NoiseProfile = originalProfile;
    }
}