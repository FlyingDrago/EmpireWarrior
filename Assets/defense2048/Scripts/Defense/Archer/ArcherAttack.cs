using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttack : MonoBehaviour
{
    [Header("AttackSettings")] public float attackRange = 3f;
    public float attackCooldowm = 1.5f;
    public LayerMask enemyLayer;

    [Header("Bullet")] public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Arrow Force")] public float shootPower = 7f;
    public float arcForce = 4f;

    [SerializeField] private Transform archerVisual;
    [SerializeField] private Animator anim;

    private float lastAttackTime;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        EnemyMove target = FindTarget();
        if (target == null) return;

        if (Time.time - lastAttackTime < attackCooldowm) return;

        Attack(target);
        lastAttackTime = Time.time;
    }

    EnemyMove FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);

        if (hit == null) return null;
        return hit ? hit.GetComponent<EnemyMove>() : null;
    }

    void Attack(EnemyMove enemy)
    {
        if (enemy == null) return;

        // 1️⃣ FLIP THEO VỊ TRÍ TARGET ENEMY
        FlipToEnemy(enemy.transform);

        // 2️⃣ SET BOOL DOWN THEO HƯỚNG DI CHUYỂN ENEMY
        SetDownByEnemyPosition(enemy);

        // 3️⃣ ATTACK
        anim.SetTrigger("attack");

        Shoot(enemy);
    }

    void FlipToEnemy(Transform enemy)
    {
        Vector3 scale = archerVisual.localScale;

        float dir = enemy.position.x - transform.position.x;

        if (dir < 0)
            scale.x = Mathf.Abs(scale.x); 
        else
            scale.x = -Mathf.Abs(scale.x); 

        archerVisual.localScale = scale;
    }

    void SetDownByEnemyPosition(EnemyMove enemy)
    {
        bool isDown = enemy.transform.position.y < transform.position.y;
        anim.SetBool("down", isDown);
    }



    void Shoot(EnemyMove enemy)
    {
        GameObject arrow = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        arrow.GetComponent<ArrowBullet>()
            .Init(enemy.transform);
    }
}