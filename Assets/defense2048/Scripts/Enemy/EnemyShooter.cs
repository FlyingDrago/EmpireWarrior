using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    public System.Action<EnemyShooter> OnMeleeExit;
    enum CombatMode
    {
        None,
        Ranged,
        Melee_Wait,
        Melee_Fight
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
    public float meleeSpeed = 3f;

    private Transform currentTarget;
    private int currentTargetID = -1;

    private CombatMode mode = CombatMode.None;
    private float lastAttackTime;
    private bool isAttacking;

    private EnemyMove enemyMove;
    private EnemyAnimation enemyAnimation;

    public bool IsInMelee => mode == CombatMode.Melee_Fight;
    

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    void Update()
    {
        if (mode == CombatMode.Melee_Wait)
        {
            if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            {
                ExitMelee();
                return;
            }
            FaceTarget();
            return;
        }

        if (mode == CombatMode.Melee_Fight)
        {
            if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            {
                ExitMelee();
                return;
            }
            FaceTarget();

            if (!isAttacking &&
                Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                isAttacking = true;
                enemyAnimation.PlayAttack();
            }

            return;
        }


        if (isAttacking) return;

        FindTarget();

        if (currentTarget == null)
        {
            enemyMove?.ResumeMove();
            return;
        }

        enemyMove?.StopMove();
        FaceTarget();

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            StartRangedAttack();
        }
    }

    // =========================
    // RANGED
    // =========================

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
        enemyAnimation.OnAttackEnd();
    }

    // =========================
    // MELEE MODE
    // =========================

    public void EnterMelee(Transform attacker)
    {
        currentTarget = attacker;
        currentTargetID = attacker.GetInstanceID();

        mode = CombatMode.Melee_Wait;

        enemyMove?.StopMove();
        isAttacking = false;
    }
    public void BeginMeleeFight()
    {
        if (mode != CombatMode.Melee_Wait) return;

        mode = CombatMode.Melee_Fight;
        lastAttackTime = 0;
    }



    void HandleMelee()
    {
        if (currentTarget == null)
        {
            ExitMelee();
            return;
        }

        FaceTarget();

        Vector2 dir =
            (currentTarget.position - transform.position).normalized;

        Vector2 targetPos =
            (Vector2)currentTarget.position - dir * meleeOffset;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            meleeSpeed * Time.deltaTime);

        float dist = Vector2.Distance(transform.position, targetPos);

        if (dist < 0.05f)
        {
            enemyAnimation.PlayAttack(); // melee attack loop
        }
    }

   public void ExitMelee()
    {
        var self = this;
        
        mode = CombatMode.None;
        currentTarget = null;
        currentTargetID = -1;
        isAttacking = false;

        enemyAnimation.OnAttackEnd();
        enemyMove?.ResumeMove();
        
        OnMeleeExit?.Invoke(self);
    }

    // =========================
    // TARGET FIND
    // =========================

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            targetLayer);

        float closest = Mathf.Infinity;
        Transform target = null;

        foreach (var hit in hits)
        {
            if (!hit || !hit.gameObject.activeInHierarchy) continue;

            var s = hit.GetComponent<SoldierHealth>();
            var d = hit.GetComponent<DefenseHealth>();

            if (s == null && d == null) continue;

            float dist =
                Vector2.Distance(transform.position, hit.transform.position);

            if (dist < closest)
            {
                closest = dist;
                target = hit.transform;
            }
        }

        currentTarget = target;
        currentTargetID = target ? target.GetInstanceID() : -1;
    }

    void FaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 scale = transform.localScale;
        scale.x = currentTarget.position.x < transform.position.x
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
