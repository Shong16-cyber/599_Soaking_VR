using UnityEngine;

public class BucketTrigger : MonoBehaviour
{
    public float downwardForce = 5f;
    
    void OnTriggerEnter(Collider other)
    {
        // check if it's an orange (add a tag to your orange prefab)
        if (other.CompareTag("Orange"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // apply downward force to make it fall through
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -downwardForce, rb.linearVelocity.z);
                
                // or alternatively, disable constraints temporarily:
                // rb.constraints = RigidbodyConstraints.None;
            }
        }
    }
}