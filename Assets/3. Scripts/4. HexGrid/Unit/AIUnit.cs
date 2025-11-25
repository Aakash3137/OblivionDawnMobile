// using UnityEngine; // V.10
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
//     private Transform primaryTarget;   // building target
//     private Transform tempTarget;      // nearby enemy unit
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
//         // Check for nearby enemy units first
//         tempTarget = FindClosestEnemyUnitInAggroRadius();
//         if (tempTarget != null && !TargetIsDead(tempTarget))
//         {
//             EngageTempTarget();
//             return; // stop marching to building while fighting
//         }

//         // Otherwise follow path toward building target
//         if (primaryTarget == null || TargetIsDead(primaryTarget))
//         {
//             RecalculatePath("Target lost or dead");
//             return;
//         }

//         if (path == null || pathIndex >= path.Count) return;

//         MoveAlongPath();

//         float distToPrimary = Vector3.Distance(transform.position, primaryTarget.position);
//         if (distToPrimary <= attackRange)
//             TryAttack(primaryTarget);
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

//     void EngageTempTarget()
//     {
//         float dist = Vector3.Distance(transform.position, tempTarget.position);
//         if (dist <= attackRange)
//         {
//             TryAttack(tempTarget);
//         }
//         else
//         {
//             // Move toward the temp target
//             transform.position = Vector3.MoveTowards(transform.position, tempTarget.position, moveSpeed * Time.deltaTime);
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
//         primaryTarget = FindPriorityTarget();
//         if (primaryTarget == null)
//         {
//             path = null;
//             return;
//         }

//         Vector2Int start = HexGridManager.Instance.WorldToHex(transform.position);
//         Vector2Int goal = HexGridManager.Instance.WorldToHex(primaryTarget.position);

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

//     // NEW: find nearest enemy unit within aggro radius
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
//                 if (d < best && d <= attackRange * 1.5f) // aggro radius
//                 {
//                     best = d;
//                     pick = u.transform;
//                 }
//             }
//         }
//         return pick;
//     }

//     // Gizmo visualization of attack range
//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = (unitSide != null && unitSide.side == Side.Player) ? Color.blue : Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);
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
//     public float aggroRadius = 3f;       // NEW: detection radius
//     public float attackInterval = 0.5f;  // overwritten by UnitData
//     public int attackDamage = 10;        // overwritten by UnitData

//     private UnitSide unitSide;
//     private Transform primaryTarget;   // building target
//     private Transform tempTarget;      // nearby enemy unit
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
//         // --- Combat check first ---
//         if (tempTarget != null && TargetIsDead(tempTarget))
//         {
//             tempTarget = null;
//             RecalculatePath("Enemy defeated, resuming march");
//         }

//         if (tempTarget == null)
//             tempTarget = FindClosestEnemyUnitInAggroRadius();

//         if (tempTarget != null && !TargetIsDead(tempTarget))
//         {
//             EngageTempTarget();
//             return; // stop marching to building while fighting
//         }

//         // --- Otherwise follow path toward building target ---
//         if (primaryTarget == null || TargetIsDead(primaryTarget))
//         {
//             RecalculatePath("Target lost or dead");
//             return;
//         }

//         if (path == null || pathIndex >= path.Count) return;

//         MoveAlongPath();

//         float distToPrimary = Vector3.Distance(transform.position, primaryTarget.position);
//         if (distToPrimary <= attackRange)
//             TryAttack(primaryTarget);
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

//     void EngageTempTarget()
//     {
//         float dist = Vector3.Distance(transform.position, tempTarget.position);

//         if (dist <= attackRange)
//         {
//             // STOP moving — just attack
//             TryAttack(tempTarget);
//         }
//         else
//         {
//             // Move toward the temp target until in range
//             transform.position = Vector3.MoveTowards(
//                 transform.position,
//                 tempTarget.position,
//                 moveSpeed * Time.deltaTime
//             );
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
//         primaryTarget = FindPriorityTarget();
//         if (primaryTarget == null)
//         {
//             path = null;
//             return;
//         }

//         Vector2Int start = HexGridManager.Instance.WorldToHex(transform.position);
//         Vector2Int goal = HexGridManager.Instance.WorldToHex(primaryTarget.position);

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

//     // NEW: find nearest enemy unit within aggro radius
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
//                 if (d < best && d <= aggroRadius) // use dedicated aggro radius
//                 {
//                     best = d;
//                     pick = u.transform;
//                 }
//             }
//         }
//         return pick;
//     }

//     // Gizmo visualization of attack range
//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = (unitSide != null && unitSide.side == Side.Player) ? Color.blue : Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, aggroRadius);
//     }
// }







// using UnityEngine;
// using System.Collections.Generic;

// // Enum to track what the unit is currently doing
// public enum UnitState { Moving, Fighting }

// public class AIUnit : MonoBehaviour
// {
//     [Header("Movement")]
//     public float moveSpeed = 2f;             
//     public float yOffset = 1f;               // Offset so unit floats above tile
//     public float tileSnapThreshold = 0.1f;   // How close before snapping to tile center

//     [Header("Combat (set from UnitData)")]
//     public float attackRange = 1.2f;         // Distance required to hit target
//     public float aggroRadius = 3f;           // Detection radius for enemies
//     public float attackInterval = 0.5f;      // Time between attacks
//     public int attackDamage = 10;            // Damage per attack

//     private UnitSide unitSide;               // Which side this unit belongs to
//     private Transform primaryTarget;         // Building target
//     private Transform tempTarget;            // Nearby enemy unit
//     private List<Vector2Int> path;           // Path to building
//     private int pathIndex;                   // Current step in path
//     private float lastAttackTime;            // Timer for attack cooldown
//     private UnitState state = UnitState.Moving; // Current state (Moving or Fighting)

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
//         // --- FIGHTING STATE ---
//         if (state == UnitState.Fighting)
//         {
//             // If enemy is gone or dead, return to Moving state
//             if (tempTarget == null || TargetIsDead(tempTarget))
//             {
//                 tempTarget = null;
//                 state = UnitState.Moving;
//                 RecalculatePath("Enemy defeated, resuming march");
//             }
//             else
//             {
//                 // Keep engaging the enemy
//                 EngageTempTarget();
//             }
//             return; // Skip building logic while fighting
//         }

//         // --- MOVING STATE ---
//         // Look for nearby enemies
//         tempTarget = FindClosestEnemyUnitInAggroRadius();
//         if (tempTarget != null && !TargetIsDead(tempTarget))
//         {
//             state = UnitState.Fighting; // Switch to combat
//             return;
//         }

//         // If no building target, recalc
//         if (primaryTarget == null || TargetIsDead(primaryTarget))
//         {
//             RecalculatePath("Target lost or dead");
//             return;
//         }

//         // Follow path toward building
//         if (path == null || pathIndex >= path.Count) return;
//         MoveAlongPath();

//         // Attack building if in range
//         float distToPrimary = Vector3.Distance(transform.position, primaryTarget.position);
//         if (distToPrimary <= attackRange)
//             TryAttack(primaryTarget);
//     }

//     // Snap unit to starting tile position
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

//     // Move along path toward building
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

//     // Engage enemy unit
//     void EngageTempTarget()
//     {
//         float dist = Vector3.Distance(transform.position, tempTarget.position);

//         if (dist <= attackRange)
//         {
//             // Stop moving and attack
//             TryAttack(tempTarget);
//         }
//         else
//         {
//             // Move closer until in attack range
//             transform.position = Vector3.MoveTowards(
//                 transform.position,
//                 tempTarget.position,
//                 moveSpeed * Time.deltaTime
//             );
//         }
//     }

//     // Check if target is dead
//     bool TargetIsDead(Transform t)
//     {
//         var h = t.GetComponent<Health>();
//         return h == null || h.currentHealth <= 0;
//     }

//     // Attack target if cooldown ready
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

//     // Recalculate path to building
//     void RecalculatePath(string reason)
//     {
//         primaryTarget = FindPriorityTarget();
//         if (primaryTarget == null)
//         {
//             path = null;
//             return;
//         }

//         Vector2Int start = HexGridManager.Instance.WorldToHex(transform.position);
//         Vector2Int goal = HexGridManager.Instance.WorldToHex(primaryTarget.position);

//         path = Pathfinding.BFS(start, goal);
//         pathIndex = 0;

//         Debug.Log($"{name} recalculated path. Reason: {reason}. Start {start} → Goal {goal} → Steps {path?.Count}");
//     }

//     // Pick best target (main building > other buildings > units)
//     Transform FindPriorityTarget()
//     {
//         var main = FindClosestEnemyByType<BuildingHealth>("MainBuilding");
//         if (main != null) return main;

//         var bld = FindClosestEnemyByType<BuildingHealth>();
//         if (bld != null) return bld;

//         return FindClosestEnemyByType<UnitHealth>();
//     }

//     // Find closest enemy of a given type
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

//     // Find nearest enemy unit within aggro radius
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

//     // Draw gizmos for debugging ranges
//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = (unitSide != null && unitSide.side == Side.Player) ? Color.blue : Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange); // attack range
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, aggroRadius); // detection radius
//     }
// }






// using UnityEngine;
// using System.Collections.Generic;

// public enum UnitState { Moving, Fighting }

// public class AIUnit : MonoBehaviour
// {
//     [Header("Movement")]
//     public float moveSpeed = 2f;
//     public float yOffset = 1f;
//     public float tileSnapThreshold = 0.1f;

//     [Header("Combat (set from UnitData)")]
//     public float attackRange = 1.2f;     // melee range
//     public float aggroRadius = 3f;       // detection radius
//     public float attackInterval = 0.5f;
//     public int attackDamage = 10;

//     private UnitSide unitSide;
//     private Transform primaryTarget;      // building
//     private Transform tempTarget;         // enemy unit
//     private List<Vector2Int> path;
//     private int pathIndex;
//     private float lastAttackTime;
//     private UnitState state = UnitState.Moving;

//     // Track current tile coordinate to manage enter/exit
//     private Vector2Int currentCoord;

//     void Awake()
//     {
//         unitSide = GetComponent<UnitSide>();
//         if (unitSide == null)
//             Debug.LogError($"AIUnit on {name} missing UnitSide component.");
//     }

//     void Start()
//     {
//         // Initialize coord, register occupancy
//         currentCoord = HexGridManager.Instance.WorldToHex(transform.position);
//         EnterTile(currentCoord);

//         RecalculatePath("Start()");
//         SnapToStartTile();
//     }

//     void Update()
//     {
//         if (state == UnitState.Fighting)
//         {
//             if (tempTarget == null || TargetIsDead(tempTarget))
//             {
//                 tempTarget = null;
//                 state = UnitState.Moving;

//                 // After combat ends, ensure tile ownership reflects survivor side
//                 UpdateTileOwnershipAtCurrentCoord();

//                 RecalculatePath("Enemy defeated, resuming march");
//             }
//             else
//             {
//                 EngageTempTargetStopAdjacent();
//             }
//             return;
//         }

//         // Moving state: look for enemies in aggro radius
//         tempTarget = FindClosestEnemyUnitInAggroRadius();
//         if (tempTarget != null && !TargetIsDead(tempTarget))
//         {
//             state = UnitState.Fighting;
//             return;
//         }

//         // March toward building
//         if (primaryTarget == null || TargetIsDead(primaryTarget))
//         {
//             RecalculatePath("Target lost or dead");
//             return;
//         }

//         if (path == null || pathIndex >= path.Count) return;
//         MoveAlongPath();

//         float distToPrimary = Vector3.Distance(transform.position, primaryTarget.position);
//         if (distToPrimary <= attackRange)
//             TryAttack(primaryTarget);
//     }

//     void SnapToStartTile()
//     {
//         var hexGO = HexGridManager.Instance.GetHex(currentCoord);
//         if (hexGO != null)
//         {
//             var p = hexGO.transform.position;
//             transform.position = new Vector3(p.x, p.y + yOffset, p.z);
//         }
//     }

//     void MoveAlongPath()
//     {
//         if (pathIndex >= path.Count) return;

//         var nextCoord = path[pathIndex];
//         var hex = HexGridManager.Instance.GetHex(nextCoord);
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
//             // Enter new tile, leave previous
//             LeaveTile(currentCoord);
//             currentCoord = nextCoord;
//             EnterTile(currentCoord);

//             pathIndex++;
//             if (pathIndex >= path.Count)
//                 RecalculatePath("Reached end of path");
//         }
//     }

//     // Stop next to enemy (adjacent), not on top
//     void EngageTempTargetStopAdjacent()
//     {
//         Vector2Int myHex = currentCoord;
//         Vector2Int enemyHex = HexGridManager.Instance.WorldToHex(tempTarget.position);

//         if (HexGridManager.Instance.AreAdjacent(myHex, enemyHex))
//         {
//             // Already adjacent: stand and fight, do not move further
//             TryAttack(tempTarget);
//             return;
//         }

//         // Path toward enemy tile but stop adjacent
//         // Strategy: path to enemyHex, but consume steps until you reach a coord adjacent to enemyHex
//         var approachPath = Pathfinding.BFS(myHex, enemyHex);
//         if (approachPath == null || approachPath.Count == 0) return;

//         // Next step toward enemy
//         var next = approachPath[Mathf.Min(1, approachPath.Count - 1)];

//         // If next is adjacent to enemy, move to that and stop
//         if (HexGridManager.Instance.AreAdjacent(next, enemyHex))
//         {
//             MoveToCoord(next);
//         }
//         else
//         {
//             // Keep approaching
//             MoveToCoord(next);
//         }
//     }

//     void MoveToCoord(Vector2Int coord)
//     {
//         var hex = HexGridManager.Instance.GetHex(coord);
//         if (hex == null) return;

//         Vector3 tileCenter = hex.transform.position;
//         Vector3 targetPos = new Vector3(tileCenter.x, tileCenter.y + yOffset, tileCenter.z);

//         transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

//         if (Vector3.Distance(transform.position, targetPos) < tileSnapThreshold)
//         {
//             LeaveTile(currentCoord);
//             currentCoord = coord;
//             EnterTile(currentCoord);
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
//         primaryTarget = FindPriorityTarget();
//         if (primaryTarget == null)
//         {
//             path = null;
//             return;
//         }

//         Vector2Int start = currentCoord;
//         Vector2Int goal = HexGridManager.Instance.WorldToHex(primaryTarget.position);

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

//     // Tile occupancy helpers
//     void EnterTile(Vector2Int coord)
//     {
//         // Register in TileManager (prevent overlapping same tile)
//         if (!TileManager.Instance.TryEnterTile(gameObject, coord))
//         {
//             // Another unit is already on this tile — do not stack; combat will be adjacent, so avoid moving onto occupied tile
//         }

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

//     void UpdateTileOwnershipAtCurrentCoord()
//     {
//         var tileGO = HexGridManager.Instance.GetHex(currentCoord);
//         var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
//         if (tile != null)
//         {
//             // If this unit survives, and stands on opposite-owned tile, flip ownership
//             if (tile.ownerSide != unitSide.side)
//                 tile.SetOwner(unitSide.side);
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = (unitSide != null && unitSide.side == Side.Player) ? Color.blue : Color.red;
//         Gizmos.DrawWireSphere(transform.position, attackRange);
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, aggroRadius);
//     }
// }






using UnityEngine;
using System.Collections.Generic;

public enum UnitState { Moving, Fighting }

public class AIUnit : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float yOffset = 1f;
    public float tileSnapThreshold = 0.1f;

    [Header("Combat")]
    public float attackRange = 1.2f;
    public float aggroRadius = 3f;
    public float attackInterval = 0.5f;
    public int attackDamage = 10;

    private UnitSide unitSide;
    private Transform primaryTarget;
    private Transform tempTarget;
    private List<Vector2Int> path;
    private int pathIndex;
    private float lastAttackTime;
    private UnitState state = UnitState.Moving;

    private Vector2Int currentCoord;

    void Awake()
    {
        unitSide = GetComponent<UnitSide>();
    }

    void Start()
    {
        currentCoord = HexGridManager.Instance.WorldToHex(transform.position);
        EnterTile(currentCoord);

        RecalculatePath("Start()");
        SnapToStartTile();
    }

    void Update()
    {
        if (state == UnitState.Fighting)
        {
            if (tempTarget == null || TargetIsDead(tempTarget))
            {
                tempTarget = null;
                state = UnitState.Moving;
                UpdateTileOwnershipAtCurrentCoord();
                RecalculatePath("Combat ended");
            }
            else
            {
                EngageTempTarget();
            }
            return;
        }

        // Look for nearby enemies
        tempTarget = FindClosestEnemyUnitInAggroRadius();
        if (tempTarget != null && !TargetIsDead(tempTarget))
        {
            state = UnitState.Fighting;
            return;
        }

        // March toward building
        if (primaryTarget == null || TargetIsDead(primaryTarget))
        {
            RecalculatePath("Target lost");
            return;
        }

        if (path == null || path.Count == 0 || pathIndex >= path.Count)
        {
            Debug.LogWarning($"{name} has no valid path!");
            return;
        }

        MoveAlongPath();

        float distToPrimary = Vector3.Distance(transform.position, primaryTarget.position);
        if (distToPrimary <= attackRange)
            TryAttack(primaryTarget);
    }

    void SnapToStartTile()
    {
        var hexGO = HexGridManager.Instance.GetHex(currentCoord);
        if (hexGO != null)
        {
            var p = hexGO.transform.position;
            transform.position = new Vector3(p.x, p.y + yOffset, p.z);
        }
    }

    void MoveAlongPath()
    {
        var nextCoord = path[pathIndex];
        var hex = HexGridManager.Instance.GetHex(nextCoord);
        if (hex == null)
        {
            RecalculatePath("Missing hex");
            return;
        }

        Vector3 targetPos = hex.transform.position + Vector3.up * yOffset;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < tileSnapThreshold)
        {
            LeaveTile(currentCoord);
            currentCoord = nextCoord;
            EnterTile(currentCoord);

            pathIndex++;
            if (pathIndex >= path.Count)
                RecalculatePath("Reached end");
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
            // Move closer
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

        Vector2Int start = currentCoord;
        Vector2Int goal = HexGridManager.Instance.WorldToHex(primaryTarget.position);

        path = Pathfinding.BFS(start, goal);
        pathIndex = 0;

        Debug.Log($"{name} recalculated path. Reason: {reason}. Steps: {path?.Count}");
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

    void EnterTile(Vector2Int coord)
    {
        TileManager.Instance.TryEnterTile(gameObject, coord);

        var tileGO = HexGridManager.Instance.GetHex(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null) tile.Occupy(unitSide);
    }

    void LeaveTile(Vector2Int coord)
    {
        TileManager.Instance.LeaveTile(coord, gameObject);

        var tileGO = HexGridManager.Instance.GetHex(coord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null) tile.Vacate(unitSide);
    }

    void UpdateTileOwnershipAtCurrentCoord()
    {
        var tileGO = HexGridManager.Instance.GetHex(currentCoord);
        var tile = tileGO != null ? tileGO.GetComponent<Tile>() : null;
        if (tile != null && tile.ownerSide != unitSide.side)
            tile.SetOwner(unitSide.side);
    }
}
