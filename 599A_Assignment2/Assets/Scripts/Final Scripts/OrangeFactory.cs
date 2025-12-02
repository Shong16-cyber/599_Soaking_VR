using UnityEngine;

public class OrangeFactory : MonoBehaviour
{
    public Transform waterPlane;      // assign your water plane here
    public GameObject orangePrefab;   // assign your orange prefab
    public int count = 20;
    public float spawnRadius = 5f;    // radius on the water surface
    public float heightOffset = 0.2f; // how high above the water to spawn

    void Start()
    {
        for (int i = 0; i < count; i++)
            SpawnOne();
    }

    void SpawnOne()
    {
        // random position in a circle on top of the water
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = waterPlane.position +
                           new Vector3(randomCircle.x, 0f, randomCircle.y);

        // place slightly above water so it can fall and float
        spawnPos.y = waterPlane.position.y + heightOffset;

        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject orange = Instantiate(orangePrefab, spawnPos, rot);

        // make sure the FloatOnWater script knows which water plane to use
        var floatScript = orange.GetComponent<FloatOnWater>();
        if (floatScript != null)
            floatScript.waterPlane = waterPlane;
    }
}
