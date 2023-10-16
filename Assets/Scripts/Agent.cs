using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Numerics;

using UnityEngine;

public class Agent : MonoBehaviour
{
    public float Reward { get; internal set; }
    private RigidBody rb;

    private void Start()
    {
        rb = GetComponent<RigidBody>();
    }

    public void Jump(float jumpForce) =>
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    public void Move(Vector3 movement) =>
        rb.velocity = movement * Time.fixedDeltaTime;


    public void Rotate(Quaternion rotation)
        => rb.rotation += rotation;
}
