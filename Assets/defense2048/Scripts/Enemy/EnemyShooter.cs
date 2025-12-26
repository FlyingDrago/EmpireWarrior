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

    [SerializeField] private Transform currentTarget;
    [SerializeField] private float lastAttackTime;
    [SerializeField] private bool isAttacking = false;

    void Start()
    {
        // Tự động tìm Animator nếu chưa assign
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // 1. Tìm mục tiêu
        FindTarget();

        // 2. Nếu đang attack animation, không làm gì cả
        if (isAttacking) return;

        // 3. Nếu có mục tiêu
        if (currentTarget != null)
        {
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
            // Không có mục tiêu, chơi idle
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
        if (currentTarget.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x); // Quay phải
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x); // Quay trái
        }

        transform.localScale = scale;
    }

    void StartAttack()
    {
        Debug.Log("StartAttack");
        // Đánh dấu đang tấn công
        isAttacking = true;

        // Chơi animation attack
        PlayAnimation(attackAnimationName);

        // Reset thời gian tấn công
        lastAttackTime = Time.time;

        // Không cần gọi Shoot() ở đây, sẽ gọi qua Animation Event
    }

    // Hàm này sẽ được gọi từ Animation Event
    public void FireBullet()
    {
        if (currentTarget == null || bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Cannot fire: Missing components!");
            return;
        }

        Vector2 targetCenter = GetMarkerCenter(currentTarget);

        Vector2 direction = (targetCenter - (Vector2)firePoint.position).normalized;
        

        // 1. Tính hướng bắn
    

        // 2. Tạo đạn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 3. Thiết lập đạn
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

        // 4. Thiết lập sát thương
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);
        }

        // 5. Tự hủy đạn sau 3 giây
        Destroy(bullet, 3f);
    }
    Vector2 GetMarkerCenter(Transform target)
    {
        Transform centerMarker = target.Find("CenterMarker");
        if (centerMarker != null)
        {
            return centerMarker.position;
        }
        return target.position;
    }

    // Hàm này sẽ được gọi từ Animation Event khi animation kết thúc
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        Debug.Log("Attack animation finished");
    }

    void PlayAnimation(string animationName)
    {
        if (animator != null && !string.IsNullOrEmpty(animationName))
        {
            Debug.Log($"PlayAnimation {animationName}");
            animator.Play(animationName);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}