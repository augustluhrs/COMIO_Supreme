using UnityEngine;
using MidiJack;

public class RotateAroundTarget : MonoBehaviour
{
    public Transform target; // The target to rotate around and look at
    public float fixedSpeed = 1.0f;


    private void Update()
    {
        transform.RotateAround(target.position, Vector3.up, fixedSpeed * Time.deltaTime);
        transform.LookAt(target);

    }
}
