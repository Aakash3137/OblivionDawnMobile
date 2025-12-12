using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class NetworkHexGridManager : NetworkBehaviour
{
    public static NetworkHexGridManager Instance;

    [Header("Grid Settings")]
    public float hexSize = 1f;      // Cube cell size
    public bool pointyTop = false;  // UNUSED but kept for compatibility
    public bool useOffset = true;   // Staggered rows like bricks

    [Header("Network Event Core Prefab")]
    public NetworkObject networkEventCorePrefab;

    // All tiles stored by grid coordinate
    public Dictionary<Vector2Int, NetworkTile> hexTiles =
        new Dictionary<Vector2Int, NetworkTile>();

    // TILE LISTS
    [Header("Tile Lists (Auto Generated)")]
    public List<NetworkTile> allTiles = new List<NetworkTile>();
    public List<NetworkTile> playerTiles = new List<NetworkTile>();
    public List<NetworkTile> enemyTiles = new List<NetworkTile>();
    public List<NetworkTile> occupiedTiles = new List<NetworkTile>();

    [SerializeField] internal NetworkTile MainBuildingTile1;
    [SerializeField] internal NetworkTile MainBuildingTile2;
    
    // Ownership statistics
    public int playerTileCount { get; private set; }
    public int enemyTileCount { get; private set; }
    public int occupiedTileCount { get; private set; }

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
        if (Runner.IsServer)
            SpawnNetworkEventCore();
    }

    // --------------------------------------------------------------------
    //  LOAD TILES AUTOMATICALLY
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
    {
        if (NetworkEventCore.Instance != null)
            return;

        if (networkEventCorePrefab == null)
        {
            Debug.LogError("[NetworkHexGridManager] NetworkEventCore prefab not assigned!");
            return;
        }

        Runner.Spawn(networkEventCorePrefab, Vector3.zero, Quaternion.identity);
    }

    // --------------------------------------------------------------------
    //  TILE REGISTRATION
    // --------------------------------------------------------------------
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
    //  UPDATE TILE LISTS
    // --------------------------------------------------------------------
    public void UpdateTileLists()
    {
        playerTiles.Clear();
        enemyTiles.Clear();
        occupiedTiles.Clear();

        playerTileCount = 0;
        enemyTileCount = 0;
        occupiedTileCount = 0;

        foreach (var tile in allTiles)
        {
            var owner = (NetworkSide)tile.OwnerInt;

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

            if (tile.IsOccupied)
            {
                occupiedTiles.Add(tile);
                occupiedTileCount++;
            }
        }

        if (MainBuildingTile1 != null && !occupiedTiles.Contains(MainBuildingTile1))
        {
            occupiedTiles.Add(MainBuildingTile1);
            occupiedTileCount++;
        }

        if (MainBuildingTile2 != null && !occupiedTiles.Contains(MainBuildingTile2))
        {
            occupiedTiles.Add(MainBuildingTile2);
            occupiedTileCount++;
        }
    }

    public void NotifyOwnerChanged()
    {
        UpdateTileLists();
    }

    // --------------------------------------------------------------------
    //  LOOKUP
    // --------------------------------------------------------------------
    public NetworkTile GetHex(Vector2Int coord)
    {
        hexTiles.TryGetValue(coord, out var tile);
        return tile;
    }

    // --------------------------------------------------------------------
    //  WORLD → GRID (Cube Staggered Grid)
    // --------------------------------------------------------------------
    public Vector2Int WorldToHex(Vector3 pos)
    {
        float size = Mathf.Max(0.01f, hexSize);

        int row = Mathf.RoundToInt(pos.z / size);

        float offset = (useOffset && (row & 1) != 0)
            ? size * 0.5f
            : 0f;

        int col = Mathf.RoundToInt((pos.x - offset) / size);

        return new Vector2Int(col, row);
    }

    // --------------------------------------------------------------------
    //  GRID → WORLD
    // --------------------------------------------------------------------
    public Vector3 HexToWorld(Vector2Int grid)
    {
        float size = Mathf.Max(0.01f, hexSize);

        float offset = (useOffset && (grid.y & 1) != 0)
            ? size * 0.5f
            : 0f;

        float x = grid.x * size + offset;
        float z = grid.y * size;

        return new Vector3(x, 0, z);
    }

    // --------------------------------------------------------------------
    //  DISTANCE AND ADJACENCY (Square-grid variants)
    // --------------------------------------------------------------------
    public int HexDistance(Vector2Int a, Vector2Int b)
    {
        // Manhattan distance with stagger awareness
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy;
    }

    public bool AreAdjacent(Vector2Int a, Vector2Int b)
    {
        return HexDistance(a, b) == 1;
    }
}
