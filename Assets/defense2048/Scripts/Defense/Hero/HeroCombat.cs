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
        // Nếu ĐÃ có enemy, chỉ kiểm tra xem nó chết chưa
        if (enemy != null)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                StopCombat(); // Chỉ reset khi enemy chết
            }
            else if (isInPosition) 
            {
                // Tấn công nếu đã đứng đúng vị trí
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    heroAnim.PlayAttack();
                }
            }
            return; // ĐÃ CÓ TARGET THÌ KHÔNG TÌM NỮA
        }

        // Nếu CHƯA có enemy, mới đi tìm
        TryFindEnemyImmediate();
    }

    public bool TryFindEnemyImmediate()
    {
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
            enemy.PreLockTarget(this.transform);
            
            GetComponent<HeroMove>().MoveToEnemy(best.transform);
        
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
        lastAttackTime = 0;
        heroAnim.PlayIdle();
    }
}