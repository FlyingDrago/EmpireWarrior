using UnityEngine;

public class SoldierMove : MonoBehaviour
{
    public float speed = 3f;
    public float combatOffset = 0.6f;

    private Transform enemy;
    private EnemyCombat enemyCombat;
    private EnemyShooter enemyShooter;

    private Transform formationAnchor;
    private Vector3 formationOffset;

    private SoldierAnimation anim;
    private SoldierCombat combat;

    private SoldierState state;

    private int combatSlot = -1;

    public bool IsMovingToEnemy => state == SoldierState.ToEnemy;
    public bool IsInCombat => state == SoldierState.InCombat;

    enum SoldierState
    {
        ToFormation,
        InFormation,
        ToEnemy,
        InCombat
    }

    void Awake()
    {
        anim = GetComponent<SoldierAnimation>();
        combat = GetComponent<SoldierCombat>();
    }

    void OnEnable()
    {
        enemy = null;
        enemyCombat = null;
        enemyShooter = null;

        state = SoldierState.ToFormation;

        if (combat != null)
        {
            combat.StopCombat();
        }

        anim?.PlayMove();
    }
    void Update()
    {
        switch (state)
        {
            case SoldierState.ToFormation:
                UpdateMoveToFormation();
                break;

            case SoldierState.ToEnemy:
                UpdateMoveToEnemy();
                break;
        }
    }

    // ================= FORMATION =================

    public void SetFormation(Transform anchor, Vector3 offset)
    {
        formationAnchor = anchor;
        formationOffset = offset;
    }

    void UpdateMoveToFormation()
    {
        if (!formationAnchor) return;

        Vector3 targetPos = formationAnchor.position + formationOffset;

        FaceMoveDirection(targetPos);
        Move(targetPos);

        if (Reached(targetPos))
        {
            state = SoldierState.InFormation;
            anim?.PlayIdle();
        }
    }

    public void ReturnFormation()
    {
        enemy = null;
        enemyCombat = null;
        enemyShooter = null;

        state = SoldierState.ToFormation;

        combat?.SetInPosition(false);
        anim?.PlayMove();
    }

    // ================= COMBAT =================

    public void MoveToEnemy(Transform enemyTf, int slot)
    {
        // if (state == SoldierState.ToEnemy || state == SoldierState.InCombat)
        //     return;
        if (slot < 0) return;
        enemy = enemyTf;


        enemyCombat = enemy.GetComponent<EnemyCombat>();
        enemyShooter = enemy.GetComponent<EnemyShooter>();


        state = SoldierState.ToEnemy;
        anim?.PlayMove();
    }

    void UpdateMoveToEnemy()
    {
        if (!enemy || !enemy.gameObject.activeInHierarchy)
        {
            ReturnFormation();
            return;
        }

        Vector3 targetPos = enemyCombat.GetSlotPosition(combatSlot);

        targetPos.x += transform.position.x < enemy.position.x
            ? -combatOffset
            : combatOffset;

        FaceMoveDirection(targetPos);
        Move(targetPos);

        float dist = Vector2.Distance(transform.position, enemy.position);

        if (dist <= combatOffset + 0.05f)
        {
            state = SoldierState.InCombat;
            anim?.PlayIdle();
            enemyCombat?.OnSoldierArrived(transform);
            combat?.SetInPosition(true);
        }
    }

    // ================= UTILS =================

    void Move(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    bool Reached(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) < 0.05f;
    }

    void FaceMoveDirection(Vector3 target)
    {
        float dx = target.x - transform.position.x;

        if (Mathf.Abs(dx) < 0.01f) return;

        Vector3 scale = transform.localScale;
        scale.x = dx > 0 ? -1 : 1;
        transform.localScale = scale;
    }
}