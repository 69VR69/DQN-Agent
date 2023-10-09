using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TODO:
 * Reward Handling:
    * In your agent script, calculate rewards based on the conditions you mentioned (+10 for completing the episode, -1 for touching an obstacle, -10 for time's out).
    * Accumulate rewards for each step within an episode.
 * Episode Handling:
    * Define episodes in your Unity environment based on your criteria (e.g., reaching the goal or time's out).
    * When an episode ends, reset the environment to its initial state.
    * Ensure that your agent script keeps track of the episode count and current state.
 */

public class GameManager : MonoBehaviour
{
    public static object Instance { get; internal set; }

    public void LevelCompleted()
    {
        Debug.Log("Level Completed!");
    }
}
