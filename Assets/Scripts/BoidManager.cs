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
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }

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

        // applying steering
        if (steering != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);
        }
        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed)) * time;
    }
}
