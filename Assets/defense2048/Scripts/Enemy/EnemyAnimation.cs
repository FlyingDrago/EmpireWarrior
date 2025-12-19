using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator animator;
    
    private Vector2 lastPosition;
    private string currentTrigger;
    private bool isAttacking;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        if(isAttacking)return;
        Vector2 currentPos = transform.position;
        Vector2 direction = currentPos - lastPosition;
        
        if(direction.sqrMagnitude<0.0001f)return;
        
        string nextTrigger;
        
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            nextTrigger = direction.y < 0 ? "move_down" : "move_up";
           
        }
        else
        {
            
            nextTrigger = "move";
        }

        if (currentTrigger != nextTrigger)
        {
            animator.ResetTrigger(currentTrigger);
            animator.SetTrigger(nextTrigger);
            currentTrigger = nextTrigger;
        }

        lastPosition = currentPos;
    }

    public void PlayAttack()
    {
        if(isAttacking)return;

        isAttacking = true;
        animator.ResetTrigger(currentTrigger);
        animator.SetTrigger("attack");
        currentTrigger = "attack";
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
    }

    public bool IsAttacking => isAttacking;
}