using Fusion;
using UnityEngine;

public class NetworkBuilding : NetworkBehaviour
{
    [Networked] public NetworkId TileId { get; set; }
    [Networked] public int Health { get; set; }
    [Networked] public NetworkBool IsDestroyed { get; set; }
    
    [Header("Unit Spawning")]
    public GameObject unitPrefab;
    public float spawnInterval = 3f;
    
    [Networked] private TickTimer SpawnTimer { get; set; }
    
    private NetworkTile _tile;
    
    public override void Spawned()
    {
        base.Spawned();
        Health = 100;
        SpawnTimer = TickTimer.CreateFromSeconds(Runner, spawnInterval);
    }
    
    public void SetTile(NetworkTile tile)
    {
        _tile = tile;
        TileId = tile.Object.Id;
        tile.OwnerInt = (int)NetworkSide.Player;
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (IsDestroyed) return;
        
        if (unitPrefab != null && SpawnTimer.Expired(Runner))
        {
            SpawnUnit();
            SpawnTimer = TickTimer.CreateFromSeconds(Runner, spawnInterval);
        }
    }
    
    private void SpawnUnit()
    {
        Vector3 spawnPos = transform.position + transform.forward * 2f;
        var unit = Runner.Spawn(unitPrefab, spawnPos, Quaternion.identity, Object.InputAuthority);
        
        var networkUnit = unit.GetComponent<NetworkUnit>();
        if (networkUnit != null)
        {
            networkUnit.SetOwner(Object.InputAuthority);
        }
        
        Debug.Log($"[Building] Spawned unit from building {Object.Id}");
    }
    
    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority) return;
        
        Health -= damage;
        if (Health <= 0)
        {
            IsDestroyed = true;
            Runner.Despawn(Object);
        }
    }
}
