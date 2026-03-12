using UnityEngine;

public class SoldierCombat : MonoBehaviour
{
    public float attackCooldown = 1.2f;
    public int damage = 1;
    public float detectRange = 2f;
    public LayerMask enemyLayer;

    private float lastAttackTime;
    [SerializeField]private bool isInPosition;

    private EnemyCombat enemy;
    private EnemyHeath enemyHealth;
    private EnemyShooter enemyShooter;

    private SoldierMove move;
    private SoldierAnimation anim;

    private float scanTimer;
    private const float SCAN_INTERVAL = 0.25f;

    void Awake()
    {
        move = GetComponent<SoldierMove>();
        anim = GetComponent<SoldierAnimation>();
    }

    void Update()
    {
        HandleCombat();
        ScanEnemy();
    }

    // =========================
    // COMBAT
    // =========================

    void HandleCombat()
    {
        if (enemy == null) return;

        if (!enemy.gameObject.activeInHierarchy)
        {
            StopCombat();
            return;
        }

        if (!isInPosition) return;
        if (enemyHealth == null) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.PlayAttack();
        }
    }

    public void AnimDealDamage()
    {
        Debug.Log("DEAL DAMAGE");
        if (enemyHealth == null) return;

        if (!enemyHealth.gameObject.activeInHierarchy) return;

        enemyHealth.TakeDamage(damage);
    }

    // =========================
    // TARGET FIND
    // =========================

    void ScanEnemy()
    {
        Debug.Log("SCAN ENEMY");
        if (enemy != null) return;
        if (move.IsMovingToEnemy) return;
        if (move.IsInCombat) return;

        scanTimer += Time.deltaTime;

        if (scanTimer < SCAN_INTERVAL) return;

        scanTimer = 0;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);

        float closest = Mathf.Infinity;
        EnemyCombat best = null;

        foreach (var hit in hits)
        {
            EnemyCombat e = hit.GetComponent<EnemyCombat>();
            if (e == null) continue;

            if (e.IsLocked && e.GetCurrentTarget() != transform)
                continue;

            float dist = Vector2.Distance(transform.position, e.transform.position);

            if (dist < closest)
            {
                closest = dist;
                best = e;
            }
        }

        if (best != null)
        {
            SetEnemy(best);
        }
    }

    // =========================
    // SET ENEMY
    // =========================

    void SetEnemy(EnemyCombat newEnemy)
    {
        enemy = newEnemy;

        enemyHealth = enemy.GetComponent<EnemyHeath>();
        enemyShooter = enemy.GetComponent<EnemyShooter>();

        isInPosition = false;

        if (enemyHealth != null)
            enemyHealth.OnEnemyDead += StopCombat;

        int slot = enemy.TryGetSlot(transform);
        if (slot == -1)
        {
            enemy = null;
            return;
        }
        enemy.PreLockTarget(transform);
        move.MoveToEnemy(enemy.transform,slot);
    }

    // =========================
    // POSITION
    // =========================

    public void SetInPosition(bool v)
    {
        Debug.Log(" SET IN POSITION" + v);
        isInPosition = v;

        if (!v || enemy == null) return;

        anim.PlayIdle();

        enemyShooter?.BeginMeleeFight();
        anim.PlayAttack();
        lastAttackTime = Time.time;
    }

    // =========================
    // STOP
    // =========================

    public void StopCombat()
    {
        if (enemyHealth != null)
            enemyHealth.OnEnemyDead -= StopCombat;

        if (enemyShooter != null)
            enemyShooter.ExitMelee();

        if (enemy != null && enemy.gameObject.activeInHierarchy)
            enemy.OnSoldierDead();

        enemy = null;
        enemyHealth = null;
        enemyShooter = null;

        isInPosition = false;
        lastAttackTime = 0;

        anim?.PlayIdle();

        move?.ReturnFormation();
    }

    // =========================
    // ENABLE / DISABLE
    // =========================

    void OnEnable()
    {
        enemy = null;
        enemyHealth = null;
        enemyShooter = null;

        isInPosition = false;
        lastAttackTime = 0;
        scanTimer = 0;
    }

    void OnDisable()
    {
        if (enemyHealth != null)
            enemyHealth.OnEnemyDead -= StopCombat;

        enemy = null;
    }
}