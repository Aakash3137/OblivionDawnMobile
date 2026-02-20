using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TextCore;

public class CubeGridManager : MonoBehaviour
{
    public static CubeGridManager Instance;

    [Header("Grid Settings")]
    [Tooltip("Size of each cube cell in world units")]
    public float cellSize = 1f;

    [Tooltip("Offset every 2nd row (like staggered grid)")]
    public bool useOffset = false;

    // Dictionary of all tiles keyed by (x,y)
    public Dictionary<Vector2Int, Tile> cubeTiles = new Dictionary<Vector2Int, Tile>();

    public int MinX { get; private set; } = int.MaxValue;
    public int MinY { get; private set; } = int.MaxValue;
    public int MaxX { get; private set; } = int.MinValue;
    public int MaxY { get; private set; } = int.MinValue;

    private GameManager gmInstance;

    void Awake()
    {
        Instance = this;
        // Debug.Log("CubeGridManager initialized");
        gmInstance = GameManager.Instance;
    }

    // -----------------------------
    // TILE REGISTRATION
    // -----------------------------
    public void RegisterCube(Vector2Int grid, Tile tile)
    {
        if (!cubeTiles.ContainsKey(grid))
            cubeTiles.Add(grid, tile);

        MinX = Mathf.Min(MinX, grid.x);
        MinY = Mathf.Min(MinY, grid.y);
        MaxX = Mathf.Max(MaxX, grid.x);
        MaxY = Mathf.Max(MaxY, grid.y);
    }

    public void UnregisterCube(Vector2Int grid)
    {
        if (cubeTiles.ContainsKey(grid))
            cubeTiles.Remove(grid);
    }

    public Tile GetCube(Vector2Int grid)
    {
        cubeTiles.TryGetValue(grid, out var tile);
        return tile;
    }

    // -----------------------------
    // WORLD ↔ GRID CONVERSION
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

                    var cube = GetCube(currentLayerGrid);
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

                    // Debug.Log($"adj tile {tile.name} distance: {distance.magnitude} dot: {dotProduct}");
                }
            }
            if (nearestOpenTile != null)
                return nearestOpenTile;
        }

        return nearestOpenTile;
    }
}
