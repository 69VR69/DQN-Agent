using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

using Unity.VisualScripting;

using UnityEngine;

public class Agent : MonoBehaviour
{
    [Header("Lidar")]
    [SerializeField] private float _lidarDistance = 10f;
    [SerializeField] private int _lidarResolutionX = 3;
    [SerializeField] private int _lidarResolutionY = 10;
    [SerializeField] private float _lidarOffsetX = 0.1f;
    [SerializeField] private float _lidarOffsetY = 45f;


    private List<List<Vector3>> _lidarPoints;
    public List<List<Vector3>> LidarPoints
    {
        get
        {
            //if (_lidarPoints == null)
            _lidarPoints = ComputeLidarPoints();

            return _lidarPoints;
        }
    }

    private List<List<Vector3>> ComputeLidarPoints()
    {
        // For each layer, compute the (x, y) coordinates of the lidar points
        var lidarPoints = new List<List<Vector3>>();
        var nbPoints = _lidarResolutionX * _lidarResolutionY;
        var distance = transform.position + new Vector3(_lidarDistance, 0, 0);
        var xSize = _lidarResolutionX * _lidarOffsetX;
        var ySize = _lidarResolutionY * _lidarOffsetY;
        Vector3 center = new((xSize - _lidarOffsetX) / 2, (ySize - _lidarOffsetY) / 2, 0);

        for (var i = 0; i < _lidarResolutionX; i++)
        {
            var layer = new List<Vector3>();
            for (var j = 0; j < _lidarResolutionY; j++)
            {
                var x = i * _lidarOffsetX;
                var y = j * _lidarOffsetY;
                var point = new Vector3(x, y, 0) - center;
                layer.Add(point);
            }
            lidarPoints.Add(layer);
        }


        return RotateLidarPoints(lidarPoints);
    }

    private List<List<Vector3>> RotateLidarPoints(List<List<Vector3>> lidarPoints)
    {
        // Get the current rotation of the agent
        var rotation = transform.rotation.eulerAngles.z;

        // Rotate the lidar points
        var rotatedLidarPoints = new List<List<Vector3>>();
        foreach (var layer in lidarPoints)
        {
            var rotatedLayer = new List<Vector3>();
            foreach (var point in layer)
            {
                var rotatedPoint = Quaternion.Euler(0, 0, rotation) * point;
                rotatedLayer.Add(rotatedPoint);
            }
            rotatedLidarPoints.Add(rotatedLayer);
        }
        return rotatedLidarPoints;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var layer in LidarPoints)
        {
            foreach (var point in layer)
            {
                Debug.Log(point);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + point, 0.1f);
                Gizmos.DrawLine(transform.position, transform.position + point);
            }
        }
    }
}
