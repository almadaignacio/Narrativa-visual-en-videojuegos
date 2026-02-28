using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SurfaceAudio
{
    public string surfaceTag;
    public AudioClip[] clips;
}

public class FootStepSound : MonoBehaviour
{
    public AudioSource audioSource;
    public SurfaceAudio[] surfaces;

    [Range(0.95f, 1.05f)]
    public float pitchMin = 0.97f;
    public float pitchMax = 1.03f;

    private Dictionary<string, AudioClip[]> surfaceDictionary;

    void Awake()
    {
        surfaceDictionary = new Dictionary<string, AudioClip[]>();

        foreach (var surface in surfaces)
        {
            surfaceDictionary.Add(surface.surfaceTag, surface.clips);
        }
    }

    public void PlayFootstep(string surfaceTag)
    {
        if (!surfaceDictionary.ContainsKey(surfaceTag)) return;

        AudioClip[] clips = surfaceDictionary[surfaceTag];
        if (clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(clip);
    }
}
