using UnityEngine;
using UnityEngine.UI;

public class OSC_DistanceBasedOpacity : MonoBehaviour
{
    [Header("OSC Object")]
    public OSC_TD_Receiver oscReceiver; // Reference to the OSC_TD_Receiver script.

    [Header("Canvas Control")]
    public RawImage targetImage; // The UI canvas element whose transparency you want to control.

    [Header("Distance Control")]
    public Transform distanceCheckTarget; // The object to check distance with.
    public float maxDistanceForTransparency = 1.0f; // Distance threshold for opacity change.

    private void Update()
    {
        if (oscReceiver == null || targetImage == null || distanceCheckTarget == null)
        {
            Debug.LogWarning("Ensure all required references are set for OSC_DistanceBasedOpacity.");
            return;
        }

        // Calculate distance from the OSC object to the distanceCheckTarget.
        float distanceToTarget = Vector3.Distance(oscReceiver.root.transform.position, distanceCheckTarget.position);

        // If within the threshold range, adjust opacity.
        if(distanceToTarget <= maxDistanceForTransparency)
        {
            float opacityValue = 1 - (distanceToTarget / maxDistanceForTransparency);
            SetOpacity(opacityValue);
        }
    }

    private void SetOpacity(float value)
    {
        Color currentColor = targetImage.color;
        currentColor.a = Mathf.Clamp01(value); // Ensure value is between 0 and 1.
        targetImage.color = currentColor;
    }
}
