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
}

public class MineStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Completed { get; private set; }

    readonly CountdownTimer timer;

    public MineStrategy(float duration, AgentStats agentStats, AnimationController animationController) {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => {
            agentStats.StartDrawingStamina();
            Completed = false;
            animationController.StartAnimating(AnimType.IsDigging.ToString());
        };
        timer.OnTimerStop += () => {
            agentStats.StopDrawingStamina();
            agentStats.Ore += 10;
            Completed = true;
            animationController.StopAnimating();
        };
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
            agentStats.StartFillingStamina();
            Completed = false;
            animationController.StartAnimating(AnimType.IsSitting.ToString());
        };
        timer.OnTimerStop += () => {
            agentStats.StopFillingStamina();
            Completed = true;
            animationController.StopAnimating();
        };
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
            agentStats.StartFillingHealth();
            Completed = false;
            animationController.StartAnimating(AnimType.IsSitting.ToString());
        };
        timer.OnTimerStop += () => {
            agentStats.StopFillingHealth();
            Completed = true;
            animationController.StopAnimating();
        };
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

    public bool Completed => navMeshAgent.remainingDistance <= 2f && !navMeshAgent.pathPending;

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