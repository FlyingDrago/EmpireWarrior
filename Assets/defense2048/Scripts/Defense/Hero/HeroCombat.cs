using UnityEngine;

public class HeroCombat : MonoBehaviour
{
    [Header("Combat")]
    public float attackCooldown = 1.2f;
    public int damage = 2;
    public float detectRange = 2.5f;
    public LayerMask enemyLayer;

    [Header("AOE")]
    public float aoeRange = 1.5f;

    private float lastAttackTime;
    private EnemyCombat enemy;
    private bool isInPosition;

    private HeroAnimation heroAnim;

    private void Awake()
    {
        heroAnim = GetComponent<HeroAnimation>();
    }

    private void Update()
    {
        if (enemy == null)
        {
            TryFindEnemyImmediate();
                return;
        }
        if(!isInPosition)return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        heroAnim.PlayAttack();
    }

    public bool TryFindEnemyImmediate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);

        float closest = Mathf.Infinity;
        EnemyCombat best = null;

        foreach (var hit in hits)
        {
            EnemyCombat e = hit.GetComponent<EnemyCombat>();
            if (e == null || e.IsLocked) continue;

            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < closest)
            {
                closest = d;
                best = e;
            }
        }

        if (best != null)
        {
            enemy = best;
            GetComponent<HeroMove>().MoveToEnemy(best.transform);
            return true;
        }

        return false;
    }


    public void AnimDealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            aoeRange,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            EnemyHeath eh = hit.GetComponent<EnemyHeath>();
            if (eh != null && eh.gameObject.activeInHierarchy)
            {
                eh.TakeDamage(damage);
            }
            else
            {
                StopCombat();
            }
        }
    }

    public void SetInPosition(bool v)
    {
        isInPosition = v;
    }

    public void StopCombat()
    {
        enemy = null;
        isInPosition = false;
        lastAttackTime = 0;
        heroAnim.PlayIdle();
    }
   
}
