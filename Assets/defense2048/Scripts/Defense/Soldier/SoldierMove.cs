using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoldierMove : MonoBehaviour
{
    public float speed = 3f;
    public float combatOffset = 0.6f; 

    private Transform enemy;
    private bool isMoving;
    private bool hasTarget;

    private SoldierAnimation soldierAnim;

    private Transform tower;
    private Vector3 formationOffset;
    private bool returningToFormation;

    private void Awake()
    {
        soldierAnim = GetComponent<SoldierAnimation>();
    }

    void Update()
    {
        if (returningToFormation)
        {
            ReturnToFormation();
            return;
        }
        
        
        
        if(!isMoving)return;
        FaceTarget(transform, enemy);
        if (enemy == null)
        {
            ClearTarget();
            return;
        }

        // TÍNH VỊ TRÍ ĐỨNG NGANG
        Vector3 targetPos = enemy.position;

        // đứng lệch sang trái hoặc phải enemy
        if (transform.position.x < enemy.position.x)
            targetPos.x = enemy.position.x - combatOffset;
        else
            targetPos.x = enemy.position.x + combatOffset;

        // cùng hàng Y 
        targetPos.y = enemy.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // tới đúng vị trí chiến đấu
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            isMoving = false;
            soldierAnim.PlayIdle();

            var enemyCombat = enemy.GetComponent<EnemyCombat>();
            enemyCombat.OnSoldierArrived(transform);

            GetComponent<SoldierCombat>().SetInPosition(true);
        }

    }

    public void MoveToEnemy(Transform enemyTf)
    {
        enemy = enemyTf;
        hasTarget = true;
        isMoving = true;
        returningToFormation = false;

        if (soldierAnim != null)
            soldierAnim.PlayMove();
    }


    public void ClearTarget()
    {
        enemy = null;
        hasTarget = false;
        isMoving = false;
    
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

    public void SetFormaiton(Transform anchor, Vector3 offset)
    {
        tower = anchor;
        formationOffset = offset;
        
    }

    public void ReturnFormation()
    {
        if(tower==null)return;

        enemy = null;
        hasTarget = false;
        isMoving = false;
        returningToFormation = true;
        
        soldierAnim.PlayMove();
  

    }

    public void ReturnToFormation()
    {
        Vector3 targetPos = tower.position + formationOffset;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            returningToFormation = false;
            soldierAnim.PlayIdle();
        }
        

    }
    void OnEnable()
    {
        ResetState();
    }

    public void ResetState()
    {
        enemy = null;
        hasTarget = false;
        isMoving = false;
        returningToFormation = false;

        if (soldierAnim != null)
            soldierAnim.PlayIdle();
    }

}
