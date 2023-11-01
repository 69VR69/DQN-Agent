
using Assets.Scripts;

using UnityEngine;

[RequireComponent(typeof(Lidar),typeof(Rigidbody))]
public class Agent : MonoBehaviour
{
    public float Reward { get; internal set; }
    private Rigidbody rb;
    private Lidar lidar;
    private Vector3 lastPosition;
    private Vector3 lastRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        lidar = GetComponent<Lidar>();
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
    }

    public void Jump(float jumpForce)
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Move(Vector3 movement)
    {
        rb.AddForce(movement, ForceMode.Impulse);
    }

    public void Rotate(Quaternion rotation)
    {
        Quaternion newRotation = transform.rotation * rotation;
        rb.MoveRotation(newRotation);
    }

    private void Update()
    {
        if (transform.position != lastPosition || transform.eulerAngles != lastRotation)
        {
            lidar.UpdateLidar();
            lastPosition = transform.position;
            lastRotation = transform.eulerAngles;
        }
    }
}
