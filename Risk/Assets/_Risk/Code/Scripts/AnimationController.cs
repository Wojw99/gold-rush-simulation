using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    string currentAnimation = null;

    void Awake()
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

    public float GetAnimationDuration(string animationName) {
        var clips = animator.runtimeAnimatorController.animationClips;
        foreach(var clip in clips) {
            if(clip.name == animationName) {
                return clip.length;
            }
        }
        var defValue = 1f;
        Debug.LogError($"Animation not found: {animationName}, returned {defValue}");
        return defValue;
    }

    public void SetSpeed(float speed) {
        animator.speed = speed;
    }

    public void ResetSpeed() {
        animator.speed = 1f;
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