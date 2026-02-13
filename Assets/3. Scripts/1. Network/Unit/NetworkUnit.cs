using Fusion;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType { Soldier, Tank, Aircraft }

public class NetworkUnit : NetworkBehaviour
{
    [Header("Unit Settings")]
    public UnitType unitType;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    public float aggroRadius = 3f;
    [SerializeField] internal NavMeshAgent agent;
    [Networked] public NetworkSide OwnerSide { get; set; }
    
    public UnitState unitState = UnitState.Moving;

    private Transform targetMainBuilding;
    private Transform currentTarget;
    private TickTimer attackTimer;
    
    //optimization 
    private TickTimer scanTimer;
    private Vector3 lastDestination;
    [SerializeField] private float scanInterval = 0.25f;


    [Header("Animation")]
    public Animator animator;
    // TODO: Add animation references
    // private Animator animator;
    // Animation states: Idle, Move, Attack, Death

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log("Unit spawned on " + OwnerSide + " side");
            FindEnemyMainBuilding();
            attackTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);
            scanTimer = TickTimer.CreateFromSeconds(Runner, scanInterval);

            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = attackRange;
        }
    }

    private void FindEnemyMainBuilding()
    {
        NetworkTile[] mainTiles = { 
            NetworkCubeGridManager.Instance.MainBuildingTile1, 
            NetworkCubeGridManager.Instance.MainBuildingTile2 
        };

       // Debug.Log("Finding enemy main building");
        foreach (var mainTile in mainTiles)
        {
            Debug.Log(" main tile");
            if (mainTile == null || !mainTile.IsOccupied) continue;

            Debug.Log(" main tile is at " + mainTile.Coord);
            foreach (var enemyTile in NetworkCubeGridManager.Instance.enemyTiles)
            {
                Debug.Log("Checking enemy tile at " + enemyTile.Coord);
                if (enemyTile.Coord == mainTile.Coord)
                {
                    Debug.Log("Enemy main building found at " + enemyTile.Coord);
                    targetMainBuilding = NetworkCubeGridManager.Instance.GetCube(mainTile.Coord).transform;
                    Debug.Log("Enemy main building found at " + targetMainBuilding.position);
                    return;
                } 
            }
           
            
            /* GameObject tileObj = NetworkCubeGridManager.Instance.GetCube(mainTile.Coord);
             if (tileObj == null) continue;

             foreach (Transform child in tileObj.transform)
             {
                 NetworkBuilding building = child.GetComponent<NetworkBuilding>();
                 if (building != null && building.OwnerSide != OwnerSide)
                 {
                     targetMainBuilding = building.enemyVisual != null ? building.enemyVisual.transform : building.transform;
                     Debug.Log("Enemy main building found at " + targetMainBuilding.position);
                     return;
                 }
             }*/
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        //FindNearbyEnemy();
        // Scan less often
        if (scanTimer.Expired(Runner))
        {
            FindNearbyEnemy();
            scanTimer = TickTimer.CreateFromSeconds(Runner, scanInterval);
        }
        
        if (animator == null)
            TryAssignAnimator();

        if (animator != null)
            animator.SetFloat("Move", agent.velocity.magnitude);
        
        if (currentTarget != null)
        {
           // Debug.Log("Attacking enemy");
            AttackTarget();
        }
        else if (targetMainBuilding != null)
        {  // Debug.Log(" MoveTowards targetMainBuilding");
            MoveTowards(targetMainBuilding.position);
        }
    }

    private void FindNearbyEnemy()
    {
        Debug.Log("Finding nearby enemy");
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius);
        float closestDist = float.MaxValue;
        Transform closest = null;

        foreach (var hit in hits)
        {
            NetworkUnit enemyUnit = hit.GetComponent<NetworkUnit>();
            if (enemyUnit != null && enemyUnit.OwnerSide != OwnerSide)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit.transform;
                }
            }
        }

        currentTarget = closest;
    }

    private void AttackTarget()
    {
        if (currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist > attackRange)
        {
            MoveTowards(currentTarget.position);
        }
        else
        {
            Vector3 lookDir = currentTarget.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }

            if (attackTimer.Expired(Runner))
            {
                // TODO: Play attack animation
                // animator.SetTrigger("Attack");
                
                DealDamage();
                attackTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);
            }
        }
    }

    private void MoveTowards(Vector3 targetPos)
    {
        
        // TODO: Play move animation
        // animator.SetBool("IsMoving", true);

        if (!agent.enabled) return;
        
        Debug.Log("Moving towards " + targetPos);
        if ((lastDestination - targetPos).sqrMagnitude > 0.1f)
        {
            agent.SetDestination(targetPos);
            lastDestination = targetPos;
        }
       
        
        //if (targetPos != null)
           // agent.SetDestination(targetPos);
        
        /*Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        transform.position += direction * moveSpeed * Runner.DeltaTime;*/
    }

    private void DealDamage()
    {
        if (currentTarget == null) return;

        NetworkUnit enemyUnit = currentTarget.GetComponent<NetworkUnit>();
        if (enemyUnit != null)
        {
            enemyUnit.RPC_TakeDamage(attackDamage);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_TakeDamage(float damage)
    {
        // TODO: Implement health system
        // health -= damage;
        // if (health <= 0)
        // {
        //     Die();
        // }
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
    
    private void Die()
    {
        // TODO: Play death animation
        // animator.SetTrigger("Death");
        
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
