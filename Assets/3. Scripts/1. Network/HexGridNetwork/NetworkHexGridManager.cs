using UnityEngine;
using System.Collections.Generic;

public class NetworkHexGridManager : MonoBehaviour
{
    public static NetworkHexGridManager Instance;

    [Header("Grid Settings")]
    public float hexSize = 1f;
    public bool pointyTop = true;

    // All tiles stored by hex coordinate
    public Dictionary<Vector2Int, NetworkTile> hexTiles =
        new Dictionary<Vector2Int, NetworkTile>();

    // Ownership statistics (auto-updated)
    public int playerTileCount { get; private set; }
    public int enemyTileCount { get; private set; }

    // Bounds for minimaps or iteration
    public int MinX { get; private set; } = int.MaxValue;
    public int MinY { get; private set; } = int.MaxValue;
    public int MaxX { get; private set; } = int.MinValue;
    public int MaxY { get; private set; } = int.MinValue;

    private void Awake()
    {
        Instance = this;
        Debug.Log("NetworkHexGridManager is Ready");
    }

    // ---------------------------------------------------------
    //  TILE REGISTRATION
    // ---------------------------------------------------------
    public void RegisterHex(Vector2Int coord, GameObject tileGO)
    {
        if (!tileGO.TryGetComponent(out NetworkTile tile))
            return;

        if (!hexTiles.ContainsKey(coord))
            hexTiles.Add(coord, tile);

        // Store bounds
        MinX = Mathf.Min(MinX, coord.x);
        MinY = Mathf.Min(MinY, coord.y);
        MaxX = Mathf.Max(MaxX, coord.x);
        MaxY = Mathf.Max(MaxY, coord.y);

        // Count ownership including this tile
        UpdateOwnershipCounts();
    }

    // ---------------------------------------------------------
    //  OWNERSHIP TRACKING
    // ---------------------------------------------------------
    public void UpdateOwnershipCounts()
    {
        playerTileCount = 0;
        enemyTileCount = 0;

        foreach (var entry in hexTiles)
        {
            NetworkTile tile = entry.Value;

            switch ((NetworkSide)tile.OwnerInt)
            {
                case NetworkSide.Player: playerTileCount++; break;
                case NetworkSide.Enemy: enemyTileCount++; break;
            }
        }
    }

    // Called from NetworkTile on ownership change
    public void NotifyOwnerChanged()
    {
        UpdateOwnershipCounts();
    }

    // ---------------------------------------------------------
    //  LOOKUP
    // ---------------------------------------------------------
    public NetworkTile GetHex(Vector2Int coord)
    {
        hexTiles.TryGetValue(coord, out var tile);
        return tile;
    }

    // ---------------------------------------------------------
    //  WORLD ? HEX CONVERSION
    // ---------------------------------------------------------
    public Vector2Int WorldToHex(Vector3 worldPos)
    {
        if (pointyTop)
        {
            float q = (Mathf.Sqrt(3f) / 3f * worldPos.x - 1f / 3f * worldPos.z) / hexSize;
            float r = (2f / 3f * worldPos.z) / hexSize;
            return HexRound(q, r);
        }
        else
        {
            float q = (2f / 3f * worldPos.x) / hexSize;
            float r = (-1f / 3f * worldPos.x + Mathf.Sqrt(3f) / 3f * worldPos.z) / hexSize;
            return HexRound(q, r);
        }
    }

    private Vector2Int HexRound(float q, float r)
    {
        int rq = Mathf.RoundToInt(q);
        int rr = Mathf.RoundToInt(r);
        return new Vector2Int(rq, rr);
    }

    // ---------------------------------------------------------
    //  CUBE COORD HELPERS (for distance, adjacency)
    // ---------------------------------------------------------
    public Vector3 OffsetToCube(Vector2Int offset)
    {
        if (pointyTop)
        {
            int x = offset.x - (offset.y - (offset.y & 1)) / 2;
            int z = offset.y;
            int y = -x - z;
            return new Vector3(x, y, z);
        }
        else
        {
            int x = offset.x;
            int z = offset.y - (offset.x - (offset.x & 1)) / 2;
            int y = -x - z;
            return new Vector3(x, y, z);
        }
    }

    public int HexDistance(Vector2Int a, Vector2Int b)
    {
        Vector3 ac = OffsetToCube(a);
        Vector3 bc = OffsetToCube(b);

        return (int)((Mathf.Abs(ac.x - bc.x) +
                      Mathf.Abs(ac.y - bc.y) +
                      Mathf.Abs(ac.z - bc.z)) / 2);
    }

    public bool AreAdjacent(Vector2Int a, Vector2Int b)
    {
        return HexDistance(a, b) == 1;
    }
}
