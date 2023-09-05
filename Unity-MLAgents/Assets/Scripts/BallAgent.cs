using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Rigidbody _rBody;

    private void Start()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_targetTransform.localPosition);
        sensor.AddObservation(_rBody.velocity.x);
        sensor.AddObservation(_rBody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        _rBody.AddForce(controlSignal);



        float distanceToTarget = Vector3.Distance(this.transform.localPosition, _targetTransform.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}