// using UnityEngine;
// using System.Collections.Generic;

// public class BuildingPlacementHelper : MonoBehaviour
// {
//     void Start()
//     {
//         // Find the tile under this building
//         Vector2Int buildingCoord = HexGridManager.Instance.WorldToHex(transform.position);

//         // Get the 6 neighbors
//         List<Vector2Int> neighbors = Pathfinding.GetNeighbors(buildingCoord);

//         foreach (var coord in neighbors)
//         {
//             GameObject tileObj = HexGridManager.Instance.GetHex(coord);
//             if (tileObj != null)
//             {
//                 // Navigate down: HexTile -> Hex -> PlusIcon
//                 Transform hexChild = tileObj.transform.Find("Hex");
//                 if (hexChild != null)
//                 {
//                     Transform plusIcon = hexChild.Find("PlusIcon");
//                     if (plusIcon != null)
//                     {
//                         plusIcon.gameObject.SetActive(true);
//                     }
//                 }
//             }
//         }
//     }
// }



// using UnityEngine;
// using System.Collections.Generic;

// public class BuildingPlacementHelper : MonoBehaviour
// {
//     private List<Transform> activatedIcons = new List<Transform>();
//     private List<Tile> openedTiles = new List<Tile>();

//     void Start()
//     {
//         var gm = HexGridManager.Instance;
//         if (gm == null) return;

//         // Find the closest registered tile to this building
//         Vector2Int buildingCoord = GetClosestCoord(transform.position);

//         // Snap the building to the tile center
//         Vector3 center = HexToWorld(buildingCoord, gm.hexSize, gm.pointyTop);
//         transform.position = new Vector3(center.x, transform.position.y, center.z);

//         // Get neighbors
//         List<Vector2Int> neighbors = Pathfinding.GetNeighbors(buildingCoord);

//         foreach (var coord in neighbors)
//         {
//             GameObject tileObj = gm.GetHex(coord);
//             if (tileObj == null) continue;

//             Transform hexChild = tileObj.transform.Find("Hex");
//             if (hexChild == null) continue;

//             Transform plusIcon = hexChild.Find("PlusIcon");
//             if (plusIcon != null)
//             {
//                 plusIcon.gameObject.SetActive(true);
//                 activatedIcons.Add(plusIcon);
//             }

//             // Mark the tile as open
//             Tile tileScript = tileObj.GetComponent<Tile>();
//             if (tileScript != null)
//             {
//                 tileScript.isOpen = true;
//                 openedTiles.Add(tileScript);
//             }
//         }
//     }

//     void OnDestroy()
//     {
//         // Clean up icons if this building is removed
//         foreach (var icon in activatedIcons)
//         {
//             if (icon != null) icon.gameObject.SetActive(false);
//         }
//         activatedIcons.Clear();

//         // Reset tile state
//         foreach (var tile in openedTiles)
//         {
//             if (tile != null) tile.isOpen = false;
//         }
//         openedTiles.Clear();
//     }

//     // --- Helpers ---
//     private Vector2Int GetClosestCoord(Vector3 worldPos)
//     {
//         var gm = HexGridManager.Instance;
//         Vector2Int best = Vector2Int.zero;
//         float bestDist = float.MaxValue;

//         foreach (var kv in gm.hexTiles)
//         {
//             Vector3 tileCenter = HexToWorld(kv.Key, gm.hexSize, gm.pointyTop);
//             float d = Vector3.SqrMagnitude(new Vector3(worldPos.x, 0, worldPos.z) - new Vector3(tileCenter.x, 0, tileCenter.z));
//             if (d < bestDist)
//             {
//                 bestDist = d;
//                 best = kv.Key;
//             }
//         }
//         return best;
//     }

//     private Vector3 HexToWorld(Vector2Int hex, float hexSize, bool pointyTop)
//     {
//         if (pointyTop)
//         {
//             float x = hexSize * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
//             float z = hexSize * 3f / 2f * hex.y;
//             return new Vector3(x, 0, z);
//         }
//         else
//         {
//             float x = hexSize * 3f / 2f * hex.x;
//             float z = hexSize * Mathf.Sqrt(3f) * (hex.y + hex.x / 2f);
//             return new Vector3(x, 0, z);
//         }
//     }

//     // Debug visualization
//     void OnDrawGizmosSelected()
//     {
//         if (HexGridManager.Instance == null) return;
//         Vector2Int coord = GetClosestCoord(transform.position);

//         Gizmos.color = Color.green;
//         Gizmos.DrawSphere(HexToWorld(coord, HexGridManager.Instance.hexSize, HexGridManager.Instance.pointyTop), 0.3f);

//         Gizmos.color = Color.yellow;
//         foreach (var n in Pathfinding.GetNeighbors(coord))
//         {
//             Gizmos.DrawSphere(HexToWorld(n, HexGridManager.Instance.hexSize, HexGridManager.Instance.pointyTop), 0.2f);
//         }
//     }
// }





using UnityEngine;
using System.Collections.Generic;

public class BuildingPlacementHelper : MonoBehaviour
{
    private List<Transform> activatedIcons = new List<Transform>();
    private List<Tile> openedTiles = new List<Tile>();

    void Start()
    {
        var gm = CubeGridManager.Instance;
        if (gm == null) return;

        // Find closest tile to this building
        Vector2Int buildingCoord = GetClosestCoord(transform.position);

        // Snap building to tile center (keep Y)
        Vector3 center = HexToWorld(buildingCoord, gm.hexSize, gm.pointyTop);
        transform.position = new Vector3(center.x, transform.position.y, center.z);

        // Get neighbors
        var neighbors = Pathfinding.GetNeighbors(buildingCoord);

        foreach (var coord in neighbors)
        {
            GameObject tileObj = gm.GetHex(coord);
            if (tileObj == null) continue;

            Tile tileScript = tileObj.GetComponent<Tile>();
            if (tileScript == null) continue;

            // Skip enemy tiles
            if (tileScript.ownerSide == Side.Enemy)
                continue;

            // Show PlusIcon
            Transform hexChild = tileObj.transform.Find("Hex");
            if (hexChild != null)
            {
                Transform plusIcon = hexChild.Find("PlusIcon");
                if (plusIcon != null)
                {
                    plusIcon.gameObject.SetActive(true);
                    activatedIcons.Add(plusIcon);
                }
            }

            // Mark as open
            tileScript.isOpen = true;
            openedTiles.Add(tileScript);
        }
    }

    void OnDestroy()
    {
        // Hide icons
        foreach (var icon in activatedIcons)
        {
            if (icon != null) icon.gameObject.SetActive(false);
        }
        activatedIcons.Clear();

        // Reset tile state
        foreach (var tile in openedTiles)
        {
            if (tile != null) tile.isOpen = false;
        }
        openedTiles.Clear();
    }

    // --- Helpers ---
    private Vector2Int GetClosestCoord(Vector3 worldPos)
    {
        var gm = CubeGridManager.Instance;
        Vector2Int best = Vector2Int.zero;
        float bestDist = float.MaxValue;

        foreach (var kv in gm.hexTiles)
        {
            Vector3 tileCenter = HexToWorld(kv.Key, gm.hexSize, gm.pointyTop);
            float d = Vector3.SqrMagnitude(
                new Vector3(worldPos.x, 0, worldPos.z) -
                new Vector3(tileCenter.x, 0, tileCenter.z)
            );
            if (d < bestDist)
            {
                bestDist = d;
                best = kv.Key;
            }
        }
        return best;
    }

    private Vector3 HexToWorld(Vector2Int hex, float hexSize, bool pointyTop)
    {
        if (pointyTop)
        {
            float x = hexSize * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
            float z = hexSize * 3f / 2f * hex.y;
            return new Vector3(x, 0, z);
        }
        else
        {
            float x = hexSize * 3f / 2f * hex.x;
            float z = hexSize * Mathf.Sqrt(3f) * (hex.y + hex.x / 2f);
            return new Vector3(x, 0, z);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (CubeGridManager.Instance == null) return;
        Vector2Int coord = GetClosestCoord(transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(HexToWorld(coord, CubeGridManager.Instance.hexSize, CubeGridManager.Instance.pointyTop), 0.3f);

        Gizmos.color = Color.yellow;
        foreach (var n in Pathfinding.GetNeighbors(coord))
        {
            Gizmos.DrawSphere(HexToWorld(n, CubeGridManager.Instance.hexSize, CubeGridManager.Instance.pointyTop), 0.2f);
        }
    }
}
