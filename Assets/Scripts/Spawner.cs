using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public BoidManager boidPrefab;

    public int spawnBoids = 100;

    private List<BoidManager> _boids;

    [SerializeField]
    float NoClumpingRadius, LocalAreaRadius, Speed, SteeringSpeed, NoClumpingArea;

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
        }
    }

    private void SpawnBoid(GameObject prefab, int swarmIndex)
    {
        var boidInstance = Instantiate(prefab);
        boidInstance.transform.localPosition += new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));

        var boidManager = boidInstance.GetComponent<BoidManager>();
        boidManager.SwarmIndex = swarmIndex;
        boidManager.Speed = Speed;
        boidManager.SteeringSpeed = SteeringSpeed;
        boidManager.LocalAreaRadius = LocalAreaRadius;
        boidManager.NoClumpingRadius = NoClumpingArea;
        _boids.Add(boidManager);
    }
}
