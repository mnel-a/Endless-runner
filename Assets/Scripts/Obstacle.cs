using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 obstaclePosition;
    public int laneIndex; 
    private SpriteRenderer spriteRenderer;

    public float horizonYOffset = 6f; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeObstacle(int lane, float spawnY, float spawnZ)
    {
        laneIndex = lane;
        obstaclePosition = new Vector3(lane, spawnY, spawnZ);
        
        UpdatePerspective();
    }

    private void Update()
    {
        UpdatePerspective();
    }

    private void UpdatePerspective()
    {
        float zDepth = Mathf.Max(0.1f, CameraComponent.focalLength + obstaclePosition.z);
        float perspective = CameraComponent.focalLength / zDepth;

        float posX = obstaclePosition.x * perspective;
        float posY = Mathf.Lerp(obstaclePosition.y, horizonYOffset, 1f - perspective);

        transform.position = new Vector2(posX, posY);
        transform.localScale = Vector3.one * perspective * 1.5f; 
    }
}
