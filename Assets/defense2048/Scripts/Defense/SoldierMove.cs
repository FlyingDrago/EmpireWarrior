using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoldierMove : MonoBehaviour
{
    public float speed = 3f;
    public float combatOffset = 0.6f; // khoảng cách đứng ngang

    private Transform enemy;
    private bool isMoving;
    private bool hasTarget;

    void Update()
    {
        if (!isMoving || enemy == null) return;
        FaceTarget(transform, enemy);
        if (hasTarget && enemy == null)
        {
            ClearTarget();
        }

        // 🔥 TÍNH VỊ TRÍ ĐỨNG NGANG
        Vector3 targetPos = enemy.position;

        // đứng lệch sang trái hoặc phải enemy
        if (transform.position.x < enemy.position.x)
            targetPos.x = enemy.position.x - combatOffset;
        else
            targetPos.x = enemy.position.x + combatOffset;

        // cùng hàng Y (2D nhìn đẹp)
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
            enemy.GetComponent<EnemyCombat>().OnSoldierArrived(transform);
        }
    }

    public void MoveToEnemy(Transform enemyTf)
    {
        if (hasTarget) return;

        enemy = enemyTf;
        hasTarget = true;
        isMoving = true;
    }

    public void ClearTarget()
    {
        enemy = null;
        hasTarget = false;
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
