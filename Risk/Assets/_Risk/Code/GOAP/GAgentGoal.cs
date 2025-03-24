using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAgentGoal
{
    public string Name { get; }
    public float Priority { get; private set; }

    public HashSet<GAgentBelief> DesiredEffects { get; } = new();

    public GAgentGoal(string name) {
        Name = name;
    }

    public class Builder {
        readonly GAgentGoal goal;

        public Builder(string name) {
            goal = new GAgentGoal(name);
        }

        public Builder WithPriority(float priority) {
            goal.Priority = priority;
            return this;
        }

        public Builder WithDesiredEffect(GAgentBelief belief) {
            goal.DesiredEffects.Add(belief);
            return this;
        }

        public GAgentGoal Build() => goal;
    }
}
