using UnityEngine;
using System.Collections.Generic;

public class HexGridManager : MonoBehaviour
{
    public float hexSize = 1f; // radius of hex tile
    public Dictionary<Vector2Int, Vector3> hexPositions = new Dictionary<Vector2Int, Vector3>();

    // Convert world position to hex coordinate
    public Vector2Int WorldToHex(Vector3 position)
    {
        float q = (Mathf.Sqrt(3f) / 3f * position.x - 1f / 3f * position.z) / hexSize;
        float r = (2f / 3f * position.z) / hexSize;
        return HexRound(new Vector2(q, r));
    }

    // Round to nearest hex
    private Vector2Int HexRound(Vector2 hex)
    {
        int q = Mathf.RoundToInt(hex.x);
        int r = Mathf.RoundToInt(hex.y);
        return new Vector2Int(q, r);
    }

    // Convert hex coordinate back to world position
    public Vector3 HexToWorld(Vector2Int hex)
    {
        float x = hexSize * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
        float z = hexSize * 3f / 2f * hex.y;
        return new Vector3(x, 0, z);
    }

    // Register a tile in dictionary
    public void RegisterTile(Vector2Int hexCoord)
    {
        if (!hexPositions.ContainsKey(hexCoord))
        {
            hexPositions.Add(hexCoord, HexToWorld(hexCoord));
        }
    }
}
