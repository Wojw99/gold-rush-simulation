using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBrain : MonoBehaviour
{
    public GoalName goal;

    public event Action<GoalName> GoalChanged;
    public event Action DepositExtracted;
    public event Action DamageTaken;

    private AgentStatus agentStatus;
    
    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        StartCoroutine(StartSitting());
        StartCoroutine(StartWalking());
    }

    private void Update() {
        goal = GoalName.SEARCH_FOR_DEPOSIT;

        if(goal == GoalName.SEARCH_FOR_DEPOSIT && agentStatus.nearestSpottedDeposit != null) {
            goal = GoalName.GO_TO_NEAREST_DEPOSIT;
        }
        if(goal == GoalName.GO_TO_NEAREST_DEPOSIT && agentStatus.collidedDeposit != null) {
            goal = GoalName.MINE_DEPOSIT;
        }

        if(agentStatus.stamina < 0f) {
            goal = GoalName.SEARCH_FOR_REST;
        }
        if(goal == GoalName.SEARCH_FOR_REST && agentStatus.nearestSpottedRest != null) {
            goal = GoalName.GO_TO_NEAREST_REST;
        }
        if(goal == GoalName.GO_TO_NEAREST_REST && agentStatus.collidedRest != null) {
            goal = GoalName.TAKE_REST;
        }
        if(goal == GoalName.TAKE_REST && agentStatus.stamina >= agentStatus.maxStamina) {
            goal = GoalName.SEARCH_FOR_DEPOSIT;
        }

        if(agentStatus.nearestSpottedUndead != null) {
            goal = GoalName.RUN_FOR_YOUR_LIFE;
        }

        if(agentStatus.depositExtracted) {
            DepositExtracted?.Invoke();
            agentStatus.depositExtracted = false; // can't be done here
        }

        GoalChanged?.Invoke(goal);
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
        RUN_FOR_YOUR_LIFE,
        LEAVE_THE_AREA,
        
        SEARCH_FOR_DEPOSIT,
        GO_TO_NEAREST_DEPOSIT,
        MINE_DEPOSIT,

        SEARCH_FOR_REST,
        GO_TO_NEAREST_REST,
        TAKE_REST,

        SEARCH_FOR_HEALING,
        GO_TO_NEAREST_HEALING,
        TAKE_HEALING,
    }
}
