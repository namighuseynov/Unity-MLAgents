using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTargetAgent : Agent
{
    public float MoveSpeed = 5f;
    [SerializeField] private Transform _targetPosition;
    [SerializeField] private Material _successMat;
    [SerializeField] private Material _failureMat;
    [SerializeField] private GameObject _env;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(4.189f, 0.5f, 4.210f);
        //  _targetPosition.position = new Vector3(-4.5f, 0.5f, -4.5f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(_targetPosition.position);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float MovX = actions.ContinuousActions[0];
        float MovZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(MovX, 0f, MovZ) * MoveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            AddReward(-10f);
            _env.GetComponent<MeshRenderer>().material = _failureMat;
            EndEpisode();
        }
        else if (other.tag == "Target")
        {
            AddReward(10f);
            _env.GetComponent<MeshRenderer>().material = _successMat;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
}