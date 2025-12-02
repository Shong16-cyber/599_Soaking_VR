using UnityEngine;

public class PlayerWaterFloat : MonoBehaviour
{
    public Transform waterPlane;
    public float floatHeight = 1.5f;
    public float waterRadius = 10f;  // 水域半径

    void Update()
    {
        if (waterPlane == null) return;
        
        // 检测是否在水域范围内（用距离判断）
        Vector3 waterPos = waterPlane.position;
        Vector3 playerPos = transform.position;
        
        float distanceXZ = Vector2.Distance(
            new Vector2(playerPos.x, playerPos.z),
            new Vector2(waterPos.x, waterPos.z)
        );
        
        bool inWater = distanceXZ < waterRadius;
        
        if (inWater)
        {
            float waterLevel = waterPos.y + floatHeight;
            if (playerPos.y < waterLevel)
            {
                playerPos.y = waterLevel;
                transform.position = playerPos;
            }
        }
    }
}