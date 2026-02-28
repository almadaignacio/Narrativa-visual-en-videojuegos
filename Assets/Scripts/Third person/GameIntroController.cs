using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameIntroController : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Camera Shake")]
    [SerializeField] private CinemachineCamera cineCamera;
    [SerializeField] private NoiseSettings shakeProfile;
    [SerializeField] private float shakeAmplitude = 2f;
    [SerializeField] private float shakeDuration = 1.5f;
    [SerializeField] private float shakeBlendSpeed = 1f;

    [Header("Particles")]
    [SerializeField] private List<GameObject> particlePrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float particlesDuration = 3f;

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour physicsCharacterController;
    [SerializeField] private float disableDuration = 4f;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shakeSound;
    [SerializeField] private float audioFadeInDuration = 0.5f;
    [SerializeField] private float audioFadeOutDuration = 0.5f;
    [SerializeField] private float maxVolume = 1f;

    private CinemachineBasicMultiChannelPerlin perlin;
    private NoiseSettings originalProfile;
    private float originalAmplitude;

    private List<GameObject> spawnedParticles = new List<GameObject>();

    private void Start()
    {
        if (physicsCharacterController != null)
            physicsCharacterController.enabled = false;

        perlin = cineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (perlin != null)
        {
            originalProfile = perlin.NoiseProfile;
            originalAmplitude = perlin.AmplitudeGain;
        }

        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        yield return StartCoroutine(FadeIn());
        yield return StartCoroutine(CameraShake());

        SpawnParticles();

        yield return new WaitForSeconds(particlesDuration);

        DisableParticles();

        yield return new WaitForSeconds(disableDuration);

        if (physicsCharacterController != null)
            physicsCharacterController.enabled = true;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            color.a = alpha;
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        fadePanel.color = color;
    }

    private IEnumerator CameraShake()
    {
        if (perlin == null)
            yield break;

        perlin.NoiseProfile = shakeProfile;

        if (audioSource != null && shakeSound != null)
        {
            audioSource.clip = shakeSound;
            audioSource.volume = 0f;
            audioSource.Play();
        }

        float elapsed = 0f;

        // FADE IN AUDIO + SHAKE
        while (elapsed < audioFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / audioFadeInDuration);

            perlin.AmplitudeGain = Mathf.Lerp(originalAmplitude, shakeAmplitude, t);

            if (audioSource != null)
                audioSource.volume = Mathf.Lerp(0f, maxVolume, t);

            yield return null;
        }

        // Esperar hasta que falte el tiempo del fade out para que termine el clip
        if (shakeSound != null)
        {
            float waitTime = shakeSound.length - audioFadeOutDuration - audioFadeInDuration;
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);
        }

        // FADE OUT
        elapsed = 0f;
        while (elapsed < audioFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / audioFadeOutDuration);

            perlin.AmplitudeGain = Mathf.Lerp(shakeAmplitude, originalAmplitude, t);

            if (audioSource != null)
                audioSource.volume = Mathf.Lerp(maxVolume, 0f, t);

            yield return null;
        }

        perlin.AmplitudeGain = originalAmplitude;
        perlin.NoiseProfile = originalProfile;
    }

    private void SpawnParticles()
    {
        foreach (var prefab in particlePrefabs)
        {
            GameObject instance = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            spawnedParticles.Add(instance);
        }
    }

    private void DisableParticles()
    {
        foreach (var particle in spawnedParticles)
        {
            if (particle != null)
                particle.SetActive(false);
        }
    }

}
