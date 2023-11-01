using UnityEngine;
using MidiJack;

public class RotateAroundTarget : MonoBehaviour
{
    public Transform target; // The target to rotate around and look at
    [SerializeField] private int knobNumber = 0;
    public AudioSource[] audioSources; // An array of AudioSources whose pitch you want to change
    public bool usingMidi = false;
    public float fixedSpeed = 1.0f;

    // Define the range of pitch values you'd like to use:
    private const float minPitch = -3.0f;  // Slowest pitch (can adjust)
    private const float maxPitch = 3.0f;  // Fastest pitch (can adjust)

    private void Update()
    {
        if (target == null) return;

            float knobValue = MidiMaster.GetKnob(knobNumber, 0); 
            float mappedValue = Remap(knobValue, 0, 1, -30, 30);

        if(usingMidi == true){
            // Rotate around the target
            transform.RotateAround(target.position, Vector3.up, mappedValue * Time.deltaTime);
        }else{
            transform.RotateAround(target.position, Vector3.up, fixedSpeed * Time.deltaTime);
        }
        // Look at the target
        transform.LookAt(target);

        // Adjust the pitch of all audio sources based on mappedValue
        float pitchValue = Remap(mappedValue, -30, 30, minPitch, maxPitch);
        foreach (AudioSource audio in audioSources)
        {
            audio.pitch = pitchValue;
        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
