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

//     private bool _mailWallPlaced = false;
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

//         // Prevent double placement
//         if (currentTile.hasBuilding) return;

//         // Spawn at tile center (adjust Y if needed)
//         Vector3 spawnPos = currentTile.transform.position;
//         spawnPos.y += 2f; // adjust depending on prefab pivot

//         // Instantiate the placeholder prefab
//         var placeholder = Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity, currentTile.transform);

//         // Inject UnitSide info
//         var unitSide = placeholder.GetComponent<UnitSide>();
//         if (unitSide != null)
//         {
//             unitSide.side = currentTile.ownerSide;

//             // Force UnitSide to inject the correct prefab immediately
//             unitSide.Refresh();
//         }
//         else
//         {
//             Debug.LogError("[TileUIPanel] Building prefab must have UnitSide attached!");
//         }

//         // Mark tile as having a building
//         currentTile.SetBuildingPlaced();

//         PlaceWallsOnMainBuilding();
//         PlaceWalls();   // wall place function 
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

//     //Wall logic
//     private void PlaceWalls()
//     {
//         Vector3 _currentTileCords = currentTile.transform.position;

//         //Debug.Log("Placing Walls at position: " + _currentTileCords + " GameObj Name: " + currentTile.name);

//         List<Vector2Int> adjacentTileCords = CubeGridManager.Instance.GetCardinalNeighbors(new Vector2Int((int)(_currentTileCords.x/CubeGridManager.Instance.cellSize), (int)(_currentTileCords.z/CubeGridManager.Instance.cellSize)));
//         List<Tile> adjacentTiles = new List<Tile>();

//         foreach(var cord in adjacentTileCords)
//         {
//             //Debug.Log("Adjacent tile cords: " + cord/2);
//             //adding TIle to a list of adjacent tiles right, left, up, down in order
//             if(CubeGridManager.Instance.GetCube(cord) == null)
//             {                
//                 adjacentTiles.Add(new GameObject().AddComponent<Tile>());   
//             }
//             else
//             {
//                 adjacentTiles.Add(CubeGridManager.Instance.GetCube(cord).GetComponent<Tile>());                 
//             }
//         }

//         WallParent currentWall = Instantiate(_wallPrefab, new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z), Quaternion.identity, currentTile.transform);

//         for(int i = 0; i < adjacentTiles.Count; i++)
//         {
//             if(adjacentTiles[i].ownerSide == Side.Enemy)
//                 continue;

//             //i=0 is right, i=1 is left, i=2 is up, i=3 is down
//             //Disable Walls which are adjacent to each other
//             if(adjacentTiles[i].hasBuilding)
//             {
//                 switch (i)
//                 {                    
//                     case 0:
//                         //Disable right wall of current tile and left wall of adjacent tile
//                         currentWall.DisableWall(0);
//                         WallParent sideWall = adjacentTiles[i].GetComponentInChildren<WallParent>();
//                         sideWall.DisableWall(1);
//                     break;
//                     case 1:
//                         //Disable left wall of current tile and right wall of adjacent tile
//                         currentWall.DisableWall(1);
//                         sideWall = adjacentTiles[i].GetComponentInChildren<WallParent>();
//                         sideWall.DisableWall(0);
//                     break;
//                     case 2:
//                         //Disable up wall of current tile and down wall of adjacent tile
//                         currentWall.DisableWall(2);
//                         sideWall = adjacentTiles[i].GetComponentInChildren<WallParent>();
//                         sideWall.DisableWall(3);
//                     break;
//                     case 3:
//                         //Disable down wall of current tile and up wall of adjacent tile
//                         currentWall.DisableWall(3);
//                         sideWall = adjacentTiles[i].GetComponentInChildren<WallParent>();
//                         sideWall.DisableWall(2);
//                     break;
//                     default:
//                     break;
//                 }
//             }
//         }       
//     }

//     private void PlaceWallsOnMainBuilding()
//     {
//         if(_mailWallPlaced)
//             return;
//         Transform mainBuildingTile = MainBuildingSpawner.Instance.playerSpawnPoint;
//         Instantiate(_wallPrefab, new Vector3(mainBuildingTile.position.x, _wallYOffset, mainBuildingTile.position.z), Quaternion.identity, mainBuildingTile);
//         _mailWallPlaced = true;
//     }   
// }






using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TileUIPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text tileInfoText;
    public CanvasGroup canvasGroup;

    [Header("Building Prefabs (indexed)")]
    [SerializeField] private WallParent _wallPrefab;
    public GameObject[] buildingPrefabs; // assign in Inspector

    private bool _mainWallPlaced = false;
    private float _wallYOffset = 1f;
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
        if (currentTile.hasBuilding) return; // Prevent double placement

        Vector3 spawnPos = currentTile.transform.position;
        spawnPos.y += 2f;

        var placeholder = Instantiate(buildingPrefabs[prefabIndex], spawnPos, Quaternion.identity, currentTile.transform);

        var unitSide = placeholder.GetComponent<UnitSide>();
        if (unitSide != null)
        {
            unitSide.side = currentTile.ownerSide;

            // Apply correct material immediately
            var slot = new AllFactionsData.BuildingSlot
            {
                prefab = buildingPrefabs[prefabIndex],
                playerMaterial = null, // optional: assign if you want per-building materials
                enemyMaterial = null
            };
            unitSide.ApplySideMaterial(slot);
        }
        else
        {
            Debug.LogError("[TileUIPanel] Building prefab must have UnitSide attached!");
        }

        currentTile.SetBuildingPlaced();
        PlaceWallsOnMainBuilding();
        PlaceWalls();
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
