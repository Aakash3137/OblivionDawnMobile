using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BattleUnit targetUnit;
    private Collider targetCollider;

    private float speed;
    private float damage;
    private float lifeTime;
    private float timer;

    private ProjectileMotion motion;
    private ProjectileDefinition definition;

    private bool hasHit; //  prevent double hit

    [Header("Visuals")]
    [SerializeField] private TrailRenderer[] trails;

    void Awake()
    {
        // Break shared material references once
        if (trails != null)
        {
            foreach (var t in trails)
            {
                if (t.material != null)
                    t.material = new Material(t.material);
            }
        }
    }

    public void Init(
        BattleUnit target,
        float damage,
        ProjectileDefinition def,
        Material trailMaterial
    )
    {
        targetUnit = target;
        targetCollider = target != null ? target.hitCollider : null;

        this.damage = damage;
        speed = def.speed;
        lifeTime = def.lifeTime;
        motion = def.motion;
        definition = def;

        timer = 0f;
        hasHit = false;

        if (trails == null) return;

        foreach (var t in trails)
        {
            t.enabled = def.hasTrail;
            if (!def.hasTrail) continue;

            t.Clear();
            t.emitting = false;

            if (trailMaterial != null)
                t.material = new Material(trailMaterial);

            t.startColor = Color.white;
            t.endColor = new Color(1, 1, 1, 0);

            t.emitting = true;
        }
    }

    void Update()
    {
        if (hasHit || targetUnit == null || targetCollider == null)
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

        Vector3 hitPoint = targetCollider.ClosestPoint(transform.position);
        Vector3 dir = (hitPoint - transform.position).normalized;

        Move(dir);

        if (Vector3.Distance(transform.position, hitPoint) <= 0.15f)
        {
            OnHit(hitPoint);
        }
    }

    void Move(Vector3 dir)
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnHit(Vector3 hitPoint)
    {
        hasHit = true;

        if (definition.hitVFX != null)
        {
            GameObject vfx = Instantiate(
                definition.hitVFX,
                hitPoint,
                Quaternion.identity
            );

            Destroy(vfx, 5f);
        }


        targetUnit.TakeDamage(damage);
        Disable();
    }

    void Disable()
    {
        if (trails != null)
        {
            foreach (var t in trails)
                t.emitting = false;
        }

        gameObject.SetActive(false);
    }
}
