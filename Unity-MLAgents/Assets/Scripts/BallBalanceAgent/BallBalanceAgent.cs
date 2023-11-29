using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallBalanceAgent : Agent
{
    public Transform Floor;
    public Rigidbody Body;
    public EnvironmentParameters defaultParameters;

    public override void Initialize()
    {
        defaultParameters = Academy.Instance.EnvironmentParameters;
        Body = GetComponent<Rigidbody>();
        ResetScene();
    }

    public override void OnEpisodeBegin()
    {
        Floor.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        Floor.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        Floor.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        Body.velocity = new Vector3(0f, 0f, 0f);
        transform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f))
            + Floor.transform.localPosition;
        ResetScene();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(Body.velocity);
        sensor.AddObservation(Floor.localEulerAngles.x);
        sensor.AddObservation(Floor.localEulerAngles.z);
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var zangle = 2f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var xangle = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        if ((Floor.transform.localRotation.z < 0.25f && zangle > 0f) ||
            (Floor.transform.localRotation.z > -0.25f && zangle < 0f))
        {
            Floor.transform.Rotate(new Vector3(0, 0, 1), zangle);
        }
        if ((Floor.transform.localRotation.x < 0.25f && xangle > 0f) ||
            (Floor.transform.localRotation.x > -0.25f && xangle < 0f))
        {
            Floor.transform.Rotate(new Vector3(1, 0, 0), xangle);
        }
        if ((transform.localPosition.y - Floor.transform.localPosition.y) < -2f ||
            Mathf.Abs(transform.localPosition.x - Floor.transform.localPosition.x) > 3f ||
            Mathf.Abs(transform.localPosition.z - Floor.transform.localPosition.z) > 3f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }

        Debug.Log(GetCumulativeReward());
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuesActionsOut = actionsOut.ContinuousActions;
        continuesActionsOut[0] = Input.GetAxisRaw("Horizontal");
        continuesActionsOut[1] = Input.GetAxisRaw("Vertical");
    }

    public void ResetScene()
    {
        Body.mass = defaultParameters.GetWithDefault("mass", 1.0f);
        var scale = defaultParameters.GetWithDefault("scale", 1.0f);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
