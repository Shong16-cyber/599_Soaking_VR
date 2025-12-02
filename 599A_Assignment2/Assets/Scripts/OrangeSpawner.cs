using UnityEngine;

public class OrangeSpawner : MonoBehaviour
{
    public Transform waterPlane;             // assign your plane
    public GameObject orangePrefab;          // assign your orange prefab
    public int count = 20;
    public float edgeMargin = 0.5f;          // keep away from the edges
    public bool respawnOverTime = false;
    public float spawnInterval = 3f;

    float waterY;
    Bounds planeBounds;

    void Awake()
    {
        if (waterPlane.TryGetComponent<Renderer>(out var r))
            planeBounds = r.bounds;
        else if (waterPlane.TryGetComponent<Collider>(out var c))
            planeBounds = c.bounds;
        else
            planeBounds = new Bounds(waterPlane.position, new Vector3(10, 0.1f, 10)); // fallback

        waterY = planeBounds.center.y; // plane is flat, so this equals its Y
    }

    void Start()
    {
        for (int i = 0; i < count; i++) SpawnOne();
        if (respawnOverTime) InvokeRepeating(nameof(SpawnOne), spawnInterval, spawnInterval);
    }

    void SpawnOne()
    {
        var min = planeBounds.min + new Vector3(edgeMargin, 0, edgeMargin);
        var max = planeBounds.max - new Vector3(edgeMargin, 0, edgeMargin);

        float x = Random.Range(min.x, max.x);
        float z = Random.Range(min.z, max.z);

        Vector3 pos = new Vector3(x, waterY, z);
        Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        var orange = Instantiate(orangePrefab, pos, rot);
        var floater = orange.GetComponent<FloatOnWater>();
        if (floater == null) floater = orange.AddComponent<FloatOnWater>();
        floater.waterY = waterY; // tell it where the surface is
    }
}

