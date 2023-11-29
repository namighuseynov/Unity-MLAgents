using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CubeAgent : Agent
{
    public float moveSpeed = 1.0f;
    public float turningSpeed = 1.0f;
    public Transform target;
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float signal1 = actions.ContinuousActions[0];
        float signal2 = actions.ContinuousActions[1];


            Vector3 rotation = transform.localEulerAngles + new Vector3(0, turningSpeed*signal1, 0);
            transform.localEulerAngles = rotation;

            transform.Translate(new Vector3 (0, 0, moveSpeed*signal2));
        

        float distance = Vector3.Distance(transform.localPosition, target.localPosition);
        if (distance < 3)
        {
            target.localPosition = new Vector3(Random.Range(-4, 4), 1, Random.Range(-4, 4));
            SetReward(1f);
        }
        else
        {
            SetReward(-0.01f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actionsContinues = actionsOut.ContinuousActions;
        actionsContinues[0] = Input.GetAxisRaw("Horizontal");
        actionsContinues[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 1.1f, 0);
        transform.localEulerAngles = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
        }
    }
}
