using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ScenarioBattleUnit : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private UnitProduceStatsSO myUnitStats;
    private UnitUpgradeData currentUnitData;

    [Header("Attack Settings Temp for attack function")]
    public ProjectileShooter projectileShooter;
    public float attackCooldown = .5f;
    private float attackTimer;


    [Header("For Debugging")]
    public GameObject target;
    public GameObject primaryTarget;
    public GameObject secondaryTarget;

    [Header("Target priority settings high score = target priority")]
    public float distanceWeight = 500f;
    public float lowHealthBonus = 40f;
    public float narrowAngleBonus = 30f;
    public float wideAngleBonus = 20f;
    public float peripheralAngleBonus = 10f;
    public float unitWeight = 10f;
    public float buildingWeight = 0f;

    [Header("Unit Specific Stats (DO NOT EDIT)")]
    private ScenarioOffenseType offenseUnitType;
    private MobilityStats mobilityStats;
    private RangeStats rangeStats;
    private VisionAngles visionAngles;
    private AttackTargets attackTargets;
    private FlyStats flyStats;
    private Animator animator;

    //private AirUnit airUnit;

    private Vector3 forward;
    private Side mySide;
    Vector2Int currentCoord;
    private float targetCheckTimer = 0f;
    private float targetCheckInterval;
    public const float checkRadiusOffset = 0.5f;

    private void Start()
    {
        myUnitStats = GetComponent<UnitStats>().spawnerBuilding.unitProduceStats;
        currentUnitData = GetComponent<UnitStats>().spawnerBuilding.currentUnitLevelData;
        navAgent = GetComponent<NavMeshAgent>();

        SetUnitStats();

        forward = transform.forward;
        navAgent.isStopped = true;

        targetCheckInterval = Random.Range(0.5f, 1f);

        if (animator != null)
            animator.SetFloat("Move", 0f);

        navAgent.speed = mobilityStats.moveSpeed;
        navAgent.stoppingDistance = rangeStats.attackRange;
        navAgent.updateRotation = true;
        ArrangeRotation();
    }
    private void SetUnitStats()
    {
        offenseUnitType = myUnitStats.unitType;
        mobilityStats = currentUnitData.unitMobilityStats;
        rangeStats = currentUnitData.unitRangeStats;
        visionAngles = currentUnitData.unitVisionAngles;
        attackTargets = currentUnitData.unitAttackTargets;
        flyStats = currentUnitData.unitFlyStats;
        animator = GetComponentInChildren<Animator>();
        mySide = GetComponent<UnitStats>().side;
    }


    private void Update()
    {
        targetCheckTimer += Time.deltaTime;

        if (targetCheckTimer >= targetCheckInterval)
        {
            FindTarget();
            targetCheckTimer = 0f;
        }

        if (target == null)
        {
            if (animator != null) animator.SetFloat("Move", 0f);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > rangeStats.attackRange)
        {
            navAgent.isStopped = false;

            if (!navAgent.hasPath || navAgent.remainingDistance > rangeStats.attackRange)
                navAgent.SetDestination(target.transform.position);
        }
        else
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
            FaceTarget();
            Attack();
        }

        if (animator != null)
            animator.SetFloat("Move", navAgent.velocity.magnitude);

        UpdateTileOwnership();
    }
    private void FindTarget()
    {
        Collider[] hits = new Collider[20];

        LayerMask enemyLayerMask = LayerMask.GetMask("PlayerAir", "PlayerGround", "EnemyAir", "EnemyGround");

        switch (mySide)
        {
            case Side.Player:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("EnemyAir", "EnemyGround");
                else if (attackTargets.canAttackAir)
                    enemyLayerMask = LayerMask.GetMask("EnemyAir");
                else if (attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("EnemyGround");
                break;
            case Side.Enemy:
                if (attackTargets.canAttackAir && attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("PlayerAir", "PlayerGround");
                else if (attackTargets.canAttackAir)
                    enemyLayerMask = LayerMask.GetMask("PlayerAir");
                else if (attackTargets.canAttackGround)
                    enemyLayerMask = LayerMask.GetMask("PlayerGround");
                break;
        }

        int count = Physics.OverlapSphereNonAlloc(transform.position, rangeStats.detectionRange, hits, enemyLayerMask);

        Stats bestTarget = null;

        float bestScore = 0f;

        for (int i = 0; i < count; i++)
        {
            // Process targets...
            Stats unit;

            float score;

            if (hits[i].TryGetComponent<UnitStats>(out var unitStat))
            {
                unit = unitStat;
                score = unitWeight;
            }
            else if (hits[i].TryGetComponent<BuildingStats>(out var buildingStats))
            {
                unit = buildingStats;
                score = buildingWeight;
            }
            else if (hits[i].TryGetComponent<WallStats>(out var wallStats))
            {
                unit = wallStats;
                score = buildingWeight;
            }
            else
            {
                continue;
            }

            if (unit == this)
                continue;

            score = CalculateScore(unit, score);

            // Is this a better candidate?
            if (score > bestScore && (target == null || ShouldSwitchTarget(target?.GetComponent<Stats>(), unit)))
            {
                bestScore = score;
                bestTarget = unit;
            }
        }

        if (bestTarget != null && (target == null || bestTarget != target.GetComponent<Stats>()))
        {
            // only assign if switching
            target = bestTarget.gameObject;

            if (!mobilityStats.canFly)
            {
                navAgent.ResetPath();
                navAgent.isStopped = false;
            }

            if (bestTarget is UnitStats)
                primaryTarget = target;
            else
                secondaryTarget = target;
        }

    }

    private void ArrangeRotation()
    {
        if (mySide == Side.Player)
        {
            transform.rotation = Quaternion.Euler(0, 45, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -135, 0);
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

        if (angle <= visionAngles.narrowViewAngle)
            score += narrowAngleBonus;
        else if (angle <= visionAngles.wideViewAngle)
            score += wideAngleBonus;
        else
            score += peripheralAngleBonus;
        return score;
    }

    bool ShouldSwitchTarget(Stats current, Stats candidate)
    {
        // no target, always switch
        if (current == null)
            return true;

        // never switch from unit to building
        if (current is UnitStats && (candidate is BuildingStats || candidate is WallStats))
            return false;

        if (current is BuildingStats || current is WallStats)
        {
            if (candidate == null)
                return false;

            // check if building is low health
            float buildingHealthPercent = current.currentHealth / current.basicStats.maxHealth;
            if (buildingHealthPercent < 0.3f)
                return false; // stick with low-health building

            // if candidate is unit, switch
            if (candidate is UnitStats)
            {
                float dist = Vector3.Distance(transform.position, candidate.transform.position);
                return dist <= rangeStats.attackRange + checkRadiusOffset;
            }
        }

        return false; // default: do not switch 
    }

    void FaceTarget()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRot,
                10f * Time.deltaTime
            );
        }
    }

    void Attack()
    {
        // if (isAirUnit && airUnit != null)
        // {
        //     if (!airUnit.CanAttack()) return;

        //     if (airUnit.ShouldFireBurst(target))
        //     {
        //         if (target != null)
        //         {
        //             Stats enemy = target.GetComponent<Stats>();
        //             if (enemy != null)
        //             {
        //                 projectileShooter.Fire(enemy);
        //                 if (animator != null)
        //                 {
        //                     animator.SetBool("Fire", true);
        //                     StartCoroutine(ResetFire());
        //                 }
        //             }
        //         }
        //     }
        //     return;
        // }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (target != null)
            {
                Stats enemy = target.GetComponent<Stats>();
                if (enemy != null)
                {
                    projectileShooter.Fire(enemy);
                    if (animator != null)
                    {
                        animator.SetBool("Fire", true);
                        StartCoroutine(ResetFire());
                    }
                }
            }
        }
    }
    IEnumerator ResetFire()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
            animator.SetBool("Fire", false);
    }



    #region Tile Ownership
    // --- Tile ownership ---
    void UpdateTileOwnership()
    {
        if (CubeGridManager.Instance == null) return;

        Vector2Int coord = CubeGridManager.Instance.WorldToGrid(transform.position);
        if (coord != currentCoord)
        {
            LeaveTile(currentCoord);
            currentCoord = coord;
            EnterTile(currentCoord);
        }
    }

    void EnterTile(Vector2Int coord)
    {
        if (TileManager.Instance != null)
            TileManager.Instance.TryEnterTile(gameObject, coord);

        var tileGO = CubeGridManager.Instance.GetCube(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null)
        {
            tile.Occupy(mySide);
        }
    }

    void LeaveTile(Vector2Int coord)
    {
        if (TileManager.Instance != null)
        {
            TileManager.Instance.LeaveTile(coord, gameObject);
            //Debug.Log($"Tile Vacated at {coord}");
        }

        var tileGO = CubeGridManager.Instance?.GetCube(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null)
            tile.Vacate(mySide);
    }
    #endregion

    // #region TargetDetection
    // public static bool AnyPlayerHasTarget()
    // {
    //     BattleUnit[] units = FindObjectsOfType<BattleUnit>();
    //     foreach (var unit in units)
    //     {
    //         if (unit.unitSide == Side.Player &&
    //             unit.target != null)
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    // public static bool AnyEnemyHasTarget()
    // {
    //     BattleUnit[] units = FindObjectsOfType<BattleUnit>();
    //     foreach (var unit in units)
    //     {
    //         if (unit.unitSide == Side.Enemy &&
    //             unit.target != null)
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    // #endregion

    // #region Helper
    // public static bool AnyPlayerAlive()
    // {
    //     foreach (var unit in FindObjectsOfType<BattleUnit>())
    //     {
    //         if (unit.unitSide == Side.Player)
    //             return true;
    //     }
    //     return false;
    // }

    // public static bool AnyEnemyAlive()
    // {
    //     foreach (var unit in FindObjectsOfType<BattleUnit>())
    //     {
    //         if (unit.unitSide == Side.Enemy)
    //             return true;
    //     }
    //     return false;
    // }
    // #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeStats.attackRange);

        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
    #endregion

    private void OnDestroy()
    {

    }

}
