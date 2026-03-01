using AtmosphericHeightFog;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]

public class FirstEvent : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string playerTag = "Player";
    private bool hasTriggered = false;

    [Header("Particles")]
    public List<ParticleSystem> particlesToActivate = new List<ParticleSystem>();
    public float particlesDuration = 3f;

    [Header("Objects To Disable")]
    public List<GameObject> objectsToDisable = new List<GameObject>();

    [Header("Objects To Enable")]
    public List<GameObject> objectsToEnable = new List<GameObject>();

    [Header("Fog Settings")]
    public HeightFogGlobal heightFog;
    [Range(0, 1)]
    public float targetFogIntensity = 1f;
    public float fogTransitionSpeed = 1f;
    public float fogHoldDuration = 3f; // Tiempo que se mantiene en intensidad máxima

    private float originalFogIntensity;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip triggerSound;

    private void Start()
    {
        if (heightFog != null)
        {
            originalFogIntensity = heightFog.fogIntensity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            StartCoroutine(TriggerSequence());
        }
    }

    private IEnumerator TriggerSequence()
    {
        // 🔊 Reproducir sonido
        if (audioSource != null && triggerSound != null)
        {
            audioSource.clip = triggerSound;
            audioSource.Play();
        }

        // 🔥 Activar partículas
        foreach (ParticleSystem ps in particlesToActivate)
        {
            if (ps != null)
                ps.Play();
        }

        // 🌫 Subir Fog
        if (heightFog != null)
        {
            yield return StartCoroutine(LerpFog(originalFogIntensity, targetFogIntensity));
        }

        // ⏳ Mantener fog en intensidad elegida
        yield return new WaitForSeconds(fogHoldDuration);

        // 🌫 Bajar Fog nuevamente
        if (heightFog != null)
        {
            yield return StartCoroutine(LerpFog(targetFogIntensity, originalFogIntensity));
        }

        // ⏳ Esperar duración partículas
        yield return new WaitForSeconds(particlesDuration);

        // 💨 Detener partículas
        foreach (ParticleSystem ps in particlesToActivate)
        {
            if (ps != null)
                ps.Stop();
        }

        // 🚫 Desactivar objetos
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // ✅ Activar otros objetos
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    private IEnumerator LerpFog(float startValue, float endValue)
    {
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * fogTransitionSpeed;
            heightFog.fogIntensity = Mathf.Lerp(startValue, endValue, time);
            yield return null;
        }

        heightFog.fogIntensity = endValue;
    }
}
