using System;
using System.Collections;
using System.Collections.Generic;

using Assets.Scripts;

using UnityEngine;


public class ActionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Agent _agent;

    [Header("Inputs")]
    [SerializeField] private float _jumpForce = 80f;
    [SerializeField] private float _moveSpeed = 80f;
    [SerializeField] private int _rotationDegree = 90;

    public void Move(Vector3 direction)
    {
        _agent.Move(direction * _moveSpeed);
    }

    public void Jump()
    {
        _agent.Jump(_jumpForce);
    }

    public void Rotate(Vector3 direction)
    {
        // Rotate the agent of 90 degrees on the Y axis in the given direction
        Quaternion rotation = Quaternion.Euler(0, _rotationDegree * direction.y, 0);
        _agent.Rotate(rotation);
    }

    public void MakeAction(AgentAction action)
    {
        switch (action)
        {
            case AgentAction.MOVE_UP:
                Debug.Log("Move up");
                Move(Vector3.forward);
                break;
            case AgentAction.MOVE_RiGHT:
                Debug.Log("Move right");
                Move(Vector3.right);
                break;
            case AgentAction.MOVE_DOWN:
                Debug.Log("Move down");
                Move(Vector3.back);
                break;
            case AgentAction.MOVE_LEFT:
                Debug.Log("Move left");
                Move(Vector3.left);
                break;
            case AgentAction.TURN_RIGHT:
                Debug.Log("Turn right");
                Rotate(Vector3.up);
                break;
            case AgentAction.TURN_LEFT:
                Debug.Log("Turn left");
                Rotate(Vector3.down);
                break;
            case AgentAction.JUMP:
                Debug.Log("Jump");
                Jump();
                break;
            case AgentAction.WAIT:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            MakeAction(AgentAction.JUMP);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            MakeAction(AgentAction.MOVE_DOWN);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            MakeAction(AgentAction.MOVE_UP);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MakeAction(AgentAction.MOVE_LEFT);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MakeAction(AgentAction.MOVE_RiGHT);

        if (Input.GetKeyDown(KeyCode.A))
            MakeAction(AgentAction.TURN_LEFT);
        if (Input.GetKeyDown(KeyCode.E))
            MakeAction(AgentAction.TURN_RIGHT);

    }
}
