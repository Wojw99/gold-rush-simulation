using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAgentAction
{
    public string Name { get; }
    public float Cost { get; private set; }

    public HashSet<GAgentBelief> Preconditions { get; } = new();
    public HashSet<GAgentBelief> Effects { get; } = new();

    IActionStrategy strategy;
    public bool Completed => strategy.Completed;

    GAgentAction(string name) {
        Name = name;
    }

    public void Start() => strategy.Start();

    /// <summary>
    /// Update the action strategy. Evaluate effects if the action is completed.
    /// </summary>
    public void Update(float deltaTime) {
        if(strategy.CanPerform) {
            strategy.Update(deltaTime);
        }
        
        if(!strategy.Completed) {
            return;
        }

        foreach(var effect in Effects) {
            effect.Evaluate();
        }
    }

    public void Stop() => strategy.Stop();

    public class Builder {
        readonly GAgentAction action;

        public Builder(string name) {
            action = new GAgentAction(name) {
                Cost = 1
            };
        }

        public Builder WithCost(float cost) {
            action.Cost = cost;
            return this;
        }

        public Builder WithStrategy(IActionStrategy strategy) {
            action.strategy = strategy;
            return this;
        }

        public Builder AddPrecondition(GAgentBelief belief) {
            action.Preconditions.Add(belief);
            return this;
        }

        public Builder AddEffect(GAgentBelief belief) {
            action.Effects.Add(belief);
            return this;
        }

        public GAgentAction Build() => action;
    }
}
