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
    List<AgentGoal> goals;
    List<AgentAction> actions;

    public event Action<AgentGoal> GoalChanged;
    public event Action<AgentAction> ActionChanged;
    private event Action ActionFinished;

    /*
    Search for a new goal if: 
        - sensors detect a new beacon
        - status reports a significant change
        - action is finished
    
    Current action can be break or finish:

    Break action if (if current action can end, otherwise wait for the action to finish): 
        - there is a new goal
        - found a more suitable action based on sensors
        - found a more suitable action based on status

    Finish action if:
        - action is finished
    */

    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<AnimationController>();
        agentStatus = GetComponent<AgentStatus>();

        goals = new List<AgentGoal>() {
            new AgentGoal(
                name: "hang_around",
                actions: new List<GoalAction> { 
                    new("move_randomly", 1), 
                    },
                priority: 2,
                canStart: (agentBrain) => agentBrain.AgentStatus.Ore == agentBrain.AgentStatus.MaxOre
            ),
            new AgentGoal(
                name: "search",
                actions: new List<GoalAction> { 
                    new("move_randomly", 1), 
                    new("go_to_deposit", 2), 
                    new("mine_deposit", 3) 
                    }, 
                priority: 1, 
                canStart: (agentBrain) => true
            ),
            new AgentGoal(
                name: "keep_stamina",
                actions: new List<GoalAction> { 
                    new("move_randomly", 1), 
                    new("go_to_rest", 2), 
                    new("take_rest", 3) 
                    },
                priority: 5,
                canStart: (agentBrain) => agentBrain.AgentStatus.Stamina < agentBrain.AgentStatus.MaxStamina / 3
            ),
            new AgentGoal(
                name: "die",
                actions: new List<GoalAction> { 
                    new("take_damage", 1), 
                    },
                priority: 10,
                canStart: (agentBrain) => agentBrain.interactionSensor.IsBeaconSensible(BeaconType.DAMAGE)
            ),
        };
        actions = new List<AgentAction>() {
            new MoveRandomlyAction(
                name: "move_randomly"
            ),
            new GoToBeaconAction(
                name: "go_to_deposit", 
                beaconType: BeaconType.DEPOSIT
            ),
            new InteractAction(
                name: "mine_deposit", 
                duration: 3,
                animationName: AnimationController.IS_DIGGING,
                beaconType: BeaconType.DEPOSIT,
                removeInteractedBeacon: true,
                statusConsequences: (agentStatus) => agentStatus.Ore += 1,
                requiredAttribute: AttributeName.Stamina,
                attributeCostPerSecond: 2
            ),
            new GoToBeaconAction(
                name: "go_to_rest", 
                beaconType: BeaconType.REST
            ),
            new InteractAction(
                name: "take_rest", 
                duration: 3,
                animationName: AnimationController.IS_SITTING,
                beaconType: BeaconType.REST,
                removeInteractedBeacon: false,
                statusConsequences: (agentStatus) => agentStatus.Stamina = agentStatus.MaxStamina,
                requiredAttribute: AttributeName.Stamina,
                attributeCostPerSecond: 0f
            ),
            new InteractAction(
                name: "take_damage", 
                duration: 0.5f,
                animationName: AnimationController.IS_DAMAGED,
                beaconType: BeaconType.DAMAGE,
                removeInteractedBeacon: false,
                requiredAttribute: AttributeName.Health,
                attributeCostPerSecond: 3f
            ),
        };

        goals.Sort((goal1, goal2) => goal1.Priority.CompareTo(goal2.Priority));
    }
    
    void Update() {
        var newGoal = FindNewGoal();
        if(newGoal != null && newGoal != CurrentGoal) {
            CurrentGoal = newGoal;
        }

        var newAction = FindActionForCurrentGoal();
        if(newAction != null && newAction != CurrentAction) {
            CurrentAction = newAction;
        }
    }

    AgentGoal FindNewGoal() {
        for(int i = 0; i < goals.Count; i++) {
            if(goals[i].CanStart(this)) {
                return goals[i];
            }
        }
        return null;
    }

    AgentAction FindActionForCurrentGoal() {
        for(int i = 0; i < CurrentGoal.Actions.Count; i++) {
            var goalAction = CurrentGoal.Actions[i];
            var action = actions.Find(a => a.Name == goalAction.Name);
            if(action.CanStart(this)) {
                return action;
            }
        }
        return null;
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
