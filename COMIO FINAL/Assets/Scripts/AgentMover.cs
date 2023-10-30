using System.Collections;
using UnityEngine;

public class AgentMover : MonoBehaviour
{
    public Transform[] waypoints; // Assign your waypoint objects here in the Inspector
    public float speed = 3.0f; // Movement speed
    public float rotationSpeed = 120.0f; // How fast the agent rotates
    private int currentWaypointIndex = 0;
    private Animator agentAnimator; // If you're using an animation for walking

    private bool isMoving = false;

    private void Start()
    {
        agentAnimator = GetComponent<Animator>();
    }

    void Update()
    {

        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
 
        if(!isMoving && Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {

            currentWaypointIndex++;
            if(currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Loop back to the first waypoint
            }
        }
          
        
            // Rotate towards the target
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    
        
            // Move towards the waypoint
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);

        
    }


}