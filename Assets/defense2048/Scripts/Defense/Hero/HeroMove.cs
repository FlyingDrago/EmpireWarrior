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

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
        float dirX = targetPos.x - transform.position.x;
        if (Mathf.Abs(dirX) > 0.01f)
        {
            spriteRenderer.flipX = dirX > 0;
        }
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            moving = false;
            GetComponent<HeroCombat>()?.SetInPosition(true);
            heroAnimation.PlayIdle();
        }
    }

    public void MoveToPosition(Vector3 pos)
    {
        targetPos = pos;
        targetPos.z = transform.position.z;
        moving = true;
        GetComponent<HeroCombat>()?.SetInPosition(false);
        heroAnimation.PlayMove();
       
    }

    public void MoveToEnemy(Transform enemy)
    {
        targetPos = enemy.position;
        targetPos.z = transform.position.z;
        moving = true;
        GetComponent<HeroCombat>()?.SetInPosition(false);
        heroAnimation.PlayMove();
    }
}