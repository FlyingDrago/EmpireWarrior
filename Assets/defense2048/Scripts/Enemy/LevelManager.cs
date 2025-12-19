using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
   public LevelData currentLevel;
   public EnemySpawner spawner;

   private void Start()
   {
      StartCoroutine(RunLevel());
   }

   IEnumerator RunLevel()
   {
      foreach (WaveData wave in currentLevel.waves)
      {
         yield return StartCoroutine(SpawnWave(wave));
         yield return new WaitForSeconds(2f);
      }
      Debug.Log("Level Compelete");
   }

   IEnumerator SpawnWave(WaveData wave)
   {
      foreach (EnemySpawnData enemyData in wave.enemies)
      {
         for (int i = 0; i < enemyData.count; i++)
         {
            spawner.SpawnEnemy(enemyData.EnemyType);
            yield return new WaitForSeconds(enemyData.spawnDelay);
         }
      }
   }
}
