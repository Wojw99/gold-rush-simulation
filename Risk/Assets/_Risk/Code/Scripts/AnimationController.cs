using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private AgentBrain agentBrain;
    private const string IS_DIGGING = "IsDigging";
    private const string IS_SITTING = "IsSitting";
    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agentBrain = GetComponent<AgentBrain>();
        agentBrain.GoalChanged += OnGoalChanged;
    }

    private void OnGoalChanged(AgentBrain.GoalName goal) {
        ClearAnimatorBools();
        ChangeAnimatorBasedOnGoal(goal);
    }

    private void ChangeAnimatorBasedOnGoal(AgentBrain.GoalName goal) {
        if (goal == AgentBrain.GoalName.FREEZE) {
            animator.SetBool(IS_SITTING, true);
        } else if (goal == AgentBrain.GoalName.SEARCH_FOR_DEPOSIT) {
            animator.SetBool(IS_WALKING, true);
        } else if (goal == AgentBrain.GoalName.GO_TO_NEAREST_DEPOSIT) {
            animator.SetBool(IS_WALKING, true);
        } else if (goal == AgentBrain.GoalName.MINE_DEPOSIT) {
            animator.SetBool(IS_DIGGING, true);
        } else if (goal == AgentBrain.GoalName.RUN_FOR_YOUR_LIFE) {
            animator.SetBool(IS_RUNNING, true);
        }
    }

    private void ClearAnimatorBools() {
        animator.SetBool(IS_SITTING, false);
        animator.SetBool(IS_WALKING, false);
        animator.SetBool(IS_RUNNING, false);
        animator.SetBool(IS_DIGGING, false);
    }

    private void OnDestroy() {
        agentBrain.GoalChanged -= OnGoalChanged;
    }
}
