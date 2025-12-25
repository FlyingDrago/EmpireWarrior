using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeath : MonoBehaviour
{
    public int maxHp = 100;
    private int _currentHp;
    private EnemyCombat Combat;

    private EnemyMove EnemyMove;

    private void Awake()
    {
        EnemyMove = GetComponent<EnemyMove>();
        Combat = GetComponent<EnemyCombat>();
    }

    void OnEnable()
    {
        _currentHp = maxHp;
    }
    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        if (_currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (Combat != null)
        {
           
            Combat.OnDead();
        }
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
    

    public void Init(EnemyType data)
    {
        maxHp = data.maxHp;
        _currentHp = maxHp;
    }
}