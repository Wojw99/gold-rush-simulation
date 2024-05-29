using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBrain2 : MonoBehaviour
{
    private GoalName _goal;
    private GameObject _destination;

    public event Action<GoalName> GoalChanged;
    public event Action<GameObject> DestinationChanged;
    public event Action DepositExtracted;
    public event Action<float> DamageTaken;

    private AgentStatus agentStatus;
    private AgentVisionSensor agentVisionSensor;
    private AgentInteractionSensor agentInteractionSensor;
    
    private void Start()
    {
        agentStatus = GetComponent<AgentStatus>();
        agentVisionSensor = GetComponent<AgentVisionSensor>();
        agentInteractionSensor = GetComponent<AgentInteractionSensor>();

        agentStatus.StaminaChanged += OnStaminaChanged;
        agentStatus.HealthChanged += OnHealChanged;

        agentVisionSensor.AgentSpotted += OnAgentSpotted;
        agentVisionSensor.DepositSpotted += OnDepositSpotted;
        agentVisionSensor.HealSpotted += OnHealSpotted;
        agentVisionSensor.RestSpotted += OnRestSpotted;
        agentVisionSensor.BeaconLost += OnBeaconLost;

        agentInteractionSensor.InteractionStarted += OnInteractionStarted;
        agentInteractionSensor.InteractionEnded += OnInteractionEnded; 
        agentInteractionSensor.InteractionExited += OnInteractionExited;
        agentInteractionSensor.ModifierStarted += OnModifierStarted;
        agentInteractionSensor.PlayerSelect += OnPlayerSelect;
        agentInteractionSensor.PlayerDeselect += OnPlayerDeselect;
        agentInteractionSensor.PlayerOrder += OnPlayerOrder;
        agentInteractionSensor.AgentApproached += OnAgentApproached;
        agentInteractionSensor.AgentLeft += OnAgentLeft;

        // Give other components time to subscribe to the GoalChanged event
        StartCoroutine(ConsiderGoalChanging(1f));

        Destination = transform.gameObject;
    }

    private void OnPlayerSelect() {
        Goal = GoalName.FREEZE;
        Destination = transform.gameObject;
    }

    private void OnPlayerDeselect() {
        ConsiderGoalChanging();
    }

    private void OnPlayerOrder(GameObject destination) {
        Goal = GoalName.GO_TO_DESTINATION;
        Destination = destination;
    }

    private IEnumerator ConsiderGoalChanging(float delay) {
        yield return new WaitForSeconds(delay);
        ConsiderGoalChanging();
    }

    private void OnInteractionStarted(InteractionType interactionType) {
        if(interactionType == InteractionType.DEPOSIT 
            && (Goal == GoalName.SEARCH_FOR_DEPOSIT || Goal == GoalName.GO_TO_NEAREST_DEPOSIT)) 
        {
            Goal = GoalName.MINE_DEPOSIT;
        }
        if(interactionType == InteractionType.REST 
            && (Goal == GoalName.SEARCH_FOR_REST || Goal == GoalName.GO_TO_NEAREST_REST)) 
        {
            Goal = GoalName.TAKE_REST;
        }
        if(interactionType == InteractionType.DAMAGE) 
        {
            Goal = GoalName.TAKE_DAMAGE;
        }
        if(interactionType == InteractionType.HEAL) 
        {
            Goal = GoalName.TAKE_HEALING;
        }
    }

    private void OnHealChanged(float heal) {
        if(heal >= agentStatus.MaxHealth) {
            ConsiderGoalChanging();
        } else if(heal <= 0) {
            Goal = GoalName.DIE;
        }
    }

    private void OnStaminaChanged(float stamina) {
        if(stamina >= agentStatus.MaxStamina) {
            ConsiderGoalChanging();
        }
    }

    private void OnModifierStarted(ModifierInfo modifierInfo) {
        if(modifierInfo.modifierType == ModifierType.DAMAGE) 
        {
            // Goal = GoalName.TAKE_DAMAGE;
            DamageTaken?.Invoke(modifierInfo.modifierValue);
            // StartCoroutine(ConsiderGoalChanging(0.5f));
        } 
        else if (modifierInfo.modifierType == ModifierType.HEAL) 
        {
            DamageTaken?.Invoke(modifierInfo.modifierValue * -1f);
        }
    }

    private void OnInteractionEnded(InteractionType interactionType) {
        if(interactionType == InteractionType.DEPOSIT) {
            DepositExtracted?.Invoke();
        }
        ConsiderGoalChanging();
    }

    private void OnInteractionExited(InteractionType interactionType) {
        ConsiderGoalChanging();
    }

    private void OnHealSpotted(BeaconInfo beaconInfo) {
        if(Goal == GoalName.SEARCH_FOR_HEALING)
        {
            Destination = beaconInfo.gameObject;
            Goal = GoalName.GO_TO_NEAREST_HEALING;
        }
    }

    private void OnDepositSpotted(BeaconInfo beaconInfo) {
        if(Goal == GoalName.SEARCH_FOR_DEPOSIT)
        {
            Destination = beaconInfo.gameObject;
            Goal = GoalName.GO_TO_NEAREST_DEPOSIT;
        }
    }

    private void OnRestSpotted(BeaconInfo beaconInfo) {
        if(Goal == GoalName.SEARCH_FOR_REST)
        {
            Destination = beaconInfo.gameObject;
            Goal = GoalName.GO_TO_NEAREST_REST;
        }
    }

    private void OnAgentSpotted(BeaconInfo beaconInfo) {
        Debug.Log("$Agent spotted");
    }

    private void OnAgentApproached(GameObject otherAgent) {
        Debug.Log("$Agent approached");
    }
 
    private void OnAgentLeft(GameObject otherAgent) {
        Debug.Log("$Agent left");
    }


    private void OnBeaconLost(BeaconInfo beaconInfo) {
        if(beaconInfo.gameObject == Destination) {
            Destination = transform.gameObject;
            ConsiderGoalChanging();
        }
        Debug.Log("$Agent vision lost");
    }

    private void Update() {
        if(Goal == GoalName.GO_TO_NEAREST_DEPOSIT) {
            if(Destination == null) {
                Destination = transform.gameObject;
                ConsiderGoalChanging();
            }
        }
    }

    private void ConsiderGoalChanging() {
        if(Goal == GoalName.DIE) {
            return;
        }

        var calculatedGoal = CalculateGoal();
        
        if (Goal != calculatedGoal) {
            Goal = calculatedGoal;
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

    public GoalName Goal {
        get => _goal;
        set {
            _goal = value;
            GoalChanged?.Invoke(_goal);
        }
    }

    public GameObject Destination {
        get => _destination;
        set {
            _destination = value;
            DestinationChanged?.Invoke(_destination);
        }
    }

    private void OnDestroy() {
        GoalChanged = null;
        DestinationChanged = null;
        DepositExtracted = null;
        DamageTaken = null;
    }

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
