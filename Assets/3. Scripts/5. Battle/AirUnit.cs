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
    private float moveSpeed;
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
    private Stats target;
    private GameObject primaryTarget;
    private GameObject secondaryTarget;
    private float targetCheckTimer = 0f;
    private const float targetCheckInterval = 1f;
    private float targetCheckOffset;
    
    // Combat
    private int shotsFired = 0;
    private float burstTimer = 0f;
    
    // Flight
    private AirState airState = AirState.Ascending;
    private Vector3 evadeCenter;
    private float evadeAngle = 0f;
    private int evadeDirection = 1;
    private float idleAngle = 0f;
    
    // Cached
    private Side unitSide;
    private Vector3 forward;
    private AttackTargets attackTargets;
    private float DetectionRange;
    private float AttackRange;
    
    // Separation timer
    private float separationTimer;
    private const float separationInterval = 0.15f;

    private void Start()
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
    }

    private void Update()
    {
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
            
            if (!IsTargetValid())
            {
                target = null;
                FindTarget();
            }
        }
        
        // Find target if none
        if (target == null)
        {
            FindTarget();
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
        if (distToTarget <= AttackRange && IsFacingTarget(target.gameObject))
        {
            Attack();
        }
        
        if (animator != null) animator.SetFloat("Move", moveSpeed);
    }

    #region Targeting (from DefenseUnit)
    
    private void FindTarget()
    {
        Collider[] hits = new Collider[20];
        LayerMask enemyLayerMask = GetEnemyLayerMask();
        
        int count = Physics.OverlapSphereNonAlloc(transform.position, DetectionRange, hits, enemyLayerMask);
        
        primaryTarget = null;
        secondaryTarget = null;
        
        Stats bestTarget = null;
        float bestScore = 0f;
        
        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent<Stats>(out var unit))
                continue;
            
            // Ignore self & same side
            if (unit == unitStats || unit.side == unitSide)
                continue;
            
            float score = unit.identity.priority;
            score = CalculateScore(unit, score);
            
            // Is this a better candidate?
            if (score > bestScore && (target == null || ShouldSwitchTarget(target, unit)))
            {
                bestScore = score;
                bestTarget = unit;
            }
        }
        
        // Assign target
        if (bestTarget != null && (target == null || bestTarget != target))
        {
            target = bestTarget;
            
            if (bestTarget is UnitStats)
                primaryTarget = target.gameObject;
            else
                secondaryTarget = target.gameObject;
        }
    }
    
    private float CalculateScore(Stats unit, float score)
    {
        // Distance score
        float distance = Vector3.Distance(transform.position, unit.transform.position);
        score += distanceWeight / Mathf.Max(distance, 0.1f);
        
        // Low health bonus
        float healthPercent = unit.currentHealth / unit.basicStats.maxHealth;
        if (healthPercent <= 0.3f) score += lowHealthBonus;
        
        // View angle
        float angle = Vector3.Angle(forward, unit.transform.position - transform.position);
        
        if (angle <= unitProduceSO.unitVisionAngles.narrowViewAngle)
            score += narrowAngleBonus;
        else if (angle <= unitProduceSO.unitVisionAngles.wideViewAngle)
            score += wideAngleBonus;
        else
            score += peripheralAngleBonus;
        
        return score;
    }
    
    private bool ShouldSwitchTarget(Stats current, Stats candidate)
    {
        if (current == null) return true;
        
        // Never switch from unit to building
        if (current is UnitStats && (candidate is BuildingStats || candidate is WallStats))
            return false;
        
        if (current is BuildingStats || current is WallStats)
        {
            if (candidate == null) return false;
            
            // Stick with low-health building
            float buildingHealthPercent = current.currentHealth / current.basicStats.maxHealth;
            if (buildingHealthPercent < 0.3f) return false;
            
            // Switch to unit if in range
            if (candidate is UnitStats)
            {
                float dist = Vector3.Distance(transform.position, candidate.transform.position);
                return dist <= AttackRange + checkRadiusOffset;
            }
        }
        
        return false;
    }
    
    private LayerMask GetEnemyLayerMask()
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
    
    private bool IsTargetValid()
    {
        return target != null && target.gameObject.activeInHierarchy;
    }
    
    #endregion

    #region Combat
    
    private void Attack()
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
            
            projectileShooter.Fire(target);
            
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
    
    private bool IsFacingTarget(GameObject targetObj)
    {
        if (!targetObj) return false;
        
        Vector3 targetPos = targetObj.transform.position;
        targetPos.y = transform.position.y;
        
        Vector3 dirToTarget = (targetPos - transform.position).normalized;
        return Vector3.Angle(transform.forward, dirToTarget) <= attackAngleTolerance;
    }
    
    private IEnumerator ResetFire()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
            animator.SetBool("Fire", false);
    }
    
    #endregion

    #region Flight Mechanics (from AirUnit)
    
    private void PerformTakeoff()
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
            FindTarget();
        }
    }
    
    private void IdleCircle()
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
    
    private void FlyTowards(Vector3 targetPosition)
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
        Vector3 pos = transform.position + transform.forward * moveSpeed * Time.deltaTime;
        pos.y = flyHeight;
        transform.position = pos;
    }
    
    private void PerformEvade()
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
    
    private void RotateAndMove(Vector3 direction, float bankMultiplier = 0.5f)
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
        
        Vector3 pos = transform.position + transform.forward * moveSpeed * Time.deltaTime;
        pos.y = flyHeight;
        transform.position = pos;
    }
    
    #endregion

    #region Separation
    
    private void ApplySeparation()
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
