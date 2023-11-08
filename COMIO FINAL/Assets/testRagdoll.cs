using UnityEngine;

public class RagdollLimbsFlailer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform pelvis;

    public float amplitude = 0.5f;
    public float frequency = 1.0f;

    private Vector3 headInitialPosition;
    private Vector3 leftHandInitialPosition;
    private Vector3 rightHandInitialPosition;
    private Vector3 leftFootInitialPosition;
    private Vector3 rightFootInitialPosition;

    private float time;

    void Start()
    {
        // Assuming all limbs including the head are children of the pelvis
        // Store the initial local positions
        headInitialPosition = head ? head.localPosition : Vector3.zero;
        leftHandInitialPosition = leftHand ? leftHand.localPosition : Vector3.zero;
        rightHandInitialPosition = rightHand ? rightHand.localPosition : Vector3.zero;
        leftFootInitialPosition = leftFoot ? leftFoot.localPosition : Vector3.zero;
        rightFootInitialPosition = rightFoot ? rightFoot.localPosition : Vector3.zero;
    }

    void Update()
    {
        time += Time.deltaTime;

        // Apply a sine wave movement pattern to each limb and the head
        if (head)
            head.localPosition = headInitialPosition + new Vector3(0f, Mathf.Sin(time * frequency) * amplitude, 0f);

        if (leftHand)
            leftHand.localPosition = leftHandInitialPosition + new Vector3(0f, Mathf.Sin(time * frequency + Mathf.PI) * amplitude, 0f); // Offset phase by PI to alternate

        if (rightHand)
            rightHand.localPosition = rightHandInitialPosition + new Vector3(0f, Mathf.Sin(time * frequency) * amplitude, 0f);

        if (leftFoot)
            leftFoot.localPosition = leftFootInitialPosition + new Vector3(0f, Mathf.Sin(time * frequency + Mathf.PI / 2) * amplitude, 0f); // Offset phase by PI/2

        if (rightFoot)
            rightFoot.localPosition = rightFootInitialPosition + new Vector3(0f, Mathf.Sin(time * frequency - Mathf.PI / 2) * amplitude, 0f); // Offset phase by -PI/2

        // The pelvis is the parent object, so it doesn't need to be moved relative to itself
        // If you wanted to move the pelvis too, you could adjust its localPosition or position similarly
    }
}
