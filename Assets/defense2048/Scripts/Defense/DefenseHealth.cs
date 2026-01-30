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
      

        IsBusy = true;
        lockedEnemy = enemy;

      if(enemy==null)return;

      HeroMove hMove = GetComponent<HeroMove>();
      if (hMove != null)
      {
          hMove.MoveToEnemy(enemy.transform);
          return;
      }

      SoldierMove sMove = GetComponent<SoldierMove>();
      if (sMove != null)
      {
          sMove.MoveToEnemy(enemy.transform);
      }
    }
    public void HeroLockTarget(EnemyCombat enemy)
    {
        IsBusy = true;
        lockedEnemy = enemy;
        // Không gọi SoldierMove ở đây nữa nếu là Hero
        GetComponent<HeroMove>()?.MoveToEnemy(enemy.transform);
    }
    public void Unlock()
    {
        IsBusy = false;
        lockedEnemy = null;
    }
    protected abstract void Die();
}
