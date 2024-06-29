using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AnimationController))]
public class GAgent : MonoBehaviour
{
    [Header("Sensors")]
    [SerializeField] Sensor followSensor;
    [SerializeField] Sensor interactionSensor;

    [Header("Known Locations")]
    [SerializeField] Transform restingPosition;
    [SerializeField] Transform foodShack;
    [SerializeField] Transform depositOnePosition;
    [SerializeField] Transform depositTwoPosition;

    NavMeshAgent navMeshAgent;
    AnimationController animationController;
    Rigidbody rb;
    AgentStats agentStats;

    GameObject target;
    Vector3 destination;

    GAgentGoal lastGoal; 
    GAgentGoal _currentGoal;
    ActionPlan actionPlan;
    GAgentAction _currentAction;

    Dictionary<string, GAgentBelief> beliefs;
    HashSet<GAgentGoal> goals;
    HashSet<GAgentAction> actions;
    IGPlanner goapPlanner;

    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<AnimationController>();
        agentStats = GetComponent<AgentStats>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        goapPlanner = new GPlanner();
    }

    void Start() {
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    void SetupBeliefs() {
        beliefs = new Dictionary<string, GAgentBelief>();
        var factory = new BeliefFactory(this, beliefs);

        factory.AddBelief("Nothing", () => false);
        factory.AddBelief("Idle", () => !navMeshAgent.hasPath);
        factory.AddBelief("Moving", () => navMeshAgent.hasPath);
        factory.AddBelief("IsDying", () => agentStats.Health < 30);
        factory.AddBelief("IsHealthy", () => agentStats.Health >= 50);
        factory.AddBelief("IsExhausted", () => agentStats.Stamina < 20);
        factory.AddBelief("IsRested", () => agentStats.Stamina >= 50);
        factory.AddBelief("HasOre", () => agentStats.Ore > 0);
        factory.AddBelief("HasFullOre", () => agentStats.Ore >= agentStats.MaxOre);

        factory.AddLocationBelief("AtRestingPosition", 3f, restingPosition);
        factory.AddLocationBelief("AtFoodShack", 3f, foodShack);
        factory.AddLocationBelief("AtDepositOnePosition", 3f, depositOnePosition);

        // factory.AddLocationBelief("DepositInInteractionRange", 3f, depositTwoPosition);


        //factory.AddBelief("DepositInFollowRange", () => followSensor.IsTargetOfType(BeaconType.DEPOSIT));
        //factory.AddBelief("DepositInInteractionRange", () => interactionSensor.IsTargetOfType(BeaconType.DEPOSIT));

        // factory.AddSensorBelief("PlayerInFollowRange", followSensor);
        // factory.AddSensorBelief("PlayerInInteracionRange", interactionSensor);

        // factory.AddSensorBelief("DepositInFollowRange", BeaconType.DEPOSIT, followSensor);
        // factory.AddSensorBelief("DepositInInteracionRange", BeaconType.DEPOSIT, interactionSensor);
    }

    void SetupActions() {
        actions = new HashSet<GAgentAction>();

        actions.Add(new GAgentAction.Builder("Relax")
            .WithStrategy(new IdleStrategy(1))
            .AddEffect(beliefs["Nothing"])
            .Build());

        actions.Add(new GAgentAction.Builder("WanderAround")
            .WithStrategy(new WanderStrategy(navMeshAgent, 10, animationController))
            .AddEffect(beliefs["Moving"])
            .Build());

        // - - - - - HEALING - - - - -

        actions.Add(new GAgentAction.Builder("MoveToEatingPosition")
            .WithStrategy(new FollowStrategy(navMeshAgent, () => foodShack.position, animationController))
            .AddEffect(beliefs["AtFoodShack"])
            .Build());

        actions.Add(new GAgentAction.Builder("Eat")
            .WithStrategy(new HealStrategy(3, agentStats, animationController))
            .AddPrecondition(beliefs["AtFoodShack"])
            .AddEffect(beliefs["IsHealthy"])
            .Build());

        // - - - - - RESTING - - - - -

        actions.Add(new GAgentAction.Builder("MoveToRestingPosition")
            .WithStrategy(new FollowStrategy(navMeshAgent, () => restingPosition.position, animationController))
            .AddEffect(beliefs["AtRestingPosition"])
            .Build());

        actions.Add(new GAgentAction.Builder("Rest")
            .WithStrategy(new RestStrategy(3, agentStats, animationController))
            .AddPrecondition(beliefs["AtRestingPosition"])
            .AddEffect(beliefs["IsRested"])
            .Build());


        // - - - - - MINING - - - - -

        actions.Add(new GAgentAction.Builder("MoveToDeposit")
            .WithStrategy(new FollowStrategy(navMeshAgent, () => depositOnePosition.position, animationController))
            .AddEffect(beliefs["AtDepositOnePosition"])
            .Build());

        actions.Add(new GAgentAction.Builder("Mine")
            .WithStrategy(new MineStrategy(5, agentStats, animationController))
            .AddPrecondition(beliefs["AtDepositOnePosition"])
            .AddEffect(beliefs["HasOre"])
            .AddEffect(beliefs["HasFullOre"])
            .Build());

        // actions.Add(new GAgentAction.Builder("MoveToDeposit")
        //     .WithStrategy(new FollowStrategy(navMeshAgent, () => beliefs['DepositInFollowRange'].Location, animationController))
        //     .AddPrecondition(beliefs["DepositInFollowRange"])
        //     .AddEffect(beliefs["DepositInInteractionRange"])
        //     .Build());

        // actions.Add(new GAgentAction.Builder("Mine")
        //     .WithStrategy(new MineStrategy(3, agentStats, animationController))
        //     .AddPrecondition(beliefs["DepositInInteractionRange"])
        //     .AddEffect(beliefs["HasOre"])
        //     .Build());
    }

    void SetupGoals() {
        goals = new HashSet<GAgentGoal>();

        goals.Add(new GAgentGoal.Builder("ChillOut")
            .WithPriority(0)
            .WithDesiredEffect(beliefs["Nothing"])
            .Build());

        goals.Add(new GAgentGoal.Builder("Wander")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["Moving"])
            .Build());

        goals.Add(new GAgentGoal.Builder("KeepHealthUp")
            .WithPriority(10)
            .WithDesiredEffect(beliefs["IsHealthy"])
            .Build());

        goals.Add(new GAgentGoal.Builder("KeepStaminaUp")
            .WithPriority(5)
            .WithDesiredEffect(beliefs["IsRested"])
            .Build());

        goals.Add(new GAgentGoal.Builder("CollectOre")
            .WithPriority(3)
            .WithDesiredEffect(beliefs["HasFullOre"])
            .Build());
    }

    void OnEnable() => followSensor.TargetChanged += OnTargetChanged;
    void OnDisable() => followSensor.TargetChanged -= OnTargetChanged;

    void OnTargetChanged() {
        Debug.Log("Target changed");
        // Force the planner to re-evaluate the plan
        // CurrentGoal = null;
        // CurrentAction = null;
    }

    void Update() {
        // Update the plan and current action if there is one
        if(CurrentAction == null) {
            Debug.Log("Calculating any potential new plan");
            CalculatePlan();

            if(actionPlan != null && actionPlan.Actions.Count > 0) {
                navMeshAgent.ResetPath();

                CurrentGoal = actionPlan.AgentGoal;
                Debug.Log($"Goal: {CurrentGoal.Name} with {actionPlan.Actions.Count} actions in plan");
                CurrentAction = actionPlan.Actions.Pop();
                Debug.Log($"Popped action: {CurrentAction.Name}");

                if (CurrentAction.Preconditions.All(b => b.Evaluate())) {
                    CurrentAction.Start();
                } else {
                    Debug.Log("Preconditions not met, clearing current action and goal");
                    CurrentAction = null;
                    CurrentGoal = null;
                }
            }
        }

        // If we have a current action, execute it
        if (actionPlan != null && CurrentAction != null) {
            CurrentAction.Update(Time.deltaTime);

            if (CurrentAction.Completed) {
                Debug.Log($"{CurrentAction.Name} complete");
                CurrentAction.Stop();
                CurrentAction = null;

                if (actionPlan.Actions.Count == 0) {
                    Debug.Log("Plan complete");
                    lastGoal = CurrentGoal;
                    CurrentGoal = null;
                }
            }
        }
    }

    void CalculatePlan() {
        var priorityLevel = CurrentGoal?.Priority ?? 0;
        var goalsToConsider = goals;

        // If we have a current goal, we only want to consider goals with a higher priority
        if(CurrentGoal != null) {
            Debug.Log("Current goal exists, checking goals with higher priority");
            goalsToConsider = new HashSet<GAgentGoal>(goals.Where(g => g.Priority > priorityLevel));
        }

        var potentialPlan = goapPlanner.Plan(this, goalsToConsider, lastGoal);
        if(potentialPlan != null) {
            actionPlan = potentialPlan;
        }
    }

    public event Action<GAgentGoal> GoalChanged;
    public event Action<GAgentAction> ActionChanged;

    public HashSet<GAgentAction> Actions => actions;

    public GAgentGoal CurrentGoal {
        get => _currentGoal;
        set {
            _currentGoal = value;
            GoalChanged?.Invoke(_currentGoal);
        }
    }
    public GAgentAction CurrentAction {
        get => _currentAction;
        set {
            _currentAction = value;
            ActionChanged?.Invoke(_currentAction);
        }
    }
}
