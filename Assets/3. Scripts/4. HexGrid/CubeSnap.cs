using UnityEngine;

[ExecuteAlways]
public class CubeSnap : MonoBehaviour
{
    [Header("Cube Grid Settings")]
    public float cellSize = 1f;     // Must NOT be zero
    public bool useOffset = true;   // Stagger every 2nd row

    private TextMesh textMesh;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
    }

    void Update()
    {
        // --- SAFETY: Prevent collapse into one point ---
        if (cellSize < 0.01f)
            cellSize = 1f;

        // Snap to grid
        Vector3 snapped = Snap(transform.position);
        transform.position = snapped;

        // Update label + name
        Vector2Int grid = WorldToGrid(snapped);
        string label = $"({grid.x}, {grid.y})";

        if (textMesh) textMesh.text = label;
        gameObject.name = label;
    }

    // -----------------------------
    // WORLD → GRID COORDINATES
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

    // -----------------------------
    // GRID → WORLD POSITION
    // -----------------------------
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
    // SNAP
    // -----------------------------
    private Vector3 Snap(Vector3 pos)
    {
        Vector2Int g = WorldToGrid(pos);
        return GridToWorld(g);
    }
}