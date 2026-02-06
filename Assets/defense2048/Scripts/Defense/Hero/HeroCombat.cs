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
    public GameObject aoeEffectPrefab;
    public float effectDestroyTime = 0.5f;

    private float lastAttackTime;
    private EnemyCombat enemy;
    private bool isInPosition;

    private HeroAnimation heroAnim;
    public EnemyCombat GetCurrentEnemy() => enemy;

    private void Awake()
    {
        heroAnim = GetComponent<HeroAnimation>();
    }

    private void Update()
    {
        if (enemy != null)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                StopCombat();
            }
            else if (isInPosition) 
            {
               
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    heroAnim.PlayAttack();
                }
               
            }  
            return;
         
        }

        
        TryFindEnemyImmediate();
    }

    public bool TryFindEnemyImmediate()
    {
        if (enemy != null) return false;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);
        EnemyCombat best = null;
        float closest = Mathf.Infinity;

        foreach (var hit in hits)
        {
            EnemyCombat e = hit.GetComponent<EnemyCombat>();
            if (e == null || e.IsLocked) continue;

            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < closest) { closest = d; best = e; }
        }

        if (best != null)
        {
            enemy = best;

            float distToEnemy = Vector2.Distance(transform.position, best.transform.position);
            float combatThreshold = 1.1f;
            if (distToEnemy <= combatThreshold)
            {
                enemy.OnSoldierArrived(this.transform);
                SetInPosition(true);
            }
            else
            {
                enemy.PreLockTarget(this.transform);
                GetComponent<HeroMove>().MoveToEnemy(best.transform);
            }
            return true;
        }
        return false;
    }


    public void AnimDealDamage()
    {
        
        
        if (aoeEffectPrefab != null)
        {
            GameObject effect = Instantiate(aoeEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect,effectDestroyTime);
        }

        if (enemy != null)
        {
            enemy.GetComponent<EnemyHeath>()?.TakeDamage(damage);
        }
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            aoeRange,
            enemyLayer
        );

        bool currentEnemyStillAlive = false;

        foreach (var hit in hits)
        {
            EnemyHeath eh = hit.GetComponent<EnemyHeath>();
            if (eh != null && eh.gameObject.activeInHierarchy)
            {
                eh.TakeDamage(damage);

                if (enemy != null && eh.gameObject == enemy.gameObject)
                {
                    currentEnemyStillAlive = true;
                }
            }
           
        }

        if (enemy != null && !currentEnemyStillAlive)
        {
            StopCombat();
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
        
        heroAnim.PlayIdle();
    }
}