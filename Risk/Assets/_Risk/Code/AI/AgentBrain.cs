using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : MonoBehaviour
{
    AgentStatus agentStatus;
    NavMeshAgent navMeshAgent; 
    AnimationController animationController;

    [SerializeField] AgentSensor visionSensor; 
    [SerializeField] AgentSensor interactionSensor; 

    AgentGoal _currentGoal;
    AgentAction _currentAction;
    List<AgentGoal> goals; // sorted from highest to lowest priority
    List<AgentAction> actions;

    public event Action<AgentGoal> GoalChanged;
    public event Action<AgentAction> ActionChanged;

    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<AnimationController>();
        agentStatus = GetComponent<AgentStatus>();

        goals = new List<AgentGoal>() {
            new AgentGoal(
                name: "search",
                actions: new List<string> { "move_randomly", "go_to_deposit", "mine_deposit" }, 
                priority: 1, 
                canStart: (agentBrain) => true
            ),
            // new AgentGoal(
            //     name: "hang_around",
            //     actions: new List<string> { "move_randomly" }, 
            //     priority: 2,
            //     canStart: (agentBrain) => agentBrain.AgentStatus.Ore > 5
            // ),
        };
        actions = new List<AgentAction>() {
            new MoveRandomlyAction(),
            new GoToDepositAction(),
            new MineDepositAction(),
        };

        GoalChanged += OnGoalChanged;
    }

    void Update() {
        UpdateCurrentGoal();

        if (CurrentAction != null) {
            CurrentAction.Update(this);
            if (CurrentAction.IsFinished(this)) {
                CurrentAction.ExecuteConsequences(this);
                // UpdateCurrentGoal();
                UpdateCurrentAction(CurrentGoal);
            }
        }
    }

    void UpdateCurrentGoal() {
        goals.Sort((a, b) => b.Priority - a.Priority);
        foreach (var goal in goals) {
            if (goal.CanStart(this)) {
                if(goal != CurrentGoal) {
                    CurrentGoal = goal;
                    Debug.Log($"Agent: Changing the current goal to \"{goal.Name}\"");
                }  
                break;
            }
        }
    }

    void OnGoalChanged(AgentGoal newGoal) {
        UpdateCurrentAction(newGoal);
    }

    void UpdateCurrentAction(AgentGoal goal) {
        if (CurrentAction != null) {
            CurrentAction.ExecuteBreak(this);
        }
        // Search from last to first action
        for (int i = goal.Actions.Count - 1; i >= 0; i--) {
            var action = actions.Find(a => a.Name == goal.Actions[i]);
            if(action == null) {
                Debug.LogError($"Agent: Action \"{goal.Actions[i]}\" not found");
                continue;
            }
            if (action.CanStart(this)) {
                StartNewAction(action);
                break;
            }
        }
    }

    void StartNewAction(AgentAction action) {
        CurrentAction = action;
        CurrentAction.Execute(this);
        Debug.Log($"Agent: Changing the current action to \"{action.Name}\"");
    }

    void UpdateCurrentAction() {
        if (CurrentAction != null && !CurrentAction.IsFinished(this)) {
            CurrentAction.ExecuteBreak(this);
        }
        foreach (var actionName in CurrentGoal.Actions) {
            var action = actions.Find(a => a.Name == actionName);
            if (action.CanStart(this)) {
                CurrentAction = action;
                CurrentAction.Execute(this);
                break;
            }
        }
    }

    public AgentGoal CurrentGoal {
        get => _currentGoal;
        set {
            _currentGoal = value;
            GoalChanged?.Invoke(_currentGoal);
        }
    }
    public AgentAction CurrentAction {
        get => _currentAction;
        set {
            _currentAction = value;
            ActionChanged?.Invoke(_currentAction);
        }
    }
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public AgentSensor VisionSensor => visionSensor;
    public AgentSensor InteractionSensor => interactionSensor;
    public AgentStatus AgentStatus => agentStatus;
    public AnimationController AnimationController => animationController;
    public GoalName Goal => GoalName.FREEZE;

    public enum GoalName
    {
        FREEZE,
        RUN_FOR_YOUR_LIFE,
        LEAVE_THE_AREA,
        TAKE_DAMAGE,
        DIE,
        GO_TO_DESTINATION,

        SEARCH_FOR_AGENT,
        GO_TO_NEAREST_AGENT,
        ATTACK,
        
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
