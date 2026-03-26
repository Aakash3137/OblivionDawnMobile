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

//     public bool TryEnterTile(GameObject unit, Vector2Int coord)
//     {
//         if (!occupiedTiles.ContainsKey(coord))
//         {
//             occupiedTiles.Add(coord, unit);
//             return true;
//         }
//         else
//         {
//             // Allow if the same unit is already registered
//             if (occupiedTiles[coord] == unit) return true;

//             // Otherwise, conflict (another unit already there)
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
