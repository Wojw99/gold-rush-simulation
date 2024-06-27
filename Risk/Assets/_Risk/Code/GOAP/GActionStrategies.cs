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

public class FollowStrategy : IActionStrategy
{
    readonly NavMeshAgent navMeshAgent;
    readonly Func<Vector3> destination;

    public bool CanPerform => !Completed;

    public bool Completed => navMeshAgent.remainingDistance <= 2f && !navMeshAgent.pathPending;

    public FollowStrategy(NavMeshAgent navMeshAgent, Func<Vector3> destination) {
        this.navMeshAgent = navMeshAgent;
        this.destination = destination;
    }

    public void Start() => navMeshAgent.SetDestination(destination());
    public void Stop() => navMeshAgent.ResetPath();
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

    public WanderStrategy(NavMeshAgent navMeshAgent, float wanderRange) {
        this.navMeshAgent = navMeshAgent;
        this.wanderRange = wanderRange;
    }

    public void Start() {
        for (int i = 0; i < 5; i++) {
            Vector3 randomDirection = (UnityEngine.Random.insideUnitSphere * wanderRange).With(y: 0);
            NavMeshHit hit;

            if (NavMesh.SamplePosition(navMeshAgent.transform.position + randomDirection, out hit, wanderRange, 1)) {
                navMeshAgent.SetDestination(hit.position);
                return;
            }
        }
    }
}