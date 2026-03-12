using System;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Action<EnemyShooter> OnMeleeExit;

    enum CombatMode
    {
        None,
        Ranged,
        MeleeWait,
        MeleeFight
    }

    [Header("Ranged")]
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public int damage = 1;
    public LayerMask targetLayer;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public GameObject hitEffectPrefab;

    [Header("Melee")]
    public float meleeOffset = 0.8f;

    private Transform currentTarget;
    private CombatMode mode = CombatMode.None;

    private float lastAttackTime;
    private bool isAttacking;

    private float scanTimer;
    private const float SCAN_INTERVAL = 0.25f;

    private EnemyMove enemyMove;
    private EnemyAnimation enemyAnimation;

    public bool IsInMelee => mode == CombatMode.MeleeFight;

    void Awake()
    {
        enemyMove = GetComponent<EnemyMove>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    void Update()
    {
        if (isAttacking && Time.time - lastAttackTime > attackCooldown + 1f)
            isAttacking = false;

        switch (mode)
        {
            case CombatMode.MeleeWait:
                UpdateMeleeWait();
                break;

            case CombatMode.MeleeFight:
                UpdateMeleeFight();
                break;

            default:
                UpdateRanged();
                break;
        }
    }

    // =========================
    // RANGED
    // =========================

    void UpdateRanged()
    {
        if (isAttacking) return;

        ScanForTarget();

        if (currentTarget == null)
        {
            enemyMove?.ResumeMove();
            return;
        }

        enemyMove?.StopMove();
        FaceTarget();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartRangedAttack();
        }
    }

    void StartRangedAttack()
    {
        mode = CombatMode.Ranged;
        isAttacking = true;
        lastAttackTime = Time.time;

        enemyAnimation.PlayAttack2();
    }

    public void FireBullet()
    {
        if (currentTarget == null) return;

        GameObject bulletObj =
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();

        if (bullet != null)
        {
            bullet.Init(null, currentTarget, damage, bulletSpeed);
            bullet.SetHitEffect(hitEffectPrefab);
        }
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        lastAttackTime = Time.time;

        if (mode == CombatMode.Ranged)
            mode = CombatMode.None;

        enemyAnimation.OnAttackEnd();
    }

    // =========================
    // MELEE
    // =========================

    public void EnterMelee(Transform attacker)
    {
        currentTarget = attacker;

        mode = CombatMode.MeleeWait;

        enemyMove?.StopMove();
        isAttacking = false;
    }

    public void BeginMeleeFight()
    {
        if (mode != CombatMode.MeleeWait) return;

        mode = CombatMode.MeleeFight;
        lastAttackTime = 0;
    }

    void UpdateMeleeWait()
    {
        if (!IsTargetValid())
        {
            ExitMelee();
            return;
        }

        FaceTarget();
    }

    void UpdateMeleeFight()
    {
        if (!IsTargetValid())
        {
            ExitMelee();
            return;
        }

        FaceTarget();

        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            isAttacking = true;

            enemyAnimation.PlayAttack();
        }
    }

    public void ExitMelee()
    {
        mode = CombatMode.None;
        currentTarget = null;

        isAttacking = false;
        lastAttackTime = 0;

        enemyAnimation.OnAttackEnd();
        enemyMove?.ResumeMove();

        OnMeleeExit?.Invoke(this);
    }

    // =========================
    // TARGET
    // =========================

    void ScanForTarget()
    {
        scanTimer += Time.deltaTime;

        if (scanTimer < SCAN_INTERVAL) return;

        scanTimer = 0;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

        float closest = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var hit in hits)
        {
            if (!hit.gameObject.activeInHierarchy) continue;
            if (hit.transform == transform) continue;

            var health = hit.GetComponent<DefenseHealth>();
            if (health == null) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (dist < closest)
            {
                closest = dist;
                bestTarget = hit.transform;
            }
        }

        currentTarget = bestTarget;
    }

    bool IsTargetValid()
    {
        return currentTarget != null && currentTarget.gameObject.activeInHierarchy;
    }

    // =========================
    // UTIL
    // =========================

    void FaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 scale = transform.localScale;

        if (currentTarget.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    public void ResetOnSpawn()
    {
        currentTarget = null;
        mode = CombatMode.None;
        isAttacking = false;

        enemyAnimation?.OnAttackEnd();
        enemyMove?.ResumeMove();
    }

    void OnEnable()
    {
        ResetOnSpawn();
    }

    void OnDisable()
    {
        currentTarget = null;
        isAttacking = false;
        mode = CombatMode.None;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}