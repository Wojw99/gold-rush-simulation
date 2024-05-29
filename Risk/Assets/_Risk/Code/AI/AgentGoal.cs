using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

// public class AgentGoal
// {
//     List<AgentAction> _actions;
//     int _priority;

//     public void CanBeStarted(AgentBrain agentBrain)
//     {
        
//     }

//     public List<AgentAction> Actions { get => _actions; set => _actions = value; }
//     public int Priority { get => _priority; set => _priority = value; }
// }

public class AgentGoal
{
    public string Name { get; private set; }
    public List<string> Actions { get; private set; }
    public int Priority { get; private set; }
    public Func<AgentBrain, bool> CanStart { get; private set; }

    public AgentGoal(string name, List<string> actions, int priority, Func<AgentBrain, bool> canStart)
    {
        Name = name;
        Actions = actions;
        Priority = priority;
        CanStart = canStart;
    }
}


