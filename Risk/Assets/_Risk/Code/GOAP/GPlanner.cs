using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IGPlanner {
    ActionPlan Plan(GAgent agent, HashSet<GAgentGoal> goals, GAgentGoal mostRecentGoal = null);
}

public class GPlanner : IGPlanner
{
    public ActionPlan Plan(GAgent agent, HashSet<GAgentGoal> goals, GAgentGoal mostRecentGoal)
    {
        var orderedGoals = goals
            .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
            .OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
            .ToList();

        // Try to solve each goal in order. DFS.
        foreach (var goal in orderedGoals) {
            var goalNode = new Node(null, null, goal.DesiredEffects, 0);

            // If we can find a path to the goal, return the plan.
            if(FindPath(goalNode, agent.Actions)) {
                if(goalNode.IsLeafDead) continue;

                var actionStack = new Stack<GAgentAction>();
                while(goalNode.Leaves.Count > 0) {
                    var cheapestLeaf = goalNode.Leaves.OrderBy(leaf => leaf.Cost).First();
                    goalNode = cheapestLeaf;
                    actionStack.Push(cheapestLeaf.Action);
                }

                return new ActionPlan(goal, actionStack, goalNode.Cost); 
            }
        }

        Debug.LogWarning("No plan found.");
        return null;
    }

    // TODO: Consider a more powerful search algorithm like A* or D*
    bool FindPath(Node parent, HashSet<GAgentAction> actions) {
        foreach(var action in actions) {
            var requiredEffects = parent.RequiredEffects;

            requiredEffects.RemoveWhere(b => b.Evaluate());

            if(requiredEffects.Count == 0) {
                return true;
            }

            if(action.Effects.Any(requiredEffects.Contains)) {
                var newRequiredEffects = new HashSet<GAgentBelief>(requiredEffects);
                newRequiredEffects.ExceptWith(action.Effects);
                newRequiredEffects.UnionWith(action.Preconditions);

                var newAvailableActions = new HashSet<GAgentAction>(actions);
                newAvailableActions.Remove(action);

                var newNode = new Node(parent, action, newRequiredEffects, parent.Cost + action.Cost);

                // Explore the new node recursively
                if(FindPath(newNode, newAvailableActions)) {
                    parent.Leaves.Add(newNode);
                    newRequiredEffects.ExceptWith(newNode.Action.Preconditions);
                }

                // If all effects at this depth have been satisfied, return true
                if(newRequiredEffects.Count == 0) {
                    return true;
                }
            }
        }

        return false;
    }
}

public class Node {
    public Node Parent { get; }
    public GAgentAction Action { get; }
    public HashSet<GAgentBelief> RequiredEffects { get; }
    public List<Node> Leaves { get; }
    public float Cost { get; }

    public bool IsLeafDead => Leaves.Count == 0 && Action == null;

    public Node(Node parent, GAgentAction action, HashSet<GAgentBelief> requiredEffects, float cost) {
        Parent = parent;
        Action = action;
        RequiredEffects = requiredEffects;
        Leaves = new List<Node>();
        Cost = cost;
    }
}

public class ActionPlan {
    public GAgentGoal AgentGoal { get; }
    public Stack<GAgentAction> Actions { get; }
    public float TotalCost { get; set; }

    public ActionPlan(GAgentGoal agentGoal, Stack<GAgentAction> actions, float totalCost) {
        AgentGoal = agentGoal;
        Actions = actions;
        TotalCost = totalCost;
    }
}
