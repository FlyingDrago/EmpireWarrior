using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private Collider2D[] _collider2Ds;
    [Header("Attack Settings")]
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public int damage = 1;

    [Header("Bullet Settings")] 
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;

    [Header("Target Settings")] 
    public LayerMask targetLayer;

    [Header("Animation")] 
    public Animator animator;
    public string attackAnimationName = "attack_left_2";
    public string idleAnimationName = "idle";
    [Header("Bullet Effect")]
    public GameObject hitEffectPrefab;


    [SerializeField] private Transform currentTarget;
    [SerializeField] private float lastAttackTime;
    [SerializeField] private bool isAttacking = false;

    private EnemyMove EnemyMove;
    private EnemyCombat EnemyCombat;

    
    void Start()
    {
        EnemyMove = GetComponent<EnemyMove>();
        EnemyCombat = GetComponent<EnemyCombat>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (EnemyCombat != null && EnemyCombat.IsInMeleeCombat)
        {
            isAttacking = false;
            currentTarget = null;
            return;
        }
        FindTarget();

        // 2. Nếu đang attack animation, không làm gì cả
        if (isAttacking) return;

        // 3. Nếu có mục tiêu
        if (currentTarget != null)
        {
            EnemyMove?.StopMove();
            
            FaceTarget();

            // Chơi animation Idle khi chờ attack
            PlayAnimation(idleAnimationName);

            // Kiểm tra cooldown để tấn công
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                StartAttack();
            }
        }
        else
        {
            // Không có mục tiêu, chơi idle--Resume move
            EnemyMove?.ResumeMove();
            PlayAnimation(idleAnimationName);
        }
    }

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);
        _collider2Ds = hits;

        if (hits.Length == 0)
        {
            currentTarget = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider2D hit in hits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        currentTarget = closestTarget;
    }

    void FaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 scale = transform.localScale;
        if (currentTarget.position.x < transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x); 
        }

        transform.localScale = scale;
    }

    void StartAttack()
    {
        Debug.Log("StartAttack");
       
        isAttacking = true;

    
        PlayAnimation(attackAnimationName);

      
        lastAttackTime = Time.time;

     
    }

  
    public void FireBullet()
    {
        if (currentTarget == null || bulletPrefab == null || firePoint == null)
            return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.Init(currentTarget, damage, bulletSpeed);
            bullet.SetHitEffect(hitEffectPrefab);
        }
    }
  


    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        if (currentTarget == null)
        {
            EnemyMove?.ResumeMove();
        }
    }

    void PlayAnimation(string animationName)
    {
        if (animator != null && !string.IsNullOrEmpty(animationName))
        {
      
            animator.Play(animationName);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}