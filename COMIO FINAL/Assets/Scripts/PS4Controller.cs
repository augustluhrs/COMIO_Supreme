using UnityEngine;
using UnityEngine.InputSystem;

public class PS4Controller : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float boostedMoveSpeed = 20f;
    public float boostedRotateSpeed = 20f;
    public float orthoSizeChangeRate = 0.1f;
    private Gamepad gamepad;
    private bool isOrthographic = false;
    private Camera[] cameras;
    private float originalMoveSpeed;
    private float originalRotateSpeed;

    private void Awake()
    {
        cameras = GetComponentsInChildren<Camera>();
        originalMoveSpeed = moveSpeed;
        originalRotateSpeed = rotateSpeed;
    }

    private void Start()
    {
        // Look for connected gamepads
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i] != null)
            {
                gamepad = Gamepad.all[i];
                break;
            }
        }

        if (gamepad == null)
        {
            Debug.LogWarning("No gamepad connected.");
        }
    }

    void Update()
    {
        if (gamepad != null)
        {
            // Toggle camera mode on X button press
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                ToggleCameraMode();
            }

            // Adjust orthographic size in orthographic mode
            if (isOrthographic)
            {
                float sizeChange = (gamepad.rightTrigger.ReadValue() - gamepad.leftTrigger.ReadValue()) * orthoSizeChangeRate;
                foreach (Camera cam in cameras)
                {
                    cam.orthographicSize = Mathf.Max(0.1f, cam.orthographicSize - sizeChange);
                }
            }

            // Toggle speed on R1 button press
            if (gamepad.rightShoulder.wasPressedThisFrame)
            {
                if (Mathf.Approximately(moveSpeed, boostedMoveSpeed))
                {
                    moveSpeed = originalMoveSpeed;  // Use original moveSpeed value
                    rotateSpeed = originalRotateSpeed;  // Use original rotateSpeed value
                }
                else
                {
                    moveSpeed = boostedMoveSpeed;
                    rotateSpeed = boostedRotateSpeed;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (gamepad != null)
        {
            Vector2 moveInput = gamepad.leftStick.ReadValue();
            Vector2 rotateInput = gamepad.rightStick.ReadValue();

            // Movement
            Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x) * moveSpeed;
            transform.Translate(moveDirection * Time.fixedDeltaTime, Space.World);

            // Rotation
            float yaw = rotateInput.x * rotateSpeed;
            float pitch = -rotateInput.y * rotateSpeed;  // Inverted pitch control
            transform.Rotate(pitch, yaw, 0, Space.Self);
        }
    }

    private void ToggleCameraMode()
    {
        isOrthographic = !isOrthographic;
        foreach (Camera cam in cameras)
        {
            cam.orthographic = isOrthographic;
        }
    }
}
