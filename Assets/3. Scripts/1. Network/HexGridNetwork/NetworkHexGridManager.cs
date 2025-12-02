using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class NetworkHexGridManager : NetworkBehaviour
{
    public static NetworkHexGridManager Instance;

    [Header("Grid Settings")]
    public float hexSize = 1f;
    public bool pointyTop = true;

    [Header("Network Event Core Prefab")]
    public NetworkObject networkEventCorePrefab; 
    
    
    // All tiles stored by hex coordinate
    public Dictionary<Vector2Int, NetworkTile> hexTiles =
        new Dictionary<Vector2Int, NetworkTile>();

    // TILE LISTS
    [Header("Tile Lists (Auto Generated)")]
    public List<NetworkTile> allTiles = new List<NetworkTile>();
    public List<NetworkTile> playerTiles = new List<NetworkTile>();
    public List<NetworkTile> enemyTiles = new List<NetworkTile>();

    // Ownership statistics (real network ownership)
    public int playerTileCount { get; private set; }
    public int enemyTileCount { get; private set; }

    // Boundaries
    public int MinX { get; private set; } = int.MaxValue;
    public int MinY { get; private set; } = int.MaxValue;
    public int MaxX { get; private set; } = int.MinValue;
    public int MaxY { get; private set; } = int.MinValue;

    private void Awake()
    {
        Instance = this;
        AutoLoadTilesFromChildren();
    }

    public override void Spawned()
    {
        Debug.Log("Spawned!!");
        if (Runner.IsServer)
        {Debug.Log("Spawned!!!");
            SpawnNetworkEventCore();
        }
    }

    // --------------------------------------------------------------------
    //  LOAD TILES AUTOMATICALLY FROM CHILDREN
    // --------------------------------------------------------------------
    private void AutoLoadTilesFromChildren()
    {
        allTiles.Clear();

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out NetworkTile tile))
            {
                allTiles.Add(tile);
            }
        }
    }

    private void SpawnNetworkEventCore()
    {Debug.Log("Spawned!!!!");
        if (NetworkEventCore.Instance != null)
        {
            Debug.Log("[NetworkHexGridManager] NetworkEventCore already exists.");
            return;
        }

        if (networkEventCorePrefab == null)
        {
            Debug.LogError("[NetworkHexGridManager] NetworkEventCore prefab not assigned!");
            return;
        }

        Runner.Spawn(networkEventCorePrefab, Vector3.zero, Quaternion.identity);
        Debug.Log("[NetworkHexGridManager] NetworkEventCore spawned by host.");
    }
    
    // ---------------------------------------------------------
    //  TILE REGISTRATION
    // ---------------------------------------------------------
    public void RegisterHex(Vector2Int coord, GameObject tileGO)
    {
        if (!tileGO.TryGetComponent(out NetworkTile tile))
            return;

        if (!hexTiles.ContainsKey(coord))
            hexTiles.Add(coord, tile);

        MinX = Mathf.Min(MinX, coord.x);
        MinY = Mathf.Min(MinY, coord.y);
        MaxX = Mathf.Max(MaxX, coord.x);
        MaxY = Mathf.Max(MaxY, coord.y);
        
        UpdateTileLists();
    }

    // --------------------------------------------------------------------
    //  UPDATE TILE LISTS (REAL NETWORK OWNERSHIP ONLY)
    // --------------------------------------------------------------------
    public void UpdateTileLists()
    {
        playerTiles.Clear();
        enemyTiles.Clear();

        playerTileCount = 0;
        enemyTileCount = 0;

        foreach (var tile in allTiles)
        {
            var owner = (NetworkSide)tile.OwnerInt;  // REAL OWNER

            if (owner == NetworkSide.Player)
            {
                playerTiles.Add(tile);
                playerTileCount++;
            }
            else if (owner == NetworkSide.Enemy)
            {
                enemyTiles.Add(tile);
                enemyTileCount++;
            }
        }
    }

    // Called from NetworkTile when OwnerInt changes on network
    public void NotifyOwnerChanged()
    {
        UpdateTileLists();
    }

    // ---------------------------------------------------------
    //  LOOKUP
    // ---------------------------------------------------------
    public NetworkTile GetHex(Vector2Int coord)
    {
        hexTiles.TryGetValue(coord, out var tile);
        return tile;
    }

    // ---------------------------------------------------------
    //  WORLD → HEX conversion
    // ---------------------------------------------------------
    public Vector2Int WorldToHex(Vector3 worldPos)
    {
        if (pointyTop)
        {
            float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.z) / hexSize;
            float r = (2f / 3f * worldPos.z) / hexSize;
            return HexRound(q, r);
        }
        else
        {
            float q = (2f / 3f * worldPos.x) / hexSize;
            float r = (-1f / 3f * worldPos.x + Mathf.Sqrt(3f) / 3f * worldPos.z) / hexSize;
            return HexRound(q, r);
        }
    }

    private Vector2Int HexRound(float q, float r)
    {
        int rq = Mathf.RoundToInt(q);
        int rr = Mathf.RoundToInt(r);
        return new Vector2Int(rq, rr);
    }

    // ---------------------------------------------------------
    //  DISTANCE, ADJACENCY (unchanged)
    // ---------------------------------------------------------
    public Vector3 OffsetToCube(Vector2Int offset)
    {
        if (pointyTop)
        {
            int x = offset.x - (offset.y - (offset.y & 1)) / 2;
            int z = offset.y;
            int y = -x - z;
            return new Vector3(x, y, z);
        }
        else
        {
            int x = offset.x;
            int z = offset.y - (offset.x - (offset.x & 1)) / 2;
            int y = -x - z;
            return new Vector3(x, y, z);
        }
    }

    public int HexDistance(Vector2Int a, Vector2Int b)
    {
        Vector3 ac = OffsetToCube(a);
        Vector3 bc = OffsetToCube(b);

        return (int)((Mathf.Abs(ac.x - bc.x) +
                      Mathf.Abs(ac.y - bc.y) +
                      Mathf.Abs(ac.z - bc.z)) / 2);
    }

    public bool AreAdjacent(Vector2Int a, Vector2Int b)
    {
        return HexDistance(a, b) == 1;
    }
}
