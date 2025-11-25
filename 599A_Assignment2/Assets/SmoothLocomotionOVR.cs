using UnityEngine;
using static OVRInput; // Use this to simplify OVRInput calls

public class SmoothLocomotionOVR : MonoBehaviour
{
    // --- Public Variables (Adjust in Inspector) ---

    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;          // Speed of continuous forward/backward movement
    public float rotationSpeed = 60.0f;     // Speed of smooth turning in degrees per second

    [Header("References")]
    // The camera's transform is used to determine the forward direction for movement.
    public Transform xrCamera; 
    // The Rigidbody component on the XR Origin/Player for physics-based movement.
    private Rigidbody rb;

    // --- Private Variables ---

    private Vector3 movementDirection = Vector3.zero;
    private float rotationAmount = 0f;

    // --- Unity Methods ---

    void Start()
    {
        // Get the Rigidbody component from the player object
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on player object. Smooth locomotion requires a Rigidbody.");
            enabled = false; // Disable the script if no Rigidbody is found
        }
        if (xrCamera == null)
        {
            Debug.LogError("XR Camera Transform is not assigned. Drag your CenterEyeAnchor here.");
            enabled = false;
        }
    }

    void Update()
    {
        // 1. Handle Input
        HandleMovementInput();
        HandleRotationInput();
    }

    void FixedUpdate()
    {
        // 2. Apply Movement (in FixedUpdate for physics consistency)
        ApplyMovement();

        // 3. Apply Rotation
        ApplyRotation();
    }

    // --- Custom Methods ---

    private void HandleMovementInput()
    {
        // Read the 2D axis input from the Primary (Left) thumbstick
        Vector2 primaryAxis = Get(Axis2D.PrimaryThumbstick);

        // Get the forward direction from the camera, ignoring the Y-axis (keep it horizontal)
        Vector3 cameraForward = xrCamera.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Get the right direction from the camera, ignoring the Y-axis
        Vector3 cameraRight = xrCamera.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // Calculate the world-space movement direction based on thumbstick input
        movementDirection = (cameraForward * primaryAxis.y) + (cameraRight * primaryAxis.x);
    }

    private void ApplyMovement()
    {
        if (movementDirection.magnitude > 0.1f) // Deadzone check
        {
            // Calculate the velocity vector
            Vector3 velocity = movementDirection * moveSpeed;

            // Apply movement using the Rigidbody's velocity
            // This is generally better for physics and collision than direct transform manipulation
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }
        else
        {
            // Stop horizontal movement when the thumbstick is not in use
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void HandleRotationInput()
    {
        // Read the 2D axis input from the Secondary (Right) thumbstick
        Vector2 secondaryAxis = Get(Axis2D.SecondaryThumbstick);

        // Only use the X-axis for rotation (left/right)
        rotationAmount = secondaryAxis.x;
    }

    private void ApplyRotation()
    {
        if (Mathf.Abs(rotationAmount) > 0.1f) // Deadzone check
        {
            // Calculate the rotation in degrees this frame
            float turnDegrees = rotationAmount * rotationSpeed * Time.fixedDeltaTime;

            // Apply the rotation around the world Y-axis (up vector)
            transform.Rotate(0, turnDegrees, 0, Space.World);
        }
    }
}