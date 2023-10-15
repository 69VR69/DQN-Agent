using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Agent : MonoBehaviour
{
    [Header("Lidar")]
    [SerializeField] private float _lidarDistance = 10;
    [SerializeField] private int _lidarResolutionX = 3;
    [SerializeField] private int _lidarResolutionY = 10;
    [SerializeField] private int _lidarLayerZ = 3;
    [SerializeField] private float _lidarOffsetZ = 3;
    [SerializeField] private float _lidarAngleX = 10;
    [SerializeField] private float _lidarAngleY = 8;

    private enum LidarTrigger
    {
        Obstacle = -1,
        None = 0,
        Goal = 1,
    }

    private List<List<Vector3>> _lidarPoints;
    public List<List<Vector3>> LidarPoints
    {
        get
        {
            // if (_lidarPoints == null)
            _lidarPoints = ComputeLidarPoints();

            return _lidarPoints;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            var lidarTrigger = GetLidarTrigger();
            Debug.Log($"Lidar trigger is {string.Join(",", lidarTrigger.SelectMany(x => x))}");
        }
    }

    private List<List<Vector3>> ComputeLidarPoints()
    {
        List<int> layers =
            Enumerable.Range(0, _lidarLayerZ)
            .ToList();

        float ax = Mathf.Deg2Rad * _lidarAngleX;
        float ay = Mathf.Deg2Rad * _lidarAngleY;
        Vector2 Q = new(Mathf.Tan(ax), Mathf.Tan(ay));
        Vector2 resolution = new(_lidarResolutionX, _lidarResolutionY);

        Debug.Log($"Lidar resolution is {resolution}, layers {string.Join(",", layers)}");

        List<List<Vector3>> lidarPoints = new();
        Vector3 agentPosition = transform.position;
        Vector3 agentRotation = transform.rotation.eulerAngles;
        lidarPoints = layers
            .AsParallel()
            .Select(layers =>
            {
                float distance = _lidarDistance + (layers * _lidarOffsetZ);
                return ComputeLidarLayer(agentPosition, agentRotation, distance, resolution, Q);
            })
            .ToList();

        return lidarPoints;
    }

    private static List<Vector3> ComputeLidarLayer(Vector3 agentPosition, Vector3 agentRotation, float distance, Vector2 resolution, Vector2 Q)
    {
        var layer = new List<Vector3>();

        Q *= distance;

        Vector3 offset = new(Q.x * resolution.x / 2, Q.y * resolution.y / 2, 0);
        Vector3 agentOffset = offset;//new(resolution.x / 2, resolution.y / 2, 0);
        agentOffset += agentPosition;
        Debug.Log($"Lidar offset is {agentOffset}");
        for (int i = 0; i <= resolution.x; i++)
            for (int j = 0; j <= resolution.y; j++)
            {
                Vector2 coord = new(i - resolution.x, j - resolution.y);
                coord *= Q;

                Vector3 point = new(coord.x, coord.y, distance);

                // Align lidar position with agent
                point += agentOffset;

                // Align lidar rotation with agent
                point = Quaternion.Euler(agentRotation).normalized * point;

                layer.Add(point);

                //Debug.Log($"Lidar point {i},{j} is {point}");
            }

        return layer;
    }

    private List<List<float>> GetLidarTrigger()
    {
        // Iterate over lidar points and get the LidarTrigger value
        List<List<float>> lidarTrigger = new();

        foreach (var layer in LidarPoints)
        {
            List<float> layerTrigger = new();
            foreach (var point in layer)
            {
                //Debug.Log(point);
                var trigger = Physics.Raycast(point, transform.forward, out RaycastHit hit, _lidarDistance) ? hit.collider.tag : "None";
                layerTrigger.Add(trigger == "Obstacle" ? (float)LidarTrigger.Obstacle : trigger == "Goal" ? (float)LidarTrigger.Goal : (float)LidarTrigger.None);
            }
            lidarTrigger.Add(layerTrigger);
        }

        return lidarTrigger;
    }

    private void OnDrawGizmos()
    {
        float maxDistance = _lidarDistance + (_lidarLayerZ * _lidarOffsetZ);
        foreach (var layer in LidarPoints)
        {
            //Change color for each layer based on the distance
            Gizmos.color = Color.Lerp(Color.yellow, Color.magenta, layer[0].z/maxDistance);
            foreach (var point in layer)
            {
                Gizmos.DrawSphere(point, 0.1f);
                Gizmos.DrawLine(transform.position, point);
            }
        }
    }
}
