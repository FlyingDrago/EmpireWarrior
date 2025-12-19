using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierHealth : DefenseHealth
{
   protected override void Die()
   {
      Destroy(gameObject);
   }
}
