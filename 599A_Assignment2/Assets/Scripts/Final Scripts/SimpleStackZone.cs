using UnityEngine;

public class SimpleStackZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Orange")) return;

        // 只关闭漂浮，让物理自然堆叠
        var floatScript = other.GetComponent<FloatOnWater>();
        if (floatScript != null)
            floatScript.enabled = false;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearDamping = 2f;  // 增加阻力，让橘子稳定
            rb.angularDamping = 2f;
        }
        
        Debug.Log("Orange entered stack zone!");
    }
}