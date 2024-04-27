using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBrain : MonoBehaviour
{
    public GoalName Goal { get; private set; }

    public event Action<GoalName> GoalChanged;
    
    private void Start()
    {
        StartCoroutine(StartSitting());
        StartCoroutine(StartWalking());
    }

    private IEnumerator StartSitting()
    {
        yield return new WaitForSeconds(1);
        Goal = GoalName.FREEZE;
        GoalChanged?.Invoke(Goal);
    }

    private IEnumerator StartWalking()
    {
        yield return new WaitForSeconds(4);
        Goal = GoalName.SEARCH_FOR_DEPOSIT;
        GoalChanged?.Invoke(Goal);
    }

    public enum GoalName
    {
        FREEZE,
        SEARCH_FOR_DEPOSIT,
        MINE_DEPOSIT,
        RUN_FOR_YOUR_LIFE
    }
}
