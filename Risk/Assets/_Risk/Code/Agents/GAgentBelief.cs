using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This factory contains informations and methods to help create dictionary of beliefs.
/// </summary>
public class BeliefFactory {
    readonly GAgent agent;
    readonly Dictionary<string, GAgentBelief> beliefs;

    public BeliefFactory(GAgent agent, Dictionary<string, GAgentBelief> beliefs) {
        this.agent = agent;
        this.beliefs = beliefs;
    }

    public void AddBelief(string key, Func<bool> condition) {
        beliefs.Add(key, new GAgentBelief.Builder(key)
            .WithCondition(condition)
            .Build());
    }

    public void AddTargetBelief(string key, Func<bool> condition, GameObject target) {
        beliefs.Add(key, new GAgentBelief.Builder(key)
            .WithCondition(condition)
            .WithTarget(target)
            .Build());
    }

    public void AddSensorBelief(string key, Sensor sensor, BeaconType type) {
        beliefs.Add(key, new GAgentBelief.Builder(key)
            .WithCondition(() => sensor.ContainsTargetOfType(type))
            .WithLocation(() => sensor.GetNearestTarget(type).transform.position)
            .Build());
    }

    public void AddLocationBelief(string key, float distance, Transform locationCondition) {
        AddLocationBelief(key, distance, locationCondition.position);
    }

    public void AddLocationBelief(string key, float distance, Vector3 locationCondition) {
        beliefs.Add(key, new GAgentBelief.Builder(key)
            .WithCondition(() => InRangeOf(locationCondition, distance))
            .WithLocation(() => locationCondition)
            .Build());
    }

    /// <summary>
    /// Check if the agent is in range of some target. Ex: if the agent is close enough to interact with the target.
    /// </summary>
    bool InRangeOf(Vector3 target, float range) {
        return Vector3.Distance(agent.transform.position, target) <= range;
    }
}

public class GAgentBelief
{
    public string Name { get; }

    Func<bool> condition = () => false;
    Func<Vector3> observedLocation = () => Vector3.zero;
    GameObject observedTarget = null;

    GAgentBelief(string name) {
        Name = name;
    }

    public bool Evaluate() => condition();
    public Vector3 GetObservedLocation() => observedLocation();
    public GameObject ObservedTarget => observedTarget;

    public class Builder {
        readonly GAgentBelief belief;

        public Builder(string name) {
            belief = new GAgentBelief(name);
        }

        public Builder WithCondition(Func<bool> condition) {
            belief.condition = condition;
            return this;
        }

        public Builder WithLocation(Func<Vector3> observedLocation) {
            belief.observedLocation = observedLocation;
            return this;
        }

        public Builder WithTarget(GameObject observedTarget) {
            belief.observedTarget = observedTarget;
            return this;
        }

        public GAgentBelief Build() => belief;
    }
}
