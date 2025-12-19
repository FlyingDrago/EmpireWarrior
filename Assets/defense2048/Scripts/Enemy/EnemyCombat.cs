using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Team team;

    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int damage = 1;

    private float lastAttackTime;
    private EnemyAnimation enemyAnim;
    private EnemyMove enemyMove;

    public bool HasTarget { get; private set; }

    void Awake()
    {
        enemyAnim = GetComponent<EnemyAnimation>();
        enemyMove = GetComponent<EnemyMove>();
    }

    void Update()
    {
        if (enemyAnim.IsAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        DefenseHealth target = FindDefense();

        if (target != null)
        {
            HasTarget = true;
            enemyMove.StopMove();

            lastAttackTime = Time.time;
            enemyAnim.PlayAttack();
        }
        else
        {
            HasTarget = false;
                        enemyMove.ResumeMove();
        }
    }


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