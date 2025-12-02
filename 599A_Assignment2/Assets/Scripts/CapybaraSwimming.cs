using UnityEngine;

public class CapybaraSwimming : MonoBehaviour
{
    [Header("Water Settings")]
    public Transform waterPlane;
    public float waterLevel = 0f;
    
    [Header("Movement Mode")]
    public bool automaticMovement = true; // Move without player input
    public float moveSpeed = 2f;
    public float turnSpeed = 50f;
    
    [Header("Automatic Swimming Pattern")]
    public float changeDirectionTime = 3f; // Change direction every X seconds
    public float randomTurnAmount = 45f; // Max random turn angle
    
    [Header("Floating - Head Above Water")]
    [Tooltip("Distance from water surface to capybara's pivot. Negative = submerged. Adjust based on your model size.")]
    public float floatHeight = -0.6f; // Head above water, body submerged
    public float buoyancyForce = 12f;
    
    [Header("Visual Effects")]
    public bool enableBobbing = true;
    public float bobbingAmount = 0.1f;
    public float bobbingSpeed = 2f;
    
    private Rigidbody rb;
    private float nextDirectionChange;
    private float targetDirection;
    
    void Start()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("Added Rigidbody to " + gameObject.name);
        }
        
        // Configure Rigidbody for swimming
        rb.isKinematic = false; // CRITICAL: Must be false to move!
        rb.useGravity = true;
        rb.mass = 2f;
        rb.linearDamping = 2f;
        rb.angularDamping = 3f;
        
        // Only freeze Z rotation to keep upright
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        targetDirection = transform.eulerAngles.y;
        nextDirectionChange = Time.time + changeDirectionTime;
        
        Debug.Log("CapybaraSwimming initialized on " + gameObject.name);
    }
    
    void Update()
    {
        // Automatic direction changes
        if (automaticMovement && Time.time >= nextDirectionChange)
        {
            float randomTurn = Random.Range(-randomTurnAmount, randomTurnAmount);
            targetDirection += randomTurn;
            nextDirectionChange = Time.time + changeDirectionTime;
        }
    }
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        // Get water surface level
        float waterSurfaceY = waterPlane != null ? waterPlane.position.y : waterLevel;
        
        // Apply floating force
        ApplyBuoyancy(waterSurfaceY);
        
        // Apply movement
        if (automaticMovement)
        {
            AutomaticSwimming();
        }
        else
        {
            ManualSwimming();
        }
        
        // Apply bobbing
        if (enableBobbing)
        {
            ApplyBobbing(waterSurfaceY);
        }
    }
    
    void ApplyBuoyancy(float waterY)
    {
        float depthUnderWater = waterY + floatHeight - transform.position.y;
        
        if (depthUnderWater > 0)
        {
            // Apply upward force proportional to how deep we are
            Vector3 buoyancy = Vector3.up * buoyancyForce * depthUnderWater;
            rb.AddForce(buoyancy, ForceMode.Acceleration);
        }
        
        // Dampen vertical velocity
        Vector3 velocity = rb.linearVelocity;
        velocity.y *= 0.8f;
        rb.linearVelocity = velocity;
    }
    
    void ApplyBobbing(float waterY)
    {
        float bobbing = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        Vector3 bobbingForce = Vector3.up * bobbing;
        rb.AddForce(bobbingForce, ForceMode.VelocityChange);
    }
    
    void AutomaticSwimming()
    {
        // Rotate towards target direction
        Quaternion targetRotation = Quaternion.Euler(0, targetDirection, 0);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation, 
            turnSpeed * Time.fixedDeltaTime
        );
        
        // Move forward
        Vector3 forward = transform.forward;
        forward.y = 0; // Keep movement horizontal
        rb.AddForce(forward.normalized * moveSpeed, ForceMode.VelocityChange);
        
        // Limit horizontal speed
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > moveSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }
    }
    
    void ManualSwimming()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            // Calculate movement direction
            Vector3 moveDir = new Vector3(horizontal, 0, vertical).normalized;
            
            // Rotate towards movement direction
            if (moveDir.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    targetRotation, 
                    turnSpeed * Time.fixedDeltaTime
                );
            }
            
            // Move
            Vector3 force = transform.forward * moveSpeed;
            rb.AddForce(force, ForceMode.VelocityChange);
            
            Debug.Log("Moving capybara - Input: " + moveDir + " Velocity: " + rb.linearVelocity);
        }
    }
    
    // Visualize in editor
    void OnDrawGizmos()
    {
        float waterY = waterPlane != null ? waterPlane.position.y : waterLevel;
        
        // Draw water surface
        Gizmos.color = new Color(0, 0.6f, 1f, 0.3f);
        Gizmos.DrawWireCube(
            new Vector3(transform.position.x, waterY, transform.position.z), 
            new Vector3(4f, 0.05f, 4f)
        );
        
        // Draw target float height
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            new Vector3(transform.position.x, waterY + floatHeight, transform.position.z), 
            0.2f
        );
        
        // Draw movement direction
        if (automaticMovement)
        {
            Gizmos.color = Color.green;
            Vector3 direction = Quaternion.Euler(0, targetDirection, 0) * Vector3.forward;
            Gizmos.DrawRay(transform.position, direction * 2f);
        }
    }
}