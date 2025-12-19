using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Team team;

    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int damage = 1;

    private float lastAttackTime;
    private EnemyAnimation enemyAnim;

    void Awake()
    {
        enemyAnim = GetComponent<EnemyAnimation>();
    }

    void Update()
    {
        if (enemyAnim.IsAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        DefenseHealth target = FindDefense();

        if (target != null)
        {
            lastAttackTime = Time.time;
            enemyAnim.PlayAttack();
        }
    }

    // GỌI BẰNG ANIMATION EVENT
    public void DealDamage()
    {
        DefenseHealth target = FindDefense();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }

    DefenseHealth FindDefense()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            LayerMask.GetMask("Defense")
        );

        foreach (var hit in hits)
        {
            DefenseHealth defense = hit.GetComponent<DefenseHealth>();
            if (defense != null)
                return defense;
        }

        return null;
    }
}