using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;

public class MidiOpacityControl : MonoBehaviour
{
    [System.Serializable]
    public class ImageControlGroup
    {
        public List<GameObject> targetObjects = new List<GameObject>();
    }

    public List<ImageControlGroup> controlGroups = new List<ImageControlGroup>();
    [SerializeField] RawImage popupRawImage;
    public int midiKnobNumber = 0;
    public float popupThreshold = 0.9f;
    public float deactivationThreshold = 0.1f;
    public float cooldownTime = 1f;
    public float displayDuration = 0.5f;
    public float fadeDuration = 0.4f;

    private AudioSource audioSource;
    private float timeOfLastPopup;
    private bool isCooldown = false;
    private float lastMidiValue;
    private RawImage controllingImage;

    private void Start()
    {
        controllingImage = GetComponent<RawImage>();
        if (popupRawImage != null)
        {
            audioSource = popupRawImage.gameObject.GetComponent<AudioSource>();
            popupRawImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Popup RawImage not assigned in the inspector", this);
        }
    }

    private void Update()
    {
        float midiValue = MidiMaster.GetKnob(midiKnobNumber, 0f);
        if (midiValue != lastMidiValue)
        {
            lastMidiValue = midiValue;
            OnMidiKnobValueChanged(midiValue);
        }

        // Check to trigger popup again after cooldown
        if (!isCooldown && controllingImage.color.a >= popupThreshold)
        {
            TriggerPopup();
        }
        else if (isCooldown && Time.time - timeOfLastPopup > cooldownTime)
        {
            isCooldown = false;
        }
    }

    public void OnMidiKnobValueChanged(float newValue)
    {
        if (float.IsNaN(newValue))
        {
            Debug.LogError("Midi value is NaN, cannot update alpha.");
            return;
        }

        Color color = controllingImage.color;
        color.a = newValue;
        controllingImage.color = color;

        foreach (ImageControlGroup group in controlGroups)
        {
            foreach (GameObject obj in group.targetObjects)
            {
                bool shouldBeActive = newValue >= deactivationThreshold;
                if (obj.activeSelf != shouldBeActive)
                {
                    obj.SetActive(shouldBeActive);
                }
            }
        }

        if (!isCooldown && newValue >= popupThreshold)
        {
            TriggerPopup();
        }

        if (isCooldown && Time.time - timeOfLastPopup > cooldownTime)
        {
            isCooldown = false;
        }
    }

    private void TriggerPopup()
    {
        if (popupRawImage != null)
        {
            Color rawImageColor = popupRawImage.color;
            if (!IsValidColor(rawImageColor))
            {
                Debug.LogError("Invalid color detected: " + rawImageColor);
                return;
            }

            popupRawImage.color = new Color(rawImageColor.r, rawImageColor.g, rawImageColor.b, 1f);
            popupRawImage.gameObject.SetActive(true);
            if (audioSource != null)
            {
                audioSource.Play();
            }
            timeOfLastPopup = Time.time;
            isCooldown = true;
            Invoke("StartFadeOut", displayDuration);
        }
    }

    private void StartFadeOut()
    {
        if (popupRawImage != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        if (popupRawImage != null)
        {
            float fadeSpeed = 1 / fadeDuration;
            Color startColor = popupRawImage.color;

            while (popupRawImage.color.a > 0)
            {
                float newAlpha = popupRawImage.color.a - fadeSpeed * Time.deltaTime;
                if (float.IsNaN(newAlpha))
                {
                    Debug.LogError("New alpha is NaN during fade out.");
                    newAlpha = 0;  // Reset to 0 to avoid breaking the game
                }
                popupRawImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
                yield return null;
            }

            popupRawImage.gameObject.SetActive(false);
        }
    }

    private bool IsValidColor(Color color)
    {
        return !float.IsNaN(color.r) && !float.IsNaN(color.g) && !float.IsNaN(color.b) && !float.IsNaN(color.a);
    }
}
