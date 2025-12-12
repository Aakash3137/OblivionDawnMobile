// using UnityEngine;
// using UnityEngine.AI;
// using System.Collections;
// using System.Collections.Generic;

// public enum UnitState { Moving, Fighting }

// [RequireComponent(typeof(NavMeshAgent))]
// public class AIUnit : MonoBehaviour
// {
//     [Header("Combat")]
//     public float attackRange = 1.2f;
//     public float aggroRadius = 3f;
//     public float attackInterval = 0.5f;
//     public int attackDamage = 10;

//     [Header("Animation")]
//     public Animator animator;  
//     private UnitSide unitSide;
//     private Transform primaryTarget;
//     private Transform tempTarget;
//     private NavMeshAgent agent;
//     private float lastAttackTime;
//     private UnitState state = UnitState.Moving;

//     private Vector2Int currentCoord;

//     void Awake()
//     {
//         unitSide = GetComponent<UnitSide>();
//         agent = GetComponent<NavMeshAgent>();
//         agent.stoppingDistance = attackRange;

//         // Only auto‑assign if you forgot to set it manually
//         if (animator == null)
//             animator = GetComponent<Animator>();
//     }

//     void Start()
//     {
//         currentCoord = HexGridManager.Instance.WorldToHex(transform.position);
//         EnterTile(currentCoord);
//     }

//     void Update()
//     {
//         // --- Movement animation ---
//         if (animator != null)
//             animator.SetFloat("Move", agent.velocity.magnitude);

//         // --- Combat state ---
//         if (state == UnitState.Fighting)
//         {
//             if (tempTarget == null || TargetIsDead(tempTarget))
//             {
//                 tempTarget = null;
//                 state = UnitState.Moving;
//                 SetPrimaryTarget();
//             }
//             else
//             {
//                 EngageTempTarget();
//             }

//             UpdateTileOwnership();
//             return;
//         }

//         // --- Aggro check ---
//         tempTarget = FindClosestEnemyUnitInAggroRadius();
//         if (tempTarget != null && !TargetIsDead(tempTarget))
//         {
//             state = UnitState.Fighting;
//             return;
//         }

//         // --- March toward building ---
//         if (primaryTarget == null || TargetIsDead(primaryTarget))
//         {
//             SetPrimaryTarget();
//             return;
//         }

//         agent.isStopped = false;
//         agent.SetDestination(primaryTarget.position);

//         if (Vector3.Distance(transform.position, primaryTarget.position) <= attackRange)
//             TryAttack(primaryTarget);

//         UpdateTileOwnership();
//     }

//     void EngageTempTarget()
//     {
//         float dist = Vector3.Distance(transform.position, tempTarget.position);
//         if (dist <= attackRange)
//         {
//             agent.isStopped = true;
//             TryAttack(tempTarget);
//         }
//         else
//         {
//             agent.isStopped = false;
//             agent.SetDestination(tempTarget.position);
//         }
//     }

//     bool TargetIsDead(Transform t)
//     {
//         var h = t.GetComponent<Health>();
//         return h == null || h.currentHealth <= 0;
//     }

//     void TryAttack(Transform t)
//     {
//         if (Time.time - lastAttackTime < attackInterval) return;
//         var h = t.GetComponent<Health>();
//         if (h != null)
//         {
//             h.TakeDamage(attackDamage);
//             lastAttackTime = Time.time;

//             if (animator != null)
//             {
//                 animator.SetBool("Fire", true);
//                 StartCoroutine(ResetFire());
//             }
//         }
//     }

//     IEnumerator ResetFire()
//     {
//         yield return new WaitForSeconds(0.1f);
//         if (animator != null)
//             animator.SetBool("Fire", false);
//     }

//     void SetPrimaryTarget()
//     {
//         primaryTarget = FindPriorityTarget();
//         if (primaryTarget != null)
//             agent.SetDestination(primaryTarget.position);
//     }

//     Transform FindPriorityTarget()
//     {
//         var main = FindClosestEnemyByType<BuildingHealth>("MainBuilding");
//         if (main != null) return main;

//         var bld = FindClosestEnemyByType<BuildingHealth>();
//         if (bld != null) return bld;

//         return FindClosestEnemyByType<UnitHealth>();
//     }

//     Transform FindClosestEnemyByType<T>(string requireTag = null) where T : Component
//     {
//         T[] objs = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
//         float best = Mathf.Infinity;
//         Transform pick = null;

//         foreach (var obj in objs)
//         {
//             if (!string.IsNullOrEmpty(requireTag) && !obj.CompareTag(requireTag)) continue;

//             var otherSide = obj.GetComponent<UnitSide>();
//             if (otherSide == null || unitSide == null) continue;
//             if (otherSide.side == unitSide.side) continue;

//             float d = Vector3.Distance(transform.position, obj.transform.position);
//             if (d < best)
//             {
//                 best = d;
//                 pick = obj.transform;
//             }
//         }
//         return pick;
//     }

//     Transform FindClosestEnemyUnitInAggroRadius()
//     {
//         UnitHealth[] allUnits = Object.FindObjectsByType<UnitHealth>(FindObjectsSortMode.None);
//         float best = Mathf.Infinity;
//         Transform pick = null;

//         foreach (var u in allUnits)
//         {
//             var otherSide = u.GetComponent<UnitSide>();
//             if (otherSide != null && otherSide.side != unitSide.side)
//             {
//                 float d = Vector3.Distance(transform.position, u.transform.position);
//                 if (d < best && d <= aggroRadius)
//                 {
//                     best = d;
//                     pick = u.transform;
//                 }
//             }
//         }
//         return pick;
//     }

//     void UpdateTileOwnership()
//     {
//         Vector2Int coord = HexGridManager.Instance.WorldToHex(transform.position);
//         if (coord != currentCoord)
//         {
//             LeaveTile(currentCoord);
//             currentCoord = coord;
//             EnterTile(currentCoord);
//         }
//     }

//     void EnterTile(Vector2Int coord)
//     {
//         TileManager.Instance.TryEnterTile(gameObject, coord);

//         var tileGO = HexGridManager.Instance.GetHex(coord);
//         var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
//         if (tile != null)
//         {
//             tile.Occupy(unitSide);
//         }
//     }

//     void LeaveTile(Vector2Int coord)
//     {
//         TileManager.Instance.LeaveTile(coord, gameObject);

//         var tileGO = HexGridManager.Instance.GetHex(coord);
//         var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
//         if (tile != null)
//         {
//             tile.Vacate(unitSide);
//         }
//     }

//     // --- External hooks for Hit/Death ---
//     public void OnHit()
//     {
//         if (animator != null)
//             animator.SetTrigger("Hit");
//     }

//     public void OnDeath()
//     {
//         if (animator != null)
//             animator.SetTrigger("Death");

//         agent.isStopped = true;
//         enabled = false;
//     }

//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);

//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, aggroRadius);
//     }
// }










// using UnityEngine;
// using UnityEngine.AI;
// using System.Collections;
// using System.Collections.Generic;

// public enum UnitState { Moving, Fighting }

// [RequireComponent(typeof(NavMeshAgent))]
// public class AIUnit : MonoBehaviour
// {
//     [Header("Combat")]
//     public float attackRange = 1.2f;
//     public float aggroRadius = 3f;
//     public float attackInterval = 0.5f;
//     public int attackDamage = 10;

//     [Header("Animation")]
//     public Animator animator;  
//     private UnitSide unitSide;
//     private Transform primaryTarget;
//     private Transform tempTarget;
//     private NavMeshAgent agent;
//     private float lastAttackTime;
//     private UnitState state = UnitState.Moving;

//     private Vector2Int currentCoord;

//     void Awake()
//     {
//         unitSide = GetComponent<UnitSide>();
//         agent = GetComponent<NavMeshAgent>();
//         agent.stoppingDistance = attackRange;

//         // Try to assign animator immediately
//         TryAssignAnimator();
//     }

//     void Start()
//     {
//         // Safe grid entry
//         if (HexGridManager.Instance != null)
//         {
//             currentCoord = HexGridManager.Instance.WorldToHex(transform.position);
//             EnterTile(currentCoord);
//         }
//         else
//         {
//             Debug.LogWarning($"{name}: HexGridManager.Instance is null, skipping tile entry.");
//         }
//     }

//     void Update()
//     {
//         // Keep trying to assign animator until found (handles late‑spawned children)
//         if (animator == null)
//             TryAssignAnimator();

//         // --- Movement animation ---
//         if (animator != null)
//             animator.SetFloat("Move", agent.velocity.magnitude);

//         // --- Combat state ---
//         if (state == UnitState.Fighting)
//         {
//             if (tempTarget == null || TargetIsDead(tempTarget))
//             {
//                 tempTarget = null;
//                 state = UnitState.Moving;
//                 SetPrimaryTarget();
//             }
//             else
//             {
//                 EngageTempTarget();
//             }

//             UpdateTileOwnership();
//             return;
//         }

//         // --- Aggro check ---
//         tempTarget = FindClosestEnemyUnitInAggroRadius();
//         if (tempTarget != null && !TargetIsDead(tempTarget))
//         {
//             state = UnitState.Fighting;
//             return;
//         }

//         // --- March toward building ---
//         if (primaryTarget == null || TargetIsDead(primaryTarget))
//         {
//             SetPrimaryTarget();
//             return;
//         }

//         agent.isStopped = false;
//         agent.SetDestination(primaryTarget.position);

//         if (Vector3.Distance(transform.position, primaryTarget.position) <= attackRange)
//             TryAttack(primaryTarget);

//         UpdateTileOwnership();
//     }

//     // --- Animator helpers ---
//     void TryAssignAnimator()
//     {
//         // Explicit path: AIUnit → Child2 (index 1) → SubChild2 (index 1)
//         if (transform.childCount > 1)
//         {
//             Transform child2 = transform.GetChild(1);
//             if (child2.childCount > 1)
//             {
//                 Transform subChild2 = child2.GetChild(1);
//                 Animator found = subChild2.GetComponent<Animator>();
//                 if (found != null)
//                 {
//                     animator = found;
//                     Debug.Log($"{name}: Animator bound from Child2/SubChild2");
//                     return;
//                 }
//             }
//         }

//         // Fallback: recursive search
//         if (animator == null)
//             animator = GetComponentInChildren<Animator>(true);
//     }

//     // --- Combat logic ---
//     void EngageTempTarget()
//     {
//         float dist = Vector3.Distance(transform.position, tempTarget.position);
//         if (dist <= attackRange)
//         {
//             agent.isStopped = true;
//             TryAttack(tempTarget);
//         }
//         else
//         {
//             agent.isStopped = false;
//             agent.SetDestination(tempTarget.position);
//         }
//     }

//     bool TargetIsDead(Transform t)
//     {
//         var h = t.GetComponent<Health>();
//         return h == null || h.currentHealth <= 0;
//     }

//     void TryAttack(Transform t)
//     {
//         if (Time.time - lastAttackTime < attackInterval) return;
//         var h = t.GetComponent<Health>();
//         if (h != null)
//         {
//             h.TakeDamage(attackDamage);
//             lastAttackTime = Time.time;

//             if (animator != null)
//             {
//                 animator.SetBool("Fire", true);
//                 StartCoroutine(ResetFire());
//             }
//         }
//     }

//     IEnumerator ResetFire()
//     {
//         yield return new WaitForSeconds(0.1f);
//         if (animator != null)
//             animator.SetBool("Fire", false);
//     }

//     void SetPrimaryTarget()
//     {
//         primaryTarget = FindPriorityTarget();
//         if (primaryTarget != null)
//             agent.SetDestination(primaryTarget.position);
//     }

//     Transform FindPriorityTarget()
//     {
//         var main = FindClosestEnemyByType<BuildingHealth>("MainBuilding");
//         if (main != null) return main;

//         var bld = FindClosestEnemyByType<BuildingHealth>();
//         if (bld != null) return bld;

//         return FindClosestEnemyByType<UnitHealth>();
//     }

//     Transform FindClosestEnemyByType<T>(string requireTag = null) where T : Component
//     {
//         T[] objs = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
//         float best = Mathf.Infinity;
//         Transform pick = null;

//         foreach (var obj in objs)
//         {
//             if (!string.IsNullOrEmpty(requireTag) && !obj.CompareTag(requireTag)) continue;

//             var otherSide = obj.GetComponent<UnitSide>();
//             if (otherSide == null || unitSide == null) continue;
//             if (otherSide.side == unitSide.side) continue;

//             float d = Vector3.Distance(transform.position, obj.transform.position);
//             if (d < best)
//             {
//                 best = d;
//                 pick = obj.transform;
//             }
//         }
//         return pick;
//     }

//     Transform FindClosestEnemyUnitInAggroRadius()
//     {
//         UnitHealth[] allUnits = Object.FindObjectsByType<UnitHealth>(FindObjectsSortMode.None);
//         float best = Mathf.Infinity;
//         Transform pick = null;

//         foreach (var u in allUnits)
//         {
//             var otherSide = u.GetComponent<UnitSide>();
//             if (otherSide != null && otherSide.side != unitSide.side)
//             {
//                 float d = Vector3.Distance(transform.position, u.transform.position);
//                 if (d < best && d <= aggroRadius)
//                 {
//                     best = d;
//                     pick = u.transform;
//                 }
//             }
//         }
//         return pick;
//     }

//     // --- Tile ownership ---
//     void UpdateTileOwnership()
//     {
//         if (HexGridManager.Instance == null) return;

//         Vector2Int coord = HexGridManager.Instance.WorldToHex(transform.position);
//         if (coord != currentCoord)
//         {
//             LeaveTile(currentCoord);
//             currentCoord = coord;
//             EnterTile(currentCoord);
//         }
//     }

//     void EnterTile(Vector2Int coord)
//     {
//         if (TileManager.Instance != null)
//             TileManager.Instance.TryEnterTile(gameObject, coord);

//         var tileGO = HexGridManager.Instance?.GetHex(coord);
//         var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
//         if (tile != null)
//             tile.Occupy(unitSide);
//     }

//     void LeaveTile(Vector2Int coord)
//     {
//         if (TileManager.Instance != null)
//             TileManager.Instance.LeaveTile(coord, gameObject);

//         var tileGO = HexGridManager.Instance?.GetHex(coord);
//         var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
//         if (tile != null)
//             tile.Vacate(unitSide);
//     }

//     // --- External hooks ---
//     public void OnHit()
//     {
//         if (animator != null)
//             animator.SetTrigger("Hit");
//     }

//     public void OnDeath()
//     {
//         if (animator != null)
//             animator.SetTrigger("Death");

//         agent.isStopped = true;
//         enabled = false;
//     }

//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);

//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, aggroRadius);
//     }
// }






using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum UnitState { Moving, Fighting }

[RequireComponent(typeof(NavMeshAgent))]
public class AIUnit : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 1.2f;
    public float aggroRadius = 3f;
    public float attackInterval = 0.5f;
    public int attackDamage = 10;

    [Header("Animation")]
    public Animator animator;
    private UnitSide unitSide;
    private Transform primaryTarget;
    private Transform tempTarget;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private UnitState state = UnitState.Moving;

    private Vector2Int currentCoord;

    void Awake()
    {
        unitSide = GetComponent<UnitSide>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;

        TryAssignAnimator();
    }

    void Start()
    {
        // Safe grid entry
        if (CubeGridManager.Instance != null)
        {
            currentCoord = CubeGridManager.Instance.WorldToGrid(transform.position);
            EnterTile(currentCoord);
        }
        else
        {
            Debug.LogWarning($"{name}: CubeGridManager.Instance is null, skipping tile entry.");
        }
    }

    void Update()
    {
        if (animator == null)
            TryAssignAnimator();

        if (animator != null)
            animator.SetFloat("Move", agent.velocity.magnitude);

        if (state == UnitState.Fighting)
        {
            if (tempTarget == null || TargetIsDead(tempTarget))
            {
                tempTarget = null;
                state = UnitState.Moving;
                SetPrimaryTarget();
            }
            else
            {
                EngageTempTarget();
            }

            UpdateTileOwnership();
            return;
        }

        tempTarget = FindClosestEnemyUnitInAggroRadius();
        if (tempTarget != null && !TargetIsDead(tempTarget))
        {
            state = UnitState.Fighting;
            return;
        }

        if (primaryTarget == null || TargetIsDead(primaryTarget))
        {
            SetPrimaryTarget();
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(primaryTarget.position);

        if (Vector3.Distance(transform.position, primaryTarget.position) <= attackRange)
            TryAttack(primaryTarget);

        UpdateTileOwnership();
    }

    void TryAssignAnimator()
    {
        if (transform.childCount > 1)
        {
            Transform child2 = transform.GetChild(1);
            if (child2.childCount > 1)
            {
                Transform subChild2 = child2.GetChild(1);
                Animator found = subChild2.GetComponent<Animator>();
                if (found != null)
                {
                    animator = found;
                    Debug.Log($"{name}: Animator bound from Child2/SubChild2");
                    return;
                }
            }
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);
    }

    void EngageTempTarget()
    {
        float dist = Vector3.Distance(transform.position, tempTarget.position);
        if (dist <= attackRange)
        {
            agent.isStopped = true;
            TryAttack(tempTarget);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(tempTarget.position);
        }
    }

    bool TargetIsDead(Transform t)
    {
        var h = t.GetComponent<Health>();
        return h == null || h.currentHealth <= 0;
    }

    void TryAttack(Transform t)
    {
        if (Time.time - lastAttackTime < attackInterval) return;
        var h = t.GetComponent<Health>();
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
        primaryTarget = FindPriorityTarget();
        if (primaryTarget != null)
            agent.SetDestination(primaryTarget.position);
    }

    Transform FindPriorityTarget()
    {
        var main = FindClosestEnemyByType<BuildingHealth>("MainBuilding");
        if (main != null) return main;

        var bld = FindClosestEnemyByType<BuildingHealth>();
        if (bld != null) return bld;

        return FindClosestEnemyByType<UnitHealth>();
    }

    Transform FindClosestEnemyByType<T>(string requireTag = null) where T : Component
    {
        T[] objs = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
        float best = Mathf.Infinity;
        Transform pick = null;

        foreach (var obj in objs)
        {
            if (!string.IsNullOrEmpty(requireTag) && !obj.CompareTag(requireTag)) continue;

            var otherSide = obj.GetComponent<UnitSide>();
            if (otherSide == null || unitSide == null) continue;
            if (otherSide.side == unitSide.side) continue;

            float d = Vector3.Distance(transform.position, obj.transform.position);
            if (d < best)
            {
                best = d;
                pick = obj.transform;
            }
        }
        return pick;
    }

    Transform FindClosestEnemyUnitInAggroRadius()
    {
        UnitHealth[] allUnits = Object.FindObjectsByType<UnitHealth>(FindObjectsSortMode.None);
        float best = Mathf.Infinity;
        Transform pick = null;

        foreach (var u in allUnits)
        {
            var otherSide = u.GetComponent<UnitSide>();
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

    // --- Tile ownership (CubeGrid version) ---
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

        var tileGO = CubeGridManager.Instance?.GetCube(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null)
            tile.Occupy(unitSide);
    }

    void LeaveTile(Vector2Int coord)
    {
        if (TileManager.Instance != null)
            TileManager.Instance.LeaveTile(coord, gameObject);

        var tileGO = CubeGridManager.Instance?.GetCube(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null)
            tile.Vacate(unitSide);
    }

    public void OnHit()
    {
        if (animator != null)
            animator.SetTrigger("Hit");
    }

    public void OnDeath()
    {
        if (animator != null)
            animator.SetTrigger("Death");

        agent.isStopped = true;
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}
