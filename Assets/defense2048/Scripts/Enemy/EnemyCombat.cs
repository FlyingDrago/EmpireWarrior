using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Team team;

    public float detectRange = 1.8f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int damage = 1;

    private float lastAttackTime;
    private float moveDirX;

    private EnemyAnimation enemyAnim;
    private EnemyMove enemyMove;

    private DefenseHealth target;
    private Transform soldier;
    private EnemyHeath _enemyHeath;

    public CombatState state = CombatState.Walking;

    public bool HasTarget => target != null;
    public bool IsLocked { get; private set; }
    public bool IsInMeleeCombat => state == CombatState.Attacking;


    void Awake()
    {
        enemyAnim = GetComponent<EnemyAnimation>();
        enemyMove = GetComponent<EnemyMove>();
        _enemyHeath = GetComponent<EnemyHeath>();
    }

    void Update()
    {
        if (soldier != null)
            FaceTarget(transform, soldier);

        if (IsLocked)
        {
            if (soldier == null || !soldier.gameObject.activeInHierarchy)
            {
                ResetToWalking();
                return;
            }
        }
        switch (state)
        {
            case CombatState.Walking:
                DetectDefense();
                break;

            case CombatState.Waiting:
                // đứng im, chờ soldier
                break;


            case CombatState.Attacking:
                HandleAttack();
                break;
        }

        if (state != CombatState.Walking && target == null)
        {
            state = CombatState.Walking;
            IsLocked = false;
            ResetToWalking();
        }

    }

    public void OnDead()
    {
        if (target != null)
            target.Unlock();

        if (soldier != null)
        {
            var sc = soldier.GetComponent<SoldierCombat>();
            if (sc != null)
                sc.StopCombat();

         
        }

        soldier = null;
        target = null;
        IsLocked = false;
        state = CombatState.Walking;
        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null)
            shooter.enabled = true;


        enemyMove.ResumeMove();
    }


    void DetectDefense()
    {
        if (IsLocked) return;

        target = FindDefense();

        if (target != null && !target.IsBusy)
        {
            IsLocked = true;
            target.LockTarget(this);

            enemyMove.StopMove();
            state = CombatState.Waiting;
            enemyAnim.PlayIdle();
        }
    }


    // Bạn có thể tạo hàm này trong EnemyCombat để Hero gọi lúc bắt đầu đi tới
    public void LockByHero(Transform heroTf)
    {
        soldier = heroTf;
        target = heroTf.GetComponent<DefenseHealth>();
        IsLocked = true;
        state = CombatState.Waiting; // ĐỨNG ĐỢI, CHƯA ĐÁNH MELEE
        enemyMove.StopMove();
        enemyAnim.PlayIdle(); 
    }

// Hàm này sẽ được gọi từ HeroMove khi Hero đã đi đến nơi
    public void OnSoldierArrived(Transform soldierTf)
    {
        soldier = soldierTf;
        state = CombatState.Attacking; // BÂY GIỜ MỚI CHUYỂN SANG ATTACK
        IsLocked = true;

        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null) shooter.enabled = false; 
    }
 


    void HandleAttack()
    {
        if (soldier != null) FaceTarget(transform, soldier);
        if (enemyAnim.IsAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        enemyAnim.PlayAttack();
    }

    public void DealDamage()
    {
        if (target != null)
            target.TakeDamage(damage);
    }

    DefenseHealth FindDefense()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            detectRange,
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

    public void OnTargetDead() // Hoặc OnSoldierDead
    {
        // ... code cũ
        IsLocked = false;
        state = CombatState.Walking;

        // BẬT LẠI SHOOTER KHI HẾT MELEE
        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null) shooter.enabled = true; 

        enemyMove.ResumeMove();
    }
    public void OnSoldierDead()
    {
        soldier = null;
        IsLocked = false;
        state = CombatState.Walking;

        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null)
            shooter.enabled = true;

        enemyMove.ResumeMove();
    }

    private void ResetToWalking()
    {
        IsLocked = false;
        target = null;
        soldier = null;
        state = CombatState.Walking;

        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null) shooter.enabled = true;
        
        enemyMove?.ResumeMove();
    }
    // Gọi hàm này trong HeroCombat.TryFindEnemyImmediate thay vì OnSoldierArrived trực tiếp
    public void PreLockTarget(Transform heroTf)
    {
        IsLocked = true;
        soldier = heroTf;
        state = CombatState.Waiting; // Đứng chờ, không được di chuyển
        enemyMove.StopMove();
        enemyAnim.PlayIdle();
    }



    public static void FaceTarget(Transform self, Transform target)
    {
        if (!self || !target) return;

        Vector3 scale = self.localScale;

        if (self.position.x > target.position.x)
            scale.x = Mathf.Abs(scale.x); // quay sang phải
        else
            scale.x = -Mathf.Abs(scale.x); // quay sang trái

        self.localScale = scale;
    }
}