using UnityEngine;
using UnityEngine.UI;

public class OSC_DistanceBasedOpacityMult : MonoBehaviour
{
    [Header("Canvas Control")]
    public RawImage targetImage;
    
    public float Range = 0.0f;
    public float smoothFactor = 5.0f;  // Controls the speed of smoothing.

    public float currentDistance = 0.0f;
    public float opacityValue = 0.0f;

    public Vector3 IncomingPelvisPosition = new Vector3(0, 0, 0);
    public Vector3 originHolder = new Vector3(-1.0f, 0, 0);

    private float targetOpacity = 0.0f;  // Target value after calculation.


    private void Start()
    {
        IncomingPelvisPosition = GameObject.Find("AvatarManager").GetComponent<BodyDataManager>().incomingPelvisPos;
    }

    private void Update()
    {
        CheckDistance(IncomingPelvisPosition);
    }

    private void CheckDistance(Vector3 oscReceiverPosition)
    {
        
        float distanceToTarget = Vector3.Distance(oscReceiverPosition, originHolder);
        currentDistance = distanceToTarget;

        if(distanceToTarget < Range)
        {
            targetOpacity = 1 - (distanceToTarget / Range);
        }
        else
        {
            targetOpacity = 0.0f;  // If distance is beyond range, make it transparent.
        }

        // Smoothing the opacity change over time.
        opacityValue = Mathf.Lerp(opacityValue, targetOpacity, smoothFactor * Time.deltaTime);
        SetOpacity(opacityValue);
    }

    private void SetOpacity(float value)
    {
        Color currentColor = targetImage.color;
        currentColor.a = Mathf.Clamp01(value);
        targetImage.color = currentColor;
    }
}
