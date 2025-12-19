using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;
    public int count = 0;

    private Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        Instance = this;

    
    }

    public GameObject GetEnemy(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab)) pools[prefab] = new Queue<GameObject>();
        if (pools[prefab].Count == 0)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pools[prefab].Enqueue(obj);
        }

        GameObject enemy = pools[prefab].Dequeue();
        enemy.SetActive(true);
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        EnemyIdentily id = enemy.GetComponent<EnemyIdentily>();

        if (id == null || id.prefab == null)
        {
            Debug.LogError("Enemy identity missing or prefab null");
            Destroy(enemy);
            return;
        }
       enemy.SetActive(false);
       pools[id.prefab].Enqueue(enemy);
    }
}
