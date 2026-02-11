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

    bool Valid()
    {
        return animator != null && animator.isActiveAndEnabled;
    }

    public void PlayMove()
    {
        if(!Valid())return;
        animator.SetTrigger("move0");
    }
    public void PlayIdle()
    {
        if(!Valid())return;
        animator.SetTrigger("idle0");
    }
    public void PlayAttack()
    {
        if(!Valid())return;
        animator.SetTrigger("attack0");
    }
}
