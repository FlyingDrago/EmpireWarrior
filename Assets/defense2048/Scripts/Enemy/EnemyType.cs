using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Enemy Type")]
public class EnemyType : ScriptableObject
{
   public string enemyName;
   public GameObject prefab;
   public int maxHp;
   public float speed;
}
