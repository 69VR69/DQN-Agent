using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Assets.Scripts;

using UnityEngine;


public class ActionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Agent _agent;

    [Header("Inputs")]
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 10f;

    public void Move(Vector3 direction)
    {
        Debug.Log($"Move to {direction}");
        _agent.Move(direction * _moveSpeed);
    }

    public void Jump()
    {
        Debug.Log("Jump");
        _agent.Jump(_jumpForce);
    }

    public void Rotate(Quaternion direction)
    {
        Debug.Log($"Rotate to {direction}");
        _agent.Rotate(direction * _rotationSpeed);
    }

    public void MakeAction(AgentAction action)
    {
        switch (action)
        {
            case AgentAction.MOVE_UP:
                Move(Vector3.up);
                break;
            case AgentAction.MOVE_RiGHT:
                Move(Vector3.right);
                break;
            case AgentAction.MOVE_DOWN:
                Move(Vector3.down);
                break;
            case AgentAction.MOVE_LEFT:
                Move(Vector3.left);
                break;
            case AgentAction.TURN_RIGHT:
                Rotate(new Quaternion(Vector3.Zero, 1));
                break;
            case AgentAction.TURN_LEFT:
                Rotate(new Quaternion(Vector3.Zero, -1));
                break;
            case AgentAction.JUMP:
                Jump();
                break;
            case AgentAction.WAIT:
                break;
            default:
                break;
        }
    }
}
