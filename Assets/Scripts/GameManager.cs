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
    public bool ResponseRequested { get; internal set; } = false;
    public bool IsDone { get; internal set; } = false;
    public bool IsActionDuring { get; internal set; } = false;


    private void Start()
    {
        Instance = this;
        ActionHandler = GetComponent<ActionHandler>();
    }

    public void ResetGame()
    {
        IsDone = false;
        Debug.Log("TODO : Game Reset");
    }

    public Matrix<float> GetState() => 
        _agent.GetComponent<Lidar>().GetLidarTrigger();

    public float GetReward() =>
        _agent.Reward;

    public void MakeAction(AgentAction action)
    {
        IsActionDuring = true;
        ActionHandler.MakeAction(action);
        // Wait for the action to be done
        StartCoroutine(WaitForAction());

        IEnumerator WaitForAction()
        {
            yield return new WaitUntil(() => !IsActionDuring);
            ResponseRequested = true;
        }
    }
}
