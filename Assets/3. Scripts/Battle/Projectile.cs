using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BattleUnit targetUnit;
    private Collider targetCollider;

    private float speed;
    private float damage;
    private float lifeTime;
    private float timer;

    private ProjectileType projectileType;
    private ProjectileMotion motion; // ✅ FIX 1: ADD THIS
    private ProjectileDefinition definition;

    private bool hasHit; // prevent double hit
    private Vector3 startPosition;

    [Header("Visuals")]
    [SerializeField] private TrailRenderer[] trails;

    [Header("Homing Steering")]
    [SerializeField] float turnSpeed = 360f;
    [SerializeField] float avoidanceDistance = 3f;
    [SerializeField] float avoidanceStrength = 2f;
    [SerializeField] float sideRayAngle = 30f;
    [SerializeField] LayerMask obstacleMask;

    
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
        projectileType = def.projectileType;
        motion = def.motion; // ✅ FIX 2: ASSIGN MOTION
        definition = def;

        timer = 0f;
        hasHit = false;
        startPosition = transform.position;

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

        switch (motion) // ✅ NOW VALID
        {
            case ProjectileMotion.Straight:
                MoveStraight(hitPoint);
                break;

            case ProjectileMotion.Homing:
                MoveHomingAvoidance();
                break;

            case ProjectileMotion.GroundToAir:
                MoveGroundToAir(hitPoint);
                break;

            case ProjectileMotion.Ballistic:
                MoveBallistic(hitPoint);
                break;

            case ProjectileMotion.AirToGround:
                MoveAirToGround(hitPoint);
                break;
        }
    }

    // ---------------- MOVEMENT ----------------

    void MoveStraight(Vector3 hitPoint)
    {
        Vector3 dir = (hitPoint - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        CheckHit(hitPoint);
    }

    void MoveHoming(Vector3 hitPoint)
    {
        Vector3 dir = (hitPoint - transform.position).normalized;

        float turnSpeed = 360f; // degrees per second

        Vector3 newDir = Vector3.RotateTowards(
            transform.forward,
            dir,
            turnSpeed * Mathf.Deg2Rad * Time.deltaTime,
            0f
        );

        transform.forward = newDir;
        transform.position += transform.forward * speed * Time.deltaTime;

        CheckHit(hitPoint);
    }

    void MoveHomingAvoidance()
    {
        if (targetUnit == null) return;

        Vector3 targetPos = targetUnit.transform.position;

        Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
        
        // SEEK (object position, not ClosestPoint)
        Vector3 seekDir = (targetPos - transform.position).normalized;

        Vector3 avoidDir = Vector3.zero;
        float avoidWeight = 0f;

        float sphereRadius = 0.25f;

        if (Physics.SphereCast(
                transform.position,
                sphereRadius,
                transform.forward,
                out RaycastHit hit,
                avoidanceDistance))
        {
            // Ignore target collider
            if (hit.collider != targetCollider)
            {
                // ✅ YOU ASKED FOR THIS
                Debug.Log("MISSILE RAY HIT: " + hit.collider.name);

                // Determine side step direction
                Vector3 right = Vector3.Cross(Vector3.up, hit.normal);

                if (Vector3.Dot(right, transform.forward) < 0)
                    right = -right;

                float distanceFactor = 1f - (hit.distance / avoidanceDistance);
                avoidWeight = distanceFactor;

                avoidDir = right.normalized;
            }
        }

        Vector3 desiredDir =
            (seekDir + avoidDir * avoidWeight * avoidanceStrength).normalized;

        transform.forward = Vector3.RotateTowards(
            transform.forward,
            desiredDir,
            turnSpeed * Mathf.Deg2Rad * Time.deltaTime,
            0f
        );

        transform.position += transform.forward * speed * Time.deltaTime;

        CheckHit(targetPos);
    }

    // Ground → Air (spears, bullets)
    void MoveGroundToAir(Vector3 hitPoint)
    {
        Vector3 dir = (hitPoint - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.LookAt(hitPoint);
        CheckHit(hitPoint);
    }

    // Ballistic arc
    void MoveBallistic(Vector3 hitPoint)
    {
        // Flattened positions
        Vector3 startFlat = startPosition;
        startFlat.y = 0f;

        Vector3 targetFlat = hitPoint;
        targetFlat.y = 0f;

        Vector3 currentFlat = transform.position;
        currentFlat.y = 0f;

        float totalDistance = Vector3.Distance(startFlat, targetFlat);
        float traveledDistance = Vector3.Distance(startFlat, currentFlat);

        // -------------------------
        // SPEED SCALING (THE FIX)
        // -------------------------

        // Weapon max range (IMPORTANT: comes from weapon, not unit)
        float maxRange = definition.maxRange;

        // How far this shot is compared to max range
        float distanceFactor = Mathf.Clamp01(totalDistance / maxRange);

        // Close = slower, Far = normal
        float speedFactor = Mathf.Lerp(0.4f, 1f, distanceFactor);

        Vector3 flatDir = (targetFlat - startFlat).normalized;
        transform.position += flatDir * (speed * speedFactor) * Time.deltaTime;

        // -------------------------
        // ARC HEIGHT (ALREADY GOOD)
        // -------------------------

        float progress =
            Mathf.Clamp01(traveledDistance / Mathf.Max(totalDistance, 0.01f));

        float height =
            Mathf.Sin(progress * Mathf.PI) * definition.arcHeight * distanceFactor;

        transform.position = new Vector3(
            transform.position.x,
            startPosition.y + height,
            transform.position.z
        );

        CheckHit(hitPoint);
    }


    // Air → Ground (bombs)
    void MoveAirToGround(Vector3 hitPoint)
    {
        Vector3 dir = (hitPoint - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        CheckHit(hitPoint);
    }

    // ---------------- HIT ----------------

    void CheckHit(Vector3 hitPoint)
    {
        if (Vector3.Distance(transform.position, hitPoint) <= 0.15f)
        {
            OnHit(hitPoint);
        }
    }

    void OnHit(Vector3 hitPoint)
    {
        hasHit = true;

        if (definition.hitVFX != null)
        {
            GameObject vfx = Instantiate(definition.hitVFX, hitPoint, Quaternion.identity);
            Destroy(vfx, 5f);
        }

        // Area damage (cannon ball / bomb)
        if (definition.isAreaDamage)
        {
            Collider[] hits = Physics.OverlapSphere(hitPoint, definition.damageRadius);
            foreach (Collider hit in hits)
            {
                BattleUnit unit = hit.GetComponent<BattleUnit>();
                if (unit != null)
                    unit.TakeDamage(damage);
            }
        }
        else
        {
            targetUnit.TakeDamage(damage);
        }

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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * avoidanceDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position,
            Quaternion.Euler(0, sideRayAngle, 0) * transform.forward * avoidanceDistance);

        Gizmos.DrawRay(transform.position,
            Quaternion.Euler(0, -sideRayAngle, 0) * transform.forward * avoidanceDistance);
    }

}