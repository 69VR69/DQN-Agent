using System;
using System.Collections;
using System.Collections.Generic;

using Assets.Scripts;

using UnityEngine;

[RequireComponent(typeof(ActionHandler))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; internal set; }
    [SerializeField]
    private Agent _agent;
    public ActionHandler ActionHandler { get; internal set; }
    public ServerManager ServerManager { get; internal set; }
    public bool ResponseRequested { get; internal set; } = false;
    public bool IsDone { get; internal set; } = false;
    public bool IsActionDuring { get; internal set; } = false;


    private void Start()
    {
        Instance = this;
        ActionHandler = GetComponent<ActionHandler>();
        ServerManager = GetComponentInChildren<ServerManager>();
        ServerManager.GameManager = this;
    }

    public void ResetGame()
    {
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
        Debug.Log("Make Action");
        IsActionDuring = true;
        ActionHandler.MakeAction(action);
    }

    public void StartWait()
    {
        Debug.Log("Start Coroutine");
        StartCoroutine(WaitForAction());
        Debug.Log("End Coroutine");
    }

    IEnumerator WaitForAction()
    {
        Debug.Log("start coroutine");
        yield return new WaitForSeconds(1);
        //yield return new WaitUntil(() => !IsActionDuring);
        Debug.Log("stop coroutine");
        //_ = ServerManager.ModelSend();
    }
}
