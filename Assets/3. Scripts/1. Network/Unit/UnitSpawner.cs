using Fusion;
using UnityEngine;

public class UnitSpawner : NetworkBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private NetworkPrefabRef unitPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private bool allowSpawning = true;

    private TickTimer spawnTimer;
    private NetworkTile buildingTile;
    private NetworkTile spawnTile;
    
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            FindBuildingTile();
            FindSpawnTile();
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnInterval);
        }
    }
    
    private void FindBuildingTile()
    {
        Vector2Int coord = NetworkCubeGridManager.Instance.WorldToGrid(transform.position);
        GameObject tileObj = NetworkCubeGridManager.Instance.GetCube(coord);
        if (tileObj != null)
        {
            buildingTile = tileObj.GetComponent<NetworkTile>();
        }
    }

    private void FindSpawnTile()
    {
        if (buildingTile == null) return;

        var neighbors = NetworkCubeGridManager.Instance.GetAllNeighbors(buildingTile.Coord);
        
        foreach (var neighborCoord in neighbors)
        {
            GameObject neighborObj = NetworkCubeGridManager.Instance.GetCube(neighborCoord);
            if (neighborObj == null) continue;

            NetworkTile tile = neighborObj.GetComponent<NetworkTile>();
            if (tile != null && !tile.IsOccupied && NetworkCubeGridManager.Instance.RegisterSpawnTile(tile))
            {
                spawnTile = tile;
                return;
            }
        }

        spawnTile = NetworkCubeGridManager.Instance.FindNearestUnoccupiedTile(buildingTile.Coord);
        if (spawnTile != null)
        {
            NetworkCubeGridManager.Instance.RegisterSpawnTile(spawnTile);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || !allowSpawning)
            return;

        if (spawnTimer.Expired(Runner))
        {
            TrySpawnUnit();
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnInterval);
        }
    }

    private void TrySpawnUnit()
    {
        if (spawnTile == null || spawnTile.IsOccupied)
        {
            if (spawnTile != null)
            {
                NetworkCubeGridManager.Instance.UnregisterSpawnTile(spawnTile);
            }
            FindSpawnTile();
            if (spawnTile == null) return;
        }

        Vector3 spawnPos = spawnTile.transform.position + Vector3.up;
        Runner.Spawn(unitPrefab, spawnPos, Quaternion.identity, Object.InputAuthority);
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (spawnTile != null)
        {
            NetworkCubeGridManager.Instance.UnregisterSpawnTile(spawnTile);
        }
    }
    
    public void StopSpawning()
    {
        if (!Object.HasStateAuthority)
            return;

        allowSpawning = false;
    }
    
    public void StartSpawning()
    {
        if (!Object.HasStateAuthority)
            return;

        allowSpawning = true;
        spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnInterval);
    }
    
}