// using UnityEngine; // V.9
// using System.Collections.Generic;

// public class AIUnit : MonoBehaviour
// {
//     [Header("Movement")]
//     public float moveSpeed = 2f;
//     public float yOffset = 1f; // height above tile
//     public float tileSnapThreshold = 0.1f;

//     [Header("Combat")]
//     public float attackRange = 1.2f;
//     public float attackInterval = 0.5f;
//     public int attackDamage = 10;

//     private UnitSide unitSide;
//     private Transform target;
//     private List<Vector2Int> path;
//     private int pathIndex;
//     private float lastAttackTime;

//     void Awake()
//     {
//         unitSide = GetComponent<UnitSide>();
//         if (unitSide == null)
//             Debug.LogError($"AIUnit on {name} missing UnitSide component.");
//     }

//     void Start()
//     {
//         RecalculatePath("Start()");
//         SnapToStartTile();
//     }

//     void Update()
//     {
//         if (target == null || TargetIsDead(target))
//         {
//             RecalculatePath("Target lost or dead");
//             return;
//         }

//         if (path == null || pathIndex >= path.Count)
//             return;

//         MoveAlongPath();

//         // Attack if close enough
//         float dist = Vector3.Distance(transform.position, target.position);
//         if (dist <= attackRange)
//             TryAttack(target);
//     }

//     void SnapToStartTile()
//     {
//         if (path != null && path.Count > 0)
//         {
//             GameObject startHex = HexGridManager.Instance.GetHex(path[0]);
//             if (startHex != null)
//             {
//                 Vector3 startPos = startHex.transform.position;
//                 transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
//             }
//         }
//     }

//     void MoveAlongPath()
//     {
//         if (pathIndex >= path.Count) return;

//         GameObject hex = HexGridManager.Instance.GetHex(path[pathIndex]);
//         if (hex == null)
//         {
//             RecalculatePath("Missing hex in path");
//             return;
//         }

//         Vector3 tileCenter = hex.transform.position;
//         Vector3 targetPos = new Vector3(tileCenter.x, tileCenter.y + yOffset, tileCenter.z);

//         transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

//         if (Vector3.Distance(transform.position, targetPos) < tileSnapThreshold)
//         {
//             pathIndex++;
//             if (pathIndex >= path.Count)
//                 RecalculatePath("Reached end of path");
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
//         }
//     }

//     void RecalculatePath(string reason)
//     {
//         target = FindPriorityTarget();
//         if (target == null)
//         {
//             Debug.LogWarning($"{name} ({unitSide?.side}) found no enemy targets. Reason: {reason}");
//             path = null;
//             return;
//         }

//         Vector2Int start = HexGridManager.Instance.WorldToHex(transform.position);
//         Vector2Int goal = HexGridManager.Instance.WorldToHex(target.position);

//         path = Pathfinding.BFS(start, goal);
//         pathIndex = 0;

//         Debug.Log($"{name} ({unitSide?.side}) recalculated path. Reason: {reason}. " +
//                   $"Start {start} → Goal {goal} → Steps {path?.Count}");
//     }

//     Transform FindPriorityTarget()
//     {
//         // 1) Prefer enemy main building (tag = MainBuilding)
//         var main = FindClosestEnemyByType<BuildingHealth>("MainBuilding");
//         if (main != null) return main;

//         // 2) Any enemy building
//         var bld = FindClosestEnemyByType<BuildingHealth>();
//         if (bld != null) return bld;

//         // 3) Enemy units
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
//             if (otherSide.side == unitSide.side) continue; // must be enemy

//             float d = Vector3.Distance(transform.position, obj.transform.position);
//             if (d < best)
//             {
//                 best = d;
//                 pick = obj.transform;
//             }
//         }
//         return pick;
//     }
// }





// using UnityEngine;
// using System.Collections.Generic;

// public class AIUnit : MonoBehaviour
// {
//     [Header("Movement")]
//     public float moveSpeed = 2f;
//     public float yOffset = 1f;
//     public float tileSnapThreshold = 0.1f;

//     [Header("Combat (set from UnitData)")]
//     public float attackRange = 1.2f;     // overwritten by UnitData
//     public float attackInterval = 0.5f;  // overwritten by UnitData
//     public int attackDamage = 10;        // overwritten by UnitData

//     private UnitSide unitSide;
//     private Transform target;
//     private List<Vector2Int> path;
//     private int pathIndex;
//     private float lastAttackTime;

//     void Awake()
//     {
//         unitSide = GetComponent<UnitSide>();
//         if (unitSide == null)
//             Debug.LogError($"AIUnit on {name} missing UnitSide component.");
//     }

//     void Start()
//     {
//         RecalculatePath("Start()");
//         SnapToStartTile();
//     }

//     void Update()
//     {
//         // 🔑 Check for nearby enemy units first
//         Transform nearbyEnemy = FindClosestEnemyUnitInRange();
//         if (nearbyEnemy != null)
//         {
//             target = nearbyEnemy;
//             TryAttack(target);
//             return; // stop moving while fighting
//         }

//         // Otherwise follow path toward building target
//         if (target == null || TargetIsDead(target))
//         {
//             RecalculatePath("Target lost or dead");
//             return;
//         }

//         if (path == null || pathIndex >= path.Count) return;

//         MoveAlongPath();

//         // Attack building if close enough
//         float dist = Vector3.Distance(transform.position, target.position);
//         if (dist <= attackRange)
//             TryAttack(target);
//     }

//     void SnapToStartTile()
//     {
//         if (path != null && path.Count > 0)
//         {
//             GameObject startHex = HexGridManager.Instance.GetHex(path[0]);
//             if (startHex != null)
//             {
//                 Vector3 startPos = startHex.transform.position;
//                 transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
//             }
//         }
//     }

//     void MoveAlongPath()
//     {
//         if (pathIndex >= path.Count) return;

//         GameObject hex = HexGridManager.Instance.GetHex(path[pathIndex]);
//         if (hex == null)
//         {
//             RecalculatePath("Missing hex in path");
//             return;
//         }

//         Vector3 tileCenter = hex.transform.position;
//         Vector3 targetPos = new Vector3(tileCenter.x, tileCenter.y + yOffset, tileCenter.z);

//         transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

//         if (Vector3.Distance(transform.position, targetPos) < tileSnapThreshold)
//         {
//             pathIndex++;
//             if (pathIndex >= path.Count)
//                 RecalculatePath("Reached end of path");
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
//         }
//     }

//     void RecalculatePath(string reason)
//     {
//         target = FindPriorityTarget();
//         if (target == null)
//         {
//             path = null;
//             return;
//         }

//         Vector2Int start = HexGridManager.Instance.WorldToHex(transform.position);
//         Vector2Int goal = HexGridManager.Instance.WorldToHex(target.position);

//         path = Pathfinding.BFS(start, goal);
//         pathIndex = 0;

//         Debug.Log($"{name} recalculated path. Reason: {reason}. Start {start} → Goal {goal} → Steps {path?.Count}");
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

//     // 🔑 NEW: find nearest enemy unit within attack range
//     Transform FindClosestEnemyUnitInRange()
//     {
//         UnitHealth[] allUnits = Object.FindObjectsByType<UnitHealth>(FindObjectsSortMode.None);
//         foreach (var u in allUnits)
//         {
//             var otherSide = u.GetComponent<UnitSide>();
//             if (otherSide != null && otherSide.side != unitSide.side)
//             {
//                 float d = Vector3.Distance(transform.position, u.transform.position);
//                 if (d <= attackRange)
//                     return u.transform;
//             }
//         }
//         return null;
//     }

//     void OnDrawGizmosSelected()
//     {
//         // Draw attack range when the unit is selected in the editor
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);
//     }

// }







using UnityEngine;
using System.Collections.Generic;

public class AIUnit : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float yOffset = 1f;
    public float tileSnapThreshold = 0.1f;

    [Header("Combat (set from UnitData)")]
    public float attackRange = 1.2f;     // overwritten by UnitData
    public float attackInterval = 0.5f;  // overwritten by UnitData
    public int attackDamage = 10;        // overwritten by UnitData

    private UnitSide unitSide;
    private Transform primaryTarget;   // building target
    private Transform tempTarget;      // nearby enemy unit
    private List<Vector2Int> path;
    private int pathIndex;
    private float lastAttackTime;

    void Awake()
    {
        unitSide = GetComponent<UnitSide>();
        if (unitSide == null)
            Debug.LogError($"AIUnit on {name} missing UnitSide component.");
    }

    void Start()
    {
        RecalculatePath("Start()");
        SnapToStartTile();
    }

    void Update()
    {
        // 🔎 Check for nearby enemy units first
        tempTarget = FindClosestEnemyUnitInAggroRadius();
        if (tempTarget != null && !TargetIsDead(tempTarget))
        {
            EngageTempTarget();
            return; // stop marching to building while fighting
        }

        // Otherwise follow path toward building target
        if (primaryTarget == null || TargetIsDead(primaryTarget))
        {
            RecalculatePath("Target lost or dead");
            return;
        }

        if (path == null || pathIndex >= path.Count) return;

        MoveAlongPath();

        float distToPrimary = Vector3.Distance(transform.position, primaryTarget.position);
        if (distToPrimary <= attackRange)
            TryAttack(primaryTarget);
    }

    void SnapToStartTile()
    {
        if (path != null && path.Count > 0)
        {
            GameObject startHex = HexGridManager.Instance.GetHex(path[0]);
            if (startHex != null)
            {
                Vector3 startPos = startHex.transform.position;
                transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
            }
        }
    }

    void MoveAlongPath()
    {
        if (pathIndex >= path.Count) return;

        GameObject hex = HexGridManager.Instance.GetHex(path[pathIndex]);
        if (hex == null)
        {
            RecalculatePath("Missing hex in path");
            return;
        }

        Vector3 tileCenter = hex.transform.position;
        Vector3 targetPos = new Vector3(tileCenter.x, tileCenter.y + yOffset, tileCenter.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < tileSnapThreshold)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
                RecalculatePath("Reached end of path");
        }
    }

    void EngageTempTarget()
    {
        float dist = Vector3.Distance(transform.position, tempTarget.position);
        if (dist <= attackRange)
        {
            TryAttack(tempTarget);
        }
        else
        {
            // Move toward the temp target
            transform.position = Vector3.MoveTowards(transform.position, tempTarget.position, moveSpeed * Time.deltaTime);
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
        }
    }

    void RecalculatePath(string reason)
    {
        primaryTarget = FindPriorityTarget();
        if (primaryTarget == null)
        {
            path = null;
            return;
        }

        Vector2Int start = HexGridManager.Instance.WorldToHex(transform.position);
        Vector2Int goal = HexGridManager.Instance.WorldToHex(primaryTarget.position);

        path = Pathfinding.BFS(start, goal);
        pathIndex = 0;

        Debug.Log($"{name} recalculated path. Reason: {reason}. Start {start} → Goal {goal} → Steps {path?.Count}");
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

    // 🔑 NEW: find nearest enemy unit within aggro radius
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
                if (d < best && d <= attackRange * 1.5f) // aggro radius
                {
                    best = d;
                    pick = u.transform;
                }
            }
        }
        return pick;
    }

    // 🔎 Gizmo visualization of attack range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = (unitSide != null && unitSide.side == Side.Player) ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
