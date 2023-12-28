using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BoidManager : MonoBehaviour
{
    public int SwarmIndex { get; set; }
    public float NoClumpingRadius { get; set; } // NoClumpingRadius controls the density of our swarms, the smaller the value, the more dense the swam can be
    public float LocalAreaRadius { get; set; } // LocalAreaRadius controls the average swarm size, A bigger LocalAreaRadius will increase the average swarm size, because each Boid has a bigger range of influence
    public float ObstacleAvoidanceRadius { get; set; }
    public float Speed { get; set; }
    public float TurnSpeed { get; set; }

    float ViewAngle = 120f; // Angle of the view cone in degrees
    int RaysPerLayer = 5; // Number of rays per layer in the cone
    int Layers = 3; // Number of layers in the cone

    public void SimulateMovement(List<BoidManager> other, float time)
    {

        // default vars
        var steering = Vector3.zero;

        // Seperation vars
        Vector3 separationDirection = Vector3.zero;
        int separationCount = 0;

        // Alignment vars
        Vector3 alignmentDirection = Vector3.zero;
        int alignmentCount = 0;

        // Cohesion vars
        Vector3 cohesionDirection = Vector3.zero;
        int cohesionCount = 0;

        // leader default vars
        var leaderBoid = other[0];
        var leaderAngle = 270f; // This is it's view POV

        foreach (BoidManager boid in other)
        {
            // Skip self
            if (boid == this)
            {
                continue;
            }

            var distance = Vector3.Distance(boid.transform.position, this.transform.position);


            // Identify local neighbour in Clumping radius
            if (distance < NoClumpingRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }

            // Identify local neighbour in Local Area Radius, also finding the leader in the local radius
            if (distance < LocalAreaRadius)
            {
                alignmentDirection += boid.transform.forward;
                alignmentCount++;

                cohesionDirection += boid.transform.position - transform.position;
                cohesionCount++;

                var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                if (angle < leaderAngle && angle < 90f)
                {
                    leaderBoid = boid;
                    leaderAngle = angle;
                }
            }
        }

        // Calcuate averages for seperation & alignment
        if (separationCount > 0)
        {
            separationDirection /= separationCount;
        }
        if (alignmentCount > 0)
        {
            alignmentDirection /= alignmentCount;
        }

        // Flip and normalize
        separationDirection = -separationDirection.normalized;

        // Don't need to make our cohesion flipped, but we need to make it relative to the boids position
        cohesionDirection -= transform.position;

        // To make them more "fishlike", we can apply rules of weight to our calculations
        // The rules are as followed:
        // Separation: 50%
        // Alignment: 34%
        // Cohesion: 16%

        // Apply separation to steering
        steering += separationDirection.normalized * 0.5f;

        // Apply alignment direction to steering
        steering += alignmentDirection.normalized * 0.34f;

        // Applying cohesion to steering
        steering += cohesionDirection.normalized * 0.16f;

        // Local leader adjustments
        if (leaderBoid != null)
        {
            steering += (leaderBoid.transform.position - transform.position).normalized;
        }

        // Just testing having the boids go to a target area
        /*
        var targetPoint = Vector3.zero;
        if (Vector3.Distance(transform.position, targetPoint) < LocalAreaRadius)
        {
            steering += (targetPoint - transform.position).normalized;
        }
        */

        // Because this is the most important rule, so we apply this in the end so it takes priority above all else and doesn't get overwritten
        // We'll be using raycasts for this based on "Dawn Studio's" implementation, but this may change if I find a more optimal approach
        /*RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, LocalAreaRadius, LayerMask.GetMask("BoidObstacle")))
            steering = ((hitInfo.point + hitInfo.normal) - transform.position).normalized;
        */

        // Commenting out the previous code, as i'll now be attempting to use my own custom raycasting method
        Vector3 avoidanceDirection;
        if (PerformConeRaycastObstacleDetection(transform.position, transform.forward, out avoidanceDirection))
        {
            steering = avoidanceDirection.normalized;
        }


        // applying steering
        if (steering != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), TurnSpeed * time);
        }

        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed)) * time;
    }

    private bool PerformConeRaycastObstacleDetection(Vector3 position, Vector3 forward, out Vector3 avoidanceDirection)
    {
        avoidanceDirection = Vector3.zero;
        bool obstacleDetected = false;

        float halfConeAngle = ViewAngle / 2f;
        float layerAngleStep = ViewAngle / (Layers - 1);
        float rayAngleStep = 360f / RaysPerLayer;

        for (int layer = 0; layer < Layers; layer++)
        {
            float pitchAngle = -halfConeAngle + layer * layerAngleStep;
            for (int ray = 0; ray < RaysPerLayer; ray++)
            {
                float yawAngle = ray * rayAngleStep;
                Quaternion rotation = Quaternion.Euler(pitchAngle, yawAngle, 0);
                Vector3 direction = rotation * forward;

                // Ensure raycasting is relative to boid's orientation
                Vector3 rayDirection = transform.TransformDirection(direction);

                if (Physics.Raycast(position, rayDirection, out RaycastHit hitInfo, ObstacleAvoidanceRadius, LayerMask.GetMask("BoidObstacle")))
                {
                    obstacleDetected = true;
                    avoidanceDirection += (position - hitInfo.point);
                    Debug.DrawRay(position, rayDirection * ObstacleAvoidanceRadius, Color.red);
                } else
                {
                    Debug.DrawRay(position, rayDirection * ObstacleAvoidanceRadius, Color.green);
                }

            }
        }

        if (obstacleDetected)
        {
            avoidanceDirection = avoidanceDirection.normalized; // Steer away from the obstacles
        }

        return obstacleDetected;
    }


}
