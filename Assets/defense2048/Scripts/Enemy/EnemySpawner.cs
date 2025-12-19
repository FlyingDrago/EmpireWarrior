using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemySpawner : MonoBehaviour
{
   public List<Way> ways;
   

  public void SpawnEnemy(EnemyType enemyType)
   {
      GameObject enemy = EnemyPool.Instance.GetEnemy(enemyType.prefab);
      enemy.GetComponentInChildren<EnemyIdentily>().prefab = enemyType.prefab;
      Way way = ways[Random.Range(0, ways.Count)];

      EnemyMove move = enemy.GetComponentInChildren<EnemyMove>();
      EnemyHeath heath = enemy.GetComponentInChildren<EnemyHeath>();
      
      heath.Init(enemyType);
      move.Init(enemyType,way.points);
   }
}
