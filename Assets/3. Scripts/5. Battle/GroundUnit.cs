using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GroundUnit : MonoBehaviour
{
    [Header("References")]
    public BattleUnitEnum battleUnitEnum;
    public ProjectileShooter projectileShooter;
    public Collider hitCollider;
    [SerializeField] internal FadeHealthBar healthBarFade;

    [Header("Stats")]
    public UnitStats unitStats;
    private UnitProduceStatsSO unitProduceSO;
    private UnitUpgradeData unitData;
    
    private float DetectionRange;
    private float AttackRange;
    private float moveSpeed;
    private float attackTimer;

    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 2f;

    [Header("Targeting Vision")]
    public float narrowViewAngle = 10f;
    public float wideViewAngle = 45f;

    public NavMeshAgent agent;
    public Animator animator;
    public GameObject target;
    public GameObject primaryTarget;
    public GameObject secondaryTarget;

    [Header("Target Priority Settings")]
    internal float PRIORITY_WEIGHT = 50f;
    internal float distanceWeight = 200f;
    internal float narrowAngleBonus = 8f;
    internal float wideAngleBonus = 4f;
    internal float peripheralAngleBonus = 1f;
    
    private AttackTargets attackTargets;
    private Side unitSide;
    
    private float targetCheckTimer = 0f;
    private const float targetCheckInterval = 0.5f;
    public const float checkRadiusOffset = 0.5f;

    private float separationTimer;
    private const float separationInterval = 0.15f;
    private float targetCheckOffset;

    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
        unitProduceSO = unitStats.unitProduceSO;
        unitData = unitProduceSO.unitUpgradeData[unitStats.identity.spawnLevel];
        
        // Initialize stats
        moveSpeed = unitData.unitMobilityStats.moveSpeed;
        AttackRange = unitData.unitRangeStats.attackRange;
        DetectionRange = unitData.unitRangeStats.detectionRange;

        unitSide = unitStats.side;
        
        if (unitStats is UnitStats unitStat)
            attackTargets = unitStat.unitAttackTargets;

        targetCheckOffset = Random.Range(0f, 1f);

        // Setup NavMeshAgent
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = AttackRange;
            agent.updateRotation = true;
            agent.isStopped = true;
        }

        if (animator != null)
            animator.SetFloat("Move", 0f);

        FindTarget();
        ArrangeRotation();
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

        // Target validation
        targetCheckTimer += Time.deltaTime;
        if (targetCheckTimer >= targetCheckInterval + targetCheckOffset)
        {
            targetCheckTimer = 0f;
            FindTarget();
        }


        // No target
        if (target == null)
        {
            if (animator != null)
                animator.SetFloat("Move", 0f);
            
            attackTimer = 0f;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        /*
        if (!agent.hasPath && distance > AttackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }*/

        // MOVE TOWARDS TARGET
        if (distance > AttackRange)
        {
            if (agent.isStopped)
                agent.isStopped = false;

            agent.SetDestination(target.transform.position);

            if (animator != null)
                animator.SetFloat("Move", agent.velocity.magnitude);
        }
        // ATTACK STATE
        else
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            FaceTarget();
            Attack();

            if (animator != null)
                animator.SetFloat("Move", 0f);
        }
        
    }
    
    private void ArrangeRotation()
    {
        if (unitSide == Side.Player)
            transform.rotation = Quaternion.Euler(0, 45, 0);
        else
            transform.rotation = Quaternion.Euler(0, -135, 0);
    }

    private bool IsTargetValid()
    {
        return target != null && target.activeInHierarchy;
    }
    private void FindTarget()
    {
        Collider[] hits = new Collider[32];
        LayerMask enemyLayerMask = GetEnemyLayerMask();

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            DetectionRange,
            hits,
            enemyLayerMask
        );

        Stats current = target != null ? target.GetComponent<Stats>() : null;

        Stats bestTarget = current;
        float bestScore = current != null ? CalculateScore(current) : float.MinValue;

        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent<Stats>(out var candidate))
                continue;

            if (candidate == unitStats || candidate.side == unitSide)
                continue;

            float distance = Vector3.Distance(transform.position, candidate.transform.position);

            //  HARD OVERRIDE RULE
            if (current is BuildingStats || current is WallStats)
            {
                if (candidate is UnitStats && distance <= AttackRange + checkRadiusOffset)
                {
                    target = candidate.gameObject;
                    primaryTarget = target;
                    return; // IMMEDIATE SWITCH
                }
            }

            float score = CalculateScore(candidate);

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = candidate;
            }
        }

        if (bestTarget != null && bestTarget != current)
        {
            target = bestTarget.gameObject;

            if (bestTarget is UnitStats)
                primaryTarget = target;
            else
                secondaryTarget = target;
        }
    }

/*
    private void FindTarget()
    {
        Collider[] hits = new Collider[20];
        LayerMask enemyLayerMask = GetEnemyLayerMask();
        
        int count = Physics.OverlapSphereNonAlloc(transform.position, DetectionRange, hits, enemyLayerMask);
        
        primaryTarget = null;
        secondaryTarget = null;

        Stats bestTarget = null;
        float bestScore = float.MinValue;

        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent<Stats>(out var unit))
                continue;

            // Ignore self & same side
            if (unit == unitStats || unit.side == unitSide)
                continue;

            float score;
            score = CalculateScore(unit);

            // Is this a better candidate?
            if (score > bestScore )
            {
                bestScore = score;
                bestTarget = unit;
            }
        }

        // Assign target
        if (bestTarget != null && (target == null || bestTarget != target.GetComponent<Stats>()))
        {
            target = bestTarget.gameObject;

            if (bestTarget is UnitStats)
                primaryTarget = target;
            else
                secondaryTarget = target;
        }
    }
*/
    private float CalculateScore(Stats stats)
    {
        // 1. priority 
        float priorityScore = stats.identity.priority * PRIORITY_WEIGHT;

        // 2. Distance
        float distance = Vector3.Distance(transform.position, stats.transform.position);
        float distanceScore = Mathf.Max(0f, distanceWeight * (1f - distance / DetectionRange));

        //3. Angle
        Vector3 dir = stats.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir);

        float angleScore =
            angle <= narrowViewAngle ? narrowAngleBonus :
            angle <= wideViewAngle ? wideAngleBonus :
            peripheralAngleBonus;

        // Attack range bias
        float attackRangeBonus = (distance <= AttackRange) ? 30f : 0f;

        return priorityScore + distanceScore + angleScore + attackRangeBonus;
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
            //float buildingHealthPercent = current.currentHealth / current.basicStats.maxHealth;
            //if (buildingHealthPercent < 0.3f) return false;

            // Switch to unit if in range
            if (candidate is UnitStats)
            {
                float dist = Vector3.Distance(transform.position, candidate.transform.position);
                return dist <= AttackRange + checkRadiusOffset;
            }
        }

        return true;
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

    private void FaceTarget()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
        }
    }

    private void ApplySeparation()
    {
        // DO NOT separate while agent is navigating
        if (agent.hasPath && !agent.isStopped)
            return;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius);

        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (Collider hit in hits)
        {
            GroundUnit unit = hit.GetComponent<GroundUnit>();

            if (unit != null && unit != this && unit.unitSide == unitSide)
            {
                Vector3 dir = transform.position - unit.transform.position;
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
            agent.Move(move * Time.deltaTime);
            
            if (animator != null)
                animator.SetFloat("Move", agent.velocity.magnitude);
        }
    }

    private void Attack()
    {
        if (target == null)
        {
            attackTimer = 0f;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > AttackRange)
        {
            attackTimer = 0f;
            return;
        }
        
        attackTimer += Time.deltaTime;

        if (attackTimer >= unitData.unitAttackStats.fireRate)
        {
            attackTimer = 0f;

            Stats enemy = target.GetComponent<Stats>();
            if (enemy == null)
                return;

            projectileShooter.Fire(enemy);

            if (animator != null)
            {   
                animator.SetBool("Fire", true);
                StartCoroutine(ResetFire());
            }
        }
    }
    

    private IEnumerator ResetFire()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
            animator.SetBool("Fire", false);
    }

    private void OnDestroy()
    {
        foreach (var unit in BattleUnitRegistry.Units)
        {
            if (unit.target == gameObject)
            {
                unit.target = null;
            }
        }
    }

    #region TargetDetection

    public static bool AnyPlayerHasTarget()
    {
        foreach (var unit in BattleUnitRegistry.Units)
        {
            if (unit.unitSide == Side.Player &&
                unit.target != null)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyEnemyHasTarget()
    {
        foreach (var unit in BattleUnitRegistry.Units)
        {
            if (unit.unitSide == Side.Enemy &&
                unit.target != null)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Helper

    public static bool AnyPlayerAlive()
    {
        foreach (var unit in BattleUnitRegistry.Units)
        {
            if (unit.unitSide == Side.Player)
                return true;
        }

        return false;
    }

    public static bool AnyEnemyAlive()
    {
        foreach (var unit in BattleUnitRegistry.Units)
        {
            if (unit.unitSide == Side.Enemy)
                return true;
        }

        return false;
    }

    #endregion
    
    #region Gizmos

    private void OnDrawGizmosSelected()
    {
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