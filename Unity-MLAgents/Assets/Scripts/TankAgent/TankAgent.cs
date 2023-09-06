using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TankAgent : Agent
{
    public WheelCollider[] leftWheels;
    public WheelCollider[] rightWheels;

    private void Start()
    {
        foreach (var wheel in leftWheels)
        {
            wheel.steerAngle = 0;
        }
        foreach (var wheel in rightWheels)
        { wheel.steerAngle = 0; }
    }

    private void FixedUpdate()
    {
        foreach (var wheel in leftWheels)
        {
            wheel.motorTorque = 1000;
        }
        foreach (var wheel in rightWheels)
        {
            wheel.motorTorque = 1000;
        }
    }

    public override void OnEpisodeBegin()
    {
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
    }
}