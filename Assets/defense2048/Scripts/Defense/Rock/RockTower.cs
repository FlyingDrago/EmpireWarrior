using UnityEngine;

public class RockTower : MonoBehaviour
{
    [Header("Attack")] public float attackRange = 4f;
    public float attackCooldown = 2f;
    public LayerMask enemyLayer;

    [Header("Fire")] public Transform firePoint;
    public GameObject rockBulletPrefab;

    [Header("Animator")] public Animator animator;

    private Transform currentTarget;
    private float lastAttackTime;
    private bool isAttacking;

    private float currentAngle;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        FindTarget();

        if (currentTarget == null)
        {
            return;
        }

        float angle = CalculateAngle(currentTarget.position);

        if (Mathf.Abs(angle - currentAngle) > 0.1f)
        {
            currentAngle = angle;
            animator.SetFloat("angle", currentAngle);
        }

        TryAttack();
    }


    void FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);
        currentTarget = hit ? hit.transform : null;
    }


    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (isAttacking) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger("attack");
    }

    void UpdateAngle()
    {
        float angle = CalculateAngle(currentTarget.position);

        if (angle != currentAngle)
        {
            currentAngle = angle;
            animator.SetFloat("angle", currentAngle);
        }
    }


    float CalculateAngle(Vector3 targetPos)
    {
        Vector2 dir = targetPos - transform.position;

        float rawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // rawAngle: -180 → 180

        // QUANTIZE THEO KHOẢNG SET TRONG ANIMATOR
        if (rawAngle <= -120f) return -150f; // trái dưới sâu
        if (rawAngle <= -60f) return -90f; // trái dưới
        if (rawAngle <= 0f) return -30f; // phải dưới
        if (rawAngle <= 60f) return 30f; // phải
        if (rawAngle <= 120f) return 90f; // phải trên
        return 150f; // trái trên
    }


    public void ThrowRock()
    {
        if (!currentTarget) return;

        GameObject rock = Instantiate(
            rockBulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        rock.GetComponent<RockBullet>()?.Init(currentTarget);
    }


    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}