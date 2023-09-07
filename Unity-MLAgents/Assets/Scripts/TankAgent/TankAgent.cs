using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TankAgent : Agent
{
    public float motorTorque = 100f;
    public float steerAngle = 30f;
    public float brakeTorque = 300f;

    public WheelCollider[] Wheels;

    private Rigidbody rbody;
    [SerializeField] private Transform targetPos;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        foreach (var wheel in Wheels) { wheel.motorTorque = motorTorque; }
    }

    public override void OnEpisodeBegin()
    {
        if (transform.position.y < 0)
        {
            Vector3 spawnPosition = new Vector3(0f, 0.5f, 0f);
            transform.position = spawnPosition;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetPos.position);

        sensor.AddObservation(rbody.velocity.x);
        sensor.AddObservation(rbody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 signal = Vector3.zero;
        signal.x = actions.ContinuousActions[0];
        signal.y = actions.ContinuousActions[1];

        float motorForce = motorTorque * signal.y;
        float steeringAngle = steerAngle * signal.x;

        foreach (var wheel in Wheels)
        {
            wheel.motorTorque = motorForce;
        }

        float distance = Vector3.Distance(transform.position, targetPos.position);
        if (distance < 1.4f)
        {
            SetReward(10f);
            EndEpisode();
        }
        if (transform.position.y < 0)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuiousActionsOut = actionsOut.ContinuousActions;
        continuiousActionsOut[0] = Input.GetAxisRaw("Horizontal");
        continuiousActionsOut[1] = Input.GetAxisRaw("Vertical");
    }
}