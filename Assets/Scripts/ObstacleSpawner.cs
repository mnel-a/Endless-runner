using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public Obstacle[] obstacleToSpawn;
    public PlayerController player;
    public Transform parent;
    
    public float gameSpeed = 200f;
    public float spawnInterval = 2f;
    
    private List<Obstacle> obstacles = new List<Obstacle>();
    private float[] lanePositionsX = new float[] { -2f, 0f, 2f };

    private void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }
        InvokeRepeating(nameof(BeginSpawn), 1f, spawnInterval);
    }

   private void BeginSpawn()
    {
        if (obstacleToSpawn == null || obstacleToSpawn.Length == 0) return;

        int obstaclesToSpawnCount = Random.Range(1, 3);
        List<int> availableLanes = new List<int> { 0, 1, 2 };

        for (int i = 0; i < obstaclesToSpawnCount; i++)
        {
            int listIndex = Random.Range(0, availableLanes.Count);
            int designatedLane = availableLanes[listIndex];
            availableLanes.RemoveAt(listIndex);

            int randomObstacleIndex = Random.Range(0, obstacleToSpawn.Length);
            Obstacle chosenPrefab = obstacleToSpawn[randomObstacleIndex];

            var spawnedItem = Instantiate(chosenPrefab, parent);

            float spawnY = -5f; 
            float targetLaneX = lanePositionsX[designatedLane];

            spawnedItem.InitializeObstacle(designatedLane, spawnY, 700f);

            spawnedItem.itemPosition.x = targetLaneX;

            obstacles.Add(spawnedItem);
        }
    }
    private void Update()
    {
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            Obstacle obstacle = obstacles[i];
            if (obstacle == null) continue;

            obstacle.itemPosition.z -= gameSpeed * Time.deltaTime;

            Vector2 playerScreenPos = player.transform.position;
            Vector2 obstacleScreenPos = obstacle.transform.position;

        
            float deltaX = playerScreenPos.x - obstacleScreenPos.x;
            float deltaY = playerScreenPos.y - obstacleScreenPos.y;
            
            float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY);

            float hitRadius = .5f; 
            float hitRadiusSquared = hitRadius * hitRadius;

            if (distanceSquared <= hitRadiusSquared && !player.IsJumping())
            {
                player.TakeDamage(5f);
                Destroy(obstacle.gameObject);
                obstacles.RemoveAt(i);
                continue;
            }
   
            if (obstacle.itemPosition.z < -20f)
            {
                Destroy(obstacle.gameObject);
                obstacles.RemoveAt(i);
            }
        }

        SortLayerDepths();
    }

    private void SortLayerDepths()
    {
        List<Obstacle> sorted = new List<Obstacle>(obstacles);
        sorted.Sort((a, b) => b.itemPosition.z.CompareTo(a.itemPosition.z));

        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }
    }

}