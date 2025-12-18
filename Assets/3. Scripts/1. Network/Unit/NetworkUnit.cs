using Fusion;
using UnityEngine;

public enum UnitType { Soldier, Tank, Aircraft }

public class NetworkUnit : NetworkBehaviour
{
    [Header("Unit Settings")]
    public UnitType unitType;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    [Networked] public NetworkSide OwnerSide { get; set; }

    private Transform targetMainBuilding;
    private Transform currentTarget;
    private TickTimer attackTimer;

    // TODO: Add animation references
    // private Animator animator;
    // Animation states: Idle, Move, Attack, Death

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            FindEnemyMainBuilding();
            attackTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);
        }
    }

    private void FindEnemyMainBuilding()
    {
        NetworkTile[] mainTiles = { 
            NetworkCubeGridManager.Instance.MainBuildingTile1, 
            NetworkCubeGridManager.Instance.MainBuildingTile2 
        };

        foreach (var mainTile in mainTiles)
        {
            if (mainTile == null || !mainTile.IsOccupied) continue;

            GameObject tileObj = NetworkCubeGridManager.Instance.GetCube(mainTile.Coord);
            if (tileObj == null) continue;

            foreach (Transform child in tileObj.transform)
            {
                NetworkBuilding building = child.GetComponent<NetworkBuilding>();
                if (building != null && building.OwnerSide != OwnerSide)
                {
                    targetMainBuilding = building.enemyVisual != null ? building.enemyVisual.transform : building.transform;
                    return;
                }
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        FindNearbyEnemy();

        if (currentTarget != null)
        {
            AttackTarget();
        }
        else if (targetMainBuilding != null)
        {
            MoveTowards(targetMainBuilding.position);
        }
    }

    private void FindNearbyEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
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

        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        transform.position += direction * moveSpeed * Runner.DeltaTime;
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
