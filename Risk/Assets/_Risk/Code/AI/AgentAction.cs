using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AgentAction
{
    protected string name;
    public string Name => name;
    public abstract bool CanStart(AgentBrain agentBrain);
    public abstract bool IsFinished(AgentBrain agentBrain);
    public abstract void Execute(AgentBrain agentBrain);
    public abstract void Update(AgentBrain agentBrain);
    public abstract void ExecuteBreak(AgentBrain agentBrain);
    public abstract void ExecuteConsequences(AgentBrain agentBrain);
}

public class MoveRandomlyAction : AgentAction
{
    private int moveRange; 
    private float speed;
    private float previousSpeed;
    private string animationName;

    public MoveRandomlyAction(string name, int moveRange = 8, float speed = 2.5f, string animationName = "IsWalking") {
        base.name = name;
        this.moveRange = moveRange;
        this.speed = speed;
        this.animationName = animationName;
    }

    public override bool CanStart(AgentBrain ab) {
        return true;
    }

    public override bool IsFinished(AgentBrain ab) {
        return false;
    }

    public override void Execute(AgentBrain ab) {
        ab.NavMeshAgent.isStopped = false;
        previousSpeed = ab.NavMeshAgent.speed;
        ab.NavMeshAgent.speed = speed;
        GoToRandomDestination(ab);
        ab.AnimationController.StartAnimating(animationName);
    }

    public override void Update(AgentBrain ab) {
        if (ab.NavMeshAgent.remainingDistance < 0.5f) {
            GoToRandomDestination(ab);
        }
    }

    public override void ExecuteBreak(AgentBrain ab) {
        StopMoving(ab);
    }

    public override void ExecuteConsequences(AgentBrain ab) {
        StopMoving(ab);
    }

    void StopMoving(AgentBrain ab) {
        ab.NavMeshAgent.isStopped = true;
        ab.NavMeshAgent.speed = previousSpeed;
        ab.NavMeshAgent.destination = ab.transform.position;
        ab.AnimationController.StopAnimating();
    }

    void GoToRandomDestination(AgentBrain ab) {
        ab.NavMeshAgent.SetDestination(ab.transform.position + new Vector3(UnityEngine.Random.Range(-moveRange, moveRange), 0, UnityEngine.Random.Range(-moveRange, moveRange)));
    }
}

public class GoToBeaconAction : AgentAction
{
    private BeaconType beaconType;
    private float lowestDistance;
    private string animationName;
    private float speed;
    private float previousSpeed;

    public GoToBeaconAction(string name, BeaconType beaconType, string animationName = "IsWalking", float lowestDistance = 1.5f, float speed = 2.5f) {
        this.beaconType = beaconType;
        base.name = name;
        this.lowestDistance = lowestDistance;
        this.animationName = animationName;
        this.speed = speed;
    }

    public override bool CanStart(AgentBrain ab) {
        if (ab.VisionSensor.IsBeaconSensible(beaconType)) {
            return true;
        }
        return false;
    }

    public override bool IsFinished(AgentBrain ab) {
        return ab.NavMeshAgent.remainingDistance < lowestDistance;
    }

    public override void Execute(AgentBrain ab) {
        ab.NavMeshAgent.isStopped = false;
        previousSpeed = ab.NavMeshAgent.speed;
        ab.NavMeshAgent.speed = speed;

        var deposit = ab.VisionSensor.GetNearestBeacon(beaconType);
        ab.NavMeshAgent.SetDestination(deposit.Position);

        ab.AnimationController.StartAnimating(animationName);
    }

    public override void Update(AgentBrain ab) { }

    public override void ExecuteBreak(AgentBrain ab) {
        StopMoving(ab);
    }

    public override void ExecuteConsequences(AgentBrain ab) {
        StopMoving(ab);
    }

    void StopMoving(AgentBrain ab) {
        ab.NavMeshAgent.isStopped = true;
        ab.NavMeshAgent.speed = previousSpeed; 
        ab.NavMeshAgent.destination = ab.transform.position;
        ab.AnimationController.StopAnimating();
    }
}

public class InteractAction : AgentAction {
    private float startTime;
    private float duration;
    private string animationName;
    private BeaconType beaconType;
    private bool removeInteractedBeacon;
    private AttributeName requiredAttribute;
    private float attributeCostPerSecond;
    private Action<AgentStatus> statusConsequences;
    private float fullAttributeCost;

    private readonly float attributeDrawingInterval = 0.05f;

    public InteractAction(string name, float duration, string animationName, BeaconType beaconType, bool removeInteractedBeacon = true, Action<AgentStatus> statusConsequences = null, AttributeName requiredAttribute = AttributeName.Stamina, float attributeCostPerSecond = 2f) {
        base.name = name;
        this.duration = duration;
        this.animationName = animationName;
        this.beaconType = beaconType;
        this.removeInteractedBeacon = removeInteractedBeacon;
        this.statusConsequences = statusConsequences;
        this.requiredAttribute = requiredAttribute;
        this.attributeCostPerSecond = attributeCostPerSecond;

        fullAttributeCost = duration * attributeCostPerSecond;
    }

    public override bool CanStart(AgentBrain ab) {
        if (ab.InteractionSensor.IsBeaconSensible(beaconType) && 
            ab.AgentStatus.GetAttribute(requiredAttribute) > fullAttributeCost
        ) {
            return true;
        }
        return false;
    }

    public override bool IsFinished(AgentBrain ab) {
        var currentTime = Time.time;
        if (currentTime - startTime > duration) {
            return true;
        }
        return false;
    }

    public override void Execute(AgentBrain ab) {
        startTime = Time.time;
        ab.AnimationController.StartAnimating(animationName);
        if(attributeCostPerSecond > 0f) {
            ab.AgentStatus.StartDrawing(requiredAttribute, attributeCostPerSecond, attributeDrawingInterval);
        }
    }

    public override void Update(AgentBrain ab) { }

    public override void ExecuteBreak(AgentBrain ab) { 
        if(attributeCostPerSecond > 0f) {
            ab.AgentStatus.StopDrawing(requiredAttribute);
        }
    }

    public override void ExecuteConsequences(AgentBrain ab) {
        statusConsequences?.Invoke(ab.AgentStatus);

        if(removeInteractedBeacon) {
            var beacon = ab.InteractionSensor.GetAndRemoveNearestBeacon(beaconType);
            beacon.Destroy();
        }

        if(attributeCostPerSecond > 0f) {
            ab.AgentStatus.StopDrawing(requiredAttribute);
        }
    }
}