using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;

public class AlphaControl : MonoBehaviour
{
    [System.Serializable]
    public class ImageControlGroup
    {
        public RawImage controllingImage;
        public List<GameObject> targetObjects = new List<GameObject>();
    }

    public List<ImageControlGroup> controlGroups = new List<ImageControlGroup>();
    [Range(0f, 1f)]
    public float activationThreshold = 0.5f;
    public int midiKnobNumber = 0;  // The number of the MIDI knob you want to use

    private float lastMidiValue;

    private void Update()
    {
        float midiValue = MidiMaster.GetKnob(midiKnobNumber, 0f);  // Get the value of the MIDI knob
        if (midiValue != lastMidiValue)  // Check if the MIDI knob value has changed
        {
            lastMidiValue = midiValue;
            OnMidiKnobValueChanged(midiValue);
        }
    }

    public void OnMidiKnobValueChanged(float newValue)
    {
        // Assuming that a lower MIDI knob value corresponds to a lower alpha value
        foreach (ImageControlGroup group in controlGroups)
        {
            // Set the alpha value of the controlling image based on the MIDI knob value
            Color color = group.controllingImage.color;
            color.a = newValue;
            group.controllingImage.color = color;

            CheckAlphaAndUpdateObjects(group);
        }
    }

    private void CheckAlphaAndUpdateObjects(ImageControlGroup group)
    {
        float alpha = group.controllingImage.color.a;
        bool shouldActivate = alpha >= activationThreshold;

        foreach (GameObject obj in group.targetObjects)
        {
            if (obj.activeSelf != shouldActivate)
            {
                obj.SetActive(shouldActivate);
            }
        }
    }
}
