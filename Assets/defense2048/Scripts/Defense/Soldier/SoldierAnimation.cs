using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAnimation : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        anim.ResetTrigger("move");
        anim.ResetTrigger("attack");
        anim.SetTrigger("idle");
    }

    public void PlayMove()
    {
        anim.ResetTrigger("idle");
        anim.ResetTrigger("attack");
        anim.SetTrigger("move");
    }

    public void PlayAttack()
    {
        anim.ResetTrigger("idle");
        anim.ResetTrigger("move");
        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");
    }
}