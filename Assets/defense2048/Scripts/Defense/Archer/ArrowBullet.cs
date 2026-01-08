using UnityEngine;

public class ArrowBullet : MonoBehaviour
{
    public int damage = 1;
    public float flightTime = 0.6f;
    public float arcHeight = 2.5f;

    private Transform target;
    private Vector3 startPos;
    private float timer;

    public void Init(Transform target)
    {
        this.target = target;
        startPos = transform.position;
        timer = 0f;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flightTime);

        Vector3 currentPos = CalculatePosition(t);
        Vector3 nextPos = CalculatePosition(Mathf.Min(1f, t + 0.02f));

        transform.position = currentPos;

        RotateAlongPath(currentPos, nextPos);

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
        target.GetComponent<EnemyHeath>()?.TakeDamage(damage);
        Destroy(gameObject);
    }
}