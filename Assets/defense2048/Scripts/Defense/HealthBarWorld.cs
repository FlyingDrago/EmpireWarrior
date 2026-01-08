using UnityEngine;

public class HealthBarWorld : MonoBehaviour
{
    [SerializeField] private Transform fillBar;
    [SerializeField] private DefenseHealth health;

    

    private Vector3 originalScale;

    void Awake()
    {
        if (health == null)
            health = GetComponentInParent<DefenseHealth>();
      

        originalScale = fillBar.localScale;
    }

    void OnEnable()
    {
     
        
        health.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(health.maxHp, health.maxHp);
    }

    void OnDisable()
    {
        if (health != null)
            health.OnHealthChanged -= UpdateHealthBar;
    
    }

    void Update()
    {
        SyncFlipWithParent();
    }

    void SyncFlipWithParent()
    {
        Transform parent = health.transform;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(parent.localScale.x) * Mathf.Abs(scale.x);
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