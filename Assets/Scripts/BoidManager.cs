using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public int SwarmIndex { get; set; }
    public float NoClumpingRadius { get; set; }
    public float LocalAreaRadius { get; set; }
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }

    public void SimulateMovement(List<BoidManager> other, float time)
    {

        // default vars
        var steering = Vector3.zero;

        // applying steering
        if (steering != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);
        }

        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed)) * time;
    }
}
