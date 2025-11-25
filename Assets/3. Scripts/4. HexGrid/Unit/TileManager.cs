using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    private Dictionary<Vector2Int, GameObject> occupiedTiles = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public bool IsTileOccupied(Vector3 worldPos)
    {
        Vector2Int coord = HexGridManager.Instance.WorldToHex(worldPos);
        return occupiedTiles.ContainsKey(coord);
    }

    public void RegisterUnit(GameObject unit, Vector3 worldPos)
    {
        Vector2Int coord = HexGridManager.Instance.WorldToHex(worldPos);
        if (!occupiedTiles.ContainsKey(coord))
        {
            occupiedTiles.Add(coord, unit);
        }
        else
        {
            // Already occupied → trigger combat
            GameObject otherUnit = occupiedTiles[coord];
            CombatManager.Instance.ResolveCombat(unit, otherUnit);
        }
    }

    public void UnregisterUnit(Vector3 worldPos)
    {
        Vector2Int coord = HexGridManager.Instance.WorldToHex(worldPos);
        if (occupiedTiles.ContainsKey(coord))
        {
            occupiedTiles.Remove(coord);
        }
    }
}
