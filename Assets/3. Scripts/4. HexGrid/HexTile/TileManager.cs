// using UnityEngine;
// using System.Collections.Generic;

// public class TileManager : MonoBehaviour
// {
//     public static TileManager Instance;

//     private Dictionary<Vector2Int, GameObject> occupiedTiles = new Dictionary<Vector2Int, GameObject>();

//     void Awake()
//     {
//         Instance = this;
//     }

//     public bool IsTileOccupied(Vector3 worldPos)
//     {
//         Vector2Int coord = HexGridManager.Instance.WorldToHex(worldPos);
//         return occupiedTiles.ContainsKey(coord);
//     }

//     public void RegisterUnit(GameObject unit, Vector3 worldPos)
//     {
//         Vector2Int coord = HexGridManager.Instance.WorldToHex(worldPos);
//         if (!occupiedTiles.ContainsKey(coord))
//         {
//             occupiedTiles.Add(coord, unit);
//         }
//         else
//         {
//             // Already occupied → trigger combat
//             GameObject otherUnit = occupiedTiles[coord];
//             CombatManager.Instance.ResolveCombat(unit, otherUnit);
//         }
//     }

//     public void UnregisterUnit(Vector3 worldPos)
//     {
//         Vector2Int coord = HexGridManager.Instance.WorldToHex(worldPos);
//         if (occupiedTiles.ContainsKey(coord))
//         {
//             occupiedTiles.Remove(coord);
//         }
//     }
// }






// using UnityEngine;
// using System.Collections.Generic;

// public class TileManager : MonoBehaviour
// {
//     public static TileManager Instance;

//     // Which unit currently occupies which hex coordinate
//     private Dictionary<Vector2Int, GameObject> occupiedTiles = new Dictionary<Vector2Int, GameObject>();

//     void Awake()
//     {
//         Instance = this;
//     }

//     public Vector2Int WorldToCoord(Vector3 worldPos)
//     {
//         return HexGridManager.Instance.WorldToHex(worldPos);
//     }

//     public bool TryEnterTile(GameObject unit, Vector2Int coord)
//     {
//         if (!occupiedTiles.ContainsKey(coord))
//         {
//             occupiedTiles.Add(coord, unit);
//             return true; // tile acquired
//         }
//         else
//         {
//             // Someone is already there → “same tile conflict”
//             return false;
//         }
//     }

//     public void LeaveTile(Vector2Int coord, GameObject unit)
//     {
//         if (occupiedTiles.TryGetValue(coord, out var current) && current == unit)
//             occupiedTiles.Remove(coord);
//     }

//     public GameObject GetOccupant(Vector2Int coord)
//     {
//         occupiedTiles.TryGetValue(coord, out var u);
//         return u;
//     }
// }




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

    public bool TryEnterTile(GameObject unit, Vector2Int coord)
    {
        if (!occupiedTiles.ContainsKey(coord))
        {
            occupiedTiles.Add(coord, unit);
            return true;
        }
        else
        {
            // Allow if the same unit is already registered
            if (occupiedTiles[coord] == unit) return true;

            // Otherwise, conflict (another unit already there)
            return false;
        }
    }

    public void LeaveTile(Vector2Int coord, GameObject unit)
    {
        if (occupiedTiles.TryGetValue(coord, out var current) && current == unit)
            occupiedTiles.Remove(coord);
    }

    public GameObject GetOccupant(Vector2Int coord)
    {
        occupiedTiles.TryGetValue(coord, out var u);
        return u;
    }
}
