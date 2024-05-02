using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBrain : MonoBehaviour
{
    public GoalName goal;

    public event Action<GoalName> GoalChanged;

    private AgentStatus agentStatus;
    
    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        StartCoroutine(StartSitting());
        StartCoroutine(StartWalking());
    }

    private void Update() {
        goal = GoalName.SEARCH_FOR_DEPOSIT;
        GoalChanged?.Invoke(goal);

        if(agentStatus.nearestSpottedDeposit != null) {
            goal = GoalName.GO_TO_NEAREST_DEPOSIT;
            GoalChanged?.Invoke(goal);
        }

        if(agentStatus.collidedDeposit != null) {
            goal = GoalName.MINE_DEPOSIT;
            GoalChanged?.Invoke(goal);
        }

        if(agentStatus.nearestSpottedUndead != null) {
            goal = GoalName.RUN_FOR_YOUR_LIFE;
            GoalChanged?.Invoke(goal);
        }
    }

    private IEnumerator StartSitting()
    {
        yield return new WaitForSeconds(1);
        goal = GoalName.FREEZE;
        GoalChanged?.Invoke(goal);
    }

    private IEnumerator StartWalking()
    {
        yield return new WaitForSeconds(4);
        goal = GoalName.SEARCH_FOR_DEPOSIT;
        GoalChanged?.Invoke(goal);
    }

    public enum GoalName
    {
        FREEZE,
        SEARCH_FOR_DEPOSIT,
        GO_TO_NEAREST_DEPOSIT,
        MINE_DEPOSIT,
        RUN_FOR_YOUR_LIFE
    }
}
