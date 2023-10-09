using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TODO : 
 * Ensure that your agent script communicates with the script responsible for applying actions using a suitable method (e.g., setting variables or invoking methods).
 * Pass the selected action to the Unity script to apply it to the scene.
 */

public class ActionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Agent _agent;

    [Header("Inputs")]
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 10f;

    public void Move(Vector2 direction)
    {
        Debug.Log($"Move to {direction}");
        //_agent.Move(direction * _moveSpeed);
    }

    public void Jump()
    {
        Debug.Log("Jump");
        //_agent.Jump(_jumpForce);
    }

    public void Rotate(Vector2 direction)
    {
        Debug.Log($"Rotate to {direction}");
        //_agent.Rotate(direction * _rotationSpeed);
    }

}
