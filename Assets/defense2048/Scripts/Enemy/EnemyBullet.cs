using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 3f;

    [Header("Impact Effects")] 
    [SerializeField] private GameObject hitEffectPrefab;

    [SerializeField] private float effectLifetime = 1f;

    private void Start()
    {
        Destroy(gameObject,lifetime);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void SetEffect(GameObject hitEffect)
    {
        hitEffectPrefab = hitEffect;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu trúng Defense
        if (other.CompareTag("Defense"))
        {
            Debug.Log($"Bullet hit: {other.name}");

            // Gây sát thương
            DefenseHealth defenseHealth = other.GetComponent<DefenseHealth>();
            if (defenseHealth != null)
            {
                defenseHealth.TakeDamage(damage);
            }
            CreateHitEffect(hitEffectPrefab,other.ClosestPoint(transform.position));

            // Hủy đạn
            Destroy(gameObject);
        }
    }
    void CreateHitEffect(GameObject effectPrefab, Vector2 position)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            
            
            Destroy(effect, effectLifetime);
            
            // Xoay effect theo hướng bắn (nếu cần)
            // effect.transform.up = GetComponent<Rigidbody2D>().velocity.normalized;
        }
        else
        {
            Debug.LogWarning("Hit effect prefab is not assigned!");
        }
    }
    
}