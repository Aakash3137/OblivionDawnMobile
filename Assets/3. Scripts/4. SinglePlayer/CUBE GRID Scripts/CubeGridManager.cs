using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class CubeGridManager : MonoBehaviour
{
    public static CubeGridManager Instance;

    [SerializeField] private Tile tilePrefab;

    [Header("Grid Settings")]
    [Tooltip("Size of each cube cell in world units")]
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(17, 17);

    [Tooltip("Offset every 2nd row (like staggered grid)")]
    [SerializeField] private bool useOffset = false;

    // Dictionary of all tiles keyed by (x,y)
    public Dictionary<Vector2Int, Tile> cubeTiles = new Dictionary<Vector2Int, Tile>();
    private GameManager gmInstance => GameManager.Instance;

    private int enemyTilesCount => enemyTiles.Count;
    private int playerTilesCount => playerTiles.Count;
    private HashSet<Tile> playerTiles = new();
    private HashSet<Tile> enemyTiles = new();
    private HashSet<Tile> tileEffectTiles = new();
    private Tile[] allTiles;

    public System.Action<int, int> onTileOccupied;


    #region Tile Generation Editor only
    [Button]
    public void GenerateTiles()
    {
#if UNITY_EDITOR
        DestroyTiles();

        int playerTileCount = 0;
        int enemyTileCount = 0;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                float rowOffset = (useOffset && (y & 1) != 0) ? cellSize * 0.5f : 0f;
                var spawnPosition = new Vector3(x * cellSize + rowOffset, 0f, y * cellSize);

                var spawnedTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform);
                spawnedTile.name = $"Tile ({x}, {y})";

                Side side;
                int diagSum = x + y;
                int midSum = gridSize.x - 1;

                if (diagSum < midSum)
                    side = Side.Player;
                else if (diagSum > midSum)
                    side = Side.Enemy;
                else
                {
                    side = (x % 2 == 0) ? Side.Player : Side.Enemy;
                }

                spawnedTile.InitializeSide(side);

                if (side == Side.Player) playerTileCount++;
                else enemyTileCount++;
            }
        }

        Debug.Log($"Generated {playerTileCount} player tiles and {enemyTileCount} enemy tiles.");
#endif
    }

    [Button]
    public void DestroyTiles()
    {
        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        cubeTiles.Clear();
    }
    #endregion

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        allTiles = GetComponentsInChildren<Tile>();

        for (int i = 0; i < allTiles.Length; i++)
        {
            var coords = WorldToGrid(allTiles[i].transform.position);
            allTiles[i].Initialize(coords);
            RegisterTile(coords, allTiles[i]);
        }

        foreach (var tile in allTiles)
        {
            if (tile.ownerSide == Side.Player)
            {
                playerTiles.Add(tile);
            }
            else if (tile.ownerSide == Side.Enemy)
            {
                enemyTiles.Add(tile);
            }
        }

        tileEffectTiles.Clear();
    }

    // Only call when tile is getting occupied (changing sides)
    public void TileOccupied(Side side, Tile tile)
    {
        if (side == Side.Player)
        {
            enemyTiles.Remove(tile);
            playerTiles.Add(tile);
        }
        else if (side == Side.Enemy)
        {
            playerTiles.Remove(tile);
            enemyTiles.Add(tile);
        }

        playerTiles.Remove(tile);
        enemyTiles.Remove(tile);

        onTileOccupied?.Invoke(playerTilesCount, enemyTilesCount);
    }

    private void RemoveRandomTileFromList(Tile tile)
    {
        playerTiles.Remove(tile);
        enemyTiles.Remove(tile);

        onTileOccupied?.Invoke(playerTilesCount, enemyTilesCount);
    }

    #region Tile Registration
    // -----------------------------
    // TILE REGISTRATION
    // -----------------------------

    public void RegisterTile(Vector2Int grid, Tile tile)
    {
        if (!cubeTiles.ContainsKey(grid))
            cubeTiles.Add(grid, tile);
    }

    public void UnregisterCube(Vector2Int grid)
    {
        if (cubeTiles.ContainsKey(grid))
            cubeTiles.Remove(grid);
    }

    public Tile GetTile(Vector2Int grid)
    {
        cubeTiles.TryGetValue(grid, out var tile);
        return tile;
    }

    // -----------------------------
    // WORLD <-> GRID CONVERSION
    // -----------------------------
    public Vector2Int WorldToGrid(Vector3 pos)
    {
        int row = Mathf.RoundToInt(pos.z / cellSize);

        float offset = (useOffset && (row & 1) != 0)
            ? cellSize * 0.5f
            : 0f;

        int col = Mathf.RoundToInt((pos.x - offset) / cellSize);

        return new Vector2Int(col, row);
    }

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

    /// <summary>Returns the 4 cardinal neighbor coordinates (up, down, left, right).</summary>
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

    /// <summary>Returns all 8 neighbor coordinates (cardinals + diagonals).</summary>
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

    public List<Tile> GetCardinalTiles(Vector2Int grid)
    {
        var neighbors = new List<Tile>();
        var neighborCoords = GetCardinalNeighbors(grid);

        foreach (var coord in neighborCoords)
        {
            neighbors.Add(GetTile(coord));
        }

        return neighbors;
    }

    public List<Tile> GetAllTiles(Vector2Int grid)
    {
        var neighbors = new List<Tile>();
        var neighborCoords = GetAllNeighbors(grid);

        foreach (var coord in neighborCoords)
        {
            neighbors.Add(GetTile(coord));
        }

        return neighbors;
    }

    // -----------------------------
    // DISTANCE / ADJACENCY
    // -----------------------------
    public int CubeDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
    #endregion

    public Tile GetRandomTile(Side side, int minDistance)
    {
        var tiles = side == Side.Player ? playerTiles : enemyTiles;
        Vector2Int spawnCoord = side == Side.Player ? gmInstance.playerSpawnCoord : gmInstance.enemySpawnCoord;

        int eligibleCount = 0;
        foreach (var tile in tiles)
        {
            if (CubeDistance(spawnCoord, tile.coord) > minDistance && !tileEffectTiles.Contains(tile))
                eligibleCount++;
        }

        if (eligibleCount == 0)
        {
            Debug.LogWarning($"GetRandomTile: No eligible {side} tiles beyond minDistance {minDistance}.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, eligibleCount);
        int current = 0;

        foreach (var tile in tiles)
        {
            if (CubeDistance(spawnCoord, tile.coord) > minDistance && !tileEffectTiles.Contains(tile))
            {
                if (current == randomIndex)
                {
                    tileEffectTiles.Add(tile);
                    RemoveRandomTileFromList(tile);
                    return tile;
                }
                current++;
            }
        }

        return null;
    }

    public List<Tile> GetGroupedTiles(int groupSize, int minDistance, int clusterGap = 2)
    {
        if (groupSize < 2)
        {
            Debug.LogWarning("GetGroupedTiles: groupSize must be at least 2.");
            return null;
        }

        int playerCount = groupSize - (groupSize / 2);
        int enemyCount = groupSize / 2;

        int diagSum = gridSize.x - 1;

        // ── Helper: is a tile too close to any reference set? ──────────────────
        bool IsTooClose(Tile tile, IEnumerable<Tile> reference)
        {
            foreach (var used in reference)
            {
                if (ManhattanDistance(tile.coord, used.coord) <= clusterGap)
                    return true;
            }
            return false;
        }

        // ── Base exclusion check shared by both pools ───────────────────────────
        bool IsBaseExcluded(Tile tile)
        {
            if (tileEffectTiles.Contains(tile)) return true;
            if (CubeDistance(gmInstance.playerSpawnCoord, tile.coord) <= minDistance) return true;
            if (CubeDistance(gmInstance.enemySpawnCoord, tile.coord) <= minDistance) return true;
            if (Mathf.Abs((tile.coord.x + tile.coord.y) - diagSum) == 0) return true;
            if (IsTooClose(tile, tileEffectTiles)) return true;
            return false;
        }

        // ── Step 1: build player eligible pool ─────────────────────────────────
        var eligiblePlayer = new List<Tile>();

        foreach (var tile in allTiles)
        {
            if (IsBaseExcluded(tile)) continue;
            if (tile.ownerSide == Side.Player)
                eligiblePlayer.Add(tile);
        }

        if (eligiblePlayer.Count < playerCount)
        {
            Debug.LogWarning($"GetGroupedTiles: Not enough eligible player tiles. " +
                             $"Need {playerCount}, found {eligiblePlayer.Count}.");
            return null;
        }

        // ── Step 2: pick player cluster from random seed ────────────────────────
        Tile seed = eligiblePlayer[UnityEngine.Random.Range(0, eligiblePlayer.Count)];

        eligiblePlayer.Sort((a, b) =>
            ManhattanDistance(seed.coord, a.coord).CompareTo(ManhattanDistance(seed.coord, b.coord)));

        var pickedPlayer = new List<Tile>();
        for (int i = 0; i < playerCount; i++)
            pickedPlayer.Add(eligiblePlayer[i]);

        // ── Step 3: build enemy eligible pool AFTER player picks are known ──────
        // This ensures the gap is correctly enforced against the actual picked
        // player tiles, not just previously registered tileEffectTiles
        var eligibleEnemy = new List<Tile>();

        foreach (var tile in allTiles)
        {
            if (IsBaseExcluded(tile)) continue;
            if (tile.ownerSide != Side.Enemy) continue;

            // Gap check against the freshly picked player tiles specifically
            if (IsTooClose(tile, pickedPlayer)) continue;

            eligibleEnemy.Add(tile);
        }

        if (eligibleEnemy.Count < enemyCount)
        {
            Debug.LogWarning($"GetGroupedTiles: Not enough eligible enemy tiles after player placement. " +
                             $"Need {enemyCount}, found {eligibleEnemy.Count}.");
            return null;
        }

        // ── Step 4: find enemy tile nearest to any picked player tile ───────────
        Tile enemySeed = null;
        int bestDist = int.MaxValue;

        foreach (var enemyTile in eligibleEnemy)
        {
            foreach (var playerTile in pickedPlayer)
            {
                int dist = ManhattanDistance(enemyTile.coord, playerTile.coord);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    enemySeed = enemyTile;
                }
            }
        }

        if (enemySeed == null)
        {
            Debug.LogWarning("GetGroupedTiles: Could not find a valid enemy seed tile.");
            return null;
        }

        // ── Step 5: grow enemy cluster from enemy seed ──────────────────────────
        eligibleEnemy.Sort((a, b) =>
            ManhattanDistance(enemySeed.coord, a.coord).CompareTo(ManhattanDistance(enemySeed.coord, b.coord)));

        var pickedEnemy = new List<Tile>();
        for (int i = 0; i < enemyCount; i++)
            pickedEnemy.Add(eligibleEnemy[i]);

        // ── Step 6: register everything ────────────────────────────────────────
        var result = new List<Tile>();

        foreach (var tile in pickedPlayer)
        {
            result.Add(tile);
            tileEffectTiles.Add(tile);
            playerTiles.Remove(tile);
            enemyTiles.Remove(tile);
        }

        foreach (var tile in pickedEnemy)
        {
            result.Add(tile);
            tileEffectTiles.Add(tile);
            playerTiles.Remove(tile);
            enemyTiles.Remove(tile);
        }

        onTileOccupied?.Invoke(playerTilesCount, enemyTilesCount);
        return result;
    }

    public Tile GetNearestOpenTile(Vector2Int currentGrid, Side side, Vector3 currentPosition, int range = 10)
    {
        Tile nearestOpenTile = null;

        currentPosition.y = 0f;

        Vector3 enemyMainBuildingDirection;
        float temp = -1f;

        if (side == Side.Player)
            enemyMainBuildingDirection = (gmInstance.enemySpawnPoint.position - currentPosition).normalized;
        else
            enemyMainBuildingDirection = (gmInstance.playerSpawnPoint.position - currentPosition).normalized;

        for (int layer = 1; layer < range; layer++)
        {
            for (int i = -layer; i <= layer; i++)
            {
                for (int j = -layer; j <= layer; j++)
                {
                    // Skip inner tiles (only perimeter of this layer)
                    if (Mathf.Abs(i) != layer && Mathf.Abs(j) != layer)
                        continue;

                    Vector2Int currentLayerGrid = new Vector2Int(currentGrid.x + i, currentGrid.y + j);

                    var cube = GetTile(currentLayerGrid);
                    if (cube == null)
                        continue;

                    if (cube.hasBuilding || cube.ownerSide != side)
                        continue;

                    Vector3 distance = cube.transform.position - currentPosition;
                    float dotProduct = Vector3.Dot(distance, enemyMainBuildingDirection);

                    if (dotProduct > temp)
                    {
                        nearestOpenTile = cube;
                        temp = dotProduct;
                    }
                }
            }

            if (nearestOpenTile != null)
                return nearestOpenTile;
        }

        return nearestOpenTile;
    }
}