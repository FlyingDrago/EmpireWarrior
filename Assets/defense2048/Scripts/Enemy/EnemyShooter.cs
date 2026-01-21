using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    [Header("Ranged Settings")] public float attackRange = 5f;
    public float attackCooldown = 2f;
    public int damage = 1;
    public LayerMask targetLayer;

    [Header("Bullet")] public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public GameObject hitEffectPrefab;

    [SerializeField] private Transform currentTarget;
    private float lastAttackTime;
    private bool isAttacking;

    private EnemyMove enemyMove;
    private EnemyCombat enemyCombat;
    private EnemyAnimation enemyAnimation;
    private int currentTargetID = -1;


    public bool IsAttacking => isAttacking;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
        enemyCombat = GetComponent<EnemyCombat>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    void Update()
    {
    
        if (enemyCombat != null && enemyCombat.IsInMeleeCombat)
            return;
        if (isAttacking) return;
        FindTarget();

        if (currentTarget == null)
        {
            enemyMove?.ResumeMove();
            return;
        }

      

        enemyMove?.StopMove();
        FaceTarget();

        if (!isAttacking && Time.time - lastAttackTime >= attackCooldown && currentTarget != null)
        {
            StartRangedAttack();
        }
    }

    void StartRangedAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        enemyAnimation.PlayAttack2();
    }


    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            targetLayer
        );

        float closest = Mathf.Infinity;
        Transform target = null;

        foreach (var hit in hits)
        {
            if (!hit) continue;
            if (!hit.gameObject.activeInHierarchy) continue;

            SoldierHealth health = hit.GetComponent<SoldierHealth>();
            if (health == null) continue;

            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d < closest)
            {
                closest = d;
                target = hit.transform;
            }
        }

        
        if (target != null)
        {
            currentTarget = target;
            currentTargetID = target.GetInstanceID();
        }
        else
        {
            currentTarget = null;
            currentTargetID = -1;
        }
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


    public void FireBullet()
    {
        if (currentTarget == null) return;
        if (!currentTarget.gameObject.activeInHierarchy) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.Init(enemyCombat, currentTarget, damage, bulletSpeed);
            bullet.SetHitEffect(hitEffectPrefab);
        }
    }


    public void OnAttackAnimationEnd()
    {
        Debug.Log("Reset");
        isAttacking = false;
        if (enemyAnimation != null)
            enemyAnimation.OnAttackEnd();
    }

    void OnEnable()
    {
        SoldierHealth.OnAnySoldierDead += OnSoldierDead;
    }

    void OnDisable()
    {
        SoldierHealth.OnAnySoldierDead -= OnSoldierDead;
    }

    void OnSoldierDead(SoldierHealth deadSoldier)
    {
        if (currentTarget == null) return;

        if (deadSoldier.GetInstanceID() == currentTargetID)
        {
            currentTarget = null;
            currentTargetID = -1;
            isAttacking = false;
            lastAttackTime = 0;

            enemyMove?.ResumeMove();
            enemyAnimation?.OnAttackEnd();
        }
    }


    public void SetAttacking(bool value)
    {
        isAttacking = value;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}