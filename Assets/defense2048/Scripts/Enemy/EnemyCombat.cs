using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    public Team team;

    public float detectRange = 1.8f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int damage = 1;

    private float lastAttackTime;
    private EnemyAnimation enemyAnim;
    private EnemyMove enemyMove;
    private DefenseHealth target;
    private Transform soldier;

    public CombatState state = CombatState.Walking;
    public bool IsLocked { get; private set; }
    //COMBAT SLOT
    private Transform[] meleeSlots = new Transform[2];

    private Vector3[] slotOffsets =
    {
        new Vector3(-0.6f, 0),
        new Vector3(0.6f, 0)
    };

    void Awake()
    {
        enemyAnim = GetComponent<EnemyAnimation>();
        enemyMove = GetComponent<EnemyMove>();
    }

    void Update()
    {
        HandleFacing();

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

        if (state == CombatState.Attacking && soldier == null && target == null)
        {
            ResetToWalking();
        }
    }
    // hàm lấy slot trống
    public int TryGetSlot(Transform soldier)
    {
        for (int i = 0; i < meleeSlots.Length; i++)
        {
            if (meleeSlots[i] == null)
            {
                meleeSlots[i] = soldier;
                return i;
            }
        }

        return -1;
    }
    // hàm giải phóng slot
    public void ReleaseSlot(Transform soldier)
    {
        for (int i = 0; i < meleeSlots.Length; i++)
        {
            if (meleeSlots[i] == soldier)
            {
                meleeSlots[i] = null;
                return;
            }
        }
    }
    // hàm lấy vị trí slot
    public Vector3 GetSlotPosition(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotOffsets.Length)
        {
            return transform.position;
        }
        return transform.position + slotOffsets[slotIndex];
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
    
    public void OnSoldierArrived(Transform soldierTf)
    {
        soldier = soldierTf;
        target = soldierTf.GetComponent<DefenseHealth>();

        IsLocked = true;
        state = CombatState.Attacking;

        enemyMove.StopMove();

        // EnemyShooter shooter = GetComponent<EnemyShooter>();
        // if (shooter != null) shooter.enabled = false;
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
        float closest = Mathf.Infinity;
        DefenseHealth best = null;
        
        foreach (var hit in hits)
        {
            DefenseHealth defense = hit.GetComponent<DefenseHealth>();
            if (defense == null)continue;

            float dist = Vector2.Distance(transform.position, defense.transform.position);

            if (dist < closest)
            {
                closest = dist;
                best = defense;
            }

        }

        return best;
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
        if (enemyMove != null)
            enemyMove?.ResumeMove();
    }

    public void PreLockTarget(Transform heroTf)
    {
        if (IsLocked) return;

        
        soldier = heroTf;
        state = CombatState.Waiting;


        if (enemyMove != null) enemyMove.StopMove();
        if (enemyAnim != null) enemyAnim.PlayIdle();
    }

    private void HandleFacing()
    {
        if (soldier != null)
        {
            FaceTarget(transform, soldier);
        }

        else if (state == CombatState.Walking && enemyMove != null)
        {
            if (enemyMove.pathPoints != null && enemyMove._currentIndex < enemyMove.pathPoints.Count)
            {
                Vector3 nextWaypoint = enemyMove.pathPoints[enemyMove._currentIndex].position;
                UpdateFacingByPoint(nextWaypoint);
            }
        }
    }

    public Transform GetCurrentTarget()
    {
        return soldier;
    }
    public bool IsInMelee()
    {
        return state == CombatState.Attacking;
    }


    private void UpdateFacingByPoint(Vector3 targetPoint)
    {
        Vector3 scale = transform.localScale;

        if (targetPoint.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }


    public static void FaceTarget(Transform self, Transform target)
    {
        if (!self || !target) return;

        float diff = target.position.x - self.position.x;
        if (Mathf.Abs(diff) < 0.05f) return;

        Vector3 scale = self.localScale;
        if (diff < 0) scale.x = Mathf.Abs(scale.x);
        else scale.x = -Mathf.Abs(scale.x);

        self.localScale = scale;
    }
    

}