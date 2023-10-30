using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLineBetweenObjects : MonoBehaviour
{
    public Transform objectA; // Assign this in the Inspector
    public Transform objectB; // Assign this in the Inspector

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Set the number of vertices (2 for a simple line)
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if(objectA != null && objectB != null)
        {
            // Set the positions
            lineRenderer.SetPosition(0, objectA.position);
            lineRenderer.SetPosition(1, objectB.position);
        }
    }
}
