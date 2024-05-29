using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static string IS_DIGGING = "IsDigging";
    public static string IS_SITTING = "IsSitting";
    public static string IS_WALKING = "IsWalking";
    public static string IS_RUNNING = "IsRunning";
    public static string IS_DAMAGED = "IsDamaged";
    public static string IS_PRAYING = "IsPraying";
    public static string IS_DYING = "IsDying";
    public static string IS_ATTACKING = "IsAttacking";

    Animator animator;
    string currentAnimation = null;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartAnimating(string animationName) {
        animator.SetBool(animationName, true);
        if(currentAnimation != null) {
            animator.SetBool(currentAnimation, false);
        }
        currentAnimation = animationName;
    }

    public void StopAnimating() {
        if(currentAnimation != null) {
            animator.SetBool(currentAnimation, false);
            currentAnimation = null;
        }
    }
}
