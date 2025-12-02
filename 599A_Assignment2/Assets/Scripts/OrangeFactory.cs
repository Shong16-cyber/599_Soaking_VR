using UnityEngine;

public class OrangeFactory : MonoBehaviour
{
    public Transform playerTransform;        // assign your player
    public GameObject orangePrefab;          // assign your orange prefab
    public int count = 20;
    public float spawnRadius = 5f;           // radius around player to spawn
    public LayerMask groundLayer;            // assign your ground layer

    void Start()
    {
        for (int i = 0; i < count; i++) SpawnOne();
    }

    void SpawnOne()
    {
        // random position within radius of player
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // raycast down to find ground height
        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            spawnPos = hit.point;
        }
        else
        {
            spawnPos.y = 0; // fallback if no ground found
        }

        Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        Instantiate(orangePrefab, spawnPos, rot);
    }
}