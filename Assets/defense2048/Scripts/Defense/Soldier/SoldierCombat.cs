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

    private bool isInPosition;


    private void Awake()
    {
        SoldierAnimation = GetComponent<SoldierAnimation>();
    }

    public void StartCombat(EnemyCombat enemyCombat)
    {
        enemy = enemyCombat;
        isInPosition = false;
        enemy.GetComponent<EnemyHeath>().OnEnemyDead += StopCombat;
    }

    private void Update()
    {
        if (enemy != null)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                StopCombat();

                return;
            }


            if (isInPosition)

            {
                if (Time.time >= lastAttackTime + attackCooldown)

                {
                    lastAttackTime = Time.time;

                    SoldierAnimation.PlayAttack();
                }
            }

            return;
        }


        TryFindEnemyImmediate();
    }


    public bool TryFindEnemyImmediate()
    {
        var move = GetComponent<SoldierMove>();

        if (enemy != null) return false;
        if (move.IsMovingToEnemy) return false;
        if (move.IsInCombat) return false;


        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);

        float closest = Mathf.Infinity;
        EnemyCombat best = null;

        foreach (var hit in hits)
        {
            EnemyCombat e = hit.GetComponent<EnemyCombat>();
            if (e == null) continue;
            
            if (e.IsLocked && e.GetCurrentTarget() != transform)
                continue;



            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < closest)
            {
                closest = d;
                best = e;
            }
        }

        if (best != null)
        {
            SetEnemy(best);
            return true;
        }

        return false;
    }

    public void AnimDealDamage()
    {
        if (enemy == null) return;

        EnemyHeath eh = enemy.GetComponent<EnemyHeath>();
        if (eh == null || !eh.gameObject.activeInHierarchy) return;

        eh.TakeDamage(damage);
    }


    public void StopCombat()
    {
        if (enemy != null)
        {
            EnemyHeath eh = enemy.GetComponent<EnemyHeath>();
            if (eh != null)
                eh.OnEnemyDead -= StopCombat;
            var shooter = enemy?.GetComponent<EnemyShooter>();
            shooter?.ExitMelee();
            
            if(enemy.gameObject.activeInHierarchy)enemy.OnSoldierDead();
        }
        
        enemy = null;
        isInPosition = false;
        lastAttackTime = 0;

        if (this != null && gameObject.activeInHierarchy) SoldierAnimation?.PlayIdle();

      

        SoldierMove move = GetComponent<SoldierMove>();
        if (move != null)
            move.ReturnFormation();
    }


    public void SetInPosition(bool v)
    {
        isInPosition = v;

        if (v && enemy != null)
        {
            var shooter = enemy.GetComponent<EnemyShooter>();
            shooter?.BeginMeleeFight();
        }
    }


    private void SetEnemy(EnemyCombat newEnemy)
    {
        if (enemy != null) return;

        enemy = newEnemy;
        isInPosition = false;

        EnemyHeath eh = enemy.GetComponent<EnemyHeath>();
        if (eh != null)
            eh.OnEnemyDead += StopCombat;


        enemy.PreLockTarget(transform);


        EnemyShooter shooter = enemy.GetComponent<EnemyShooter>();
        if (shooter != null)
        {
            shooter.EnterMelee(transform);
        }

        GetComponent<SoldierMove>().MoveToEnemy(enemy.transform);

    }


    void OnEnable()
    {
        lastAttackTime = 0;
        isInPosition = false;
        enemy = null;

        Invoke(nameof(TryFindEnemyImmediate), 0.05f);
    }

    void OnDisable()
    {
        if (enemy != null)
        {
            EnemyHeath eh = enemy.GetComponent<EnemyHeath>();
            if (eh != null)
                eh.OnEnemyDead -= StopCombat;
        }

        enemy = null;
    }
}