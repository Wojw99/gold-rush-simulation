using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

public interface IActionStrategy
{
    bool CanPerform { get; }
    bool Completed { get; }

    void Start() {

    }

    void Update(float deltaTime) {

    }

    void Stop() {
        
    }

    void Interrupt() {
        
    }
}

public class AttackStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;

    public AttackStrategy(AgentStats agentStats, AnimationController animationController, Sensor sensor, Transform transform) {
        var duration = animationController.GetAnimationDuration(AnimType.IsAttacking.ToString());
        var attackSpeed = agentStats.CalculateFinalAttackSpeed();
        var modifiedDuration = duration / attackSpeed;
        timer = new CountdownTimer(modifiedDuration);
        
        timer.OnTimerStart += () => {
            agentStats.Stamina -= 10;
            Completed = false;
            
            var attackSpeed = agentStats.CalculateFinalAttackSpeed();
            animationController.SetSpeed(attackSpeed);
            animationController.StartAnimating(AnimType.IsAttacking.ToString());
            transform.LookAt(sensor.TryGetEnemyPosistion(agentStats.TeamId));
        };

        timer.OnTimerStop += () => {
            Completed = true;
            animationController.ResetSpeed();
            animationController.StopAnimating();
            
            if(sensor.TryGetEnemyStats(agentStats.TeamId, out AgentStats enemyStats)){
                enemyStats.Health -= agentStats.CalculateFinalAttack();
            }

            if(agentStats.Stamina <= 0){
                // slow down attack speed
                var attackSpeed = agentStats.CalculateFinalAttackSpeed();
                timer.Reset(duration / attackSpeed);
            }
        };
    }

    public void Interrupt() {
        timer.Interrupt();
    }

    public void Start() {
        timer.Start();
    }

    public void Update(float deltaTime) {
        timer.Tick(deltaTime);
    }
}

public class StorageStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;
    readonly AgentStats agentStats;

    public StorageStrategy(float duration, AgentStats agentStats, AnimationController animationController, Sensor sensor) {
        timer = new CountdownTimer(duration);
        this.agentStats = agentStats;
        timer.OnTimerStart += () => {
            agentStats.isDrawingStamina = true;
            Completed = false;
            animationController.StartAnimating(AnimType.IsDigging.ToString());
        };
        timer.OnTimerStop += () => {
            agentStats.isDrawingStamina = false;
            Completed = true;
            animationController.StopAnimating();

            if(sensor.TryGetStorage(out Building building)){
                building.AddGold(agentStats.Ore, agentStats.PyriteModifier);
                agentStats.Ore = 0;
                agentStats.PyriteModifier = 0;
            }
        };
    }

    public void Interrupt() {
        agentStats.isDrawingStamina = false;
        timer.Interrupt();
    }

    public void Start() {
        timer.Start();
    }

    public void Update(float deltaTime) {
        timer.Tick(deltaTime);
    }
}


public class MineStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;
    readonly AgentStats agentStats;
    readonly Sensor sensor;

    public MineStrategy(float duration, AgentStats agentStats, AnimationController animationController, Sensor sensor) {
        this.agentStats = agentStats;
        this.sensor = sensor;

        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => {
            agentStats.isDrawingStamina = true;
            Completed = false;
            animationController.StartAnimating(AnimType.IsDigging.ToString());
            
            if(sensor.TryGetFreeDeposit(agentStats.ID, out Deposit deposit)){
                deposit.Occupy(agentStats.ID);
            }
        };
        timer.OnTimerStop += () => {
            agentStats.isDrawingStamina = false;
            Completed = true;
            animationController.StopAnimating();

            if(sensor.TryGetBeaconOfType(BeaconType.DEPOSIT, out Beacon beacon)){
                var deposit = beacon.GetComponent<Deposit>();

                agentStats.Ore += deposit.GoldAmount;

                if(deposit.IsPyrite) {
                    var diceRoll = UnityEngine.Random.Range(0, 100);
                    if(agentStats.MiningExpertise < diceRoll) {
                        agentStats.PyriteModifier += deposit.GoldAmount;
                    }
                    agentStats.MiningExpertise += 1;
                } 
                if(deposit.IsPoisonIvy) {
                    var diceRoll = UnityEngine.Random.Range(0, 100);
                    if(agentStats.PlantExpertise < diceRoll) {
                        agentStats.Health -= deposit.PoisonDamage;
                    }
                    agentStats.PlantExpertise += 1;
                }
                
                beacon.Destroy();
            }
        };
    }

    public void Interrupt() {
        agentStats.isDrawingStamina = false;
        if(sensor.TryGetFreeDeposit(agentStats.ID, out Deposit deposit)){
            deposit.Release(agentStats.ID);
        }
        timer.Interrupt();
    }

    public void Start() {
        timer.Start();
    }

    public void Update(float deltaTime) {
        timer.Tick(deltaTime);
    }
}

public class RestStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;

    public RestStrategy(float duration, AgentStats agentStats, AnimationController animationController) {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => {
            agentStats.isFillingStamina = true;
            Completed = false;
            animationController.StartAnimating(AnimType.IsSitting.ToString());
        };
        timer.OnTimerStop += () => {
            agentStats.isFillingStamina = false;
            Completed = true;
            animationController.StopAnimating();
        };
    }

    public void Interrupt() {
        timer.Stop();
    }

    public void Start() {
        timer.Start();
    }

    public void Update(float deltaTime) {
        timer.Tick(deltaTime);
    }
}

public class HealStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;

    public HealStrategy(float duration, AgentStats agentStats, AnimationController animationController) {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => {
            agentStats.isFillingHealth = true;
            Completed = false;
            animationController.StartAnimating(AnimType.IsSitting.ToString());
        };
        timer.OnTimerStop += () => {
            agentStats.isFillingHealth = false;
            Completed = true;
            animationController.StopAnimating();
        };
    }

    public void Interrupt() {
        timer.Stop();
    }

    public void Start() {
        timer.Start();
    }

    public void Update(float deltaTime) {
        timer.Tick(deltaTime);
    }
}

public class FollowStrategy : IActionStrategy
{
    readonly NavMeshAgent navMeshAgent;
    readonly Func<Vector3> destination;
    AnimationController animationController;

    public bool CanPerform => !Completed;

    public bool Completed => navMeshAgent.remainingDistance <= 1.5f && !navMeshAgent.pathPending;

    public FollowStrategy(NavMeshAgent navMeshAgent, Func<Vector3> destination, AnimationController animationController) {
        this.navMeshAgent = navMeshAgent;
        this.destination = destination;
        this.animationController = animationController;
    }

    public void Start() {
        navMeshAgent.SetDestination(destination());
        animationController.StartAnimating(AnimType.IsWalking.ToString());
    }

    public void Stop() {
        navMeshAgent.ResetPath();
        animationController.StopAnimating();
    }
}

public class IdleStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;

    public IdleStrategy(float duration) {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Completed = false;
        timer.OnTimerStop += () => Completed = true;
    }

    public void Interrupt() {
        timer.Interrupt();
    }

    public void Start() {
        timer.Start();
    }

    public void Update(float deltaTime) {
        timer.Tick(deltaTime);
    }
}

public class RelaxStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;

    public RelaxStrategy(float duration, AnimationController animationController, AgentStats agentStats) {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => {
            Completed = false;
            agentStats.isFillingRelax = true;
            animationController.StartAnimating(AnimType.IsSitting.ToString());
        };
        timer.OnTimerStop += () => {
            Completed = true;
            agentStats.isFillingRelax = false;
            animationController.StopAnimating();
        };
    }

    public void Interrupt() {
        timer.Interrupt();
    }

    public void Start() {
        timer.Start();
    }

    public void Update(float deltaTime) {
        timer.Tick(deltaTime);
    }
}

public class WanderStrategy : IActionStrategy
{
    readonly NavMeshAgent navMeshAgent;
    readonly float wanderRange;

    public bool CanPerform => !Completed;
    public bool Completed => navMeshAgent.remainingDistance <= 2f && !navMeshAgent.pathPending;

    AnimationController animationController;

    public WanderStrategy(NavMeshAgent navMeshAgent, float wanderRange, AnimationController animationController) {
        this.navMeshAgent = navMeshAgent;
        this.wanderRange = wanderRange;
        this.animationController = animationController;
    }

    public void Start() {
        for (int i = 0; i < 5; i++) {
            Vector3 randomDirection = (UnityEngine.Random.insideUnitSphere * wanderRange).With(y: 0);
            NavMeshHit hit;

            if (NavMesh.SamplePosition(navMeshAgent.transform.position + randomDirection, out hit, wanderRange, 1)) {
                navMeshAgent.SetDestination(hit.position);
                animationController.StartAnimating(AnimType.IsWalking.ToString());
                return;
            }
        }
    }

    public void Stop() {
        animationController.StopAnimating();
    }
}