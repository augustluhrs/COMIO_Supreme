using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThresholdPopup : MonoBehaviour
{
    [SerializeField] RawImage imageToCheck; // Reference to the RawImage
    [SerializeField] RawImage popupRawImage; // Reference to the RawImage you want to show/hide
    public float alphaThreshold = 0.5f;
    public float cooldownTime = 30f;
    public float displayDuration = 5f;
    public float fadeDuration = 1f; // Duration for the fade effect

    private AudioSource audioSource;
    private float timeOfLastPopup;
    private bool isCooldown = false;

    private void Start()
    {
        if (!imageToCheck)
        {
            Debug.LogError("RawImage reference not set!");
            return;
        }

        if (!popupRawImage)
        {
            Debug.LogError("Popup RawImage reference not set!");
            return;
        }

        audioSource = popupRawImage.gameObject.GetComponent<AudioSource>();
        popupRawImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        float currentAlpha = imageToCheck.color.a;


        // Check threshold based on RawImage's alpha and other images' alpha values
        if (!isCooldown && currentAlpha > alphaThreshold )
        {
            TriggerPopup();
        }

        // Check cooldown
        if (isCooldown && Time.time - timeOfLastPopup > cooldownTime)
        {
            isCooldown = false;
        }
    }

    private void TriggerPopup()
    {
        Color rawImageColor = popupRawImage.color;
        popupRawImage.color = new Color(rawImageColor.r, rawImageColor.g, rawImageColor.b, 1f); // Reset alpha to 1
        popupRawImage.gameObject.SetActive(true);
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource component missing on " + popupRawImage.gameObject.name);
        }
        timeOfLastPopup = Time.time;
        isCooldown = true;
        Invoke("StartFadeOut", displayDuration);
    }

    private void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
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
