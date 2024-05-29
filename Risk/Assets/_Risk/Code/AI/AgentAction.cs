using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AgentAction
{
    public abstract string Name { get; }
    public abstract bool CanStart(AgentBrain agentBrain);
    public abstract bool IsFinished(AgentBrain agentBrain);
    public abstract void Execute(AgentBrain agentBrain);
    public abstract void Update(AgentBrain agentBrain);
    public abstract void ExecuteBreak(AgentBrain agentBrain);
    public abstract void ExecuteConsequences(AgentBrain agentBrain);
}

public class MoveRandomlyAction : AgentAction
{
    private readonly int moveRange = 8; 
    public override string Name => "move_randomly";

    public override bool CanStart(AgentBrain agentBrain) {
        return true;
    }

    public override bool IsFinished(AgentBrain agentBrain) {
        return false;
    }

    public override void Execute(AgentBrain agentBrain) {
        agentBrain.NavMeshAgent.isStopped = false;
        GoToRandomDestination(agentBrain);
        agentBrain.AnimationController.StartAnimating(AnimationController.IS_WALKING);
    }

    public override void Update(AgentBrain agentBrain) {
        if (agentBrain.NavMeshAgent.remainingDistance < 0.5f) {
            GoToRandomDestination(agentBrain);
        }
    }

    public override void ExecuteBreak(AgentBrain agentBrain) {
        StopMoving(agentBrain);
    }

    public override void ExecuteConsequences(AgentBrain agentBrain) {
        StopMoving(agentBrain);
    }

    void StopMoving(AgentBrain agentBrain) {
        agentBrain.NavMeshAgent.isStopped = true;
        agentBrain.NavMeshAgent.destination = agentBrain.transform.position;
        agentBrain.AnimationController.StopAnimating();
    }

    void GoToRandomDestination(AgentBrain agentBrain) {
        agentBrain.NavMeshAgent.SetDestination(agentBrain.transform.position + new Vector3(Random.Range(-moveRange, moveRange), 0, Random.Range(-moveRange, moveRange)));
    }
}


public class GoToDepositAction : AgentAction
{
    public override string Name { get; } = "go_to_deposit";

    public override bool CanStart(AgentBrain agentBrain) {
        if (agentBrain.VisionSensor.IsBeaconSensible(BeaconType.DEPOSIT)) {
            return true;
        }
        return false;
    }

    public override bool IsFinished(AgentBrain agentBrain) {
        return agentBrain.NavMeshAgent.remainingDistance < 1.5f;
    }

    public override void Execute(AgentBrain agentBrain) {
        agentBrain.NavMeshAgent.isStopped = false;
        var deposit = agentBrain.VisionSensor.GetNearestBeacon(BeaconType.DEPOSIT);
        agentBrain.NavMeshAgent.SetDestination(deposit.Position);
        agentBrain.AnimationController.StartAnimating(AnimationController.IS_WALKING);
    }

    public override void Update(AgentBrain agentBrain) {
        
    }

    public override void ExecuteBreak(AgentBrain agentBrain) {
        StopMoving(agentBrain);
    }

    public override void ExecuteConsequences(AgentBrain agentBrain) {
        StopMoving(agentBrain);
    }

    void StopMoving(AgentBrain agentBrain) {
        agentBrain.NavMeshAgent.isStopped = true;
        agentBrain.NavMeshAgent.destination = agentBrain.transform.position;
        agentBrain.AnimationController.StopAnimating();
    }
}

public class MineDepositAction : AgentAction {
    private float startTime;
    private float durationSec = 3f;

    public override string Name { get; } = "mine_deposit";

    public override bool CanStart(AgentBrain agentBrain) {
        if (agentBrain.InteractionSensor.IsBeaconSensible(BeaconType.DEPOSIT)) {
            return true;
        }
        return false;
    }

    public override bool IsFinished(AgentBrain agentBrain) {
        var currentTime = Time.time;
        if (currentTime - startTime > durationSec) {
            return true;
        }
        return false;
    }

    public override void Execute(AgentBrain agentBrain) {
        startTime = Time.time;
        agentBrain.AnimationController.StartAnimating(AnimationController.IS_DIGGING);
    }

    public override void Update(AgentBrain agentBrain) {
        
    }

    public override void ExecuteBreak(AgentBrain agentBrain) {
        
    }

    public override void ExecuteConsequences(AgentBrain agentBrain) {
        agentBrain.AgentStatus.Ore += 10;
        agentBrain.AgentStatus.Stamina -= 10;
        var beacon = agentBrain.InteractionSensor.GetAndRemoveNearestBeacon(BeaconType.DEPOSIT);
        beacon.Destroy();
    }
}