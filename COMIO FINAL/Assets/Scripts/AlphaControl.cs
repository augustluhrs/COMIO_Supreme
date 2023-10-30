using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaDeactivator : MonoBehaviour
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

    // Serialized field to assign CameraSwitcher from another object.
    [SerializeField]
    public CameraSwitcher cameraSwitcher; 

    private void Update()
    {
        foreach (ImageControlGroup group in controlGroups)
        {
            float alpha = group.controllingImage.color.a;
            bool shouldActivate = alpha >= activationThreshold;

            foreach (GameObject obj in group.targetObjects)
            {
                if (obj.activeSelf != shouldActivate)
                {
                    obj.SetActive(shouldActivate);
                    
                    // If the object is being deactivated due to threshold
                    if (!shouldActivate && cameraSwitcher)
                    {
                        cameraSwitcher.SwitchCameras();
                    }
                }
            }
        }
    }
}
