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
    private AgentVisionSensor agentVisionSensor;
    private AgentInteractionSensor agentInteractionSensor;
    
    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        agentVisionSensor = GetComponent<AgentVisionSensor>();
        agentInteractionSensor = GetComponent<AgentInteractionSensor>();
    }

    private void Update() {
        var calculatedGoal = CalculateGoal();
        
        if (goal != calculatedGoal) {
            goal = calculatedGoal;
            GoalChanged?.Invoke(goal);
        }
    }

    private GoalName CalculateGoal() {
        var goal = GoalName.SEARCH_FOR_DEPOSIT;

        if(IsVisible(VisionType.DEPOSIT)) {
            goal = GoalName.GO_TO_NEAREST_DEPOSIT;
        }
        if(IsInteractible(InteractionType.DEPOSIT)) {
            goal = GoalName.MINE_DEPOSIT;
        }

        if(agentStatus.stamina <= 0f) {
            goal = GoalName.SEARCH_FOR_REST;

            if(IsVisible(VisionType.REST)) {
                goal = GoalName.GO_TO_NEAREST_REST;
            }
            if(IsInteractible(InteractionType.REST)) {
                goal = GoalName.TAKE_REST;
            }
        }

        if(agentStatus.health <= agentStatus.maxHealth / 2f) {
            goal = GoalName.SEARCH_FOR_HEALING;

            if(IsVisible(VisionType.HEAL)) {
                goal = GoalName.GO_TO_NEAREST_HEALING;
            }
            if(IsInteractible(InteractionType.HEAL)) {
                goal = GoalName.TAKE_HEALING;
            }
        }

        return goal;
    }

    private bool IsInteractible(InteractionType interactionType) {
        foreach (InteractionObject interactible in agentInteractionSensor.interactibles) {
            if (interactible.interactionType == interactionType) {
                return true;
            }
        }
        return false;
    }

    private bool IsVisible(VisionType visionType) {
        foreach (VisionObject visible in agentVisionSensor.visibles) {
            if (visible.visionType == visionType) {
                return true;
            }
        }
        return false;
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
