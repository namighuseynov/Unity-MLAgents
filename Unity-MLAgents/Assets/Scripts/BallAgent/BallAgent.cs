using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Rigidbody _rBody;

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            this._rBody.angularVelocity = Vector3.zero;
            this._rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
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
        _rBody.AddForce(controlSignal*10);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, _targetTransform.localPosition);

        if (distanceToTarget < 1.3f)
        {
            SetReward(10f);
            Vector3 newTargetPosition = new Vector3(Random.Range(-19, 19), 0.5f, Random.Range(-19, 19));
            _targetTransform.localPosition = newTargetPosition;
            EndEpisode();
        }

        if (this.transform.localPosition.y < 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxisRaw("Horizontal");
        continuousActionsOut[1] = Input.GetAxisRaw("Vertical");
    }
}