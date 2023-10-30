using UnityEngine;
using UnityEngine.UI;
using MidiJack; // Ensure you have the MidiJack namespace.

public class MidiOpacityControl : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private int knobNumber = 0; // Default knob number.

    private void Update()
    {
        float midiValue = MidiMaster.GetKnob(knobNumber, 0); // This returns a value between 0 and 1.
        SetOpacity(midiValue);
    }

    private void SetOpacity(float value)
    {
        if (rawImage != null)
        {
            Color currentColor = rawImage.color;
            currentColor.a = value; // Set the alpha value.
            rawImage.color = currentColor;
        }
    }
}
