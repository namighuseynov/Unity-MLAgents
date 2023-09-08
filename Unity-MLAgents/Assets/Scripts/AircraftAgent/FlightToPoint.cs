using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using UnityEngine.UI;

public class FlightToPoint : Agent
{
    [Range(-1, 1)] public float Pitch;
    [Range(-1, 1)] public float Roll;
    [Range(-1, 1)] public float Yaw;
    [Range(-1, 1)] public float Flap;

    float thrustPercent;
    float brakesTorque;

    AircraftPhysics aircraftPhysics;
    Rigidbody rb;


    [SerializeField]
    List<AeroSurface> controlSurfaces = null;
    [SerializeField]
    List<WheelCollider> wheels = null;
    [SerializeField]
    float rollControlSensitivity = 0.2f;
    [SerializeField]
    float pitchControlSensitivity = 0.2f;
    [SerializeField]
    float yawControlSensitivity = 0.2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        aircraftPhysics = GetComponent<AircraftPhysics>();
        thrustPercent = 1f;
    }

    public override void OnEpisodeBegin()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Roll = actions.ContinuousActions[0];
        Pitch = actions.ContinuousActions[1];
        Yaw = actions.ContinuousActions[2];

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actionSegment = actionsOut.ContinuousActions;
        actionSegment[0] = Input.GetAxisRaw("Roll");
        actionSegment[1] = Input.GetAxisRaw("Pitch");
        actionSegment[2] = Input.GetAxisRaw("Yaw");
    }

    //private void Update()
    //{
    //    Pitch = Input.GetAxis("Vertical");
    //    Roll = Input.GetAxis("Horizontal");
    //    Yaw = Input.GetAxis("Yaw");

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        thrustPercent = thrustPercent > 0 ? 0 : 1f;
    //    }

    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        Flap = Flap > 0 ? 0 : 0.3f;
    //    }

    //    if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        brakesTorque = brakesTorque > 0 ? 0 : 100f;
    //    }
    //}

    private void FixedUpdate()
    {
        SetControlSurfecesAngles(Pitch, Roll, Yaw, Flap);
        aircraftPhysics.SetThrustPercent(thrustPercent);
        foreach (var wheel in wheels)
        {
            wheel.brakeTorque = brakesTorque;
            // small torque to wake up wheel collider
            wheel.motorTorque = 0.01f;
        }
    }

    public void SetControlSurfecesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var surface in controlSurfaces)
        {
            if (surface == null || !surface.IsControlSurface) continue;
            switch (surface.InputType)
            {
                case ControlInputType.Pitch:
                    surface.SetFlapAngle(pitch * pitchControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Roll:
                    surface.SetFlapAngle(roll * rollControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Yaw:
                    surface.SetFlapAngle(yaw * yawControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Flap:
                    surface.SetFlapAngle(Flap * surface.InputMultiplyer);
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            SetControlSurfecesAngles(Pitch, Roll, Yaw, Flap);
    }
}




    

