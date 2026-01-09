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
        if (enemy == null|| !isInPosition) return;

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

    public bool TryFindEnemyImmediate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);

        float closest = Mathf.Infinity;
        EnemyCombat best = null;

        foreach (var hit in hits)
        {
            EnemyCombat e = hit.GetComponent<EnemyCombat>();
            if (e == null||e.IsLocked) continue;

            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < closest)
            {
                closest = d;
                best = e;
            }
        }

        if (best != null)
        {
        
            GetComponent<SoldierMove>().MoveToEnemy(best.transform);
            return true;
        }

        return false;
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
        isInPosition = false;
        lastAttackTime = 0;

        if (!this || !gameObject.activeInHierarchy) return;

        if (SoldierAnimation != null)
            SoldierAnimation.PlayIdle();

        var move = GetComponent<SoldierMove>();
        if (move != null)
            move.ClearTarget();

        TryFindEnemy();
    }

    public void SetInPosition(bool v)
    {
        isInPosition = v;
    }


    void OnEnable()
    {
        lastAttackTime = 0;
        isInPosition = false;
        enemy = null;

        Invoke(nameof(TryFindEnemyImmediate), 0.05f);
    }

}