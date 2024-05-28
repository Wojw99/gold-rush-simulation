using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AnimationController))]
public class GoapAgent : MonoBehaviour
{
    [Header("Sensors")]
    [SerializeField] private Sensor visionSensor;
    [SerializeField] private Sensor interactionSensor;

    [Header("Known Locations")]
    [SerializeField] private Transform restingPosition;
    [SerializeField] private Transform healingPosition;
    [SerializeField] private Transform doorOnePosition;
    [SerializeField] private Transform doorTwoPosition;

    NavMeshAgent navMeshAgent;
    AnimationController animationController;
    Rigidbody rb;

    [Header("Stats")]
    public float health = 100;
    public float stamina = 100;

    CountdownTimer statsTimer;
    GameObject target;
    Vector3 destination;

    AgentGoal lastGoal;
    public AgentGoal currentGoal; 
    public ActionPlan actionPlan; 
    public AgentAction currentAction;

    public Dictionary<string, AgentBelief> beliefs;
    public HashSet<AgentAction> actions;
    public HashSet<AgentGoal> goals;

    IGoapPlanner goapPlanner;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animationController = GetComponent<AnimationController>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        goapPlanner = new GoapPlanner(); 
    }

    private void Start() {
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    private void SetupBeliefs() {
        beliefs = new Dictionary<string, AgentBelief>();
        var factory = new BeliefFactory(this, beliefs);

        factory.AddBelief("Nothing", () => false);

        factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
        factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);

        factory.AddBelief("AgentHealthLow", () => health < 30);
        factory.AddBelief("AgentIsHealthy", () => health >= 50);
        factory.AddBelief("AgentStaminaLow", () => stamina < 10);
        factory.AddBelief("AgentIsRested", () => stamina >= 50);

        factory.AddSensorBelief("DepositInVisionRange", visionSensor);
        factory.AddSensorBelief("DepositInInteractionRange", interactionSensor);
        factory.AddBelief("MiningDeposit", () => stamina > 0);
    }

    private void SetupActions() {
        actions = new HashSet<AgentAction>();
    
        actions.Add(new AgentAction.Builder("DoNothing")
            .WithStrategy(new IdleStrategy(3))
            .WithEffect(beliefs["Nothing"])
            .Build());
        
        actions.Add(new AgentAction.Builder("WanderAround")
            .WithStrategy(new WanderStrategy(navMeshAgent, 10))
            .WithEffect(beliefs["AgentMoving"])
            .Build());

        actions.Add(new AgentAction.Builder("GoToDeposit")
            .WithStrategy(new MoveStrategy(navMeshAgent, () => visionSensor.TargetPosition))
            .WithPrecondition(beliefs["DepositInVisionRange"])
            .WithEffect(beliefs["DepositInInteractionRange"])
            .Build());

        actions.Add(new AgentAction.Builder("MineDeposit")
            .WithStrategy(new IdleStrategy(3))
            .WithPrecondition(beliefs["DepositInInteractionRange"])
            .WithEffect(beliefs["MiningDeposit"])
            .Build());
    }

    private void SetupGoals() {
        goals = new HashSet<AgentGoal>();

        goals.Add(new AgentGoal.Builder("Chill Out")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["Nothing"])
            .Build());
        
        goals.Add(new AgentGoal.Builder("Wander")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["AgentMoving"])
            .Build());

        goals.Add(new AgentGoal.Builder("MineDeposit")
            .WithPriority(3)
            .WithDesiredEffect(beliefs["MiningDeposit"])
            .Build());
        
        goals.Add(new AgentGoal.Builder("KeepHealthUp")
            .WithPriority(5)
            .WithDesiredEffect(beliefs["AgentIsHealthy"])
            .Build());

        goals.Add(new AgentGoal.Builder("KeepStaminaUp")
            .WithPriority(5)
            .WithDesiredEffect(beliefs["AgentIsRested"])
            .Build());
        
        goals.Add(new AgentGoal.Builder("SeekAndDestroy")
            .WithPriority(10)
            .WithDesiredEffect(beliefs["AttackingPlayer"])
            .Build());
    }

    private void SetupTimers() {
        statsTimer = new CountdownTimer(2f);
        statsTimer.OnTimerStop += () => {
            UpdateStats();
            statsTimer.Start();
        };  
        statsTimer.Start();
    }

    // TODO: move to stats system
    void UpdateStats() {
        // stamina += InRangeOf(restingPosition.position, 3f) ? 20 : -10;
        // health += InRangeOf(healingPosition.position, 3f) ? 20 : -5;
        stamina = Mathf.Clamp(stamina, 0, 100);
        health = Mathf.Clamp(health, 0, 100);
    }
    
    bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(transform.position, pos) < range;
    
    void OnEnable() => visionSensor.OnTargetChanged += HandleTargetChanged;
    void OnDisable() => visionSensor.OnTargetChanged -= HandleTargetChanged;
    
    void HandleTargetChanged() {
        Debug.Log("Target changed, clearing current action and goal");
        // Force the planner to re-evaluate the plan
        currentAction = null;
        currentGoal = null;
    }

    void Update() {
        statsTimer.Tick(Time.deltaTime);
        // animations.SetSpeed(navMeshAgent.velocity.magnitude);
        
        // Update the plan and current action if there is one
        if (currentAction == null) {
            Debug.Log("Calculating any potential new plan");
            CalculatePlan();

            if (actionPlan != null && actionPlan.Actions.Count > 0) {
                navMeshAgent.ResetPath();

                currentGoal = actionPlan.AgentGoal;
                Debug.Log($"Goal: {currentGoal.Name} with {actionPlan.Actions.Count} actions in plan");
                currentAction = actionPlan.Actions.Pop();
                Debug.Log($"Popped action: {currentAction.Name}");
                // Verify all precondition effects are true
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

            if (currentAction.Complete) {
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
        
        HashSet<AgentGoal> goalsToCheck = goals;
        
        // If we have a current goal, we only want to check goals with higher priority
        if (currentGoal != null) {
            Debug.Log("Current goal exists, checking goals with higher priority");
            goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.Priority > priorityLevel));
        }
        
        var potentialPlan = goapPlanner.Plan(this, goalsToCheck, lastGoal);
        if (potentialPlan != null) {
            actionPlan = potentialPlan;
        }
    }
}