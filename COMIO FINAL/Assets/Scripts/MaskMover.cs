using UnityEngine;

public class MaskMover : MonoBehaviour
{
    public float speed = 1f;  // Speed at which the mask moves
    public float distance = 5f;  // Distance the mask moves up and down
    public Transform imageTransform;  // Reference to the transform of the image

    private Vector3 startPosition;
    private Vector3 endPositionUp;
    private Vector3 endPositionDown;
    
    void Start()
    {
        startPosition = transform.position;
        endPositionUp = startPosition + Vector3.up * distance;
        endPositionDown = startPosition + Vector3.down * distance;
    }

    void Update()
    {
        float pingPong = Mathf.PingPong(Time.time * speed, 1);
        Vector3 newPosition = Vector3.Lerp(endPositionDown, endPositionUp, pingPong);
        Vector3 deltaPosition = newPosition - transform.position;  // Calculate the change in position
        transform.position = newPosition;  // Update the position of the mask
        imageTransform.position -= deltaPosition;  // Move the image inversely to keep it stationary
    }
}
