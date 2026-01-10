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
    public bool canAttackAir = false;
    public bool canAttackGround = true;

    private AirUnit airUnit;
    
    public UnitStats unitStats;

    public NavMeshAgent agent;
    public Animator animator;
    public GameObject target;

    SideScenario unitSide;
    Vector2Int currentCoord;
    private float targetCheckTimer;
    private const float targetCheckInterval = 0.5f;

    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
        unitSide = GetComponent<SideScenario>();
        unitStats.currentHealth = unitStats.maxHealth;

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
        ApplySeparation();
    }

    private void Update()
    {
        if (target != null && (!target || target.GetComponent<BattleUnit>() == null))
        {
            target = null;
            if (!isAirUnit) agent.isStopped = true;
        }

        // Air units: don't find targets until airborne
        if (isAirUnit && airUnit != null && !airUnit.IsAirborne())
        {
            if (animator != null) animator.SetFloat("Move", moveSpeed);
            return;
        }

        if (target == null)
        {
            targetCheckTimer += Time.deltaTime;
            if (targetCheckTimer >= targetCheckInterval)
            {
                targetCheckTimer = 0f;
                FindTarget();
            }

            if (!isAirUnit) agent.isStopped = true;
            if (animator != null) animator.SetFloat("Move", 0f);
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
                ApplySeparation();
                FaceTarget();
                Attack();
            }
        }

        if (!isAirUnit) UpdateTileOwnership();
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
        if (GetComponent<SideScenario>().side == Side.Player)
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
        Collider[] hits = Physics.OverlapSphere(transform.position, DetectionRange);

        BattleUnit bestTarget = null;
        float bestScore = float.MaxValue;

        Vector3 forward = transform.forward;
        Side mySide = GetComponent<SideScenario>().side;

        foreach (Collider hit in hits)
        {
            BattleUnit unit = hit.GetComponent<BattleUnit>();
            if (unit == null || unit == this) continue;

            if (unit.GetComponent<SideScenario>().side == mySide) continue;

            // Can't target air units during takeoff
            if (unit.isAirUnit)
            {
                AirUnit targetAir = unit.GetComponent<AirUnit>();
                if (targetAir != null && !targetAir.CanBeTargeted()) continue;
                if (!canAttackAir) continue;
            }

            if (!unit.isAirUnit && !canAttackGround) continue;

            Vector3 dir = unit.transform.position - transform.position;
            float distance = dir.magnitude;
            float angle = Vector3.Angle(forward, dir);

            float score = angle <= narrowViewAngle ? distance : 
                         angle <= wideViewAngle ? distance + 5f : distance + 15f;

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = unit;
            }
        }

        if (bestTarget != null)
        {
            target = bestTarget.gameObject;
            attackTimer = 0f;
            if (!isAirUnit)
            {
                agent.ResetPath();
                agent.isStopped = false;
            }
        }
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
                if (unit.GetComponent<SideScenario>().side == GetComponent<SideScenario>().side
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
                    BattleUnit enemy = target.GetComponent<BattleUnit>();
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
                BattleUnit enemy = target.GetComponent<BattleUnit>();
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

    public void TakeDamage(float damage)
    {
        // Air units invulnerable during takeoff
        if (isAirUnit && airUnit != null && !airUnit.CanBeTargeted())
            return;

        unitStats.currentHealth -= damage;
        unitStats.currentHealth = Mathf.Clamp(unitStats.currentHealth, 0, unitStats.maxHealth);

        if (healthBarFade != null)
        {
            healthBarFade.ShowOnHit();
            healthBarFade.Isvisible = true;
        }

        if (unitStats.currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        BattleUnit[] units = FindObjectsOfType<BattleUnit>();
        foreach (var unit in units)
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
        BattleUnit[] units = FindObjectsOfType<BattleUnit>();
        foreach (var unit in units)
        {
            if (unit.GetComponent<SideScenario>().side == Side.Player &&
                unit.target != null)
            {
                return true;
            }
        }
        return false;
    }

    public static bool AnyEnemyHasTarget()
    {
        BattleUnit[] units = FindObjectsOfType<BattleUnit>();
        foreach (var unit in units)
        {
            if (unit.GetComponent<SideScenario>().side == Side.Enemy &&
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
        foreach (var unit in FindObjectsOfType<BattleUnit>())
        {
            if (unit.GetComponent<SideScenario>().side == Side.Player)
                return true;
        }
        return false;
    }

    public static bool AnyEnemyAlive()
    {
        foreach (var unit in FindObjectsOfType<BattleUnit>())
        {
            if (unit.GetComponent<SideScenario>().side == Side.Enemy)
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
