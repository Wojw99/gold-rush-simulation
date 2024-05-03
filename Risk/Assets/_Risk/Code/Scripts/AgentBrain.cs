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

        agentVisionSensor.EnemySpotted += OnEnemySpotted;
        agentVisionSensor.DepositSpotted += OnDepositSpotted;
        agentVisionSensor.HealSpotted += OnHealSpotted;
        agentVisionSensor.RestSpotted += OnRestSpotted;

        agentInteractionSensor.InteractionStarted += OnInteractionStarted;
        agentInteractionSensor.InteractionEnded += OnInteractionEnded; 
        agentInteractionSensor.InteractionExited += OnInteractionExited;

        ConsiderGoalChanging();
    }

    private void OnInteractionStarted(InteractionType interactionType) {
        Debug.Log("OnInteractionStarted: " + interactionType);
        if(interactionType == InteractionType.DEPOSIT 
            && (goal == GoalName.SEARCH_FOR_DEPOSIT || goal == GoalName.GO_TO_NEAREST_DEPOSIT)) 
        {
            Debug.Log("Mine");
            goal = GoalName.MINE_DEPOSIT;
            GoalChanged?.Invoke(goal);
        }
        if(interactionType == InteractionType.REST 
            && (goal == GoalName.SEARCH_FOR_REST || goal == GoalName.GO_TO_NEAREST_REST)) 
        {
            goal = GoalName.TAKE_REST;
            GoalChanged?.Invoke(goal);
        }
    }

    private void OnInteractionEnded(InteractionType interactionType) {
        Debug.Log("OnInteractionEnded: " + interactionType);
        ConsiderGoalChanging();
    }

    private void OnInteractionExited(InteractionType interactionType) {
        Debug.Log("OnInteractionExited: " + interactionType);
        ConsiderGoalChanging();
    }

    private void OnHealSpotted() {
        if(goal == GoalName.SEARCH_FOR_HEALING)
        {
            goal = GoalName.GO_TO_NEAREST_HEALING;
            GoalChanged?.Invoke(goal);
        }
    }

    private void OnDepositSpotted() {
        if(goal == GoalName.SEARCH_FOR_DEPOSIT)
        {
            goal = GoalName.GO_TO_NEAREST_DEPOSIT;
            GoalChanged?.Invoke(goal);
        }
    }

    private void OnRestSpotted() {
        if(goal == GoalName.SEARCH_FOR_REST)
        {
            goal = GoalName.GO_TO_NEAREST_REST;
            GoalChanged?.Invoke(goal);
        }
    }

    private void OnEnemySpotted() {
        goal = GoalName.RUN_FOR_YOUR_LIFE;
        GoalChanged?.Invoke(goal);
    }

    private void Update() {
        
    }

    private void ConsiderGoalChanging() {
        var calculatedGoal = CalculateGoal();
        
        if (goal != calculatedGoal) {
            goal = calculatedGoal;
            GoalChanged?.Invoke(goal);
        }
    }

    private GoalName CalculateGoal() {
        var goal = GoalName.SEARCH_FOR_DEPOSIT;

        if(agentStatus.Stamina <= agentStatus.MaxStamina / 3f) {
            goal = GoalName.SEARCH_FOR_REST;
        }

        if(agentStatus.Health <= agentStatus.MaxHealth / 2f) {
            goal = GoalName.SEARCH_FOR_HEALING;
        }

        return goal;
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
