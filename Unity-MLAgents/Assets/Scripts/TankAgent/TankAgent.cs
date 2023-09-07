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
    private bool ended = false;
    [SerializeField] private Transform targetPos;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 v_up = transform.TransformDirection(Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast(transform.localPosition, v_up, out hit, 1.5f))
        {
            Debug.DrawRay(transform.localPosition, v_up*1.5f, Color.red);

            if (hit.collider.gameObject.tag == "Plane")
            {
                ended = true;
                SetReward(-1f);
                EndEpisode();
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y < 0 || ended)
        {
            this.rbody.angularVelocity = Vector3.zero;
            this.rbody.velocity = Vector3.zero;
            Vector3 spawnPosition = new Vector3(0f, 0.6f, 0f);
            Vector3 angles = new Vector3(0f, 0f, 0f);
            transform.localPosition = spawnPosition;
            transform.rotation = Quaternion.Euler(angles);
            ended = false;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetPos.localPosition);

        sensor.AddObservation(rbody.velocity.x);
        sensor.AddObservation(rbody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float signalX = actions.ContinuousActions[0];
        float signalY = actions.ContinuousActions[1];


        float rotSpeed = 0.1f + rbody.velocity.magnitude / 20;
        float dRot = signalX * rotSpeed;

        float motorForce = maxTorque * signalY;

        foreach (var wheel in Wheels)
        {
            wheel.motorTorque = motorForce;
        }

        transform.Rotate(Vector3.up * dRot);

        float distance = Vector3.Distance(transform.localPosition, targetPos.localPosition);
        if (distance < 5f)
        {
            SetReward(100f);
            Vector3 newTargetPosition = new Vector3(Random.Range(-9, 9), 0.5f, Random.Range(-9, 9));
            targetPos.localPosition = newTargetPosition;
            EndEpisode();
        }
        if (transform.localPosition.y < 0)
        {
            AddReward(-1f);
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