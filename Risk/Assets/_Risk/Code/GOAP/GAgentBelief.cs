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

    public void AddSensorBelief(string key, Sensor sensor) {
        beliefs.Add(key, new GAgentBelief.Builder(key)
            .WithCondition(() => sensor.IsTargetInRange)
            .WithLocation(() => sensor.TargetPosition)
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

    GAgentBelief(string name) {
        Name = name;
    }

    public bool Evaluate() => condition();

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

        public GAgentBelief Build() => belief;
    }
}
