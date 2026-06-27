using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 itemPosition;
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
        itemPosition = new Vector3(lane, spawnY, spawnZ);
        
        UpdatePerspectivePosition();
    }

    private void Update()
    {
        UpdatePerspectivePosition();
    }

    private void UpdatePerspectivePosition()
    {
        float zDepth = Mathf.Max(0.1f, CameraComponent.focalLength + itemPosition.z);
        float perspective = CameraComponent.focalLength / zDepth;

        float calculatedX = itemPosition.x * perspective;
        float calculatedY = Mathf.Lerp(itemPosition.y, horizonYOffset, 1f - perspective);

        transform.position = new Vector2(calculatedX, calculatedY);
        transform.localScale = Vector3.one * perspective * 1.5f; 
    }
}
