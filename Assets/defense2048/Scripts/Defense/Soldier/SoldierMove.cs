using UnityEngine;

public class SoldierMove : MonoBehaviour
{
    public float speed = 3f;
    public float combatOffset = 0.6f;

    private Transform enemy;
    private Transform formationAnchor;
    private Vector3 formationOffset;

    private SoldierAnimation anim;

    private SoldierState state;
    public bool IsMovingToEnemy => state == SoldierState.ToEnemy;
    public bool IsInCombat => state == SoldierState.InCombat;


    enum SoldierState
    {
        ToFormation,
        InFormation,
        ToEnemy,
        InCombat
    }

    private void Awake()
    {
        anim = GetComponent<SoldierAnimation>();
    }

    private void OnEnable()
    {
        state = SoldierState.ToFormation;
        anim?.PlayMove();
    }

    private void Update()
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
        if (formationAnchor == null) return;

        Vector3 targetPos = formationAnchor.position + formationOffset;

        FaceMoveDirection(targetPos); // 🔥 FLIP THEO HƯỚNG DI CHUYỂN
        Move(targetPos);

        if (Reached(targetPos))
        {
            state = SoldierState.InFormation;
            anim?.PlayIdle();
        }
    }

    public void ReturnFormation()
    {
        state = SoldierState.ToFormation;
        enemy = null;
        GetComponent<SoldierCombat>()?.SetInPosition(false);
        anim?.PlayMove();
    }

    // ================= COMBAT =================

    public void MoveToEnemy(Transform enemyTf)
    {
        if(state==SoldierState.ToEnemy|| state==SoldierState.InCombat)return;
        
        enemy = enemyTf;
        state = SoldierState.ToEnemy;
        anim?.PlayMove();
    }

    void UpdateMoveToEnemy()
    {
        if (enemy == null)
        {
            ReturnFormation();
            return;
        }

        Vector3 targetPos = enemy.position;
        targetPos.x += transform.position.x < enemy.position.x
            ? -combatOffset
            : combatOffset;

        FaceMoveDirection(targetPos); // 🔥
        Move(targetPos);

        if (Reached(targetPos))
        {
            // anim?.PlayIdle();
            state = SoldierState.InCombat;

            var ec = enemy.GetComponent<EnemyCombat>();
            ec?.OnSoldierArrived(transform);


            var shooter = enemy.GetComponent<EnemyShooter>();
            shooter?.BeginMeleeFight();

            GetComponent<SoldierCombat>()
                ?.SetInPosition(true);
            
            return;
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

    public static void FaceTarget(Transform self, Transform target)
    {
        if (!self || !target) return;

        Vector3 scale = self.localScale;
        scale.x = self.position.x > target.position.x
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        self.localScale = scale;
    }
    void FaceMoveDirection(Vector3 target)
    {
        float dx = target.x - transform.position.x;

        if (Mathf.Abs(dx) < 0.01f) return;

        Vector3 scale = transform.localScale;
        scale.x = dx > 0
            ? -Mathf.Abs(scale.x)   // đi sang phải → quay phải
            : Mathf.Abs(scale.x);   // đi sang trái → quay trái

        transform.localScale = scale;
    }

}
