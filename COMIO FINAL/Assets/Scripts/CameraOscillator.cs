using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOscillator : MonoBehaviour
{

    public Camera targetCamera;
    public float minValue = 4.0f;
    public float maxValue = 40.0f;
    public float speed = 1.0f;

    public float time;
    // Start is called before the first frame update
    void Start()
    {
        if(targetCamera == null){
            targetCamera = GetComponent<Camera>();
        }

        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * speed;

        float newSize = Mathf.Lerp(minValue, maxValue, (Mathf.Sin(time) +1) * 0.5f);

        targetCamera.orthographicSize = newSize;
    }
}
