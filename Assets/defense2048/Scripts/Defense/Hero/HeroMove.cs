using System;
using UnityEngine;

public class HeroMove : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Vector3 targetPos;
    private bool moving;
    private HeroAnimation heroAnimation;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        heroAnimation = GetComponent<HeroAnimation>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

  

    void Update()
    {
        if (!moving) return;

        // Di chuyển mượt mà tới điểm mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Lật mặt Sprite chuẩn xác
        float diffX = targetPos.x - transform.position.x;
        if (Mathf.Abs(diffX) > 0.01f)
        {
            spriteRenderer.flipX = diffX > 0;
        }

        // Kiểm tra khoảng cách cực nhỏ để dừng lại (dùng sqrMagnitude để tối ưu hiệu năng)
        if ((transform.position - targetPos).sqrMagnitude < 0.001f)
        {
            transform.position = targetPos; // Khớp vị trí tuyệt đối
            moving = false;

            // Kích hoạt trạng thái Melee cho Enemy
            var enemyScript = GetComponent<HeroCombat>()?.GetCurrentEnemy();
            if (enemyScript != null)
            {
                enemyScript.OnSoldierArrived(this.transform);
            }

            GetComponent<HeroCombat>()?.SetInPosition(true);
            heroAnimation.PlayIdle();
        }
    }
    public void MoveToEnemy(Transform enemy)
    {
        // Khoảng cách ngang chuẩn 1 unit
        float combatDistanceX = 1.0f; 

        // Xác định hướng đứng dựa trên vị trí X
        float direction = (transform.position.x < enemy.position.x) ? -1f : 1f;

        // ÉP BUỘC: Target Y phải bằng đúng Enemy Y
        // Trục Z giữ nguyên theo Hero để tránh lỗi hiển thị Layer
        targetPos = new Vector3(enemy.position.x + (direction * combatDistanceX), 
            enemy.position.y, 
            transform.position.z);
    
        moving = true;
        GetComponent<HeroCombat>()?.SetInPosition(false);
        heroAnimation.PlayMove();
    }
    public void MoveToPosition(Vector3 pos)
    {
        targetPos = pos;
        targetPos.z = transform.position.z;
        moving = true;
        GetComponent<HeroCombat>()?.SetInPosition(false);
        heroAnimation.PlayMove();
       
    }
}