using System;
using System.Collections;
using System.Collections.Generic;

using Assets.Scripts;

using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(ActionHandler))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; internal set; }
    [SerializeField]
    private Agent _agent;
    [SerializeField]
    private bool _isMockedMode = false;
    public ActionHandler ActionHandler { get; internal set; }
    public ServerManager ServerManager { get; internal set; }
    public bool IsFullAnswerRequested { get; internal set; } = false;
    public bool IsDone { get; internal set; } = false;
    public bool IsActionDuring { get; internal set; } = false;


    private void Start()
    {
        Instance = this;
        ActionHandler = GetComponent<ActionHandler>();
        if (_isMockedMode)
            ServerManager = GetComponentInChildren<ServerManagerMock>();
        else
            ServerManager = GetComponentInChildren<ServerManager>();
        ServerManager.GameManager = this;
    }

    public void ResetGame()
    {
        IsActionDuring = true;
        IsDone = false;
        Debug.Log("TODO : Game Reset");
        IsActionDuring = false;
    }

    public Matrix<float> GetState() =>
        _agent.GetComponent<Lidar>().GetLidarTrigger();

    public float GetReward() =>
        _agent.Reward;

    public void MakeAction(AgentAction action)
    {
        IsActionDuring = true;
        Debug.Log("Make Action");
        ActionHandler.MakeAction(action);
    }

    private void Update()
    {
        //Debug.Log($"is response requested : {IsFullAnswerRequested} and is action during : {IsActionDuring}");
        if (IsFullAnswerRequested && !IsActionDuring)
            ServerManager.ModelSend();
    }
}
