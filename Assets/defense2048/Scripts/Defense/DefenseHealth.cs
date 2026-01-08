using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefenseHealth : MonoBehaviour
{
    public int maxHp = 10;
    protected int currentHp;
    public bool IsBusy { get; private set; }
    private EnemyCombat lockedEnemy;
    public event Action<int, int> OnHealthChanged; 
        

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        
        OnHealthChanged?.Invoke(currentHp,maxHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }
    public void OnEnemyDetected(EnemyCombat enemy)
    {
        GetComponent<SoldierMove>().MoveToEnemy(enemy.transform);
    }
    public void LockTarget(EnemyCombat enemy)
    {
        if (IsBusy) return;

        IsBusy = true;
        lockedEnemy = enemy;

        GetComponent<SoldierMove>().MoveToEnemy(enemy.transform);
    }
    public void Unlock()
    {
        IsBusy = false;
        lockedEnemy = null;
    }
    protected abstract void Die();
}
