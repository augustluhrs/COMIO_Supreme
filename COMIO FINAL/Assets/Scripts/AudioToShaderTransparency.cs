using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AudioToShaderTransparency : MonoBehaviour
{
    public float sensitivity = 100;
    public float lerpSpeed = 1.0f;
    public AudioSource externalAudioSource; // Public field to drag an external AudioSource

    private Material _material;
    private float[] _samples = new float[512];
    private static readonly int TransparencyID = Shader.PropertyToID("_Transparency");

    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;

        // Test if we're accessing the material and setting the transparency correctly
        _material.SetFloat(TransparencyID, 0.5f);

        if (externalAudioSource.clip == null)
        {
            Debug.LogError("No AudioClip assigned to the external AudioSource!");
        }
    }

    void Update()
    {
        if (externalAudioSource && externalAudioSource.isPlaying)
        {
            externalAudioSource.GetOutputData(_samples, 0);
            float average = 0;
            foreach (var sample in _samples)
            {
                average += Mathf.Abs(sample);
            }
            average /= _samples.Length;

            float newTransparency = average * sensitivity;
            float currentTransparency = _material.GetFloat(TransparencyID);
            float targetTransparency = Mathf.Lerp(currentTransparency, newTransparency, lerpSpeed * Time.deltaTime);
            _material.SetFloat(TransparencyID, targetTransparency);
        }
    }
}
