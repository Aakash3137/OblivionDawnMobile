using System.Collections;
using UnityEngine;

public enum AirState
{
    Ascending,
    Airborne,
    Attacking,
    Evading
}

public class AirUnit : MonoBehaviour
{
    [Header("References")]
    public ProjectileShooter projectileShooter;
    public Animator animator;
    
    [Header("Stats")]
    public UnitStats unitStats;
    private UnitProduceStatsSO unitProduceSO;
    private UnitUpgradeData unitData;
    
    [Header("Target Priority Settings")]
    public float distanceWeight = 50f;
    public float lowHealthBonus = 40f;
    public float narrowAngleBonus = 30f;
    public float wideAngleBonus = 20f;
    public float peripheralAngleBonus = 10f;
    public float checkRadiusOffset = 0.5f;
    
    [Header("Flight Settings")]
    private float flyHeight;
    private float climbAngle;
    protected float moveSpeed;
    private float turnSpeed;
    private float bankAngle;
    private float evadeRadius;
    private float attackAngleTolerance;
    
    [Header("Combat Settings")]
    public int burstCount;
    public float burstCooldown;
    public bool invulnerableDuringTakeoff = true;
    
    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 2f;
    
    // Targeting
    public GameObject target;
    public Stats primaryTarget;
    public Stats detectionTarget;
    public Stats replyTarget;
    protected float targetCheckTimer = 0f;
    protected const float targetCheckInterval = 1f;
    protected float targetCheckOffset;
    
    // Combat
    protected int shotsFired = 0;
    protected float burstTimer = 0f;
    
    // Flight
    protected AirState airState = AirState.Ascending;
    protected Vector3 evadeCenter;
    protected float evadeAngle = 0f;
    protected int evadeDirection = 1;
    protected float idleAngle = 0f;
    
    // Cached
    protected Side unitSide;
    protected Vector3 forward;
    protected AttackTargets attackTargets;
    protected float DetectionRange;
    protected float AttackRange;
    
    // Separation timer
    protected float separationTimer;
    protected const float separationInterval = 0.15f;
    private bool targetLocked = false;
    public Stats airTarget;
    
    protected virtual void Start()
    {
        unitStats = GetComponent<UnitStats>();
        unitProduceSO = unitStats.unitProduceSO;
        unitData = unitProduceSO.unitUpgradeData[unitStats.identity.spawnLevel];
        
        // Initialize stats
        moveSpeed = unitData.unitMobilityStats.moveSpeed;
        AttackRange = unitData.unitRangeStats.attackRange;
        DetectionRange = unitData.unitRangeStats.detectionRange;
        burstCooldown = unitData.unitAttackStats.fireRate;
        
        // Flight stats
        flyHeight = unitProduceSO.unitFlyStats.flyHeight;
        climbAngle = unitProduceSO.unitFlyStats.climbAngle;
        turnSpeed = unitProduceSO.unitFlyStats.turnSpeed;
        bankAngle = unitProduceSO.unitFlyStats.bankAngle;
        evadeRadius = unitProduceSO.unitFlyStats.evadeRadius;
        attackAngleTolerance = unitProduceSO.unitFlyStats.attackAngleTolerance;
        
        // Cached values
        unitSide = unitStats.side;
        forward = transform.forward;
        attackTargets = unitStats.unitAttackTargets;
        
        targetCheckOffset = Random.Range(0f, 1f);
        evadeDirection = Random.value > 0.5f ? 1 : -1;

        SetPrimaryTarget();
    }

    protected virtual void Update()
    {
        // Target Validation
        if (target != null)
        {
            Stats s = target.GetComponent<Stats>();
            if (s == null || !target.activeInHierarchy)
            {
                target = null;
            }
        }
        
        if (replyTarget != null && !replyTarget.gameObject.activeInHierarchy)
        {
            replyTarget = null;
        }
        
        // Separation
        separationTimer -= Time.deltaTime;
        if (separationTimer <= 0f)
        {
            ApplySeparation();
            separationTimer = separationInterval;
        }
        
        // Takeoff phase
        if (airState == AirState.Ascending)
        {
            PerformTakeoff();
            if (animator != null) animator.SetFloat("Move", moveSpeed);
            return;
        }
        
        // Evading phase
        if (airState == AirState.Evading)
        {
            PerformEvade();
            if (animator != null) animator.SetFloat("Move", moveSpeed);
            return;
        }
        
        // Target validation
        targetCheckTimer += Time.deltaTime;
        if (targetCheckTimer >= targetCheckInterval + targetCheckOffset)
        {
            targetCheckTimer = 0f;
            FindAirTarget();
            FindDetectionTarget();
            ResolveFinalTarget();
        }
        
        // No target - idle circle
        if (target == null)
        {
            IdleCircle();
            if (animator != null) animator.SetFloat("Move", moveSpeed);
            return;
        }
        
        // Has target - fly towards
        FlyTowards(target.transform.position);
        
        // Attack only if in range and facing target
        float distToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distToTarget <= AttackRange && IsFacingTarget(target))
        {
            Attack();
        }
        
        if (animator != null) animator.SetFloat("Move", moveSpeed);
    }

    #region Targeting

    private void SetPrimaryTarget()
    {
        if (unitSide == Side.Player)
            primaryTarget = GameManager.Instance.EnemyMainBuilding;
        else
            primaryTarget = GameManager.Instance.PlayerMainBuilding;
    }
    
    protected void FindAirTarget()
    {
        Collider[] hits = new Collider[20];

        LayerMask airMask;

        if (unitSide == Side.Player)
            airMask = LayerMask.GetMask("EnemyAir");
        else
            airMask = LayerMask.GetMask("PlayerAir");

        int count = Physics.OverlapSphereNonAlloc(transform.position, DetectionRange, hits, airMask);

        Stats closestAir = null;
        float closestDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent<Stats>(out var unit))
                continue;

            if (unit.side == unitSide)
                continue;

            float distance = Vector3.Distance(transform.position, unit.transform.position);

            if (distance < closestDist)
            {
                closestDist = distance;
                closestAir = unit;
            }
        }

        airTarget = closestAir;
    }

    private void ResolveFinalTarget()
    {
        // If reply target was destroyed (Missing), clear it
        if (!replyTarget)
            replyTarget = null;

        if (!detectionTarget)
            detectionTarget = null;
        
        if (!airTarget)
            airTarget = null;
        
        // Priority: Air Target > Reply Target > Detection Target > Primary Target
        if (airTarget != null && CanAttackTarget(airTarget))
        {
            target = airTarget.gameObject;
        }
        else if (detectionTarget != null && CanAttackTarget(detectionTarget))
        {
            target = detectionTarget.gameObject;
        }
        else if (primaryTarget != null)
        {
            target = primaryTarget.gameObject;
        } 
        else
        {
            target = null;
        }
    }
    
    protected void FindDetectionTarget()
    {
        Collider[] hits = new Collider[32];
        LayerMask enemyLayerMask = GetEnemyLayerMask();
        
        int count = Physics.OverlapSphereNonAlloc(transform.position, DetectionRange, hits, enemyLayerMask);
        
        Stats closestGroundUnit = null;
        Stats closestDefense = null;
        //Stats closestWall = null;
        Stats closestBuilding = null;

        float closestGroundDist = float.MaxValue;
        float closestDefenseDist = float.MaxValue;
        //float closestWallDist = float.MaxValue;
        float closestBuildingDist = float.MaxValue;
        
        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent<Stats>(out var candidate))
                continue;
            
            if (candidate == unitStats || candidate.side == unitSide)
                continue;
            
            float distance = Vector3.Distance(transform.position, candidate.transform.position);

            // FIRST PRIORITY: Ground Units (skip air units - they're handled by FindAirTarget)
            if (candidate is UnitStats && !candidate.CanFly)
            {
                if (distance < closestGroundDist)
                {
                    closestGroundDist = distance;
                    closestGroundUnit = candidate;
                }
            }
            // SECOND PRIORITY: Defense buildings
            else if (candidate is DefenseBuildingStats)
            {
                if (distance < closestDefenseDist)
                {
                    closestDefenseDist = distance;
                    closestDefense = candidate;
                }
            }
            /*// THIRD PRIORITY: Walls
            else if (candidate is WallStats)
            {
                if (distance < closestWallDist)
                {
                    closestWallDist = distance;
                    closestWall = candidate;
                }
            }*/
            // FOURTH PRIORITY: Resource or main buildings
            else if (candidate is BuildingStats || candidate is ResourceBuildingStats)
            {
                if (distance < closestBuildingDist)
                {
                    closestBuildingDist = distance;
                    closestBuilding = candidate;
                }
            }
        }
        
        // Apply hierarchy: Ground Units > Defense > Walls > Buildings
        if (closestGroundUnit != null)
            detectionTarget = closestGroundUnit;
        else if (closestDefense != null)
            detectionTarget = closestDefense;
        //else if (closestWall != null)
          //  detectionTarget = closestWall;
        else
            detectionTarget = closestBuilding;
    }
    
    protected LayerMask GetEnemyLayerMask()
    {
        switch (unitSide)
        {
            case Side.Player:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    return LayerMask.GetMask("EnemyAir", "EnemyGround");
                else if (attackTargets.canAttackAir)
                    return LayerMask.GetMask("EnemyAir");
                else if (attackTargets.canAttackGround)
                    return LayerMask.GetMask("EnemyGround");
                break;
                
            case Side.Enemy:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    return LayerMask.GetMask("PlayerAir", "PlayerGround");
                else if (attackTargets.canAttackAir)
                    return LayerMask.GetMask("PlayerAir");
                else if (attackTargets.canAttackGround)
                    return LayerMask.GetMask("PlayerGround");
                break;
        }
        
        return LayerMask.GetMask("PlayerAir", "PlayerGround", "EnemyAir", "EnemyGround");
    }
    
    private bool CanAttackTarget(Stats target)
    {
        if (target.CanFly)
            return attackTargets.canAttackAir;
        else
            return attackTargets.canAttackGround;
    }

    protected bool IsTargetValid()
    {
        return target != null && target.activeInHierarchy;
    }
    
    #endregion

    #region Combat
    
    protected virtual void Attack()
    {
        if (airState != AirState.Airborne && airState != AirState.Attacking)
            return;
        
        if (target == null) return;
        
        if (shotsFired >= burstCount)
            return;
        
        burstTimer += Time.deltaTime;
        
        if (burstTimer >= burstCooldown)
        {
            burstTimer = 0f;
            shotsFired++;

            Stats enemy = target.GetComponent<Stats>();
            if (enemy == null) return;
            
            projectileShooter.Fire(enemy);
            
            if (animator != null)
            {
                animator.SetBool("Fire", true);
                StartCoroutine(ResetFire());
            }
            
            airState = AirState.Attacking;
            
            // Enter evade after burst complete
            if (shotsFired >= burstCount)
            {
                airState = AirState.Evading;
                evadeCenter = transform.position;
                evadeAngle = 0f;
            }
        }
    }
    
    protected bool IsFacingTarget(GameObject targetObj)
    {
        if (!targetObj) return false;
        
        Vector3 targetPos = targetObj.transform.position;
        targetPos.y = transform.position.y;
        
        Vector3 dirToTarget = (targetPos - transform.position).normalized;
        return Vector3.Angle(transform.forward, dirToTarget) <= attackAngleTolerance;
    }
    
    protected IEnumerator ResetFire()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
            animator.SetBool("Fire", false);
    }
    
    #endregion

    #region Flight Mechanics (from AirUnit)
    
    protected void PerformTakeoff()
    {
        float currentHeight = transform.position.y;
        float progress = Mathf.Clamp01(currentHeight / flyHeight);
        
        float pitchAngle = Mathf.Lerp(climbAngle, 0f, progress);
        Quaternion pitchRotation = Quaternion.AngleAxis(-pitchAngle, Vector3.right);
        Quaternion yawRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, yawRotation * pitchRotation, 3f * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        
        if (currentHeight >= flyHeight - 0.1f)
        {
            Vector3 pos = transform.position;
            pos.y = flyHeight;
            transform.position = pos;
            airState = AirState.Airborne;
            FindDetectionTarget();
        }
    }
    
    protected virtual void IdleCircle()
    {
        if (airState == AirState.Ascending || airState == AirState.Evading)
            return;
        
        idleAngle += 45f * Time.deltaTime;
        
        float rad = idleAngle * Mathf.Deg2Rad;
        float circleRadius = 20f;
        
        Vector3 center = transform.position;
        float x = Mathf.Cos(rad) * circleRadius;
        float z = Mathf.Sin(rad) * circleRadius;
        
        Vector3 targetPos = center + new Vector3(x, 0, z);
        targetPos.y = flyHeight;
        
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir == Vector3.zero) return;
        
        RotateAndMove(dir);
    }
    
    protected virtual void FlyTowards(Vector3 targetPosition)
    {
        if (airState == AirState.Ascending || airState == AirState.Evading)
            return;
        
        targetPosition.y = flyHeight;
        
        Vector3 dir = (targetPosition - transform.position).normalized;
        if (dir == Vector3.zero) return;
        
        // Always face target while flying
        Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
        if (flatDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
        
        // Move forward
        Vector3 pos = transform.position + transform.forward * (moveSpeed * Time.deltaTime);
        pos.y = flyHeight;
        transform.position = pos;
    }
    
    protected virtual void PerformEvade()
    {
        evadeAngle += 60f * evadeDirection * Time.deltaTime;
        
        float rad = evadeAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * evadeRadius;
        float z = Mathf.Sin(rad) * evadeRadius;
        
        Vector3 targetPos = evadeCenter + new Vector3(x, 0, z);
        targetPos.y = flyHeight;
        
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir == Vector3.zero) return;
        
        RotateAndMove(dir, 0.8f);
        
        // Complete evade
        if (Mathf.Abs(evadeAngle) >= 360f)
        {
            airState = AirState.Airborne;
            shotsFired = 0;
            evadeDirection = Random.value > 0.5f ? 1 : -1;
            evadeAngle = 0f;
        }
    }
    
    protected void RotateAndMove(Vector3 direction, float bankMultiplier = 0.5f)
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatDir = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
        
        if (flatDir == Vector3.zero) return;
        
        float signedAngle = Vector3.SignedAngle(flatForward, flatDir, Vector3.up);
        float bank = Mathf.Clamp(signedAngle * bankMultiplier, -bankAngle, bankAngle);
        
        Quaternion yawRotation = Quaternion.LookRotation(flatDir);
        Quaternion bankRotation = Quaternion.AngleAxis(-bank, Vector3.forward);
        Quaternion finalRotation = yawRotation * bankRotation;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, turnSpeed * Time.deltaTime);
        
        Vector3 pos = transform.position + transform.forward * (moveSpeed * Time.deltaTime);
        pos.y = flyHeight;
        transform.position = pos;
    }
    
    #endregion

    #region Separation
    
    protected virtual void ApplySeparation()
    {
        if (airState != AirState.Airborne && airState != AirState.Attacking)
            return;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius);
        
        Vector3 separation = Vector3.zero;
        int count = 0;
        
        foreach (Collider hit in hits)
        {
            AirUnit otherAir = hit.GetComponent<AirUnit>();
            
            if (otherAir != null && otherAir != this && otherAir.unitSide == unitSide)
            {
                Vector3 dir = transform.position - otherAir.transform.position;
                float dist = dir.magnitude;
                
                if (dist > 0.01f)
                {
                    separation += dir.normalized / dist;
                    count++;
                }
            }
        }
        
        if (count > 0)
        {
            Vector3 move = separation.normalized * separationStrength;
            Vector3 pos = transform.position + move * Time.deltaTime;
            pos.y = flyHeight;
            transform.position = pos;
        }
    }
    
    #endregion

    #region Queries
    
    public bool IsAirborne()
    {
        return airState != AirState.Ascending;
    }
    
    public bool CanBeTargeted()
    {
        return !invulnerableDuringTakeoff || airState == AirState.Airborne;
    }
    
    #endregion

    #region Gizmos
    
    private void OnDrawGizmosSelected()
    {
        if (unitData == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
    
    #endregion
}
