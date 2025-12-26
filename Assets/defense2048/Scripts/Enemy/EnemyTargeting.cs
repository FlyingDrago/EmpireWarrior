using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargeting : MonoBehaviour
{
    public float attackRange = 4f;
    public LayerMask defenseLayer;

    public Transform CurrentTarget { get; private set; }

    private void Update()
    {
        FindTarget();
    }

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, defenseLayer);

        if (hits.Length == 0)
        {
            CurrentTarget = null;
            return;
        }

        float minDist = Mathf.Infinity;
        Transform closet = null;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closet = hit.transform;
            }
            
        }

        CurrentTarget = closet;
    }
}