using UnityEngine;
using TMPro;

public class TileUIPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text tileInfoText;
    public CanvasGroup canvasGroup;

    [Header("Building Prefabs (indexed)")]
    public GameObject[] buildingPrefabs; // assign in Inspector

    private Tile currentTile;

    public void Open(Tile tile)
    {
        currentTile = tile;

        if (tileInfoText != null)
            tileInfoText.text = $"Tile: {tile.name}\nOwner: {tile.ownerSide}";

        ShowPanel(true);
    }

    public void Close()
    {
        currentTile = null;
        ShowPanel(false);
    }

    public void PlaceBuilding(int prefabIndex)
    {
        if (currentTile == null) return;
        if (prefabIndex < 0 || prefabIndex >= buildingPrefabs.Length) return;

        // Prevent double placement
        if (currentTile.hasBuilding) return;

        // Spawn at tile center (adjust Y if needed)
        Vector3 spawnPos = currentTile.transform.position;
        spawnPos.y += 2f; // adjust depending on prefab pivot

        Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity, currentTile.transform);

        // Mark tile as having a building
        currentTile.SetBuildingPlaced();

        Close();
    }

    private void ShowPanel(bool show)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = show ? 1f : 0f;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
        }
        else
        {
            gameObject.SetActive(show);
        }
    }

    public void OnCloseButtonClicked() => Close();
}











// using UnityEngine;
// using TMPro;
// using System.Collections.Generic;

// public class TileUIPanel : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public TMP_Text tileInfoText;
//     public CanvasGroup canvasGroup;

//     [Header("Building Prefabs (indexed)")]
//     [SerializeField] private GameObject _wallPrefab;
//     public GameObject[] buildingPrefabs; // assign in Inspector

//     private float _wallYOffset = 1.2f;
//     private Tile currentTile;

//     public void Open(Tile tile)
//     {
//         currentTile = tile;

//         if (tileInfoText != null)
//             tileInfoText.text = $"Tile: {tile.name}\nOwner: {tile.ownerSide}";

//         ShowPanel(true);
//     }

//     public void Close()
//     {
//         currentTile = null;
//         ShowPanel(false);
//     }

//     public void PlaceBuilding(int prefabIndex)
//     {
//         if (currentTile == null) return;
//         if (prefabIndex < 0 || prefabIndex >= buildingPrefabs.Length) return;

//         if (currentTile.hasBuilding) return; // Prevent double placement

//         Vector3 spawnPos = currentTile.transform.position;
//         spawnPos.y += 2f; // adjust depending on prefab pivot

//         Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity, currentTile.transform);

//         currentTile.SetBuildingPlaced();

//         PlaceWalls();
//         Close();
//     }

//     private void ShowPanel(bool show)
//     {
//         if (canvasGroup != null)
//         {
//             canvasGroup.alpha = show ? 1f : 0f;
//             canvasGroup.interactable = show;
//             canvasGroup.blocksRaycasts = show;
//         }
//         else
//         {
//             gameObject.SetActive(show);
//         }
//     }

//     private void PlaceWalls()
//     {
//         Vector3 currentTilePos = currentTile.transform.position;

//         List<Vector2Int> neighborCoords = CubeGridManager.Instance.GetCardinalNeighbors(
//             new Vector2Int(
//                 (int)(currentTilePos.x / CubeGridManager.Instance.cellSize),
//                 (int)(currentTilePos.z / CubeGridManager.Instance.cellSize)
//             )
//         );

//         List<Tile> neighbors = new List<Tile>();
//         foreach (var coord in neighborCoords)
//         {
//             var cube = CubeGridManager.Instance.GetCube(coord);
//             neighbors.Add(cube ? cube.GetComponent<Tile>() : null);
//         }

//         for (int i = 0; i < neighbors.Count; i++)
//         {
//             Tile neighbor = neighbors[i];
//             if (neighbor == null || neighbor.ownerSide == Side.Enemy) continue;

//             Vector3 wallPos = Vector3.zero;
//             Quaternion wallRot = Quaternion.identity;

//             switch (i)
//             {
//                 case 0: wallPos = new Vector3(currentTilePos.x + 1, _wallYOffset, currentTilePos.z); wallRot = Quaternion.Euler(0, 90, 0); break; // right
//                 case 1: wallPos = new Vector3(currentTilePos.x - 1, _wallYOffset, currentTilePos.z); wallRot = Quaternion.Euler(0, 90, 0); break; // left
//                 case 2: wallPos = new Vector3(currentTilePos.x, _wallYOffset, currentTilePos.z + 1); wallRot = Quaternion.identity; break; // up
//                 case 3: wallPos = new Vector3(currentTilePos.x, _wallYOffset, currentTilePos.z - 1); wallRot = Quaternion.identity; break; // down
//             }

//             // CASE 1: Neighbor has NO building → place wall
//             if (!neighbor.hasBuilding)
//             {
//                 if (!currentTile.HasWallAt(wallPos))
//                 {
//                     GameObject wall = Instantiate(_wallPrefab, wallPos, wallRot);
//                     currentTile.RegisterWall(wallPos, wall);
//                 }
//             }
//             // CASE 2: Neighbor HAS building → remove wall
//             else
//             {
//                 currentTile.RemoveWallAt(wallPos);
//             }
//         }
//     }

//     public void OnCloseButtonClicked() => Close();
// }
