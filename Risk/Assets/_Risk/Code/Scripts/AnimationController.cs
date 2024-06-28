using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
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

public enum AnimType {
    IsDigging,
    IsSitting,
    IsWalking,
    IsRunning,
    IsDamaged,
    IsPraying,
    IsDying,
    IsAttacking
}