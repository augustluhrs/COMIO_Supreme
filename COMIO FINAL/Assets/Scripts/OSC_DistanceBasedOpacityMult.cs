using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OSC_DistanceBasedOpacityMult : MonoBehaviour
{
    [Header("Canvas Control")]
    public RawImage targetImage; // The UI canvas element whose transparency you want to control.

    [Header("Distance Control")]
    public float maxDistanceForTransparency = 1.0f; // Distance threshold for opacity change.

    [Header("OSC Receivers")]
    public List<GameObject> oscReceivers; // List of OSC receivers to monitor.

    public Transform distanceCheckTarget; //
    
    private float furthestDistance = 0.0f;

    private void Update()
    {
        furthestDistance = 0.0f;

        // Iterate through all OSC Transforms and calculate distances.
        foreach (var oscReceiver in oscReceivers)
        {
            if (oscReceiver == null)
            {
                // Skip null references.
                continue;
            }

            // Calculate distance from the OSC object to the distanceCheckTarget.
            float distanceToTarget = Vector3.Distance(oscReceiver.transform.position, distanceCheckTarget.position);

            if (distanceToTarget <= maxDistanceForTransparency)
            {
                // Update the furthest distance within the threshold.
                furthestDistance = Mathf.Max(furthestDistance, distanceToTarget);
            }
            
        }

        // Calculate opacity based on the furthest distance within the threshold.
        float opacityValue = 1 - (furthestDistance / maxDistanceForTransparency);
        SetOpacity(opacityValue);
    }

    private void SetOpacity(float value)
    {
        Color currentColor = targetImage.color;
        currentColor.a = Mathf.Clamp01(value); // Ensure value is between 0 and 1.
        targetImage.color = currentColor;
    }

    // You can add OSC receivers to the list using the Unity Inspector.
}
