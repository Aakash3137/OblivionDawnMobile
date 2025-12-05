using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class NetworkUnit : NetworkBehaviour
{
    [Networked] public int Health { get; set; }
    [Networked] public NetworkBool IsMoving { get; set; }
    [Networked] public NetworkBool IsAttacking { get; set; }
    [Networked] public Vector3 TargetPosition { get; set; }
    [Networked] public NetworkId TargetEnemyId { get; set; }
    
    [Header("Stats")]
    public int maxHealth = 50;
    public float moveSpeed = 2f;
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    
    [Networked] private TickTimer AttackTimer { get; set; }
    [Networked] private NetworkSide OwnerSide { get; set; }
    
    private Vector2Int _currentHexCoord;
    private Animator _animator;
    
    public override void Spawned()
    {
        base.Spawned();
        Health = maxHealth;
        _animator = GetComponent<Animator>();
        UpdateCurrentHexCoord();
    }
    
    public void SetOwner(PlayerRef owner)
    {
        bool isHost = owner == PlayerRef.FromIndex(0);
        OwnerSide = isHost ? NetworkSide.Player : NetworkSide.Enemy;
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        
        if (Health <= 0)
        {
            Runner.Despawn(Object);
            return;
        }
        
        if (TargetEnemyId != default)
        {
            HandleCombat();
        }
        else
        {
            MoveForward();
        }
    }
    
    private void MoveForward()
    {
        Vector3 forward = transform.forward;
        Vector3 nextPos = transform.position + forward * moveSpeed * Runner.DeltaTime;
        transform.position = nextPos;
        
        UpdateCurrentHexCoord();
        CaptureTile();
        CheckForEnemies();
        
        IsMoving = true;
        _animator?.SetBool("IsMoving", true);
    }
    
    private void UpdateCurrentHexCoord()
    {
        if (NetworkHexGridManager.Instance != null)
        {
            _currentHexCoord = NetworkHexGridManager.Instance.WorldToHex(transform.position);
        }
    }
    
    private void CaptureTile()
    {
        var tile = NetworkHexGridManager.Instance?.GetHex(_currentHexCoord);
        if (tile != null && tile.OwnerInt != (int)OwnerSide)
        {
            tile.OwnerInt = (int)OwnerSide;
        }
    }
    
    private void CheckForEnemies()
    {
        var allUnits = FindObjectsOfType<NetworkUnit>();
        foreach (var unit in allUnits)
        {
            if (unit.OwnerSide != OwnerSide)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist <= attackRange)
                {
                    TargetEnemyId = unit.Object.Id;
                    IsMoving = false;
                    return;
                }
            }
        }
        
        var allBuildings = FindObjectsOfType<NetworkBuilding>();
        foreach (var building in allBuildings)
        {
            float dist = Vector3.Distance(transform.position, building.transform.position);
            if (dist <= attackRange)
            {
                TargetEnemyId = building.Object.Id;
                IsMoving = false;
                return;
            }
        }
    }
    
    private void HandleCombat()
    {
        if (AttackTimer.ExpiredOrNotRunning(Runner))
        {
            var targetObj = Runner.FindObject(TargetEnemyId);
            if (targetObj == null)
            {
                TargetEnemyId = default;
                return;
            }
            
            var targetUnit = targetObj.GetComponent<NetworkUnit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damage);
                AttackTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);
                _animator?.SetTrigger("Attack");
            }
            else
            {
                var targetBuilding = targetObj.GetComponent<NetworkBuilding>();
                if (targetBuilding != null)
                {
                    targetBuilding.TakeDamage(damage);
                    AttackTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);
                    _animator?.SetTrigger("Attack");
                }
            }
        }
    }
    
    public void TakeDamage(int dmg)
    {
        if (!Object.HasStateAuthority) return;
        Health -= dmg;
    }
}
