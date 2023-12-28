using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public BoidManager boidPrefab;

    public int spawnBoids = 100;

    private List<BoidManager> _boids;

    [SerializeField]
    float NoClumpingRadius, LocalAreaRadius, Speed, TurnSpeed, ObstacleAvoidanceRadius, ViewAngle;

    [SerializeField]
    int RaysPerLayer, NumberOfLayers;

    [SerializeField]
    float boidAreaX = 14f;

    [SerializeField]
    float boidAreaY = 8f;

    [SerializeField]
    float boidAreaZ = 23f;

    private void Start()
    {
        _boids = new List<BoidManager>();

        for (int i = 0; i < spawnBoids; i++)
        {
            SpawnBoid(boidPrefab.gameObject, 0);
        }
    }

    private void Update()
    {
        foreach (BoidManager boid in _boids)
        {
            boid.SimulateMovement(_boids, Time.deltaTime);


            // Making sure the boids stay within the simulation area
            var boidPos = boid.transform.position;

            if (boidPos.x > boidAreaX)
            {
                boidPos.x -= boidAreaX * 2;
            } else if (boidPos.x < -boidAreaX)
            {
                boidPos.x += boidAreaX * 2;
            }

            if (boidPos.y > boidAreaY)
            {
                boidPos.y -= boidAreaY * 2;
            }
            else if (boidPos.y < -boidAreaY)
            {
                boidPos.y += boidAreaY * 2;
            }

            if (boidPos.z > boidAreaZ)
            {
                boidPos.z -= boidAreaZ * 2;
            }
            else if (boidPos.z < -boidAreaZ)
            {
                boidPos.z += boidAreaZ * 2;
            }

            boid.transform.position = boidPos;

        }
    }

    private void SpawnBoid(GameObject prefab, int swarmIndex)
    {
        var boidInstance = Instantiate(prefab);
        boidInstance.transform.localPosition += new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));

        var boidManager = boidInstance.GetComponent<BoidManager>();
        boidManager.SwarmIndex = swarmIndex;
        boidManager.Speed = Speed;
        boidManager.TurnSpeed = TurnSpeed;
        boidManager.LocalAreaRadius = LocalAreaRadius;
        boidManager.NoClumpingRadius = NoClumpingRadius;
        boidManager.ObstacleAvoidanceRadius = ObstacleAvoidanceRadius;
        boidManager.ViewAngle = ViewAngle;
        boidManager.RaysPerLayer = RaysPerLayer;
        boidManager.NumberOfLayers = NumberOfLayers;
        _boids.Add(boidManager);
    }
}
