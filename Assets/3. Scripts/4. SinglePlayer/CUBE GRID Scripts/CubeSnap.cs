using UnityEngine;

[ExecuteAlways]
public class CubeSnap : MonoBehaviour
{
    //Optimized
    [Header("Cube Grid Settings")]
    public float cellSize = 1f;
    public bool useOffset = false;

    private TextMesh textMesh;
    private Vector3 lastPosition;
    private Vector2Int lastGrid;

    void OnEnable()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        SnapIfNeeded(force: true);
    }

    void OnValidate()
    {
        if (cellSize < 0.01f)
            cellSize = 1f;

        SnapIfNeeded(force: true);
    }

    void Update()
    {
        // Only do work if the object actually moved
        if (transform.position != lastPosition)
        {
            SnapIfNeeded(force: false);
        }
    }

    void SnapIfNeeded(bool force)
    {
        Vector2Int grid = WorldToGrid(transform.position);

        if (!force && grid == lastGrid)
            return;

        lastGrid = grid;
        lastPosition = transform.position;

        transform.position = GridToWorld(grid);

        string label = $"({grid.x}, {grid.y})";
        if (textMesh != null)
            textMesh.text = label;

        gameObject.name = label;
    }

    // WORLD → GRID
    public Vector2Int WorldToGrid(Vector3 pos)
    {
        int row = Mathf.RoundToInt(pos.z / cellSize);

        float offset = (useOffset && (row & 1) != 0)
            ? cellSize * 0.5f
            : 0f;

        int col = Mathf.RoundToInt((pos.x - offset) / cellSize);

        return new Vector2Int(col, row);
    }

    // GRID → WORLD
    public Vector3 GridToWorld(Vector2Int grid)
    {
        float offset = (useOffset && (grid.y & 1) != 0)
            ? cellSize * 0.5f
            : 0f;

        return new Vector3(
            grid.x * cellSize + offset,
            0f,
            grid.y * cellSize
        );
    }
}