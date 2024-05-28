using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeliefFactory {
    private readonly GoapAgent agent;
    private readonly Dictionary<string, AgentBelief> beliefs;

    public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs) {
        this.agent = agent;
        this.beliefs = beliefs;
    }

    public void AddBelief(string key, Func<bool> condition) {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(condition)
            .Build());
    }

    public void AddSensorBelief(string key, Sensor sensor) {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => sensor.IsTargetInRange)
            .WithObservedLocation(() => sensor.TargetPosition)
            .Build());
    }

    public void AddLocationBelief(string key, float distance, Transform locationCondition) {
        AddLocationBelief(key, distance, locationCondition.position);
    }

    public void AddLocationBelief(string key, float distance, Vector3 locationCondition) {
        beliefs.Add(key, new AgentBelief.Builder(key)
            .WithCondition(() => InRangeOf(locationCondition, distance))
            .WithObservedLocation(() => locationCondition)
            .Build());
    }

    private bool InRangeOf(Vector3 position, float range) => Vector3.Distance(agent.transform.position, position) < range;
}

public class AgentBelief {
    public string Name { get; }

    Func<bool> condition = () => false;
    Func<Vector3> observedLocation = () => Vector3.zero; // default value of Vector3.zero means that the thing does not has a location

    public Vector3 ObservedLocation => observedLocation();

    private AgentBelief(string name) {
        Name = name;
    }

    public bool Evaluate() => condition();

    public class Builder {
        private readonly AgentBelief belief;

        public Builder(string name) {
            belief = new AgentBelief(name);
        }

        public Builder WithCondition(Func<bool> condition) {
            belief.condition = condition;
            return this;
        }

        public Builder WithObservedLocation(Func<Vector3> observedLocation) {
            belief.observedLocation = observedLocation;
            return this;
        }

        public AgentBelief Build() {
            return belief;
        }
    }
}
