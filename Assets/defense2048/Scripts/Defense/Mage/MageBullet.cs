using UnityEngine;

public class MageBullet : MonoBehaviour
{
    public float speed = 8f;
    public float hitDistance = 0.15f;
    public int damage = 1;

    private Transform target;


    public void Init(Transform enemy)
    {
        target = enemy;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        
        transform.position += dir.normalized * speed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation=Quaternion.Euler(0,0,angle);

        if (dir.magnitude<=hitDistance)
        {
            Hit();
        }
    }

    void Hit()
    {
       target.GetComponent<EnemyHeath>()?.TakeDamage(damage);
       Destroy(gameObject);
    }
}