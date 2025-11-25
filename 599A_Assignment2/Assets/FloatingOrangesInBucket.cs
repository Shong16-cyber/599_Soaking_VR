using UnityEngine;

public class FloatingOrangesInBucket : MonoBehaviour
{
    [Header("Orange Setup")]
    [Tooltip("Prefab of orange to spawn")]
    public GameObject orangePrefab;
    
    [Tooltip("How many oranges to create")]
    public int numberOfOranges = 5;
    
    [Tooltip("Bucket's water level (local Y position inside bucket)")]
    public float bucketWaterLevel = 0.5f;
    
    [Header("Orange Spawn Area")]
    [Tooltip("Spawn radius inside bucket")]
    public float spawnRadius = 0.3f;
    
    [Header("Orange Physics")]
    public float orangeMass = 0.1f;
    public float orangeBuoyancy = 5f;
    public float orangeDrag = 2f;
    
    [Header("Bucket Collider Settings")]
    [Tooltip("Radius of your bucket's capsule collider minus 0.05")]
    public float bucketRadius = 0.35f;
    
    [Header("Spawn Position Offset")]
    [Tooltip("Offset from bucket pivot to inside of bucket (adjust if pivot is at handle)")]
    public Vector3 spawnOffset = new Vector3(0, -0.5f, 0);
    
    private Transform bucketTransform;
    
    void Start()
    {
        bucketTransform = transform;
        
        if (orangePrefab == null)
        {
            Debug.LogError("Please assign an Orange Prefab!");
            return;
        }
        
        SpawnOranges();
    }
    
    void SpawnOranges()
    {
        for (int i = 0; i < numberOfOranges; i++)
        {
            // Random position inside bucket
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            
            // Apply spawn offset to position oranges inside bucket (not at pivot/handle)
            Vector3 spawnPos = bucketTransform.position + spawnOffset + new Vector3(
                randomCircle.x,
                bucketWaterLevel + Random.Range(0f, 0.2f),
                randomCircle.y
            );
            
            // Create orange
            GameObject orange = Instantiate(orangePrefab, spawnPos, Random.rotation);
            orange.name = "Orange_" + i;
            orange.transform.parent = bucketTransform;
            
            // Setup physics
            SetupOrangePhysics(orange);
        }
    }
    
    void SetupOrangePhysics(GameObject orange)
    {
        // Add Rigidbody
        Rigidbody rb = orange.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = orange.AddComponent<Rigidbody>();
        }
        
        rb.mass = orangeMass;
        rb.linearDamping = orangeDrag;
        rb.angularDamping = 1f;
        rb.useGravity = true;
        
        // Add collider
        if (orange.GetComponent<Collider>() == null)
        {
            SphereCollider col = orange.AddComponent<SphereCollider>();
            col.radius = 0.05f;
        }
        
        // Add floating script
        FloatingOrange floatingScript = orange.AddComponent<FloatingOrange>();
        floatingScript.bucketTransform = bucketTransform;
        floatingScript.waterLevel = bucketWaterLevel;
        floatingScript.buoyancyForce = orangeBuoyancy;
        floatingScript.bucketRadius = bucketRadius;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0.5f, 1f, 0.3f);
        Vector3 waterPos = transform.position + spawnOffset + Vector3.up * bucketWaterLevel;
        
        // Draw water level circle
        for (int i = 0; i < 20; i++)
        {
            float angle1 = (float)i / 20 * Mathf.PI * 2f;
            float angle2 = (float)(i + 1) / 20 * Mathf.PI * 2f;
            Vector3 p1 = waterPos + new Vector3(Mathf.Cos(angle1) * spawnRadius, 0, Mathf.Sin(angle1) * spawnRadius);
            Vector3 p2 = waterPos + new Vector3(Mathf.Cos(angle2) * spawnRadius, 0, Mathf.Sin(angle2) * spawnRadius);
            Gizmos.DrawLine(p1, p2);
        }
        
        // Draw spawn offset indicator
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + spawnOffset);
        Gizmos.DrawSphere(transform.position + spawnOffset, 0.05f);
    }
}