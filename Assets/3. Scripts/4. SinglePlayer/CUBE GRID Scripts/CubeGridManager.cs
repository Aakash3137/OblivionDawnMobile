using UnityEngine;
using System.Collections.Generic;

public class CubeGridManager : MonoBehaviour
{
    public static CubeGridManager Instance;

    [Header("Grid Settings")]
    [Tooltip("Size of each cube cell in world units")]
    public float cellSize = 1f;

    [Tooltip("Offset every 2nd row (like staggered grid)")]
    public bool useOffset = false;

    // Dictionary of all tiles keyed by (x,y)
    public Dictionary<Vector2Int, GameObject> cubeTiles = new Dictionary<Vector2Int, GameObject>();

    public int MinX { get; private set; } = int.MaxValue;
    public int MinY { get; private set; } = int.MaxValue;
    public int MaxX { get; private set; } = int.MinValue;
    public int MaxY { get; private set; } = int.MinValue;

    void Awake()
    {
        Instance = this;
        Debug.Log("CubeGridManager initialized");
    }

    // -----------------------------
    // TILE REGISTRATION
    // -----------------------------
    public void RegisterCube(Vector2Int grid, GameObject tile)
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

    public GameObject GetCube(Vector2Int grid)
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

    public Tile GetNearestOpenTile(Tile currentTile, int range = 10)
    {
        Vector2Int currentGrid = WorldToGrid(currentTile.transform.position);
        Tile nearestOpenTile;
        Vector2Int checkGrid;
        int neighborLayer = 0;

        //Debug.Log("Current Tile Grid coordinates: " + currentGrid.x + ", " + currentGrid.y);

        while (neighborLayer < range)
        {
            neighborLayer++;

            // Directional looping setup
            int jStart = currentTile.ownerSide == Side.Player ? neighborLayer : -neighborLayer;
            int jEnd = currentTile.ownerSide == Side.Player ? -neighborLayer : neighborLayer;
            int jStep = currentTile.ownerSide == Side.Player ? -1 : 1;

            int iStart = currentTile.ownerSide == Side.Player ? neighborLayer : -neighborLayer;
            int iEnd = currentTile.ownerSide == Side.Player ? -neighborLayer : neighborLayer;
            int iStep = currentTile.ownerSide == Side.Player ? -1 : 1;

            for (int j = jStart; j != jEnd + jStep; j += jStep)
            {
                for (int i = iStart; i != iEnd + iStep; i += iStep)
                {
                    // Skip inner tiles (only perimeter of this layer)
                    if (Mathf.Abs(i) != neighborLayer && Mathf.Abs(j) != neighborLayer)
                        continue;

                    // Skip current tile
                    if (i == 0 && j == 0)
                        continue;

                    checkGrid = new Vector2Int(currentGrid.x + i, currentGrid.y + j);

                    var cube = GetCube(checkGrid);
                    if (cube == null)
                        continue;

                    nearestOpenTile = cube.GetComponent<Tile>();
                    if (nearestOpenTile == null)
                        continue;

                    if (!nearestOpenTile.hasBuilding && nearestOpenTile.ownerSide == currentTile.ownerSide)
                    {
                        return nearestOpenTile;
                    }
                }
            }
        }

        return null;
    }

}
