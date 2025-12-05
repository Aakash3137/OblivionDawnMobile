using UnityEngine;

[ExecuteAlways]   // run in editor AND play mode
public class CubeSnap : MonoBehaviour
{
    [Header("Cube Grid Settings")]
    public float cellSize = 1f;     // Must NOT be zero
    public bool useOffset = false;  // Stagger every 2nd row

    private TextMesh textMesh;

    void OnEnable()   // safer than Awake in edit mode
    {
        textMesh = GetComponentInChildren<TextMesh>();
    }

    void Update()
    {
        if (cellSize < 0.01f)
            cellSize = 1f;

        // Snap to nearest grid cell
        Vector2Int grid = WorldToGrid(transform.position);
        Vector3 snapped = GridToWorld(grid);
        transform.position = snapped;

        // Update label + name
        string label = $"({grid.x}, {grid.y})";
        if (textMesh != null) textMesh.text = label;
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
}
