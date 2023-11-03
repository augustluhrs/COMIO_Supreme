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
    public float popupThreshold = 0.5f;
    public float deactivationThreshold = 0.5f;
    public float cooldownTime = 30f;
    public float displayDuration = 5f;
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    private float timeOfLastPopup;
    private bool isCooldown = false;
    private float lastMidiValue;
    private RawImage controllingImage;

    private void Awake()
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
        float currentAlpha = controllingImage.color.a;
        bool shouldPopup = currentAlpha >= popupThreshold;
        if (!isCooldown && shouldPopup)
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
        Color color = controllingImage.color;
        color.a = newValue;
        controllingImage.color = color;

        float currentAlpha = controllingImage.color.a;
        bool shouldActivate = currentAlpha >= deactivationThreshold;
        bool shouldPopup = currentAlpha >= popupThreshold;

        foreach (ImageControlGroup group in controlGroups)
        {
            foreach (GameObject obj in group.targetObjects)
            {
                if (obj.activeSelf != shouldActivate)
                {
                    obj.SetActive(shouldActivate);
                }
            }
        }

        if (!isCooldown && shouldPopup)
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

            while (popupRawImage.color.a > 0)
            {
                float newAlpha = popupRawImage.color.a - fadeSpeed * Time.deltaTime;
                popupRawImage.color = new Color(popupRawImage.color.r, popupRawImage.color.g, popupRawImage.color.b, newAlpha);
                yield return null;
            }

            popupRawImage.gameObject.SetActive(false);
        }
    }
}
