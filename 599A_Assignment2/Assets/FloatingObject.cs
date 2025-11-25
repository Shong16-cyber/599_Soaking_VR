using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Water Settings")]
    [Tooltip("Reference to the water plane transform")]
    public Transform waterPlane;
    
    [Tooltip("Water surface Y position (if no water plane reference)")]
    public float waterLevel = 0f;
    
    [Header("Buoyancy Settings")]
    [Tooltip("How much buoyancy force to apply")]
    public float buoyancyForce = 15f;
    
    [Tooltip("How quickly object stabilizes (damping)")]
    public float waterDrag = 0.99f;
    
    [Tooltip("Angular drag for rotation damping")]
    public float waterAngularDrag = 0.5f;
    
    [Header("Wave Settings (Optional)")]
    [Tooltip("Enable simple wave motion")]
    public bool enableWaves = false;
    
    public float waveHeight = 0.2f;
    public float waveSpeed = 1f;
    
    private Rigidbody rb;
    private float waterSurfaceY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("FloatingObject requires a Rigidbody component!");
            enabled = false;
            return;
        }
        
        // Set rigidbody properties for realistic water physics
        rb.linearDamping = waterDrag;
        rb.angularDamping = waterAngularDrag;
        
        UpdateWaterLevel();
    }

    void FixedUpdate()
    {
        UpdateWaterLevel();
        
        // Calculate submersion depth
        float depth = waterSurfaceY - transform.position.y;
        
        // Apply buoyancy force when submerged
        if (depth > 0)
        {
            // Buoyancy force proportional to submersion depth
            Vector3 buoyancy = Vector3.up * buoyancyForce * depth;
            rb.AddForce(buoyancy, ForceMode.Acceleration);
            
            // Apply additional damping to velocity for water resistance
            rb.linearVelocity *= waterDrag;
            rb.angularVelocity *= waterAngularDrag;
        }
    }

    void UpdateWaterLevel()
    {
        if (waterPlane != null)
        {
            waterSurfaceY = waterPlane.position.y;
            
            // Add wave effect if enabled
            if (enableWaves)
            {
                float wave = Mathf.Sin(Time.time * waveSpeed + transform.position.x) * waveHeight;
                waterSurfaceY += wave;
            }
        }
        else
        {
            waterSurfaceY = waterLevel;
        }
    }
    
    // Optional: Visualize water level in editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
        Vector3 waterPos = transform.position;
        waterPos.y = waterPlane != null ? waterPlane.position.y : waterLevel;
        Gizmos.DrawWireCube(waterPos, new Vector3(2f, 0.1f, 2f));
    }
}