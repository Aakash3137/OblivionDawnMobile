using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum UnitStance
{
    Attacking,
    Defending
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public UnitStance unitStance;
    [Header("Spawn points")]
    [field: SerializeField] public Vector2Int playerSpawnCoord { get; private set; }
    [field: SerializeField] public Vector2Int enemySpawnCoord { get; private set; }

    public float buildingYOffset = 2f;

    public GameObject defaultObstacle;
    public NeutralBuildingsData[] neutralBuildingsData;

    internal Stats PlayerMainBuilding;
    internal Stats EnemyMainBuilding;
    private Transform ObstaclesPool;

    private CubeGridManager cgmInstance => CubeGridManager.Instance;

    // [Space(10)]
    public Tile playerTile { get; private set; }
    public Tile enemyTile { get; private set; }
    public Transform playerSpawnPoint { get; private set; }
    public Transform enemySpawnPoint { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        ObstaclesPool = GameObject.FindGameObjectWithTag("ObstaclesPool").transform;
    }

    public async void Initialize()
    {
        playerTile = cgmInstance.GetTile(playerSpawnCoord);
        enemyTile = cgmInstance.GetTile(enemySpawnCoord);

        playerSpawnPoint = playerTile.transform;
        enemySpawnPoint = enemyTile.transform;

        await SpawnMainBuilding();
        //SpawnNeutralBuildings();
    }

    private async Awaitable SpawnMainBuilding()
    {
        var playerFaction = GameData.playerFaction;
        var enemyFaction = GameData.enemyFaction;
        var charDb = CharacterDatabase.Instance;

        var playerMainBuilding = charDb.mainBuildingPrefabs[(int)playerFaction];
        var enemyMainBuilding = charDb.mainBuildingPrefabs[(int)enemyFaction];

        // Load both in parallel
        var playerHandle = Addressables.InstantiateAsync(
            playerMainBuilding.name,
            playerSpawnPoint.position + Vector3.up * buildingYOffset,
            Quaternion.identity);

        var enemyHandle = Addressables.InstantiateAsync(
            enemyMainBuilding.name,
            enemySpawnPoint.position + Vector3.up * buildingYOffset,
            Quaternion.identity);

        await playerHandle.Task;
        await enemyHandle.Task;

        if (playerHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var playerBuilding = playerHandle.Result.GetComponent<MainBuildingStats>();
            playerBuilding.SetBuildingTile(playerTile);
            playerBuilding.Initialize();
            PlayerMainBuilding = playerBuilding;
        }
        else
            Debug.LogError($"[GameManager] Failed to load player main building: {playerMainBuilding.name}");

        if (enemyHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var enemyBuilding = enemyHandle.Result.GetComponent<MainBuildingStats>();
            enemyBuilding.SetBuildingTile(enemyTile);
            enemyBuilding.Initialize();
            EnemyMainBuilding = enemyBuilding;
        }
        else
            Debug.LogError($"[GameManager] Failed to load enemy main building: {enemyMainBuilding.name}");
    }


    private void SpawnNeutralBuildings(Side side = Side.NeutralAlly)
    {
        foreach (var building in neutralBuildingsData)
        {
            if (building.tile == null) continue;

            var prefab = defaultObstacle;

            if (building.prefab != null)
                prefab = building.prefab;

            var pos = building.tileTransform.position + Vector3.up * building.yOffset;
            var neutralBuilding = Instantiate(prefab, pos, Quaternion.identity, building.tileTransform);

            if (ObstaclesPool != null)
                neutralBuilding.transform.parent = ObstaclesPool;

            BuildingStats buildingStats = neutralBuilding.GetComponent<BuildingStats>();

            building.tile.ChangeSide(side);
            buildingStats.SetBuildingTile(building.tile);

            //Initialize if scriptable object is not null
            // buildingStats.Initialize();
        }
    }
}

[Serializable]
public struct NeutralBuildingsData
{
    public Vector2Int neutralSpawnCoord;
    public Tile tile => CubeGridManager.Instance.GetTile(neutralSpawnCoord);
    public Transform tileTransform => tile.transform;
    public GameObject prefab;
    public float yOffset;
}