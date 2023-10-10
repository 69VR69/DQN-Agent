using System;
using System.Collections;
using System.Collections.Generic;

using Assets.Scripts;

using UnityEngine;

[RequireComponent(typeof(ActionHandler))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; internal set; }

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

    public float GetReward()
    {
        Debug.Log("TODO : Get Reward");
        return 1;
    }

    public int GetState()
    {
        Debug.Log("TODO : Get State");
        return 0;
    }

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
