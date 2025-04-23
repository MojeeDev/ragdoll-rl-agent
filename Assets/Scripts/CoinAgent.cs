using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class CoinAgent : Agent
{
    public Transform target; 
    public float speed = 5f;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        target.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

     public override void CollectObservations(VectorSensor sensor)
    {
        // observe agent & coin positions
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Debug.Log($"MoveX: {moveX}, MoveZ: {moveZ}");

        Vector3 move = new Vector3(moveX, 0, moveZ) * speed;
        rb.AddForce(move * 10f, ForceMode.VelocityChange);

        // reward for getting closer
        float distance = Vector3.Distance(transform.localPosition, target.localPosition);
        SetReward(-distance * 0.01f);
        // reward for collecting the coin
        if (distance < 1f)
        {
            SetReward(1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
