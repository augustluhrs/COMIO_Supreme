using UnityEngine;

public class RandomMover : MonoBehaviour
{
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [SerializeField] private Transform lookAtObject;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float waitTime = 2.0f;
    [SerializeField] private Animator animator; // Assuming you're using Unity's Animator component

    private bool isMoving = true;
    private Vector3 targetPosition;
    private float currentWaitTime = 0;

    private void Start()
    {
        SetNewRandomPosition();
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTargetPosition();
        }
        else
        {
            FaceCentralObject();
        }
    }

    private void MoveToTargetPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            currentWaitTime = waitTime;
            animator.SetTrigger("LookAt"); // Assuming the animation name is LookAt
        }
        else
        {
            animator.SetTrigger("Move"); // Assuming the animation name is Move
        }
    }

    private void FaceCentralObject()
    {
        Vector3 directionToLook = lookAtObject.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);

        currentWaitTime -= Time.deltaTime;

        if (currentWaitTime <= 0)
        {
            SetNewRandomPosition();
            isMoving = true;
        }
    }

    private void SetNewRandomPosition()
    {
        targetPosition = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            Random.Range(minBounds.z, maxBounds.z)
        );
    }
}
