using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float damage;
    private float lifeTime;
    private float timer;

    public void Init(Transform target, float speed, float damage, float lifeTime)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.lifeTime = lifeTime;
        timer = 0f;
    }

    void Update()
    {
        if (target == null)
        {
            Disable();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Disable();
            return;
        }

        // Move towards target (NO PHYSICS)
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Hit check
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= 0.2f)
        {
            BattleUnit unit = target.GetComponent<BattleUnit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);
            }

            Disable();
        }
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}