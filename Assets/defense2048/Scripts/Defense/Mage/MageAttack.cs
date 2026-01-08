using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAttack : MonoBehaviour
{
    [Header("Attacks")] public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;
    public LayerMask enemyLayer;

    [Header("Spell")] public GameObject spellPrefab;
    public Transform firePoint;

    private Animator anim;
    private float lastAttackTime;
    private EnemyMove currentTarget;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        currentTarget = FindTarget();
        if (currentTarget == null) return;

        SetDirection(currentTarget);

        if (CanAttack())
        {
            Attack();
        }
    }

    void SetDirection(EnemyMove enemy)
    {
        Vector3 enemyPos = enemy.transform.position;
        Vector3 myPos = transform.position;

        bool isLeft = enemyPos.x < myPos.x;
        bool isDown = enemyPos.y < myPos.y;

        anim.SetBool("left", isLeft);
        anim.SetBool("down", isDown);
    }

    bool CanAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return false;

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("attack")) return false;

        return true;
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        anim.SetTrigger("attack");
    }

    EnemyMove FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);

        return hit ? hit.GetComponent<EnemyMove>() : null;
    }

    public void CastSpell()
    {
        if(currentTarget==null)return;
     
        GameObject bullet = Instantiate(spellPrefab, firePoint.position, Quaternion.identity);
        
        bullet.GetComponent<MageBullet>().Init(currentTarget.transform);
    }
}