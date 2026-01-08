using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Transform target;
    private int damage;
    private float speed;

    [Header("Hit Settings")]
    [SerializeField] private float hitDistance = 0.1f;

    [Header("Effect")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float effectLifetime = 1f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5f;

    private EnemyCombat attacker;
    

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // 🔹 Init khi bắn
    

    public void Init(EnemyCombat attacker, Transform target, int damage, float speed)
    {
        this.attacker = attacker;
        this.target = target;
        this.damage = damage;
        this.speed = speed;
    }


    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 targetPos = GetTargetCenter(target);

        // Bay tới mục tiêu
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Check trúng
        if (Vector2.Distance(transform.position, targetPos) <= hitDistance)
        {
            HitTarget(targetPos);
        }
        
    }

    void HitTarget(Vector2 hitPosition)
    {
        DefenseHealth defenseHealth = target.GetComponentInParent<DefenseHealth>();

        if (defenseHealth != null)
        {
            defenseHealth.TakeDamage(damage);
        }

        SpawnHitEffect(hitPosition);
        Destroy(gameObject);
    }


    void SpawnHitEffect(Vector2 position)
    {
        if (hitEffectPrefab == null) return;

        GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(effect, effectLifetime);
    }

    Vector2 GetTargetCenter(Transform target)
    {
        Transform marker = target.Find("CenterMarker");
        if (marker != null)
            return marker.position;

        return target.position;
    }
    public void SetHitEffect(GameObject effect)
    {
        hitEffectPrefab = effect;
    }
}
