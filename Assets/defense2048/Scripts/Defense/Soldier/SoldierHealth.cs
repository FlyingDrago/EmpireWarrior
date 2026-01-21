using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierHealth : DefenseHealth
{
   public static event Action<SoldierHealth> OnAnySoldierDead; 
   
   public TowerFormation ownerTower;
   public Vector3 formationOffset;
   protected override void Die()
   {
       OnAnySoldierDead?.Invoke(this);
      if (ownerTower != null)
      {
          ownerTower.OnSoldierDead(this);
      }
      Destroy(gameObject);
   }
}
