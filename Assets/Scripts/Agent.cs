using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Unity.VisualScripting;

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

    private Vector3 _agentPosition;
    private Quaternion _agentRotation;


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

    private void Start()
    {
        _agentPosition = transform.position;
        _agentRotation = transform.rotation;
    }

    private List<List<Vector3>> ComputeLidarPoints()
    {
        List<int> layers =
            Enumerable.Range(0, _lidarLayerZ)
            .ToList();

        float ax = Mathf.Deg2Rad * _lidarAngleX;
        float ay = Mathf.Deg2Rad * _lidarAngleY;
        Vector2 Q = new(Mathf.Tan(ax), Mathf.Tan(ay));
        Vector2 resolution = new(_lidarResolutionX - 1, _lidarResolutionY - 1);

        Debug.Log($"Lidar resolution is {resolution}, layers {string.Join(",", layers)}");

        List<List<Vector3>> lidarPoints = new();
        lidarPoints = layers
            .AsParallel()
            .Select(layers => ComputeLidarLayer(_lidarDistance + (layers * _lidarOffsetZ), resolution, Q))
            .ToList();

        return lidarPoints;
    }

    private List<Vector3> ComputeLidarLayer(float distance, Vector2 resolution, Vector2 Q)
    {
        var layer = new List<Vector3>();

        Q *= distance;

        for (int i = 0; i < _lidarResolutionX; i++)
            for (int j = 0; j < _lidarResolutionY; j++)
            {
                var x = i - resolution.x / 2;
                var y = j - resolution.y / 2;

                var point = new Vector3(x * Q.x, y * Q.y, distance);
                Debug.Log($"Lidar point {i},{j} is {point}");
                layer.Add(point);
            }

        return layer;
    }

    private void OnDrawGizmos()
    {
        foreach (var layer in LidarPoints)
            foreach (var point in layer)
            {
                //Debug.Log(point);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(point, 0.1f);
            }

        // Draw line only for last layer
        foreach (var point in LidarPoints.Last())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, point);
        }
    }
}
