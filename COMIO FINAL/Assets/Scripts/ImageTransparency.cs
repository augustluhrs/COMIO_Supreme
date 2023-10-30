using UnityEngine;
using UnityEngine.UI;

public class ImageTransparency : MonoBehaviour
{
    public RawImage rawImage;  // Drag your Raw Image here in the inspector

    public float Alpha
    {
        get
        {
            return rawImage.color.a;
        }
        set
        {
            Color color = rawImage.color;
            color.a = Mathf.Clamp01(value);  // Clamp the value to ensure it's between 0 and 1
            rawImage.color = color;
        }
    }

    void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();  // Automatically get the Raw Image component if not set
        }
    }
}
