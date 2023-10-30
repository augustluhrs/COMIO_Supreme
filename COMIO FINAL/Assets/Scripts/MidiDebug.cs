using UnityEngine;
using MidiJack;

public class MidiDebug : MonoBehaviour
{
    private float[] previousKnobValues = new float[128]; // Store previous values

    void Update()
    {
        for (int i = 0; i < 128; i++)  // MIDI has 128 possible control change numbers
        {
            float currentValue = MidiMaster.GetKnob(MidiChannel.All, i);

            if (currentValue != previousKnobValues[i]) // If the knob value has changed since the last frame
            {
                Debug.Log($"Knob {i} has value: {currentValue}");
                previousKnobValues[i] = currentValue; // Update the stored value
            }
        }
    }
}
