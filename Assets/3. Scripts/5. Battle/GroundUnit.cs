
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum UnitState
{
    MovingToPrimary,
    Chasing,
    Attacking,
    Idle
}

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

    [Header("UTDS")]
    public Stats primaryTarget;
    public Stats detectionTarget;
    public Stats replyTarget;

    public Queue<Stats> replytargetQueue = new Queue<Stats>();

    [Header("Target Priority Settings")]
    internal float PRIORITY_WEIGHT = 50f;
    internal float distanceWeight = 200f;
    internal float narrowAngleBonus = 8f;
    internal float wideAngleBonus = 4f;
    internal float peripheralAngleBonus = 1f;

    private UnitState currentState;
    
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

        SetPrimaryTarget();
        FindDetectionTarget();
        ArrangeRotation();
    }
    
    private void Update()
    {
        HandleTargetDetection();
        HandleState();
    }
    
    private void HandleTargetDetection()
    {
        targetCheckTimer += Time.deltaTime;

        if (targetCheckTimer >= targetCheckInterval + targetCheckOffset)
        {
            targetCheckTimer = 0f;

            FindDetectionTarget();

            ResolveFinalTarget();
        }
    }
    
    private void ResolveFinalTarget()
    {
        // If reply target was destroyed (Missing), clear it
        if (!replyTarget)
            replyTarget = null;

        if (!detectionTarget)
            detectionTarget = null;
        
        
        if (replyTarget != null && CanAttackTarget(replyTarget))
        {
            target = replyTarget.gameObject;
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
    
    private void HandleState()
    {
        if (target == null)
        {
            ChangeState(UnitState.Idle);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        
        // Account for collider sizes
        float targetRadius = 0f;
        if (target.TryGetComponent<Collider>(out var targetCollider))
            targetRadius = targetCollider.bounds.extents.magnitude;
        
        float myRadius = hitCollider != null ? hitCollider.bounds.extents.magnitude : 0f;
        float effectiveDistance = distance - targetRadius - myRadius;
        
        bool withinAttackRange = effectiveDistance <= AttackRange || (agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        switch (currentState)
        {
            case UnitState.Idle:
                ChangeState(UnitState.MovingToPrimary);
                break;

            case UnitState.MovingToPrimary:

                if (detectionTarget != null)
                {
                   ChangeState(UnitState.Chasing);
                }
                else if (withinAttackRange)
                {
                    ChangeState(UnitState.Attacking);
                    return;
                }

                MoveToTarget();
                ApplySeparation();   
                break;

            case UnitState.Chasing:

                if (withinAttackRange)
                {
                   ChangeState(UnitState.Attacking);
                    return;
                }

                MoveToTarget();
                ApplySeparation();  
                break;

            case UnitState.Attacking:

                if (effectiveDistance > AttackRange + 0.5f)
                {
                  ChangeState(UnitState.Chasing);
                    return;
                }

                Attack();
                break;
        }
    }
    
    private void ChangeState(UnitState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (newState)
        {
            case UnitState.Idle:
                agent.isStopped = true;
                animator?.SetFloat("Move", 0f);
                break;

            case UnitState.MovingToPrimary:
            case UnitState.Chasing:
                agent.isStopped = false;
                break;

            case UnitState.Attacking:
                agent.isStopped = true;
                animator?.SetFloat("Move", 0f);
                attackTimer = unitData.unitAttackStats.fireRate; // Fire immediately
                break;
        }
    }
    
    private void MoveToTarget()
    {
        if (target == null) return;

        agent.SetDestination(target.transform.position);

        if (animator != null)
            animator.SetFloat("Move", agent.velocity.magnitude);
    }


    private void ArrangeRotation()
    {
        if (unitSide == Side.Player)
            transform.rotation = Quaternion.Euler(0, 45, 0);
        else
            transform.rotation = Quaternion.Euler(0, -135, 0);
    }

    public void SetReplyTarget(Stats attacker)
    {
        if (attacker != null && attacker.side != unitSide)
            replyTarget = attacker;
    }

    #region UTDS (Unit Target Detection System)
    

    private void SetPrimaryTarget()
    {
        if (unitSide == Side.Player)
            primaryTarget = GameManager.Instance.EnemyMainBuilding;
        else
            primaryTarget = GameManager.Instance.PlayerMainBuilding;
    }

    
    private void FindDetectionTarget()
    {
        Collider[] hits = new Collider[32];
        LayerMask enemyLayerMask = GetEnemyLayerMask();

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            DetectionRange,
            hits,
            enemyLayerMask
        );

        Stats closestUnit = null;
        Stats closestDefense = null;
        Stats closestWall = null;
        Stats closestBuilding = null;

        float closestUnitDist = float.MaxValue;
        float closestDefenseDist = float.MaxValue;
        float closestWallDist = float.MaxValue;
        float closestBuildingDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (!hits[i].TryGetComponent(out Stats candidate))
                continue;

            if (candidate == unitStats || candidate.side == unitSide)
                continue;
            

            float distance = GetAdjustedDistance(candidate);

            //  FIRST PRIORITY: Units
            if (candidate is UnitStats )
            {
                if (distance < closestUnitDist)
                {
                    closestUnitDist = distance;
                    closestUnit = candidate;
                }
            }
            //  SECOND PRIORITY: Defense buildings
            else if (candidate is DefenseBuildingStats)
            {
                if (distance < closestDefenseDist)
                {
                    closestDefenseDist = distance;
                    closestDefense = candidate;
                }
            }
            //  THIRD PRIORITY: Walls
            else if (candidate is WallStats)
            {
                if (distance < closestWallDist)
                {
                    closestWallDist = distance;
                    closestWall = candidate;
                }
            }
            //  FOURTH PRIORITY: Resource or main buildings
            else if (candidate is BuildingStats || candidate is ResourceBuildingStats)
            {
                if (distance < closestBuildingDist)
                {
                    closestBuildingDist = distance;
                    closestBuilding = candidate;
                }
            }
        }

        // Apply hierarchy
        if (closestUnit != null)
            detectionTarget = closestUnit;
        else if (closestDefense != null)
            detectionTarget =  closestDefense;
        else if (closestWall != null)
            detectionTarget = closestWall;
        else
            detectionTarget = closestBuilding;
    }
    
    private float GetAdjustedDistance(Stats target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        Vector3 dir = target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir);

        float anglePenalty = angle * 0.05f; // very small influence

        return distance + anglePenalty;
    }

    private bool CanAttackTarget(Stats target)
    {
        if (target.CanFly)
            return attackTargets.canAttackAir;
        else
            return attackTargets.canAttackGround;
    }
    
    #endregion


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
        if (agent.isStopped)
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
        }
    }

    private void Attack()
    {
        if (target == null)
        {
          //  GameDebug.Log($"[{name}] Attack: Target is NULL");
            attackTimer = 0f;
            return;
        }
        
        FaceTarget();
        
        float distance = Vector3.Distance(transform.position, target.transform.position);
        
        // Account for collider sizes
        float targetRadius = 0f;
        if (target.TryGetComponent<Collider>(out var targetCollider))
            targetRadius = targetCollider.bounds.extents.magnitude;
        
        float myRadius = hitCollider != null ? hitCollider.bounds.extents.magnitude : 0f;
        float effectiveDistance = distance - targetRadius - myRadius;
        
        if (effectiveDistance > AttackRange + 0.5f)
        {
          //  GameDebug.Log($"[{name}] Attack: Out of range (EffectiveDist: {effectiveDistance:F2} > {AttackRange + 0.5f:F2})");
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= unitData.unitAttackStats.fireRate)
        {
            attackTimer = 0f;

            Stats enemy = target.GetComponent<Stats>();
            if (enemy == null)
            {
              //  GameDebug.Log($"[{name}] Attack: Target has no Stats component");
                return;
            }

          //  GameDebug.Log($"[{name}] FIRING at {target.name}");
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