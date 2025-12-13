using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class NetworkCubeGridManager : NetworkBehaviour
{
    public static NetworkCubeGridManager Instance;

    [Header("Grid Settings")]
    public float cellSize = 1f;

    [Tooltip("Offset every 2nd row (like staggered grid)")]
    public bool useOffset = false;

    // Dictionary of all tiles keyed by (x,y)
    public Dictionary<Vector2Int, GameObject> cubeTiles = new Dictionary<Vector2Int, GameObject>();

    [Header("Network Event Core Prefab")]
    public NetworkObject networkEventCorePrefab;
    

    // TILE LISTS
    [Header("Tile Lists (Auto Generated)")]
    public List<NetworkTile> allTiles = new List<NetworkTile>();
    public List<NetworkTile> playerTiles = new List<NetworkTile>();
    public List<NetworkTile> enemyTiles = new List<NetworkTile>();
    public List<NetworkTile> occupiedTiles = new List<NetworkTile>();
    public List<NetworkTile> SelectableTiles = new List<NetworkTile>();

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
    /*public void RegisterHex(Vector2Int coord, GameObject tileGO)
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
    }*/
    public void RegisterCube(Vector2Int grid, GameObject tile)
    {
        
        if (!cubeTiles.ContainsKey(grid))
            cubeTiles.Add(grid, tile);

        MinX = Mathf.Min(MinX, grid.x);
        MinY = Mathf.Min(MinY, grid.y);
        MaxX = Mathf.Max(MaxX, grid.x);
        MaxY = Mathf.Max(MaxY, grid.y);
        
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
    public GameObject GetCube(Vector2Int grid)
    {
        cubeTiles.TryGetValue(grid, out var tile);
        return tile;
    }

    // --------------------------------------------------------------------
    //  WORLD → GRID (Cube Staggered Grid)
    // --------------------------------------------------------------------
    public Vector2Int WorldToGrid(Vector3 pos)
    {
        int row = Mathf.RoundToInt(pos.z / cellSize);

        float offset = (useOffset && (row & 1) != 0)
            ? cellSize * 0.5f
            : 0f;

        int col = Mathf.RoundToInt((pos.x - offset) / cellSize);

        return new Vector2Int(col, row);
    }

    // --------------------------------------------------------------------
    //  GRID → WORLD
    // --------------------------------------------------------------------
    public Vector3 GridToWorld(Vector2Int grid)
    {
        float offset = (useOffset && (grid.y & 1) != 0)
            ? cellSize * 0.5f
            : 0f;

        return new Vector3(
            grid.x * cellSize + offset,
            0,
            grid.y * cellSize
        );
    }

    // -----------------------------
    // NEIGHBORS
    // -----------------------------
    /// <summary>
    /// Get 4 cardinal neighbors (up, down, left, right).
    /// </summary>
    public List<Vector2Int> GetCardinalNeighbors(Vector2Int grid)
    {
        return new List<Vector2Int>
        {
            new Vector2Int(grid.x + 1, grid.y), // right
            new Vector2Int(grid.x - 1, grid.y), // left
            new Vector2Int(grid.x, grid.y + 1), // up
            new Vector2Int(grid.x, grid.y - 1)  // down
        };
    }

    /// <summary>
    /// Get all 8 neighbors (cardinals + diagonals).
    /// </summary>
    public List<Vector2Int> GetAllNeighbors(Vector2Int grid)
    {
        return new List<Vector2Int>
        {
            // 4 cardinal
            new Vector2Int(grid.x + 1, grid.y),
            new Vector2Int(grid.x - 1, grid.y),
            new Vector2Int(grid.x, grid.y + 1),
            new Vector2Int(grid.x, grid.y - 1),

            // 4 diagonals
            new Vector2Int(grid.x + 1, grid.y + 1),
            new Vector2Int(grid.x - 1, grid.y + 1),
            new Vector2Int(grid.x + 1, grid.y - 1),
            new Vector2Int(grid.x - 1, grid.y - 1)
        };
    }

    // -----------------------------
    // DISTANCE / ADJACENCY
    // -----------------------------
    public int CubeDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public bool AreAdjacent(Vector2Int a, Vector2Int b)
    {
        return CubeDistance(a, b) == 1;
    }
}
