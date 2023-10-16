
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float Reward { get; internal set; }
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
}
