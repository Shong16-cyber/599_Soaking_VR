using UnityEngine;

public class FloatingOrange : MonoBehaviour
{
    [HideInInspector] public Transform bucketTransform;
    [HideInInspector] public float waterLevel;
    [HideInInspector] public float buoyancyForce = 5f;
    [HideInInspector] public float bucketRadius = 0.35f;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        if (rb == null || bucketTransform == null) return;
        
        // Calculate world water level
        float worldWaterLevel = bucketTransform.position.y + waterLevel;
        
        // Calculate depth under water
        float depth = worldWaterLevel - transform.position.y;
        
        // Apply buoyancy when submerged
        if (depth > 0)
        {
            Vector3 buoyancy = Vector3.up * buoyancyForce * Mathf.Clamp01(depth);
            rb.AddForce(buoyancy, ForceMode.Acceleration);
            
            // Damping
            rb.linearVelocity *= 0.98f;
        }
        
        // Keep orange inside bucket boundaries
        Vector3 localPos = transform.position - bucketTransform.position;
        float horizontalDistance = new Vector2(localPos.x, localPos.z).magnitude;
        
        if (horizontalDistance > bucketRadius)
        {
            Vector3 direction = (bucketTransform.position - transform.position).normalized;
            direction.y = 0;
            rb.AddForce(direction * 3f, ForceMode.VelocityChange);
        }
        
        // Prevent oranges from going too high or too low
        if (localPos.y < -0.2f || localPos.y > waterLevel + 0.3f)
        {
            Vector3 centerDirection = bucketTransform.position - transform.position;
            rb.AddForce(centerDirection * 1f, ForceMode.VelocityChange);
        }
    }
}