using System;
using UnityEngine;

public class HeroCombat : MonoBehaviour
{
    [Header("Combat")] public float attackCooldown = 1.2f;
    public int damage = 2;
    public float detectRange = 2.5f;
    public LayerMask enemyLayer;

    [Header("AOE")] public float aoeRange = 1.5f;
    public GameObject aoeEffectPrefab;
    public float effectDestroyTime = 0.5f;

    private float lastAttackTime;
    private EnemyCombat enemy;
    private EnemyShooter enemyShooter;
    
    private bool isInPosition;

    private HeroAnimation heroAnim;
    private HeroMove heroMove;
    public EnemyCombat GetCurrentEnemy() => enemy;

    private void Awake()
    {
        heroAnim = GetComponent<HeroAnimation>();
        heroMove = GetComponent<HeroMove>();
    }

    private void Update()
    {
        if (enemyShooter != null)
        {
            if (enemyShooter==null||!enemyShooter.gameObject.activeInHierarchy)
            {
                StopCombat();
                return;
            }

            if (isInPosition && Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                heroAnim.PlayAttack();
            }
            return;
        }
        
        
        
        if (enemy != null)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                StopCombat();
                return;
            }

            if (!enemy.IsLocked || enemy.GetCurrentTarget() != transform)
            {
                StopCombat();
                return;
            }


            if (isInPosition)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    heroAnim.PlayAttack();
                }
            }

            return;
        }

        if (heroMove != null && heroMove.IsMoving) return;
        TryFindEnemyImmediate();
    }


    // public bool TryFindEnemyImmediate()
    // {
    //     if (enemy != null) return false;
    //
    //
    //
    //     Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);
    //
    //     EnemyCombat best = null;
    //     float closest = Mathf.Infinity;
    //
    //     foreach (var hit in hits)
    //     {
    //         EnemyCombat e = hit.GetComponent<EnemyCombat>();
    //         if (e == null || e.IsLocked ||!e.gameObject.activeInHierarchy) continue;
    //
    //         float d = Vector2.Distance(transform.position, e.transform.position);
    //         if (d < closest)
    //         {
    //             closest = d;
    //             best = e;
    //         }
    //     }
    //
    //     if (best != null)
    //     {
    //         enemy = best;
    //         enemy.PreLockTarget(transform);
    //         var myHealth = GetComponent<DefenseHealth>();
    //         if(myHealth!=null)myHealth.LockTarget(best);
    //         heroMove.MoveToEnemy(best.transform);
    //
    //         return true;
    //     }
    //
    //     return false;
    // }
    public bool TryFindEnemyImmediate()
    {
        if (enemy != null || enemyShooter != null)
            return false;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                detectRange,
                enemyLayer);

        float closest = Mathf.Infinity;
        Transform best = null;

        foreach (var hit in hits)
        {
            if (!hit.gameObject.activeInHierarchy) continue;

            float d = Vector2.Distance(
                transform.position,
                hit.transform.position);

            if (d < closest)
            {
                closest = d;
                best = hit.transform;
            }
        }

        if (best == null) return false;

        // ===== TRY OLD SYSTEM =====
        EnemyCombat e = best.GetComponent<EnemyCombat>();
        if (e != null && !e.IsLocked)
        {
            enemy = e;
            enemy.PreLockTarget(transform);

            heroMove.MoveToEnemy(best);
            return true;
        }

        // ===== TRY SHOOTER =====
        EnemyShooter s = best.GetComponent<EnemyShooter>();
        if (s != null)
        {
            enemyShooter = s;

            s.OnMeleeExit += HandleShooterExit; 
            s.EnterMelee(transform);

            heroMove.MoveToEnemy(best);


            return true;
        }

        return false;
    }
    void HandleShooterExit(EnemyShooter s)
    {
        if(!this||!gameObject.activeInHierarchy)return;
        if (enemyShooter == s)
            StopCombat();
    }

    


    public void AnimDealDamage()
    {
        if (aoeEffectPrefab != null)
        {
            GameObject effect = Instantiate(aoeEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, effectDestroyTime);
        }


        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            aoeRange,
            enemyLayer
        );


        foreach (var hit in hits)
        {
            EnemyHeath eh = hit.GetComponent<EnemyHeath>();
            if (eh != null && eh.gameObject.activeInHierarchy)
            {
                eh.TakeDamage(damage);
            }
        }

        // if (enemy != null)
        // {
        //     if (!enemy.gameObject.activeInHierarchy)
        //     {
        //         StopCombat();
        //     }
        // }
    }

    public void SetInPosition(bool v)
    {
        isInPosition = v;
        if(v&& enemyShooter!=null)enemyShooter.BeginMeleeFight();
    }

    public void StopCombat()
    {
        if (enemyShooter != null) enemyShooter.OnMeleeExit -= HandleShooterExit;
        
        enemy = null;
        enemyShooter = null;
        isInPosition = false;

        heroAnim.PlayIdle();
    }

    private void OnDisable()
    {
        if (enemyShooter != null) enemyShooter.OnMeleeExit -= HandleShooterExit;
    }
}