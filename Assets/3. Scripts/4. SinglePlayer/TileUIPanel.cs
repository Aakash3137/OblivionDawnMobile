using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

public class TileUIPanel : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private BuildPanel buildPanel;
    [SerializeField] private AllBuildingData allBuildingData;
    [SerializeField] private WallParent _wallPrefab;

    [Header("Error UI")]
    [SerializeField] private TMP_Text errorText;
    private Vector3 originalAnchoredPosition;
    private MotionHandle errorMotion;

    private PlayerResourceManager prmInstance;

    private CanvasGroup canvasGroup;
    private bool _mainWallPlaced = false;
    private float _wallYOffset = 1f;
    private Tile currentTile;
    private BuildingStats placedBuilding;


    private void Start()
    {
        prmInstance = PlayerResourceManager.Instance;
        canvasGroup = GetComponentInParent<CanvasGroup>();
        buildPanel.HideBuildPanel(canvasGroup);

        originalAnchoredPosition = errorText.rectTransform.anchoredPosition3D;

        errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 0f);

        _wallPrefab = allBuildingData.wallParentBuildings[(int)GameData.playerFaction].wallParentBuilding;
    }

    public void Open(Tile tile)
    {
        currentTile = tile;

        buildPanel.ShowBuildPanel(canvasGroup);

        //Debug.Log($"<color=green>[TileUIPanel] Opened for tile {tile.name}</color>");
    }

    public void Close()
    {
        currentTile = null;
        buildPanel.HideBuildPanel(canvasGroup);
    }

    public void PlaceBuilding(BuildingStats buildingPrefab, UnitProduceStatsSO unitSO = null)
    {
        if (currentTile == null || buildingPrefab == null || buildingPrefab == null) return;
        if (currentTile.hasBuilding) return;

        if (!CanPlaceBuilding(buildingPrefab))
        {
            errorText.text = "Not enough resources";

            HandleError();

            // if (buildPanel != null)
            //     buildPanel.HideBuildPanel(canvasGroup);

            return;
        }

        Vector3 spawnPos = currentTile.transform.position + Vector3.up * 2f;

        placedBuilding = Instantiate(buildingPrefab, spawnPos, Quaternion.identity, currentTile.transform);

        if (placedBuilding is OffenseBuildingStats offenseBuilding)
            offenseBuilding.SetUnitPrefab(CharacterDatabase.Instance.GetUnitPrefab(unitSO), unitSO.GetUnitSpawnTime());

        // This will set current tile and owner side order is important
        placedBuilding.SetBuildingTile(currentTile);
        placedBuilding.Initialize();
        // Initialize tile buff after all values are set
        currentTile.InitializeTileBuffs();

        // Fade out build panel
        if (buildPanel != null)
            buildPanel.HideBuildPanel(canvasGroup);

        PlaceWallsOnMainBuilding();
        PlaceWalls();
        Close();
    }
    private void HandleError()
    {
        if (errorMotion.IsActive())
            errorMotion.Cancel();

        LMotion.Shake.Create(originalAnchoredPosition, new Vector3(10f, 10f, 0f), 0.5f)
        .WithFrequency(25)
        .WithDampingRatio(0.5f)
        .BindToAnchoredPosition3D(errorText.rectTransform);

        errorMotion = LMotion.Create(1f, 0f, 1.5f)
             .WithEase(Ease.OutQuad)
             .BindToColorA(errorText);
    }

    private bool CanPlaceBuilding(BuildingStats buildingPrefab)
    {
        var buildingBuildCost = buildingPrefab.buildingStatsSO.buildingBuildCost;

        if (buildingBuildCost == null || !prmInstance.HasResources(buildingBuildCost))
            return false;

        prmInstance.SpendResources(buildingBuildCost);
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
        adjacentTiles[0] = cgmInstance.GetTile(adjacentTileCords[0]);
        adjacentTiles[1] = cgmInstance.GetTile(adjacentTileCords[1]);
        adjacentTiles[2] = cgmInstance.GetTile(adjacentTileCords[2]);
        adjacentTiles[3] = cgmInstance.GetTile(adjacentTileCords[3]);

        var spawnPos = new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z);

        WallParent currentWall = Instantiate(_wallPrefab, spawnPos, Quaternion.identity, placedBuilding.transform);

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
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(1);
                        break;
                    case 1:
                        currentWall.DisableWall(1);
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(0);
                        break;
                    case 2:
                        currentWall.DisableWall(2);
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(3);
                        break;
                    case 3:
                        currentWall.DisableWall(3);
                        // adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(2);
                        break;
                }
            }
        }
    }

    private void PlaceWallsOnMainBuilding()
    {
        if (_mainWallPlaced) return;

        Transform mainBuildingTile = GameManager.Instance.playerSpawnPoint;
        Transform mainBuilding = null;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("MainBuilding");

        foreach (var obj in objects)
        {
            if (obj.GetComponent<BuildingStats>().side == Side.Player)
            {
                mainBuilding = obj.transform;
                break;
            }
        }

        if (mainBuilding != null)
            Instantiate(_wallPrefab,
                new Vector3(mainBuildingTile.position.x, _wallYOffset, mainBuildingTile.position.z),
                Quaternion.identity, mainBuilding);
        else
            Debug.Log("Main building not found");

        _mainWallPlaced = true;
    }
}

