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
    public event Action<int, int> EnemyOnHealthChanged;
    public event Action OnEnemyDead; 

    private void Awake()
    {
        EnemyMove = GetComponent<EnemyMove>();
        Combat = GetComponent<EnemyCombat>();
    }

    void OnEnable()
    {
        _currentHp = maxHp;
        EnemyOnHealthChanged?.Invoke(_currentHp,maxHp);
    }
    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Clamp(_currentHp, 0, maxHp);

   
        EnemyOnHealthChanged?.Invoke(_currentHp, maxHp);
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
        OnEnemyDead?.Invoke();
        OnEnemyDead = null;
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
    

    public void Init(EnemyType data)
    {
        maxHp = data.maxHp;
        _currentHp = maxHp;
        EnemyOnHealthChanged?.Invoke(_currentHp,maxHp);
    }
}