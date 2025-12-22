using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Team team;

    public float detectRange = 1.8f;
    public float attackRange = 1.2f;
    public float alignOffset = 0.5f;
    public float attackCooldown = 1.5f;
    public int damage = 1;

    private float lastAttackTime;

    private EnemyAnimation enemyAnim;
    private EnemyMove enemyMove;

    private DefenseHealth target;
    private Transform soldier;

    public CombatState state = CombatState.Walking;

    public bool HasTarget => target != null;
    public bool IsLocked { get; private set; }

    void Awake()
    {
        enemyAnim = GetComponent<EnemyAnimation>();
        enemyMove = GetComponent<EnemyMove>();
    }

    void Update()
    {
        if (soldier != null)
            FaceTarget(transform, soldier);
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
            if (soldier != null)
                soldier.GetComponent<SoldierMove>()?.ClearTarget();

            state = CombatState.Walking;
            IsLocked = false;
            enemyMove.ResumeMove();
        }
    }
    public void OnDead()
    {
        if (soldier != null)
        {
            SoldierMove sm = soldier.GetComponent<SoldierMove>();
            if (sm != null)
                sm.ClearTarget();
        }

        soldier = null;
        target = null;
        IsLocked = false;

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
        }
    }

    public void OnSoldierArrived(Transform soldierTf)
    {
        soldier = soldierTf;
        state = CombatState.Attacking; 
    }

      
    

    void HandleAttack()
    {
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

    public void OnTargetDead()
    {
        IsLocked = false;
        target = null;

        state = CombatState.Walking;
        enemyMove.ResumeMove();
    }
    public static void FaceTarget(Transform self, Transform target)
    {
        if (!self || !target) return;

        Vector3 scale = self.localScale;

        if (self.position.x > target.position.x)
            scale.x = Mathf.Abs(scale.x);   // quay sang phải
        else
            scale.x = -Mathf.Abs(scale.x);  // quay sang trái

        self.localScale = scale;
    }



}