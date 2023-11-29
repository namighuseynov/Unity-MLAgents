using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TankAgent : Agent
{
    public float maxTorque = 100f;
    public float steerAngle = 30f;
    public float brakeTorque = 300f;

    public WheelCollider[] Wheels;

    private Rigidbody rbody;
    [SerializeField] private Transform targetPos;

    public RayPerceptionSensorComponent3D rayPerception;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rayPerception = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public override void OnEpisodeBegin()
    {
        float rotZ = transform.localEulerAngles.z;

        rbody.angularVelocity = Vector3.zero;
        rbody.velocity = Vector3.zero;

        Vector3 spawnPosition = new Vector3(0f, 1.7f, 0f);
        Vector3 angles = new Vector3(0f, 0f, 0f);

        transform.localPosition = spawnPosition;
        transform.localEulerAngles = angles;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(targetPos.localPosition.z);
        sensor.AddObservation(targetPos.localPosition.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float signalX = actions.ContinuousActions[0];
        float signalY = Mathf.Abs(actions.ContinuousActions[1]);

        float rotSpeed = 0.1f + rbody.velocity.magnitude / 20;
        float dRot = signalX * rotSpeed;

        float motorForce = maxTorque * signalY;

        foreach (var wheel in Wheels)
        {
            wheel.motorTorque = motorForce;
        }

        transform.Rotate(Vector3.up * dRot);

        float distance = Vector3.Distance(transform.localPosition, targetPos.localPosition);
        if (distance < 6f)
        {
            AddReward(1f);

            Vector3 newTargetPosition = new Vector3(Random.Range(-99, 99), 0.5f, Random.Range(-99, 99));
            targetPos.localPosition = newTargetPosition;
        }
        if (transform.localPosition.y < 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
        float RotZ = transform.localEulerAngles.z;
        if ((RotZ > 90 && RotZ < 270))
        {
            AddReward(-1f);
            EndEpisode();
        }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuiousActionsOut = actionsOut.ContinuousActions;
        continuiousActionsOut[0] = Input.GetAxisRaw("Horizontal");
        continuiousActionsOut[1] = Mathf.Abs(Input.GetAxisRaw("Vertical"));
    }
}