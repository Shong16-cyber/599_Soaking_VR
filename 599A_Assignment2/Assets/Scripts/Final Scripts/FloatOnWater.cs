using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatOnWater : MonoBehaviour
{
    [Header("Water")]
    public Transform waterPlane;          // drag your water plane here
    public float surfaceOffset = 0f;      // adjust if visual water is not exactly at plane Y

    [Header("Buoyancy")]
    public float floatHeight = 0.3f;      // how deep the center sits in water
    public float buoyancyStrength = 10f;  // upward force
    public float damping = 2f;           // stabilizes motion, prevents big jumps

    [Header("Fake Waves")]
    public float waveAmplitude = 0.05f;   // vertical bobbing height
    public float waveFrequency = 1.5f;    // speed of bobbing

    [Header("Drift")]
    public float driftStrength = 0.3f;    // sideways drift in water
    public float driftFrequency = 0.4f;

    Rigidbody rb;
    float wavePhase;      // random phase so each orange bobs differently
    float driftSeed;      // random seed for drift

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        wavePhase = Random.Range(0f, Mathf.PI * 2f);
        driftSeed = Random.Range(0f, 1000f);
    }

    void FixedUpdate()
    {
        if (!waterPlane || rb == null || rb.isKinematic) return;

        // base water height
        float waterLevel = waterPlane.position.y + surfaceOffset;

        // add small fake wave height
        float waveOffset = Mathf.Sin(Time.time * waveFrequency + wavePhase) * waveAmplitude;
        float targetSurface = waterLevel + waveOffset;

        float objectHeight = transform.position.y;

        // positive depth => we are under the (wavy) surface
        float depth = (targetSurface - objectHeight) + floatHeight;

        if (depth > 0f)
        {
            // smooth buoyancy
            float force = depth * buoyancyStrength;
            float damp = -rb.linearVelocity.y * damping;

            rb.AddForce(new Vector3(0f, force + damp, 0f), ForceMode.Acceleration);

            // gentle horizontal drift
            float t = Time.time * driftFrequency;
            float dx = (Mathf.PerlinNoise(driftSeed, t) - 0.5f);
            float dz = (Mathf.PerlinNoise(driftSeed + 123.4f, t) - 0.5f);
            Vector3 drift = new Vector3(dx, 0f, dz) * driftStrength;
            rb.AddForce(drift, ForceMode.Acceleration);
        }
    }
}
