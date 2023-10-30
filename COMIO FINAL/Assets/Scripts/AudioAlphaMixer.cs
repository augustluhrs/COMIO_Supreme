using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioAlphaMixer : MonoBehaviour
{
    public RawImage imageToCheck;

    [Header("Volume Settings in dB")]
    [Range(-80, 0)]
    public float minVolumeInDB = -80f;  // Minimum volume when the image is fully transparent

    [Range(-80, 24)]
    public float maxVolumeInDB = 12f;   // Max volume when the image is fully opaque

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float alpha = imageToCheck.color.a;

        // Convert linear volume scale [0, 1] to decibels.
        float volumeInDB = Mathf.Lerp(minVolumeInDB, maxVolumeInDB, alpha);
        audioSource.volume = Mathf.Pow(10, volumeInDB / 20);
    }
}
