using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierHealth : DefenseHealth
{
   public TowerFormation ownerTower;
   public Vector3 formationOffset;
   protected override void Die()
   {
      if (ownerTower != null)
      {
         ownerTower.OnSoldierDead(this);
      }
      Destroy(gameObject);
   }
}
