using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBuildPanel : MonoBehaviour
{
    private Tile currentTile;

    [SerializeField] private Button enemyAirBuilding;
    [SerializeField] private Button enemyInfantryBuilding;
    [SerializeField] private Button enemyTankBuilding;
    [SerializeField] private Button enemyAOERangedBuilding;
    [SerializeField] private AllFactionsData factionData;

    [SerializeField] private FactionName enemyFactionName;
    [SerializeField] private DecSelectionData AIDecSelectionData;

    [SerializeField] private WallParent _wallPrefab;
    private bool _mainWallPlaced = false;
    private float _wallYOffset = 1f;
    private BuildingStats spawnedBuilding;

    private void Awake()
    {
        GameManager.SetFactionNameThroughEnemyBuildPanel(enemyFactionName);
    }
    private void Start()
    {
        enemyAirBuilding.onClick.AddListener(PlaceEnemyAirBuilding);
        enemyInfantryBuilding.onClick.AddListener(PlaceEnemyInfantryBuilding);
        enemyTankBuilding.onClick.AddListener(PlaceEnemyTankBuilding);
        enemyAOERangedBuilding.onClick.AddListener(PlaceEnemyAOERangedBuilding);
        gameObject.SetActive(false);
    }
    public void PlaceEnemyAirBuilding()
    {
        var slot = GetBuildingByType(0);
        PlaceBuilding(slot);
    }
    public void PlaceEnemyInfantryBuilding()
    {
        var slot = GetBuildingByType(1);
        PlaceBuilding(slot);
    }
    public void PlaceEnemyTankBuilding()
    {
        var slot = GetBuildingByType(2);
        PlaceBuilding(slot);
    }
    public void PlaceEnemyAOERangedBuilding()
    {
        var slot = GetBuildingByType(3);
        PlaceBuilding(slot);
    }


    // updated code with enemy faction selection from inspector.
    private BuildingStats GetBuildingByType(int k)
    {
        return CharacterDatabase.Instance.GetSpawnerBuilding(AIDecSelectionData.AllFactionDecData[(int)enemyFactionName]
            .SelectedUnitDeck[k]);
    }

    private void PlaceBuilding(BuildingStats buildingPrefab)
    {
        if (currentTile == null || buildingPrefab == null) return;

        if (currentTile.hasBuilding) return;

        if (!CanPlaceBuilding(buildingPrefab))
            return;

        Vector3 spawnPos = currentTile.transform.position + Vector3.up * 2f;

        spawnedBuilding = Instantiate(buildingPrefab, spawnPos, Quaternion.identity, currentTile.transform);

        spawnedBuilding.GetComponent<BuildingStats>().Initialize();

        // currentTile.SetBuildingPlaced();

        PlaceWallsOnMainBuilding();
        PlaceWalls();
        CloseBuildPanel();
    }

    public bool PlaceBuildingAI(BuildingStats buildingPrefab, Vector3 spawnPos, Tile tile)
    {

        if (tile == null || buildingPrefab == null) return false;
        currentTile = tile;

        if (currentTile.hasBuilding) return false;

        if (!CanPlaceBuilding(buildingPrefab))
        {
            // GameDebug.Log("[Enemy AI] Can't place building" + currentTile.transform.position);
            // GameDebug.Log("HAsBuilding0 " + currentTile.hasBuilding);
            return false;
        }

        spawnedBuilding = Instantiate(buildingPrefab, spawnPos, Quaternion.identity, currentTile.transform);
        spawnedBuilding.GetComponent<BuildingStats>().Initialize();

        // currentTile.SetBuildingPlaced();

        PlaceWallsOnMainBuilding();

        PlaceWalls();

        return true;
    }

    private bool CanPlaceBuilding(BuildingStats buildingPrefab)
    {
        BuildCost[] buildingBuildCost = null;

            buildingBuildCost = buildingPrefab.buildingStatsSO.buildingBuildCost;
        
        if (buildingBuildCost == null || !EnemyResourceManager.Instance.HasResources(buildingBuildCost))
        {
            //Debug.Log("<color=red>Insufficient Resources Building cannot be placed</color>");
            return false;
        }

        EnemyResourceManager.Instance.SpendResources(buildingBuildCost);

        return true;
    }

    public void CloseBuildPanel()
    {
        currentTile = null;
        gameObject.SetActive(false);
    }
    public void OpenBuildPanel(Tile tile)
    {
        currentTile = tile;
        gameObject.SetActive(true);
    }

    // Wall logic (unchanged)
    private void PlaceWalls()
    {
        if (currentTile == null || CubeGridManager.Instance == null) return;

        Vector3 _currentTileCords = currentTile.transform.position;
        var cgmInstance = CubeGridManager.Instance;
        Vector2Int currentGrid = cgmInstance.WorldToGrid(_currentTileCords);

        List<Vector2Int> adjacentTileCords = cgmInstance.GetCardinalNeighbors(currentGrid);

        Tile[] adjacentTiles = new Tile[4]; // 0 : Right, 1 : Left, 2 : Up, 3 : Down;

        // Directions are fixed with index 0 : Right, 1 : Left, 2 : Up, 3 : Down
        adjacentTiles[0] = cgmInstance.GetCube(adjacentTileCords[0]);
        adjacentTiles[1] = cgmInstance.GetCube(adjacentTileCords[1]);
        adjacentTiles[2] = cgmInstance.GetCube(adjacentTileCords[2]);
        adjacentTiles[3] = cgmInstance.GetCube(adjacentTileCords[3]);

        WallParent currentWall = Instantiate(_wallPrefab,
            new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z),
            Quaternion.identity, spawnedBuilding.transform);

        for (int i = 0; i < adjacentTiles.Length; i++)
        {
            if (adjacentTiles[i] == null || adjacentTiles[i].ownerSide == Side.Player)
                continue;

            if (adjacentTiles[i].hasBuilding)
            {
                switch (i)
                {
                    case 0:
                        currentWall.DisableWall(0);
                        // Most common error here is null reference exception when tile.hasBuilding is not set to false after the building is destroyed
                        adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(1);
                        break;
                    case 1:
                        currentWall.DisableWall(1);
                        adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(0);
                        break;
                    case 2:
                        currentWall.DisableWall(2);
                        adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(3);
                        break;
                    case 3:
                        currentWall.DisableWall(3);
                        adjacentTiles[i].GetOccupant().GetComponentInChildren<WallParent>()?.DisableWall(2);
                        break;
                }
            }
        }
    }

    private void PlaceWallsOnMainBuilding()
    {
        if (_mainWallPlaced) return;

        Transform mainBuildingTile = GameManager.Instance.enemySpawnPoint;
        Transform mainBuilding = null;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("MainBuilding");

        foreach (var obj in objects)
        {
            if (obj.GetComponent<BuildingStats>().side == Side.Enemy)
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
