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
    private const string IS_DAMAGED = "IsDamaged";
    private const string IS_PRAYING = "IsPraying";
    private const string IS_DYING = "IsDying";

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
        if (goal == AgentBrain.GoalName.FREEZE 
            || goal == AgentBrain.GoalName.TAKE_REST) {
            animator.SetBool(IS_SITTING, true);
        } 
        if (goal == AgentBrain.GoalName.SEARCH_FOR_DEPOSIT 
            || goal == AgentBrain.GoalName.SEARCH_FOR_REST
            || goal == AgentBrain.GoalName.SEARCH_FOR_HEALING) {
            animator.SetBool(IS_WALKING, true);
        } 
        if (goal == AgentBrain.GoalName.GO_TO_NEAREST_DEPOSIT 
            || goal == AgentBrain.GoalName.GO_TO_NEAREST_REST
            || goal == AgentBrain.GoalName.GO_TO_NEAREST_HEALING) {
            animator.SetBool(IS_WALKING, true);
        } 
        if (goal == AgentBrain.GoalName.MINE_DEPOSIT) {
            animator.SetBool(IS_DIGGING, true);
        } 
        if (goal == AgentBrain.GoalName.RUN_FOR_YOUR_LIFE) {
            animator.SetBool(IS_RUNNING, true);
        }
        if(goal == AgentBrain.GoalName.TAKE_DAMAGE) {
            animator.SetBool(IS_DAMAGED, true);
        }
        if(goal == AgentBrain.GoalName.TAKE_HEALING) {
            animator.SetBool(IS_SITTING, true);
        }
        if(goal == AgentBrain.GoalName.DIE) {
            animator.SetBool(IS_DYING, true);
            Debug.Log("Dying");
        }
    }

    private void ClearAnimatorBools() {
        animator.SetBool(IS_SITTING, false);
        animator.SetBool(IS_WALKING, false);
        animator.SetBool(IS_RUNNING, false);
        animator.SetBool(IS_DIGGING, false);
        animator.SetBool(IS_DAMAGED, false);
        animator.SetBool(IS_PRAYING, false);
        animator.SetBool(IS_DYING, false);
    }

    private void OnDestroy() {
        agentBrain.GoalChanged -= OnGoalChanged;
    }
}
