using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnit : MonoBehaviour
{
    [Header("References")]
    public BattleUnitEnum battleUnitEnum;
    public ProjectileShooter projectileShooter;
    public Collider hitCollider;

    [SerializeField] internal FadeHealthBar healthBarFade;

    [Header("Stats")]
    //public float currentHealth;
    public float DetectionRange = 10f;
    public float AttackRange = 3f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float flyHeight = 5f; // Air units only

    [Header("Combat")]
    public float attackDamage = 10f;
    public float attackCooldown = .5f;
    private float attackTimer;

    [Header("Separation")]
    public float separationRadius = 1.5f;
    public float separationStrength = 2f;

    [Header("Targeting Vision")]
    public float narrowViewAngle = 10f; // straight ahead
    public float wideViewAngle = 45f;   // front arc

    [Header("Unit Type")]
    public bool isAirUnit;

    public Stats unitStats;
    private AirUnit airUnit;

    public NavMeshAgent agent;
    public Animator animator;
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
    Vector3 forward;

    public Side unitSide;
    Vector2Int currentCoord;
    private float targetCheckTimer = 0f;
    private const float targetCheckInterval = 0.5f;
    public const float checkRadiusOffset = 0.5f;
    
    //Separation timer variables
    private float separationTimer;
    private const float separationInterval = 0.15f;
    
    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
        unitSide = unitStats.side;
        forward = transform.forward;
        agent.isStopped = true;

        if (animator != null)
            animator.SetFloat("Move", 0f);

        unitStats.currentHealth = unitStats.basicStats.maxHealth;

        if (isAirUnit)
        {
            airUnit = GetComponent<AirUnit>();
            if (agent != null) agent.enabled = false;
        }
        else
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = AttackRange;
            agent.updateRotation = true;
        }

        ArrangeRotation();
        //ApplySeparation();
    }

    private void Update()
    {
        separationTimer -= Time.deltaTime;
        if (separationTimer <= 0f)
        {
            ApplySeparation();
            separationTimer = separationInterval;
        }
        
        //no navmesh agent need for air units
        if (target != null)
        {
            if (!isAirUnit) agent.isStopped = true;
        }

        // air units dont find targets until airborne
        if (isAirUnit && airUnit != null && !airUnit.IsAirborne())
        {
            if (animator != null)
                animator.SetFloat("Move", moveSpeed);
            return;
        }

        // find target
        if (target == null)
        {
            targetCheckTimer += Time.deltaTime;
            if (targetCheckTimer >= targetCheckInterval)
            {
                FindTarget();
                targetCheckTimer = 0f;
            }

            if (isAirUnit && airUnit != null)
            {
                airUnit.IdleCircle();
                if (animator != null) animator.SetFloat("Move", moveSpeed);
            }
            else
            {
                if (animator != null) animator.SetFloat("Move", 0f);
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (!isAirUnit && !agent.hasPath && distance > AttackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }

        if (distance > AttackRange)
        {
            if (isAirUnit)
            {
                airUnit.FlyTowards(target.transform.position);
                Attack();
            }
            else
            {
                agent.isStopped = false;
                if (!agent.hasPath || agent.remainingDistance > AttackRange)
                    agent.SetDestination(target.transform.position);
            }

            if (animator != null)
                animator.SetFloat("Move", isAirUnit ? moveSpeed : agent.velocity.magnitude);
        }
        else
        {
            if (isAirUnit && airUnit != null)
            {
                airUnit.FlyTowards(target.transform.position);
                if (animator != null) animator.SetFloat("Move", moveSpeed);
                Attack();
            }
            else if (!isAirUnit)
            {
                agent.isStopped = true;
                agent.ResetPath();
                if (animator != null)
                    animator.SetFloat("Move", agent.velocity.magnitude);
                //ApplySeparation();
                FaceTarget();
                Attack();
            }
        }

        if (!isAirUnit)
        {
            if (agent.velocity.sqrMagnitude > 0.01f)    // for optimization
            {
                UpdateTileOwnership();
            }
        }
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
            tile.Occupy(unitSide);
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
            tile.Vacate(unitSide);
    }
    #endregion

    private void ArrangeRotation()
    {
        if (unitSide == Side.Player)
        {
            transform.rotation = Quaternion.Euler(0, 45, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -135, 0);
        }
    }


    void FindTarget()
    {
        primaryTarget = null;
        secondaryTarget = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, DetectionRange);
        
        Stats bestTarget = null;
        float bestScore = 0f;
        Side mySide = unitSide;

        foreach (Collider hit in hits)
        {
            Stats unit;
            float score = 0f;

            if (hit.TryGetComponent<UnitStats>(out var unitStat))
            {
                unit = unitStat;
                score = unitWeight;
            }
            else if (hit.TryGetComponent<BuildingStats>(out var buildingStats))
            {
                unit = buildingStats;
                score = buildingWeight;
            }
            else if (hit.TryGetComponent<WallStats>(out var wallStats))
            {
                unit = wallStats;
                score = buildingWeight;
            }
            else
            {
                continue;
            }

            // Ignore self & same side
            if (unit == this || unit.side == mySide)
                continue;

            // if (unit.isAirUnit && !canAttackAir)
            //     continue;

            // Can't target air units during takeoff
            if (unit.CanFly)
            {
                AirUnit targetAir = unit.GetComponent<AirUnit>();
                if (targetAir != null && !targetAir.CanBeTargeted()) continue;
                if (!unitStats.canAttackAir) continue;
            }

            if (!unit.CanFly && !unitStats.canAttackGround)
                continue;

            score = CalculateScore(unit, score);

            // Is this a better candidate?
            if (score > bestScore && (target == null || ShouldSwitchTarget(target?.GetComponent<Stats>(), unit)))
            {
                bestScore = score;
                bestTarget = unit;
            }
        }

        // Assign target
        if (bestTarget != null && (target == null || bestTarget != target.GetComponent<Stats>()))
        {
            // only assign if switching
            target = bestTarget.gameObject;
            attackTimer = 0f;

            if (!isAirUnit)
            {
                agent.ResetPath();
                agent.isStopped = false;
            }

            if (bestTarget is UnitStats)
                primaryTarget = target;
            else
                secondaryTarget = target;
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

        if (angle <= narrowViewAngle)
            score += narrowAngleBonus;
        else if (angle <= wideViewAngle)
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
                return dist <= AttackRange + checkRadiusOffset;
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

    void ApplySeparation()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius);

        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (Collider hit in hits)
        {
            BattleUnit unit = hit.GetComponent<BattleUnit>();

            if (unit != null && unit != this)
            {
                if (unit.unitSide == unitSide
                    && unit.isAirUnit == isAirUnit)
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
        }

        if (count > 0)
        {
            Vector3 move = separation.normalized * separationStrength;
            if (isAirUnit && airUnit != null)
            {
                airUnit.ApplySeparation(move);
            }
            else if (!isAirUnit)
            {
                agent.Move(move * Time.deltaTime);
                if (animator != null)
                    animator.SetFloat("Move", agent.velocity.magnitude);
            }
        }
    }

    void Attack()
    {
        if (isAirUnit && airUnit != null)
        {
            if (!airUnit.CanAttack()) return;

            if (airUnit.ShouldFireBurst(target))
            {
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
            return;
        }

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

    // here units get damage from enemies
    public void TakeDamage(float damage)
    {
        // Air units invulnerable during takeoff
        if (isAirUnit && airUnit != null && !airUnit.CanBeTargeted())
            return;

        unitStats.currentHealth -= damage;
        unitStats.currentHealth =
            Mathf.Clamp(unitStats.currentHealth, 0, unitStats.basicStats.maxHealth);

        if (healthBarFade != null)
        {
            healthBarFade.ShowOnHit();
            healthBarFade.Isvisible = true;
        }

        if (unitStats.currentHealth <= 0)
        {
            Die();
        }
    }

    //Destroy enemy
    public void Die()
    {
        Destroy(gameObject);
    }

    //Below code is optimized code 
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
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
    #endregion

}
