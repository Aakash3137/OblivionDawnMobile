using UnityEngine;
using System.Collections.Generic;

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance;

    [Header("Grid Settings")]
    public float hexSize = 1f;        // radius of hex tile
    public bool pointyTop = true;     // true = pointy-top axial, false = flat-top axial

    // Dictionary of all tiles keyed by axial (q,r)
    public Dictionary<Vector2Int, GameObject> hexTiles = new Dictionary<Vector2Int, GameObject>();

    public int MinX { get; private set; } = int.MaxValue;
    public int MinY { get; private set; } = int.MaxValue;
    public int MaxX { get; private set; } = int.MinValue;
    public int MaxY { get; private set; } = int.MinValue;

    void Awake()
    {
        Instance = this;
        Debug.Log("HexGridManager initialized");
    }

    // Register a tile by axial (q,r)
    public void RegisterHex(Vector2Int axial, GameObject tile)
    {
        if (!hexTiles.ContainsKey(axial))
            hexTiles.Add(axial, tile);

        MinX = Mathf.Min(MinX, axial.x);
        MinY = Mathf.Min(MinY, axial.y);
        MaxX = Mathf.Max(MaxX, axial.x);
        MaxY = Mathf.Max(MaxY, axial.y);
    }

    public void UnregisterHex(Vector2Int axial)
    {
        if (hexTiles.ContainsKey(axial))
            hexTiles.Remove(axial);
    }

    public GameObject GetHex(Vector2Int axial)
    {
        hexTiles.TryGetValue(axial, out var tile);
        return tile;
    }

    // Convert world position → axial (q,r)
    public Vector2Int WorldToHex(Vector3 worldPos)
    {
        if (pointyTop)
        {
            // pointy-top axial
            float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.z) / hexSize;
            float r = (2f / 3f * worldPos.z) / hexSize;
            return HexRound(new Vector2(q, r));
        }
        else
        {
            // flat-top axial
            float q = (2f / 3f * worldPos.x) / hexSize;
            float r = (-1f / 3f * worldPos.x + Mathf.Sqrt(3f) / 3f * worldPos.z) / hexSize;
            return HexRound(new Vector2(q, r));
        }
    }

    private Vector2Int HexRound(Vector2 axial)
    {
        int q = Mathf.RoundToInt(axial.x);
        int r = Mathf.RoundToInt(axial.y);
        return new Vector2Int(q, r);
    }

    // --- Axial <-> Cube conversions ---
    // axial (q,r) -> cube (x,y,z) with x = q, z = r, y = -x - z
    public Vector3 AxialToCube(Vector2Int axial)
    {
        int x = axial.x;
        int z = axial.y;
        int y = -x - z;
        return new Vector3(x, y, z);
    }

    // cube -> axial (q,r)
    public Vector2Int CubeToAxial(Vector3 cube)
    {
        int q = (int)cube.x;
        int r = (int)cube.z;
        return new Vector2Int(q, r);
    }

    // Distance using cube
    public int HexDistance(Vector2Int aAxial, Vector2Int bAxial)
    {
        Vector3 a = AxialToCube(aAxial);
        Vector3 b = AxialToCube(bAxial);
        return (int)((Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2);
    }

    public bool AreAdjacent(Vector2Int aAxial, Vector2Int bAxial)
    {
        return HexDistance(aAxial, bAxial) == 1;
    }

    // Optional: axial -> world position (for snapping / gizmos)
    public Vector3 HexToWorld(Vector2Int axial)
    {
        if (pointyTop)
        {
            float x = hexSize * Mathf.Sqrt(3f) * (axial.x + axial.y / 2f);
            float z = hexSize * 3f / 2f * axial.y;
            return new Vector3(x, 0, z);
        }
        else
        {
            float x = hexSize * 3f / 2f * axial.x;
            float z = hexSize * Mathf.Sqrt(3f) * (axial.y + axial.x / 2f);
            return new Vector3(x, 0, z);
        }
    }
}
