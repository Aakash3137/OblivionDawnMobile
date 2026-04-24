using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
    [SerializeField] private AllBuildingData allBuildingData;

    [SerializeField] private WallParent _wallPrefab;
    private bool _mainWallPlaced = false;
    private float _wallYOffset = 1f;
    private BuildingStats placedBuilding;

    private void Awake()
    {
        // GameManager.SetFactionNameThroughEnemyBuildPanel(enemyFactionName);
    }
    private void Start()
    {
        enemyAirBuilding.onClick.AddListener(PlaceEnemyAirBuilding);
        enemyInfantryBuilding.onClick.AddListener(PlaceEnemyInfantryBuilding);
        enemyTankBuilding.onClick.AddListener(PlaceEnemyTankBuilding);
        enemyAOERangedBuilding.onClick.AddListener(PlaceEnemyAOERangedBuilding);

        Generic.Delay(SetWall, 0.1f);

        gameObject.SetActive(false);
    }

    public void SetWall()
    {
        _wallPrefab = allBuildingData.wallParentBuilding[GameData.enemyFaction];
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

    private BuildingStats GetBuildingByType(int k)
    {
        var unitsSO = AIDecSelectionData.GetUnitsSOInDeck(enemyFactionName);

        if (unitsSO == null)
            return null;
        else
            return CharacterDatabase.Instance.GetSpawnerBuilding(unitsSO[k]);
    }

    private async void PlaceBuilding(BuildingStats buildingPrefab)
    {
        if (currentTile == null || buildingPrefab == null) return;
        if (currentTile.hasBuilding) return;
        if (!CanPlaceBuilding(buildingPrefab)) return;

        Tile tileSnapshot = currentTile;
        Vector3 spawnPos = tileSnapshot.transform.position + Vector3.up * 2f;

        var handle = Addressables.InstantiateAsync(buildingPrefab.name, spawnPos, Quaternion.identity, tileSnapshot.transform);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[EnemyBuildPanel] Failed to instantiate: {buildingPrefab.name}");
            return;
        }

        placedBuilding = handle.Result.GetComponent<BuildingStats>();
        placedBuilding.SetBuildingTile(tileSnapshot);
        placedBuilding.Initialize();
        tileSnapshot.InitializeTileBuffs();
        PlaceWallsOnMainBuilding();
        PlaceWalls(tileSnapshot);
        CloseBuildPanel();
    }

    // Returns Task<bool> so EnemyAIHandler can await the result
    public async Task<bool> PlaceBuildingAI(BuildingStats buildingPrefab, Vector3 spawnPos, Tile tile)
    {
        if (tile == null || buildingPrefab == null) return false;
        if (tile.hasBuilding) return false;
        if (!CanPlaceBuilding(buildingPrefab)) return false;

        var handle = Addressables.InstantiateAsync(buildingPrefab.name, spawnPos, Quaternion.identity, tile.transform);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[EnemyBuildPanel] Failed to instantiate: {buildingPrefab.name}");
            return false;
        }

        placedBuilding = handle.Result.GetComponent<BuildingStats>();
        placedBuilding.SetBuildingTile(tile);
        placedBuilding.Initialize();
        tile.InitializeTileBuffs();
        PlaceWallsOnMainBuilding();
        PlaceWalls(tile);

        return true;
    }

    private bool CanPlaceBuilding(BuildingStats buildingPrefab)
    {
        var buildingBuildCost = buildingPrefab.buildingStatsSO.buildingBuildCost;

        if (buildingBuildCost == null || !EnemyResourceManager.Instance.HasResources(buildingBuildCost))
            return false;

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

    private async void PlaceWalls(Tile tile)
    {
        if (tile == null || CubeGridManager.Instance == null) return;

        Vector3 _currentTileCords = tile.transform.position;
        var cgmInstance = CubeGridManager.Instance;
        Vector2Int currentGrid = cgmInstance.WorldToGrid(_currentTileCords);

        List<Vector2Int> adjacentTileCords = cgmInstance.GetCardinalNeighbors(currentGrid);

        Tile[] adjacentTiles = new Tile[4]; // 0 : Right, 1 : Left, 2 : Up, 3 : Down;

        adjacentTiles[0] = cgmInstance.GetTile(adjacentTileCords[0]);
        adjacentTiles[1] = cgmInstance.GetTile(adjacentTileCords[1]);
        adjacentTiles[2] = cgmInstance.GetTile(adjacentTileCords[2]);
        adjacentTiles[3] = cgmInstance.GetTile(adjacentTileCords[3]);

        // Capture placedBuilding before any await — it's a field and could change
        BuildingStats buildingSnapshot = placedBuilding;

        var wallHandle = Addressables.InstantiateAsync(
            _wallPrefab.name,
            new Vector3(_currentTileCords.x, _wallYOffset, _currentTileCords.z),
            Quaternion.identity,
            buildingSnapshot.transform);
        await wallHandle.Task;

        if (wallHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[EnemyBuildPanel] Failed to instantiate wall: {_wallPrefab.name}");
            return;
        }

        WallParent currentWall = wallHandle.Result.GetComponent<WallParent>();

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
                        // adjacentTiles[i].GetCurrentBuilding().GetComponentInChildren<WallParent>()?.DisableWall(1);
                        break;
                    case 1:
                        currentWall.DisableWall(1);
                        // adjacentTiles[i].GetCurrentBuilding().GetComponentInChildren<WallParent>()?.DisableWall(0);
                        break;
                    case 2:
                        currentWall.DisableWall(2);
                        // adjacentTiles[i].GetCurrentBuilding().GetComponentInChildren<WallParent>()?.DisableWall(3);
                        break;
                    case 3:
                        currentWall.DisableWall(3);
                        // adjacentTiles[i].GetCurrentBuilding().GetComponentInChildren<WallParent>()?.DisableWall(2);
                        break;
                }
            }
        }
    }

    private async void PlaceWallsOnMainBuilding()
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

        if (mainBuilding == null)
        {
            Debug.Log("Main building not found");
            return;
        }

        var wallHandle = Addressables.InstantiateAsync(
            _wallPrefab.name,
            new Vector3(mainBuildingTile.position.x, _wallYOffset, mainBuildingTile.position.z),
            Quaternion.identity,
            mainBuilding);
        await wallHandle.Task;

        if (wallHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[EnemyBuildPanel] Failed to instantiate wall: {_wallPrefab.name}");
            return;
        }

        _mainWallPlaced = true;
    }
}