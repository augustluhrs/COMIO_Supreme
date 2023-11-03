using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 10f;  // Speed of rotation in degrees per second

    void Update()
    {
        float rotationAmount = spinSpeed * Time.deltaTime;  // Calculate rotation amount
        transform.Rotate(0, rotationAmount, 0, Space.Self);  // Apply rotation around local Y-axis
    }
}
