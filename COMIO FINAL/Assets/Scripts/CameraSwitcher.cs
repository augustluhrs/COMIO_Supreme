using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class Reality
    {
        public List<Camera> cameras = new List<Camera>();
    }

    public List<Reality> realitiesList = new List<Reality>();

    private Camera[][] realities;

    // Current active camera index for each reality.
    private int currentCameraIndex = 0;

    void Awake()
    {
        // Convert serialized list to 2D array for internal usage
        realities = new Camera[realitiesList.Count][];
        for (int i = 0; i < realitiesList.Count; i++)
        {
            realities[i] = realitiesList[i].cameras.ToArray();
        }

        // Initially activate the first camera of each reality and deactivate the rest.
        for (int i = 0; i < realities.Length; i++)
        {
            for (int j = 0; j < realities[i].Length; j++)
            {
                realities[i][j].gameObject.SetActive(j == 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCameras();
        }
    }

public void SwitchCameras() // make this method public
{
    // Increment the current camera index and loop back if necessary.
    currentCameraIndex = (currentCameraIndex + 1) % realities[0].Length;

    // Switch cameras in all realities.
    for (int i = 0; i < realities.Length; i++)
    {
        for (int j = 0; j < realities[i].Length; j++)
        {
            realities[i][j].gameObject.SetActive(j == currentCameraIndex);
        }
    }
}
}
