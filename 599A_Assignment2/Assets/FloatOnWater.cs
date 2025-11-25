using UnityEngine;

public class FloatOnWater : MonoBehaviour
{
    public float waterY = 0f;
    public float bobAmplitude = 0.08f;
    public float bobFrequency = 1.2f;
    public float driftSpeed = 0.25f;        // slow x/z drift
    public float spinSpeed = 15f;           // degrees/sec around Y
    public float surfaceOffset = 0.0f;      // tweak if collider makes it sit too high/low

    Vector2 driftDir;
    float seed;

    void Start()
    {
        seed = Random.value * 1000f;
        // random unit direction on XZ
        float a = Random.Range(0f, Mathf.PI * 2f);
        driftDir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    void Update()
    {
        float t = Time.time + seed;

        // Bob up/down
        float bob = Mathf.Sin(t * bobFrequency) * bobAmplitude;

        // Drift on surface
        Vector3 p = transform.position;
        p.x += driftDir.x * driftSpeed * Time.deltaTime;
        p.z += driftDir.y * driftSpeed * Time.deltaTime;

        // Lock to water surface height
        p.y = waterY + surfaceOffset + bob;
        transform.position = p;

        // Gentle spin
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }
}
