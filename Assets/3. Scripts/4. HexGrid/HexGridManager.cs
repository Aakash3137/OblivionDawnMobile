// using System.Collections.Generic; // V.3
// using UnityEngine;

// public class HexGridManager : MonoBehaviour
// {
//     public static HexGridManager Instance;

//     [Header("Grid settings (match HexSnap)")]
//     public float hexSize = 1f;
//     public bool pointyTop = true;

//     private Dictionary<Vector2Int, GameObject> hexMap = new Dictionary<Vector2Int, GameObject>();

//     void Awake()
//     {
//         Instance = this;
//         BuildMap();
//     }

//     // Call this if tiles change in editor or runtime
//     public void BuildMap()
//     {
//         hexMap.Clear();
//         foreach (Transform child in transform)
//         {
//             // Accept both parent tile or nested child
//             HexSnap snap = child.GetComponent<HexSnap>();
//             if (snap != null)
//             {
//                 Vector2Int coord = WorldToHex(child.position);
//                 if (!hexMap.ContainsKey(coord))
//                     hexMap.Add(coord, child.gameObject);
//             }
//         }
//     }

//     public GameObject GetHex(Vector2Int coord)
//     {
//         hexMap.TryGetValue(coord, out GameObject hex);
//         return hex;
//     }

//     public Vector2Int WorldToHex(Vector3 position)
//     {
//         if (pointyTop)
//         {
//             float q = (Mathf.Sqrt(3f) / 3f * position.x - 1f / 3f * position.z) / hexSize;
//             float r = (2f / 3f * position.z) / hexSize;
//             return new Vector2Int(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
//         }
//         else
//         {
//             float q = (2f / 3f * position.x) / hexSize;
//             float r = (-1f / 3f * position.x + Mathf.Sqrt(3f) / 3f * position.z) / hexSize;
//             return new Vector2Int(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
//         }
//     }

//     public Vector3 HexToWorld(Vector2Int hex)
//     {
//         if (pointyTop)
//         {
//             float x = hexSize * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
//             float z = hexSize * 3f / 2f * hex.y;
//             return new Vector3(x, 0f, z);
//         }
//         else
//         {
//             float x = hexSize * 3f / 2f * hex.x;
//             float z = hexSize * Mathf.Sqrt(3f) * (hex.y + hex.x / 2f);
//             return new Vector3(x, 0f, z);
//         }
//     }

//     // Optional: find nearest existing hex to a world position
//     public Vector2Int NearestExistingHex(Vector3 position)
//     {
//         Vector2Int approx = WorldToHex(position);
//         if (hexMap.ContainsKey(approx)) return approx;

//         // search local neighborhood
//         Vector2Int[] dirs = {
//             new Vector2Int(+1, 0), new Vector2Int(-1, 0),
//             new Vector2Int(0, +1), new Vector2Int(0, -1),
//             new Vector2Int(+1, -1), new Vector2Int(-1, +1)
//         };
//         for (int ring = 1; ring <= 3; ring++)
//         {
//             foreach (var d in dirs)
//             {
//                 Vector2Int cand = approx + ring * d;
//                 if (hexMap.ContainsKey(cand)) return cand;
//             }
//         }
//         // fallback: any tile
//         foreach (var kvp in hexMap) return kvp.Key;
//         return approx;
//     }
// }









using UnityEngine;
using System.Collections.Generic;

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance;

    [Header("Grid Settings")]
    public float hexRadius = 1f; // distance from center to corner

    // Dictionary of all tiles keyed by their hex coordinate
    public Dictionary<Vector2Int, GameObject> hexTiles = new Dictionary<Vector2Int, GameObject>();

    // Track grid bounds for corner placement
    public int MinX { get; private set; } = int.MaxValue;
    public int MinY { get; private set; } = int.MaxValue;
    public int MaxX { get; private set; } = int.MinValue;
    public int MaxY { get; private set; } = int.MinValue;

    void Awake()
    {
        Instance = this;
        Debug.Log("HexGridManager initialized");
    }

    // Register a tile in the dictionary
    public void RegisterHex(Vector2Int coord, GameObject tile)
    {
        if (!hexTiles.ContainsKey(coord))
            hexTiles.Add(coord, tile);

        // Update bounds
        MinX = Mathf.Min(MinX, coord.x);
        MinY = Mathf.Min(MinY, coord.y);
        MaxX = Mathf.Max(MaxX, coord.x);
        MaxY = Mathf.Max(MaxY, coord.y);
    }

    // Optional: remove tile
    public void UnregisterHex(Vector2Int coord)
    {
        if (hexTiles.ContainsKey(coord))
            hexTiles.Remove(coord);
    }

    // Get tile GameObject by coordinate
    public GameObject GetHex(Vector2Int coord)
    {
        hexTiles.TryGetValue(coord, out var tile);
        return tile;
    }

    // Convert world position to hex coordinate (odd-r offset grid)
    public Vector2Int WorldToHex(Vector3 worldPos)
    {
        float q = (worldPos.x * Mathf.Sqrt(3f) / 3f - worldPos.z / 3f) / hexRadius;
        float r = worldPos.z * 2f / 3f / hexRadius;

        Vector3 cube = HexRound(new Vector3(q, -q - r, r));
        return CubeToOffset(cube);
    }

    // Round cube coordinates to nearest hex
    private Vector3 HexRound(Vector3 cube)
    {
        float rx = Mathf.Round(cube.x);
        float ry = Mathf.Round(cube.y);
        float rz = Mathf.Round(cube.z);

        float xDiff = Mathf.Abs(rx - cube.x);
        float yDiff = Mathf.Abs(ry - cube.y);
        float zDiff = Mathf.Abs(rz - cube.z);

        if (xDiff > yDiff && xDiff > zDiff)
            rx = -ry - rz;
        else if (yDiff > zDiff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return new Vector3(rx, ry, rz);
    }

    // Convert cube coords to offset (odd-r)
    private Vector2Int CubeToOffset(Vector3 cube)
    {
        int col = (int)cube.x + ((int)cube.z - ((int)cube.z & 1)) / 2;
        int row = (int)cube.z;
        return new Vector2Int(col, row);
    }

    // Convert offset back to cube
    private Vector3 OffsetToCube(Vector2Int offset)
    {
        int x = offset.x - (offset.y - (offset.y & 1)) / 2;
        int z = offset.y;
        int y = -x - z;
        return new Vector3(x, y, z);
    }

    // Hex distance (cube coords)
    public int HexDistance(Vector2Int a, Vector2Int b)
    {
        Vector3 ac = OffsetToCube(a);
        Vector3 bc = OffsetToCube(b);
        return (int)((Mathf.Abs(ac.x - bc.x) + Mathf.Abs(ac.y - bc.y) + Mathf.Abs(ac.z - bc.z)) / 2);
    }

    // Check adjacency (distance = 1)
    public bool AreAdjacent(Vector2Int a, Vector2Int b)
    {
        return HexDistance(a, b) == 1;
    }
}
