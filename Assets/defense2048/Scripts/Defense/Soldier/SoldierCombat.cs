using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierCombat : MonoBehaviour
{
    public float attackCooldown = 1.2f;
    public int damage = 1;
    public float detectRange = 2f;
    public LayerMask enemyLayer;

    private float lastAttackTime;
    private EnemyCombat enemy;
    private SoldierAnimation SoldierAnimation;
    

    private void Awake()
    {
        SoldierAnimation = GetComponent<SoldierAnimation>();
    }

    public void StartCombat(EnemyCombat enemyCombat)
    {
        enemy = enemyCombat;
    }

    private void Update()
    {
        if (enemy == null) return;
        
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        SoldierAnimation.PlayAttack();
    }

    public void TryFindEnemy()
    {
       

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);

        foreach (var hit in hits)
        {
            EnemyCombat enemyCombat = hit.GetComponent<EnemyCombat>();
            if (enemyCombat != null && !enemyCombat.IsLocked)
            {
          
                GetComponent<SoldierMove>().MoveToEnemy(enemyCombat.transform);
                return;
            }
        }
    }

    public void AnimDealDamage()
    {
        if (enemy != null)
        {
           
            enemy.GetComponent<EnemyHeath>()?.TakeDamage(damage);
        }
    }

    public void StopCombat()
    {
        enemy = null;
        lastAttackTime = 0;
        SoldierAnimation.PlayIdle();
        

        GetComponent<SoldierMove>().ClearTarget();
        TryFindEnemy();
    }
}