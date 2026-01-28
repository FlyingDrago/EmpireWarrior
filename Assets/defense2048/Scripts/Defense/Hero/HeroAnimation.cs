using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayMove()
    {
        animator.SetTrigger("move0");
    }
    public void PlayIdle()
    {
        animator.SetTrigger("idle0");
    }
    public void PlayAttack()
    {
        animator.SetTrigger("attack0");
    }
}
