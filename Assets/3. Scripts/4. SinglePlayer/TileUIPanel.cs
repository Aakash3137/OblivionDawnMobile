using System.Collections.Generic;
using UnityEngine;

public class TileUIPanel : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private BuildPanel buildPanel;
    [SerializeField] private WallParent _wallPrefab;

    private bool _mainWallPlaced = false;
    private float _wallYOffset = 1f;
    private Tile currentTile;

    public void Open(Tile tile)
    {
        currentTile = tile;

        gameObject.SetActive(true);

        //Debug.Log($"<color=green>[TileUIPanel] Opened for tile {tile.name}</color>");
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

        if (!CanPlaceBuilding(slot))
        {
            if (buildPanel != null) buildPanel.gameObject.SetActive(false);
            return;
        }

        Vector3 spawnPos = currentTile.transform.position + Vector3.up * 2f;
        var go = Instantiate(slot.prefab, spawnPos, Quaternion.identity, currentTile.transform);

        currentTile.SetBuildingPlaced();

        // Fade out build panel
        if (buildPanel != null)
            buildPanel.gameObject.SetActive(false);

        PlaceWallsOnMainBuilding();

        if (go.GetComponent<Stats>() != null)
            PlaceWalls();

        Close();
    }

    private bool CanPlaceBuilding(AllFactionsData.BuildingSlot slot)
    {
        UpgradeCost[] buildingUpgradeData = slot.prefab.GetComponent<BuildingStats>().buildingStats.buildingLevelData[0].buildingUpgradeCosts;
        if (buildingUpgradeData == null || !PlayerResourceManager.Instance.HasResources(buildingUpgradeData, true))
        {
            Debug.Log("<color=red>Insufficient Resources Building cannot be placed/upgraded</color>");
            return false;
        }

        PlayerResourceManager.Instance.SpendResources(buildingUpgradeData);
        return true;
    }

    // Wall logic (unchanged)
    private void PlaceWalls()
    {
        Vector3 _currentTileCords = currentTile.transform.position;
        var cgmInstance = CubeGridManager.Instance;
        Vector2Int currentGrid = cgmInstance.WorldToGrid(_currentTileCords);

        List<Vector2Int> adjacentTileCords = cgmInstance.GetCardinalNeighbors(currentGrid);

        Tile[] adjacentTiles = new Tile[4]; // 0 : Right, 1 : Left, 2 : Up, 3 : Down;

        // Directions are fixed with index 0 : Right, 1 : Left, 2 : Up, 3 : Down
        adjacentTiles[0] = cgmInstance.GetCube(adjacentTileCords[0])?.GetComponent<Tile>();
        adjacentTiles[1] = cgmInstance.GetCube(adjacentTileCords[1])?.GetComponent<Tile>();
        adjacentTiles[2] = cgmInstance.GetCube(adjacentTileCords[2])?.GetComponent<Tile>();
        adjacentTiles[3] = cgmInstance.GetCube(adjacentTileCords[3])?.GetComponent<Tile>();

        // List<Tile> adjacentTiles = new List<Tile>();

        // foreach (var cord in adjacentTileCords)
        // {
        //     if (cgmInstance.GetCube(cord) == null)
        //         adjacentTiles.Add(new GameObject().AddComponent<Tile>());
        //     else
        //         adjacentTiles.Add(cgmInstance.GetCube(cord).GetComponent<Tile>());
        // }

        WallParent currentWall = Instantiate(_wallPrefab,
            new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z),
            Quaternion.identity, currentTile.transform);

        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] == null || adjacentTiles[i].ownerSide == Side.Enemy)
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
            Quaternion.identity,
            mainBuildingTile);

        _mainWallPlaced = true;
    }
}

