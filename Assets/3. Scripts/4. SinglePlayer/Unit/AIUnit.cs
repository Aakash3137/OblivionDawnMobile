using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum UnitState { Moving, Fighting, Dead }

[RequireComponent(typeof(NavMeshAgent))]
public class AIUnit : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 1.2f;
    public float aggroRadius = 3f;
    public float attackInterval = 0.5f;
    public float attackDamage = 10;

    [Header("Animation")]
    public Animator animator;
    SideScenario unitSide;
    Transform primaryTarget;
    Transform tempTarget;
    NavMeshAgent agent;
    float lastAttackTime;
    UnitState state = UnitState.Moving;

    Vector2Int currentCoord;
    bool isDead = false;

    void Awake()
    {
        unitSide = GetComponent<SideScenario>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"{name}: NavMeshAgent missing. AIUnit cannot run.");
            enabled = false;
            return;
        }

        agent.stoppingDistance = attackRange;
        agent.radius = 0.5f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 70);

        TryAssignAnimator();
    }

    void Start()
    {
        if (CubeGridManager.Instance != null)
        {
            currentCoord = CubeGridManager.Instance.WorldToGrid(transform.position);
            EnterTile(currentCoord);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (animator != null)
            animator.SetFloat("Move", agent.velocity.magnitude);

        // --- Priority 1: Units ---
        tempTarget = FindClosestEnemyUnitInAggroRadius();
        if (tempTarget != null && !TargetIsDead(tempTarget))
        {
            state = UnitState.Fighting;
            EngageTempTarget();
            return;
        }

        // --- Priority 2: Other buildings (not MainBuilding) ---
        Transform buildingTarget = FindClosestEnemyBuildingInAggroRadius();
        if (buildingTarget != null && !TargetIsDead(buildingTarget))
        {
            tempTarget = buildingTarget;
            state = UnitState.Fighting;
            EngageTempTarget();
            return;
        }

        // --- Priority 3: MainBuilding ---
        if (primaryTarget == null || TargetIsDead(primaryTarget))
        {
            SetPrimaryTarget(); // finds MainBuilding
            return;
        }

        float dist = Vector3.Distance(transform.position, primaryTarget.position);
        if (dist <= attackRange)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
            FaceTarget(primaryTarget);
            TryAttack(primaryTarget);
        }
        else
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(primaryTarget.position);
        }

        UpdateTileOwnership();
    }

    void TryAssignAnimator()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);
    }

    void EngageTempTarget()
    {
        if (tempTarget == null) return;

        float dist = Vector3.Distance(transform.position, tempTarget.position);
        if (dist <= attackRange)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
            FaceTarget(tempTarget);
            TryAttack(tempTarget);
        }
        else
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.SetDestination(tempTarget.position);
        }
    }

    bool TargetIsDead(Transform t)
    {
        //Stats get inherited on both Buildings and Units
        var h = t.GetComponent<Stats>();        
        return h == null || h.currentHealth <= 0;
    }

    void TryAttack(Transform t)
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        var h = t.GetComponent<Stats>();
        if (h != null)
        {
            h.TakeDamage(attackDamage);
            lastAttackTime = Time.time;

            if (animator != null)
            {
                animator.SetBool("Fire", true);
                StartCoroutine(ResetFire());
            }
        }
    }

    IEnumerator ResetFire()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator != null)
            animator.SetBool("Fire", false);
    }

    void SetPrimaryTarget()
    {
        primaryTarget = FindClosestMainBuilding();
        if (primaryTarget != null)
            agent.SetDestination(primaryTarget.position);
    }

    // --- Targeting helpers ---
    Transform FindClosestEnemyUnitInAggroRadius()
    {
        UnitStats[] allUnits = Object.FindObjectsByType<UnitStats>(FindObjectsSortMode.None);
        float best = Mathf.Infinity;
        Transform pick = null;

        foreach (var u in allUnits)
        {
            var otherSide = u.GetComponent<SideScenario>();
            if (otherSide != null && otherSide.side != unitSide.side)
            {
                float d = Vector3.Distance(transform.position, u.transform.position);
                if (d < best && d <= aggroRadius)
                {
                    best = d;
                    pick = u.transform;
                }
            }
        }
        return pick;
    }

    Transform FindClosestEnemyBuildingInAggroRadius()
    {
        BuildingStats[] allBuildings = Object.FindObjectsByType<BuildingStats>(FindObjectsSortMode.None);
        float best = Mathf.Infinity;
        Transform pick = null;

        foreach (var b in allBuildings)
        {
            if (b.CompareTag("MainBuilding")) continue; // skip main building

            var otherSide = b.GetComponent<SideScenario>();
            if (otherSide != null && otherSide.side != unitSide.side)
            {
                float d = Vector3.Distance(transform.position, b.transform.position);
                if (d < best && d <= aggroRadius)
                {
                    best = d;
                    pick = b.transform;
                }
            }
        }
        return pick;
    }

    Transform FindClosestMainBuilding()
    {
        BuildingStats[] allBuildings = Object.FindObjectsByType<BuildingStats>(FindObjectsSortMode.None);
        float best = Mathf.Infinity;
        Transform pick = null;

        foreach (var b in allBuildings)
        {
            if (!b.CompareTag("MainBuilding")) continue;

            var otherSide = b.GetComponent<SideScenario>();
            if (otherSide != null && otherSide.side != unitSide.side)
            {
                float d = Vector3.Distance(transform.position, b.transform.position);
                if (d < best)
                {
                    best = d;
                    pick = b.transform;
                }
            }
        }
        return pick;
    }

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

    void FaceTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
        }
    }

    public void OnHit()
    {
        if (animator != null)
            animator.SetTrigger("Hit");
    }

    public void OnDeath()
    {
        if (animator != null) animator.SetTrigger("Death");
        agent.isStopped = true;
        agent.enabled = false;
        isDead = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);

        if (primaryTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, primaryTarget.position);
        }
        if (tempTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, tempTarget.position);
        }
    }
}
