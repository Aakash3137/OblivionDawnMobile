// using UnityEngine;
// using TMPro;
// using System.Collections.Generic;

// public class TileUIPanel : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public TMP_Text tileInfoText;
//     public CanvasGroup canvasGroup;

//     [Header("Building Prefabs (indexed)")]
//     [SerializeField] private WallParent _wallPrefab;
//     public GameObject[] buildingPrefabs; // assign in Inspector

//     private bool _mainWallPlaced = false;
//     private float _wallYOffset = 1f;
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
//         spawnPos.y += 2f;

//         var placeholder = Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity, currentTile.transform);

//         var unitSide = placeholder.GetComponent<UnitSide>();
//         if (unitSide != null)
//         {
//             unitSide.side = currentTile.ownerSide;

//             // Apply correct material immediately
//             var slot = new AllFactionsData.BuildingSlot
//             {
//                 prefab = buildingPrefabs[prefabIndex],
//                 playerMaterial = null, // optional: assign if you want per-building materials
//                 enemyMaterial = null
//             };
//             unitSide.ApplySideMaterial(slot);
//         }
//         else
//         {
//             Debug.LogError("[TileUIPanel] Building prefab must have UnitSide attached!");
//         }

//         currentTile.SetBuildingPlaced();
//         PlaceWallsOnMainBuilding();
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

//     public void OnCloseButtonClicked() => Close();

//     // Wall logic (unchanged)
//     private void PlaceWalls()
//     {
//         Vector3 _currentTileCords = currentTile.transform.position;

//         List<Vector2Int> adjacentTileCords = CubeGridManager.Instance.GetCardinalNeighbors(
//             new Vector2Int((int)(_currentTileCords.x / CubeGridManager.Instance.cellSize),
//                            (int)(_currentTileCords.z / CubeGridManager.Instance.cellSize)));

//         List<Tile> adjacentTiles = new List<Tile>();

//         foreach (var cord in adjacentTileCords)
//         {
//             if (CubeGridManager.Instance.GetCube(cord) == null)
//                 adjacentTiles.Add(new GameObject().AddComponent<Tile>());
//             else
//                 adjacentTiles.Add(CubeGridManager.Instance.GetCube(cord).GetComponent<Tile>());
//         }

//         WallParent currentWall = Instantiate(_wallPrefab,
//             new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z),
//             Quaternion.identity, currentTile.transform);

//         for (int i = 0; i < adjacentTiles.Count; i++)
//         {
//             if (adjacentTiles[i].ownerSide == Side.Enemy)
//                 continue;

//             if (adjacentTiles[i].hasBuilding)
//             {
//                 switch (i)
//                 {
//                     case 0:
//                         currentWall.DisableWall(0);
//                         adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(1);
//                         break;
//                     case 1:
//                         currentWall.DisableWall(1);
//                         adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(0);
//                         break;
//                     case 2:
//                         currentWall.DisableWall(2);
//                         adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(3);
//                         break;
//                     case 3:
//                         currentWall.DisableWall(3);
//                         adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(2);
//                         break;
//                 }
//             }
//         }
//     }

//     private void PlaceWallsOnMainBuilding()
//     {
//         if (_mainWallPlaced) return;

//         Transform mainBuildingTile = MainBuildingSpawner.Instance.playerSpawnPoint;
//         Instantiate(_wallPrefab,
//             new Vector3(mainBuildingTile.position.x, _wallYOffset, mainBuildingTile.position.z),
//             Quaternion.identity, mainBuildingTile);

//         _mainWallPlaced = true;
//     }
// }






using System.Collections.Generic;
using UnityEngine;

public class TileUIPanel : MonoBehaviour
{
    [SerializeField] private BuildPanel buildPanel;

    [SerializeField] private WallParent _wallPrefab;


    private bool _mainWallPlaced = false;
    private float _wallYOffset = 1f;
    private Tile currentTile;

    public void Open(Tile tile)
    {
        currentTile = tile;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        currentTile = null;
        gameObject.SetActive(false);
    }

    public void PlaceBuilding(AllFactionsData.BuildingSlot slot)
    {
        if (currentTile == null || slot == null || slot.prefab == null) return;
        if (currentTile.hasBuilding) return;

        Vector3 spawnPos = currentTile.transform.position + Vector3.up * 2f;
        var go = Instantiate(slot.prefab, spawnPos, Quaternion.identity, currentTile.transform);

        var unitSide = go.GetComponent<UnitSide>();
        if (unitSide != null)
        {
            unitSide.side = currentTile.ownerSide;
            unitSide.ApplySideMaterial(slot);
        }

        currentTile.SetBuildingPlaced();

        // Fade out build panel
        if (buildPanel != null) buildPanel.gameObject.SetActive(false);
        PlaceWallsOnMainBuilding();
        PlaceWalls();

        Close();
    }


    // Wall logic (unchanged)
    private void PlaceWalls()
    {
        Vector3 _currentTileCords = currentTile.transform.position;

        List<Vector2Int> adjacentTileCords = CubeGridManager.Instance.GetCardinalNeighbors(
            new Vector2Int((int)(_currentTileCords.x / CubeGridManager.Instance.cellSize),
                           (int)(_currentTileCords.z / CubeGridManager.Instance.cellSize)));

        List<Tile> adjacentTiles = new List<Tile>();

        foreach (var cord in adjacentTileCords)
        {
            if (CubeGridManager.Instance.GetCube(cord) == null)
                adjacentTiles.Add(new GameObject().AddComponent<Tile>());
            else
                adjacentTiles.Add(CubeGridManager.Instance.GetCube(cord).GetComponent<Tile>());
        }

        WallParent currentWall = Instantiate(_wallPrefab,
            new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z),
            Quaternion.identity, currentTile.transform);

        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            if (adjacentTiles[i].ownerSide == Side.Enemy)
                continue;

            if (adjacentTiles[i].hasBuilding)
            {
                switch (i)
                {
                    case 0:
                        currentWall.DisableWall(0);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(1);
                        break;
                    case 1:
                        currentWall.DisableWall(1);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(0);
                        break;
                    case 2:
                        currentWall.DisableWall(2);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(3);
                        break;
                    case 3:
                        currentWall.DisableWall(3);
                        adjacentTiles[i].GetComponentInChildren<WallParent>().DisableWall(2);
                        break;
                }
            }
        }
    }

    private void PlaceWallsOnMainBuilding()
    {
        if (_mainWallPlaced) return;

        Transform mainBuildingTile = MainBuildingSpawner.Instance.playerSpawnPoint;
        Instantiate(_wallPrefab,
            new Vector3(mainBuildingTile.position.x, _wallYOffset, mainBuildingTile.position.z),
            Quaternion.identity, mainBuildingTile);

        _mainWallPlaced = true;
    }
}

