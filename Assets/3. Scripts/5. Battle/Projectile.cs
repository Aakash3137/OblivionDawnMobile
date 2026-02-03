using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Stats targetUnit;
    private Collider targetCollider;

    private float speed;
    private float damage;
    private float lifeTime;
    private float timer;

    private ProjectileType projectileType;
    private ProjectileMotion motion;
    private ProjectileDefinition definition;

    private Vector3 fixedHitPoint;
    private bool hasHit;
    private Vector3 startPosition;
    private Vector3 lastPosition;
    private Vector3 aimPoint;
    private bool passedAimPoint;

    [ShowInInspector]private Side ShooterSide;

    [Header("Visuals")][SerializeField] private TrailRenderer[] trails;

    [Header("Homing Steering")]
    [SerializeField]
    float turnSpeed = 360f;

    [SerializeField] float avoidanceDistance = 3f;
    [SerializeField] float avoidanceStrength = 2f;
    [SerializeField] float sideRayAngle = 30f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask groundMask;

    public Material playerTrailMaterial;
    public Material enemyTrailMaterial;


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

    public void Initialize(Stats target, float damage, ProjectileDataSO projectileData, Side side)
    {
        targetUnit = target;
        ShooterSide = side;

        targetCollider = target != null ? target.hitCollider : null;

        this.damage = damage;
        speed = projectileData.projectileBasicStats.speed;
        lifeTime = projectileData.projectileBasicStats.lifeTime;
        projectileType = projectileData.projectileType;
        motion = projectileData.projectileMotion;

        timer = 0f;
        hasHit = false;
        startPosition = transform.position;

        aimPoint = targetCollider != null
            ? targetCollider.bounds.center
            : transform.position + transform.forward * projectileData.projectileBasicStats.maxRange;
        passedAimPoint = false;


        //  LOCK TARGET POSITION ON FIRE
        if (targetCollider != null)
            fixedHitPoint = targetCollider.ClosestPoint(transform.position);
        else
            fixedHitPoint = transform.position + transform.forward * projectileData.projectileBasicStats.maxRange;


        if (trails == null) return;

        foreach (var t in trails)
        {
            t.enabled = projectileData.projectileVisuals.hasTrail;

            if (!projectileData.projectileVisuals.hasTrail) continue;

            t.sharedMaterial = (side == Side.Player) ? playerTrailMaterial : enemyTrailMaterial;

            t.Clear();
            t.emitting = false;

            t.startColor = Color.white;
            t.endColor = new Color(1, 1, 1, 0);
            t.emitting = true;
        }

        definition = new ProjectileDefinition();

        definition.isAreaDamage = projectileData.projectileAOE.isAOE;
        definition.damageRadius = projectileData.projectileAOE.aoeRadius;
        definition.maxRange = projectileData.projectileBasicStats.maxRange;
        definition.speed = projectileData.projectileBasicStats.speed;
        definition.lifeTime = projectileData.projectileBasicStats.lifeTime;
        definition.projectileType = projectileData.projectileType;
        definition.motion = projectileData.projectileMotion;
        definition.hasTrail = projectileData.projectileVisuals.hasTrail;
        definition.arcHeight = projectileData.projectileBasicStats.maxArcHeight;
        definition.hitVFX = projectileData.projectileVisuals.hitVFX;

        lastPosition = transform.position;
    }

    public void Init(Stats target, float damage, ProjectileDefinition def, Material trailMaterial, Side shooterside)
    {
        targetUnit = target;
        ShooterSide = shooterside;

        targetCollider = target != null ? target.hitCollider : null;

        this.damage = damage;
        speed = def.speed;
        lifeTime = def.lifeTime;
        projectileType = def.projectileType;
        motion = def.motion;
        definition = def;

        timer = 0f;
        hasHit = false;
        startPosition = transform.position;

        aimPoint = targetCollider != null
            ? targetCollider.bounds.center
            : transform.position + transform.forward * definition.maxRange;

        passedAimPoint = false;


        //  LOCK TARGET POSITION ON FIRE
        if (targetCollider != null)
            fixedHitPoint = targetCollider.ClosestPoint(transform.position);
        else
            fixedHitPoint = transform.position + transform.forward * def.maxRange;


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


        lastPosition = transform.position;
    }

    void Update()
    {
        if (hasHit)
        {
            Disable();
            return;
        }

        if (motion == ProjectileMotion.Homing &&
            (targetUnit == null || targetCollider == null))
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

        Vector3 hitPoint = fixedHitPoint;
        //Vector3 hitPoint = targetCollider.ClosestPoint(transform.position);

        switch (motion) //  NOW VALID
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

        lastPosition = transform.position;
    }

    // ---------------- MOVEMENT ----------------

    void MoveStraight(Vector3 hitPoint)
    {
        Vector3 dir = (hitPoint - startPosition).normalized;
        transform.position += dir * speed * Time.deltaTime;

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
                //  YOU ASKED FOR THIS
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

        Vector3 hitPoint =
            targetCollider != null
                ? targetCollider.ClosestPoint(transform.position)
                : targetPos;

        CheckHit(hitPoint);
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
        // HORIZONTAL MOVEMENT (XZ)
        Vector3 currentXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 startXZ = new Vector3(startPosition.x, 0f, startPosition.z);
        Vector3 aimXZ = new Vector3(aimPoint.x, 0f, aimPoint.z);

        Vector3 flatDir = (aimXZ - startXZ).normalized;
        transform.position += flatDir * speed * Time.deltaTime;

        // PROGRESS BASED ON XZ DISTANCE

        float totalDistance = Vector3.Distance(startXZ, aimXZ);
        float traveledDistance = Vector3.Distance(startXZ, currentXZ);
        float progress = Mathf.Clamp01(traveledDistance / Mathf.Max(totalDistance, 0.01f));

        // ARC HEIGHT

        float distanceFactor = Mathf.Clamp01(totalDistance / definition.maxRange);
        float arc = Mathf.Sin(progress * Mathf.PI) * definition.arcHeight * distanceFactor;

        // VERTICAL POSITION (AIM POINT Y)

        if (!passedAimPoint)
        {
            float baseY = Mathf.Lerp(startPosition.y, aimPoint.y, progress);
            transform.position = new Vector3(
                transform.position.x,
                baseY + arc,
                transform.position.z
            );
        }
        else
        {
            // AFTER AIM POINT → FALL DOWN
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        // CHECK COLLISIONS (ENEMY / WALL / GROUND)
        CheckHit(hitPoint);

        // DETECT PASSING AIM POINT (XZ)
        if (!passedAimPoint &&
            Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                aimXZ) < 0.2f)
        {
            passedAimPoint = true;
        }

        // FINAL GROUND HIT (FALLBACK)
        if (passedAimPoint)
        {
            if (Physics.Raycast(
                    transform.position,
                    Vector3.down,
                    out RaycastHit hit,
                    2f,
                    groundMask))
            {
                OnHit(hit.point, false);
            }
        }
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
        Vector3 move = transform.position - lastPosition;
        float dist = move.magnitude;

        if (dist <= 0f)
            return;

        // ONE authoritative sweep
        if (Physics.Raycast(
                lastPosition,
                move.normalized,
                out RaycastHit hit,
                dist))
        {
            // 1️⃣ GROUND HIT
            if (((1 << hit.collider.gameObject.layer) & groundMask) != 0)
            {
                OnHit(hit.point, false);
                return;
            }

            // 2️⃣ UNIT HIT (ANY unit)
            Stats unit = hit.collider.GetComponent<Stats>();
            if (unit != null && unit.side != ShooterSide)
            {
                targetUnit = unit;
                targetCollider = hit.collider;
                OnHit(hit.point, true);
                return;
            }

            // 3️⃣ ANY OTHER COLLIDER (walls, props, etc.)
            OnHit(hit.point, false);
        }
    }

    void OnHit(Vector3 hitPoint, bool hitTarget)
    {
        hasHit = true;

        if (definition.hitVFX != null)
        {
            GameObject vfx = Instantiate(definition.hitVFX, hitPoint, Quaternion.identity);
            Destroy(vfx, 5f);
        }

        // AREA DAMAGE
        if (definition.isAreaDamage)
        {
            Collider[] hits = Physics.OverlapSphere(hitPoint, definition.damageRadius);
            foreach (Collider hit in hits)
            {
                Stats unit = hit.GetComponent<Stats>();
                if (unit != null)
                    unit.TakeDamage(damage);
            }
        }
        // DIRECT DAMAGE ONLY IF TARGET HIT
        else if (hitTarget && targetUnit != null)
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