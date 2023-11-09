using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // This namespace is for URP's Volume

public class PS4Controller : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float orthoSizeChangeRate = 0.1f;
    public float effectStrengthChangeRate = 0.1f; // Rate at which the post-processing effect changes

    public GameObject volumeGameObject; // Assign the GameObject with the Volume component in the Inspector

    private Gamepad gamepad;
    private Camera[] cameras;
    private Volume volumeComponent; // Reference to the Volume component

    public List<GameObject> toggleObjects; // Assign this list in the Inspector with all the GameObjects you want to toggle
    private bool isRandomToggleActive = false;
    private bool isBackgroundUninitialized = false;

    private Coroutine randomToggleCoroutine;

    private bool isOrthographic = false;

    public CameraSwitcher cameraSwitcher;

    private void Start()
    {
        gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.LogWarning("No gamepad connected.");
            return;
        }

        cameras = GetComponentsInChildren<Camera>(true);

        // Assign the Volume component from the provided GameObject
        if (volumeGameObject != null)
        {
            volumeComponent = volumeGameObject.GetComponent<Volume>();
            if (volumeComponent == null)
            {
                Debug.LogError("Volume component not found on the referenced GameObject.");
            }
        }
        else
        {
            Debug.LogError("Volume GameObject reference not set in the Inspector.");
        }
    }

    void Update()
    {
        if (gamepad == null) return;


    // Toggle random activation state with the square button
if (gamepad.buttonWest.wasPressedThisFrame)
{
    isRandomToggleActive = !isRandomToggleActive; // Toggle the state

    if (isRandomToggleActive)
    {
        // Start the random toggle routine
        randomToggleCoroutine = StartCoroutine(RandomToggleRoutine());
    }
    else
    {
        // Stop the random toggle routine if it's running
        if (randomToggleCoroutine != null)
        {
            StopCoroutine(randomToggleCoroutine);
        }

        // Deactivate all objects and stop all audio when exiting the random toggle state
        foreach (var obj in toggleObjects)
        {
            if (obj != null)
            {
                var rawImage = obj.GetComponentInChildren<RawImage>(true);
                if (rawImage != null)
                {
                    rawImage.enabled = false;
                }

                // Stop the AudioSource as well
                AudioSource audio = obj.GetComponent<AudioSource>();
                if (audio != null && audio.isPlaying)
                {
                    audio.Stop();
                }
            }
        }
    }
}


        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            ToggleCameraBackground();
        }



        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            ToggleCameraMode();
        }

          if (gamepad.buttonNorth.wasPressedThisFrame)
        {
            cameraSwitcher.SwitchCameras();
        }

        float sizeChange = (gamepad.leftTrigger.ReadValue() - gamepad.rightTrigger.ReadValue()) * orthoSizeChangeRate;
        if (isOrthographic)
        {
            foreach (Camera cam in cameras)
            {
                if (cam.orthographic)
                {
                    cam.orthographicSize = Mathf.Max(0.1f, cam.orthographicSize + sizeChange);
                }
            }
        }

    if (volumeComponent != null)
        {
            if (gamepad.rightShoulder.isPressed)
            {
                AdjustEffectStrength(effectStrengthChangeRate * Time.deltaTime);
            }
            else if (gamepad.leftShoulder.isPressed)
            {
                AdjustEffectStrength(-effectStrengthChangeRate * Time.deltaTime);
            }
        }
    }

    private void AdjustEffectStrength(float change)
    {
        if (volumeComponent.profile.TryGet<LensDistortion>(out var lensDistortion))
        {
            lensDistortion.intensity.value = Mathf.Clamp(lensDistortion.intensity.value + change, -1.0f, 1.0f); // Set range appropriately
            lensDistortion.scale.value = Mathf.Clamp(lensDistortion.scale.value + change, 0f, 5.0f);
            lensDistortion.xMultiplier.value = Mathf.Clamp(lensDistortion.xMultiplier.value + change, 0f, 1.0f);
            lensDistortion.yMultiplier.value = Mathf.Clamp(lensDistortion.yMultiplier.value + change, 0f, 1.0f);  // Set range appropriately
        }
         if (volumeComponent.profile.TryGet<ChromaticAberration>(out var chromaticAberration))
        {
            chromaticAberration.intensity.value = Mathf.Clamp(chromaticAberration.intensity.value + change, 0.0f, 1.0f); // Set range appropriately
        }

    }

    void FixedUpdate()
    {
        if (gamepad != null)
        {
            Vector2 moveInput = gamepad.leftStick.ReadValue();
            Vector2 rotateInput = gamepad.rightStick.ReadValue();

            Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x) * moveSpeed;
            transform.Translate(moveDirection * Time.fixedDeltaTime, Space.World);

            float yaw = rotateInput.x * rotateSpeed;
            float pitch = -rotateInput.y * rotateSpeed;
            transform.Rotate(pitch, yaw, 0, Space.Self);
        }
    }

    private void ToggleCameraMode()
    {
        isOrthographic = !isOrthographic;
        foreach (Camera cam in cameras)
        {
            if (cam != null)
            {
                cam.orthographic = isOrthographic;
            }
        }
    }

private List<RawImage> eligibleRawImages = new List<RawImage>(); // Holds parent RawImages with eligible children

private IEnumerator RandomToggleRoutine()
{
    // Clear the list and check the alpha values at the initiation of the coroutine
    eligibleRawImages.Clear();
    foreach (var parentObj in toggleObjects)
    {
        if (parentObj != null)
        {
            // Check all RawImage components in children
            RawImage[] childRawImages = parentObj.GetComponentsInChildren<RawImage>();
            foreach (var childRawImage in childRawImages)
            {
                if (childRawImage.color.a >= 0.5f)
                {
                    // If a child has the minimum alpha, add the parent's RawImage to the eligible list
                    var parentRawImage = parentObj.GetComponent<RawImage>();
                    if (parentRawImage != null && !eligibleRawImages.Contains(parentRawImage))
                    {
                        eligibleRawImages.Add(parentRawImage);
                    }
                    break; // No need to check other children, one is enough to make the parent eligible
                }
            }
        }
    }

    // Proceed with the random toggling using the eligible list
 while (isRandomToggleActive)
    {
        foreach (var rawImage in eligibleRawImages)
        {
            if (rawImage != null)
            {
                bool shouldBeEnabled = Random.value > 0.5f;
                rawImage.enabled = shouldBeEnabled;

                AudioSource audio = rawImage.GetComponent<AudioSource>();
                if (audio != null)
                {
                    if (shouldBeEnabled)
                    {
                        if (!audio.isPlaying)
                        {
                            audio.time = Random.Range(0f, audio.clip.length);
                            audio.Play();
                        }
                    }
                    else if (audio.isPlaying)
                    {
                        // If the RawImage is disabled, we should stop the audio as well.
                        audio.Stop();
                    }
                }
            }
        }

        // Wait for a short random duration before toggling again
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
    }

    // Disable all eligible parent RawImage components and stop corresponding audio when exiting the random toggle state
    foreach (var rawImage in eligibleRawImages)
    {
        if (rawImage != null)
        {
            rawImage.enabled = false;
            AudioSource audio = rawImage.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Stop();
            }
        }
    }
}


private void ToggleCameraBackground()
{
    foreach (Camera cam in cameras)
    {
        if (isBackgroundUninitialized)
        {
            // Set to black with full transparency
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0); // Black with alpha 0
            if (volumeComponent.profile.TryGet<LiftGammaGain>(out var liftGammaGain)){
                liftGammaGain.active = false;
            }
            if (volumeComponent.profile.TryGet<SplitToning>(out var splitToning)){
                splitToning.active = false;
            }
             if (volumeComponent.profile.TryGet<ColorAdjustments>(out var colorAdjustments)){
                colorAdjustments.active = false;
            }
            if (volumeComponent.profile.TryGet<FilmGrain>(out var filmGrain)){
                filmGrain.active = false;
            }
        }
        else
        {
            if (volumeComponent.profile.TryGet<LiftGammaGain>(out var liftGammaGain)){
                liftGammaGain.active = true;
            }
            if (volumeComponent.profile.TryGet<SplitToning>(out var splitToning)){
                splitToning.active = true;
            }
            if (volumeComponent.profile.TryGet<ColorAdjustments>(out var colorAdjustments)){
                colorAdjustments.active = true;
            }
            if (volumeComponent.profile.TryGet<FilmGrain>(out var filmGrain)){
                filmGrain.active = true;
            }
            // Set to Uninitialized
            cam.clearFlags = CameraClearFlags.Nothing; // This is equivalent to 'Uninitialized' in the Camera UI
        }
    }

    // Toggle the state
    isBackgroundUninitialized = !isBackgroundUninitialized;
}







}
