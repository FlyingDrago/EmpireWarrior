using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefenseHealth : MonoBehaviour
{
   public int maxHp = 10;
   protected int currentHp;

   protected void Awake()
   {
      currentHp = maxHp;
   }

   public virtual void TakeDamage(int damage)
   {
      currentHp -= damage;

      if (currentHp <= 0)
      {
         Die();
      }
   }

   protected abstract void Die();
}
