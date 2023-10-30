using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DistanceBasedSound : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    private AudioSource audioSource;
    private float[] samples = new float[44100];

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Continually play sound based on distance
        PlayDistanceSound();
    }

    void PlayDistanceSound()
    {
        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        float frequency = distance * 50; // Adjust the multiplier as needed for the desired frequency range.

        GenerateTone(frequency);

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void GenerateTone(float frequency)
    {
        int sampleRate = audioSource.clip != null ? audioSource.clip.frequency : 44100;

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = Mathf.Sin(2 * Mathf.PI * i * frequency / sampleRate);
        }

        AudioClip clip = AudioClip.Create("Tone", samples.Length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        audioSource.clip = clip;
    }
}
