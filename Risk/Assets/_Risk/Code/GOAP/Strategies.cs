using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

public interface IActionStrategy {
    bool CanPerform { get; }
    bool Complete { get; }

    void Start() {
        // ...
    }

    void Update(float deltaTime) {
        // ...
    }

    void Stop() {
        // ...
    }
}

public class AttackStrategy : IActionStrategy {
    public bool CanPerform => true; // Agent can always attack
    public bool Complete { get; private set; }
    
    readonly CountdownTimer timer;
    readonly AnimationController animations;

    public AttackStrategy(AnimationController animations) {
        this.animations = animations;
        // timer = new CountdownTimer(animations.GetAnimationLength(animations.attackClip));
        timer = new CountdownTimer(2f);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }
    
    public void Start() {
        timer.Start();
        // animations.Attack();
    }
    
    public void Update(float deltaTime) => timer.Tick(deltaTime);
}

public class MoveStrategy : IActionStrategy {
    readonly NavMeshAgent agent;
    readonly Func<Vector3> destination;
    
    public bool CanPerform => !Complete;
    public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;
    
    public MoveStrategy(NavMeshAgent agent, Func<Vector3> destination) {
        this.agent = agent;
        this.destination = destination;
    }
    
    public void Start() => agent.SetDestination(destination());
    public void Stop() => agent.ResetPath();
}

public class WanderStrategy : IActionStrategy {
    private readonly NavMeshAgent navMeshAgent;
    private readonly float wanderRadius;

    public bool CanPerform => !Complete;
    public bool Complete => navMeshAgent.remainingDistance < 2f && !navMeshAgent.pathPending;

    public WanderStrategy(NavMeshAgent navMeshAgent, float wanderRadius) {
        this.navMeshAgent = navMeshAgent;
        this.wanderRadius = wanderRadius;
    }

    public void Start() {
        // Check 5 times if there is a hit in the radius and if so set the destination to the hit
        for(int i = 0; i < 5; i++) {
            Vector3 randomDirection = (UnityEngine.Random.insideUnitSphere * wanderRadius).With(y: 0);
            NavMeshHit hit;

            if(NavMesh.SamplePosition(navMeshAgent.transform.position + randomDirection, out hit, wanderRadius, 1)) {
                navMeshAgent.SetDestination(hit.position);
                return;
            }
        }
    }
}

public class IdleStrategy : IActionStrategy {
    public bool CanPerform => true; // Agent can always Idle
    public bool Complete { get; private set; }

    private readonly CountdownTimer timer;

    public IdleStrategy(float duration) {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start() => timer.Start();
    public void Update(float deltaTime) => timer.Tick(deltaTime);
} 