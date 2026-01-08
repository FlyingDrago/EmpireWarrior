using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBullet : MonoBehaviour
{
    [Header("Damage")] public int damage = 1;

    [Header("Parabola Settings")] public float flightTime = 0.8f;
    public float arcHeight = 3f;

    public GameObject hitEffectPrefab;
    public GameObject hitTextEffect;
    private Transform target;
    private Vector3 startPos;
    private float timer;


    public void Init(Transform target)
    {
        this.target = target;
        startPos = transform.position;
        timer = 0f;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flightTime);

        Vector3 currentPos = CalculatePosition(t);
        Vector3 nestPos = CalculatePosition(Mathf.Min(1f, t + 0.02f));

        transform.position = currentPos;

        RotateAlongPath(currentPos, nestPos);

        if (t >= 1f)
        {
            HitTarget();
        }
    }

    Vector3 CalculatePosition(float t)
    {
        Vector3 pos = Vector3.Lerp(startPos, target.position, t);

        pos.y += arcHeight * Mathf.Sin(Mathf.PI * t);

        return pos;
    }

    void RotateAlongPath(Vector3 current, Vector3 next)
    {
        Vector3 dir = next - current;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HitTarget()
    {
        if (target != null)
        {
            target.GetComponent<EnemyHeath>()?.TakeDamage(damage);
        }

        if (hitEffectPrefab)
        {
            GameObject fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        if (hitTextEffect)
        {
            Vector3 offset = new Vector3(0f, 0.6f, 0f);
            GameObject text = Instantiate(hitTextEffect, transform.position+offset, Quaternion.identity);
            Destroy(text, 1f);
        }

        Destroy(gameObject);
    }
}