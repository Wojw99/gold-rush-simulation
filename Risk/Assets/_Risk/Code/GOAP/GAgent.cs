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
    [SerializeField] Transform doorOnePosition;
    [SerializeField] Transform doorTwoPosition;

    NavMeshAgent navMeshAgent;
    AnimationController animationController;
    Rigidbody rb;

    // TODO: Move stats to a proper class
    [Header("Sensors")]
    public float health = 100;
    public float stamina = 100;

    CountdownTimer statsTimer;

    GameObject target;
    Vector3 destination;

    public GAgentGoal lastGoal; 
    public GAgentGoal currentGoal; // TODO: Make private
    public ActionPlan actionPlan;
    public GAgentAction currentAction;

    public Dictionary<string, AgentBelief> beliefs;
    public HashSet<GAgentGoal> goals;
    public HashSet<GAgentAction> actions;
    IGoapPlanner goapPlanner;

    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<AnimationController>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        goapPlanner = new GoapPlanner();
    }

    void Start() {
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    void SetupBeliefs() {
        beliefs = new Dictionary<string, AgentBelief>();
        var factory = new BeliefFactory(this, beliefs);

        factory.AddBelief("Nothing", () => false);
        factory.AddBelief("Idle", () => !navMeshAgent.hasPath);
        factory.AddBelief("Moving", () => navMeshAgent.hasPath);
        factory.AddBelief("IsDying", () => health < 30);
        factory.AddBelief("IsHealthy", () => health >= 50);
        factory.AddBelief("IsExhausted", () => stamina < 20);
        factory.AddBelief("IsRested", () => stamina >= 50);

        factory.AddLocationBelief("AtRestingPosition", 3f, restingPosition);
        factory.AddLocationBelief("AtFoodShack", 3f, foodShack);

        factory.AddSensorBelief("PlayerInFollowRange", followSensor);
        factory.AddSensorBelief("PlayerInInteracionRange", interactionSensor);
    }

    void SetupActions() {
        actions = new HashSet<GAgentAction>();

        actions.Add(new GAgentAction.Builder("Relax")
            .WithStrategy(new IdleStrategy(2))
            .AddEffect(beliefs["Nothing"])
            .Build());

        actions.Add(new GAgentAction.Builder("WanderAround")
            .WithStrategy(new WanderStrategy(navMeshAgent, 10))
            .AddEffect(beliefs["Moving"])
            .Build());

        actions.Add(new GAgentAction.Builder("MoveToEatingPosition")
            .WithStrategy(new FollowStrategy(navMeshAgent, () => foodShack.position))
            .AddEffect(beliefs["AtFoodShack"])
            .Build());

        actions.Add(new GAgentAction.Builder("Eat")
            .WithStrategy(new IdleStrategy(3))
            .AddPrecondition(beliefs["AtFoodShack"])
            .AddEffect(beliefs["IsHealthy"])
            .Build());

        actions.Add(new GAgentAction.Builder("MoveToRestingPosition")
            .WithStrategy(new FollowStrategy(navMeshAgent, () => restingPosition.position))
            .AddEffect(beliefs["AtRestingPosition"])
            .Build());

        actions.Add(new GAgentAction.Builder("Rest")
            .WithStrategy(new IdleStrategy(3))
            .AddPrecondition(beliefs["AtRestingPosition"])
            .AddEffect(beliefs["IsRested"])
            .Build());
    }

    void SetupGoals() {
        goals = new HashSet<GAgentGoal>();

        goals.Add(new GAgentGoal.Builder("ChillOut")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["Nothing"])
            .Build());

        goals.Add(new GAgentGoal.Builder("Wander")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["Moving"])
            .Build());

        goals.Add(new GAgentGoal.Builder("KeepHealthUp")
            .WithPriority(3)
            .WithDesiredEffect(beliefs["IsHealthy"])
            .Build());

        goals.Add(new GAgentGoal.Builder("KeepStaminaUp")
            .WithPriority(2)
            .WithDesiredEffect(beliefs["IsRested"])
            .Build());
    }

    void SetupTimers() {
        statsTimer = new CountdownTimer(2f);
        statsTimer.OnTimerStop += () => {
            UpdateStats();
            statsTimer.Start();
        };
        statsTimer.Start();
    }

    // TODO: Move to a proper class
    void UpdateStats() {
        health += InRangeOf(foodShack.position, 3f) ? 20 : -10;
        stamina += InRangeOf(restingPosition.position, 3f) ? 20 : -5;
        stamina = Mathf.Clamp(stamina, 0, 100);
        health = Mathf.Clamp(health, 0, 100);
    }

    bool InRangeOf(Vector3 target, float range) {
        return Vector3.Distance(transform.position, target) < range;
    }

    void OnEnable() => followSensor.TargetChanged += OnTargetChanged;
    void OnDisable() => followSensor.TargetChanged -= OnTargetChanged;

    void OnTargetChanged() {
        Debug.Log("Target changed, clearing current goal and action");
        // Force the planner to re-evaluate the plan
        currentGoal = null;
        currentAction = null;
    }

    void Update() {
        statsTimer.Tick(Time.deltaTime);

        // Update the plan and current action if there is one
        if(currentAction == null) {
            Debug.Log("Calculating any potential new plan");
            CalculatePlan();

            if(actionPlan != null && actionPlan.Actions.Count > 0) {
                navMeshAgent.ResetPath();

                currentGoal = actionPlan.AgentGoal;
                Debug.Log($"Goal: {currentGoal.Name} with {actionPlan.Actions.Count} actions in plan");
                currentAction = actionPlan.Actions.Pop();
                Debug.Log($"Popped action: {currentAction.Name}");

                if (currentAction.Preconditions.All(b => b.Evaluate())) {
                    currentAction.Start();
                } else {
                    Debug.Log("Preconditions not met, clearing current action and goal");
                    currentAction = null;
                    currentGoal = null;
                }
            }
        }

        // If we have a current action, execute it
        if (actionPlan != null && currentAction != null) {
            currentAction.Update(Time.deltaTime);

            if (currentAction.Completed) {
                Debug.Log($"{currentAction.Name} complete");
                currentAction.Stop();
                currentAction = null;

                if (actionPlan.Actions.Count == 0) {
                    Debug.Log("Plan complete");
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }
    }

    void CalculatePlan() {
        var priorityLevel = currentGoal?.Priority ?? 0;
        var goalsToConsider = goals;

        // If we have a current goal, we only want to consider goals with a higher priority
        if(currentGoal != null) {
            Debug.Log("Current goal exists, checking goals with higher priority");
            goalsToConsider = new HashSet<GAgentGoal>(goals.Where(g => g.Priority > priorityLevel));
        }

        var potentialPlan = goapPlanner.Plan(this, goalsToConsider, lastGoal);
        if(potentialPlan != null) {
            actionPlan = potentialPlan;
        }
    }
}
