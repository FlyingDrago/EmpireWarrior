using UnityEngine;

public class EnemyHealthBarWorld : MonoBehaviour
{
    [SerializeField] private Transform fillBar;
    [SerializeField] private EnemyHeath health;
   

    private Vector3 originalScale;

    void Awake()
    {
        if (health == null)
            health = GetComponentInParent<EnemyHeath>();

        originalScale = fillBar.localScale;
    }

    void OnEnable()
    {
        health.EnemyOnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(health.maxHp, health.maxHp);
    }

    void OnDisable()
    {
        if (health != null)
            health.EnemyOnHealthChanged -= UpdateHealthBar;
    }

    void Update()
    {
        SyncFlipWithEnemy();
    }

    void SyncFlipWithEnemy()
    {
        Transform enemy = health.transform;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(enemy.localScale.x) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void UpdateHealthBar(int current, int max)
    {
        float percent = (float)current / max;

        Vector3 scale = originalScale;
        scale.x = originalScale.x * percent;
        fillBar.localScale = scale;

    
    }
}