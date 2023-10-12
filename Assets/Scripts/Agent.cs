using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Unity.VisualScripting;

using UnityEngine;

public class Agent : MonoBehaviour
{
    [Header("Lidar")]
    [SerializeField] private int _lidarDistance = 10;
    [SerializeField] private int _lidarResolutionX = 3;
    [SerializeField] private int _lidarResolutionY = 10;
    [SerializeField] private int _lidarLayerZ = 3;
    [SerializeField] private int _lidarOffsetZ = 3;
    [SerializeField] private int _lidarAngleX = 10;
    [SerializeField] private int _lidarAngleY = 8;


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
        int u = _lidarDistance;
        int ax = _lidarAngleX;
        int ay = _lidarAngleY;
        int resX = _lidarResolutionX - 1;
        int resY = _lidarResolutionY - 1;

        float Qx = Mathf.Abs(2 * Mathf.Tan(ax / 2));
        float Qy = Mathf.Abs(2 * Mathf.Tan(ay / 2));

        var lidarPoints = new List<List<Vector3>>();

        List<int> layers = Enumerable.Range(0, _lidarLayerZ).Where(i => i % _lidarOffsetZ == 0).ToList();

        ParallelQuery<List<Vector3>> t = layers.AsParallel().Select(layers => ComputeLidarLayer(u, ax, ay, resX, resY, Qx, Qy, layers));

        //TODO: Improve performances + points are placed as square starting from player, should be a projection instead

        lidarPoints = t.ToList();

        return lidarPoints; //RotateLidarPoints(lidarPoints);
    }

    private static List<Vector3> ComputeLidarLayer(int u, int ax, int ay, int resX, int resY, float Qx, float Qy, int z)
    {
        u += z;
        int widht = (int)(u * Qx); //CD
        int height = (int)(u * Qy); //BC

        int offsetX = widht / resX;
        int offsetY = height / resY;

        Vector3 center = new(((float)widht) / 2, ((float)height) / 2, z);

        Debug.Log($"Computing lidar points with u={u}, ax={ax}, ay={ay}, Qx={Qx}, Qy={Qy}, widht={widht}, height={height}, offsetX={offsetX}, offsetY={offsetY}, center={center}");

        var layer = new List<Vector3>();

        for (int x = 0; x <= widht; x += offsetX)
            for (int y = 0; y <= height; y += offsetY)
            {
                // Find where to place the first point between 0 and width
                Vector3 pos = new(x, y, z);
                Debug.Log($"Adding point {pos}");

                layer.Add(pos);
            }

        return layer;
    }

    private List<List<Vector3>> RotateLidarPoints(List<List<Vector3>> lidarPoints)
    {
        var rotationMatrix = Matrix4x4.Rotate(new(0, 90, 0, 0));
        // Get the center of the lidar points
        var center = new Vector3(lidarPoints[0].Count / 2, lidarPoints[0][0].y, lidarPoints[0][0].z);

        var rotatedLidarPoints = new List<List<Vector3>>();



        return lidarPoints;
    }

    private void OnDrawGizmos()
    {
        foreach (var layer in LidarPoints)
            foreach (var point in layer)
            {
                //Debug.Log(point);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(point, 0.1f);
                Gizmos.DrawLine(transform.position, point);
            }
    }
}
