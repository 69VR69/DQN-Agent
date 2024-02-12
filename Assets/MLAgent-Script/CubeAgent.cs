using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Collections.Generic;
using System.Linq;

public class CubeAgent : Unity.MLAgents.Agent
{
    private Vector3 _initialPosition;
    private Rigidbody _rb;

    [SerializeField]
    private GameObject _goal;

    [SerializeField]
    private RayPerceptionSensorComponent3D _rayPerceptionSensorComponent3D;

    [SerializeField]
    private List<GameObject> _checkpoints;
    private GameObject _nextCheckPoint = null;

    private List<Vector3> _oldPositions = new List<Vector3>();
    private float gamma = 0.009f;

    private void Start()
    {
        // Cache
        _rb = GetComponent<Rigidbody>();

        // Set initial position
        _initialPosition = transform.position;

        // Check parameters set in the inspector
        if (_goal == null)
            Debug.LogError("Goal not set");
    }

    public override void OnEpisodeBegin()
    {
        // Reset
        transform.position = _initialPosition;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        // Reset old positions
        _oldPositions.Clear();

        // Reset checkpoints
        foreach (GameObject checkpoint in _checkpoints)
            checkpoint.SetActive(true);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add Cube informations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(_rb.velocity);

        // Add Goal informations
        sensor.AddObservation(_goal.transform.position);

        // Add Goal distance
        sensor.AddObservation(Vector3.Distance(transform.position, _goal.transform.position));

        // Add RayPerceptionSensorComponent3D observations
        sensor.AddObservation(_rayPerceptionSensorComponent3D);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // actions [0] = Forward/Backward, actions [1] = Left/Right
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        _rb.velocity = Vector3.Lerp(_rb.velocity, controlSignal * 5f, Time.deltaTime * 5f);

        // Old positions
        //_oldPositions.Add(transform.position);

        // Rewards
        if (!IsOnGround())
        {
            SetReward(-10f);
            EndEpisode();
        }


        // Calculate the percentage of the distance to the goal from the initial position
        float distanceToGoal = Vector3.Distance(transform.position, _goal.transform.position);
        float distanceFromInitialPosition = Vector3.Distance(transform.position, _initialPosition);
        float percentage = (float)Math.Pow(distanceFromInitialPosition / distanceToGoal, 2);
        SetReward(percentage);

        // Calculate the percentage of the distance to the next checkpoint from the initial position
        if (_nextCheckPoint == null)
        {
            _nextCheckPoint = _checkpoints.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
        }
        float distanceToNextCheckPoint = Vector3.Distance(transform.position, _nextCheckPoint.transform.position);
        float percentageToNextCheckPoint = (float)Math.Pow(distanceFromInitialPosition / distanceToNextCheckPoint, 4);
        SetReward(percentageToNextCheckPoint);


        //// Reward for going in the direction of the goal
        //if (_oldPositions.Count > 100)
        //{
        //    float distanceToGoal = Vector3.Distance(transform.position, _goal.transform.position);
        //    float oldDistanceToGoal = Vector3.Distance(_oldPositions.Last(), _goal.transform.position);
        //    float reward = (oldDistanceToGoal - distanceToGoal) * 10f;
        //    SetReward(reward);


        //    //// Add penalty for going in the direction of the old positions
        //    //float cumulativeDistanceToOldPositions = 0f;
        //    //for (int i = 0; i < _oldPositions.Count; i++)
        //    //    cumulativeDistanceToOldPositions += Vector3.Distance(transform.position, _oldPositions[i]) * (i * gamma);
        //    //cumulativeDistanceToOldPositions /= _oldPositions.Count;

        //    //Debug.Log($"cumulativeDistanceToOldPositions: {cumulativeDistanceToOldPositions}");
        //    //SetReward(cumulativeDistanceToOldPositions);
        //}

        //////Reward for going far from initial position
        ////float distanceFromInitialPosition = Vector3.Distance(transform.position, _initialPosition) * 10f;
        ////Debug.Log($"distanceFromInitialPosition: {distanceFromInitialPosition}");
        ////SetReward(distanceFromInitialPosition);

        //// Remove the 500 oldest positions on 5000
        //if (_oldPositions.Count > 1000)
        //    _oldPositions.RemoveAt(0);

        Debug.Log($"Reward: {GetCumulativeReward()}");

        // Add time penalty
        //SetReward(-0.01f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Goal"))
        {
            SetReward(100f);
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            SetReward(-3f);
            EndEpisode();
        }
        else if (other.CompareTag("CheckPoint"))
        {
            SetReward(10f);
            other.SetActive(false);
            _nextCheckPoint = _checkpoints.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
        }
    }

    private bool IsOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        // Forward/Backward
        continuousActionsOut[0] = Input.GetAxis("Vertical");

        // Left/Right
        continuousActionsOut[1] = -Input.GetAxis("Horizontal");
    }

    private void OnDrawGizmos()
    {
        // Draw a point for each old position
        foreach (Vector3 oldPosition in _oldPositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(oldPosition, 0.1f);
        }
    }
}
