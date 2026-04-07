using System;
using UnityEngine;

public enum UnitStance
{
    Attacking,
    Defending
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Data")]
    public AllBuildingData allBuildingData;

    public UnitStance unitStance;

    [Header("Spawn points")]
    [field: SerializeField] public Vector2Int playerSpawnCoord { get; private set; }
    [field: SerializeField] public Vector2Int enemySpawnCoord { get; private set; }


    public Transform obstaclePool;

    public float yOffset = 2f;

    public GameObject defaultObstacle;
    public NeutralBuildingsData[] neutralBuildingsData;

    internal Stats PlayerMainBuilding;
    internal Stats EnemyMainBuilding;

    private CubeGridManager cgmInstance => CubeGridManager.Instance;

    // [Space(10)]
    public Tile playerTile { get; private set; }
    public Tile enemyTile { get; private set; }
    public Transform playerSpawnPoint { get; private set; }
    public Transform enemySpawnPoint { get; private set; }

    //Universal Abilities
    public GameObject UniversalAbilityContainer;
    public AbilitySO AllSpeedReduction;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        playerTile = cgmInstance.GetTile(playerSpawnCoord);
        enemyTile = cgmInstance.GetTile(enemySpawnCoord);

        playerSpawnPoint = playerTile.transform;
        enemySpawnPoint = enemyTile.transform;

        SpawnMainBuilding();
        //SpawnNeutralBuildings();
    }

    private void SpawnMainBuilding()
    {
        var playerFaction = GameData.playerFaction;
        var enemyFaction = GameData.enemyFaction;
        var charDb = CharacterDatabase.Instance;

        var playerMainBuilding = charDb.mainBuildingPrefabs[(int)playerFaction];
        var enemyMainBuilding = charDb.mainBuildingPrefabs[(int)enemyFaction];

        var playerBuilding = Instantiate(playerMainBuilding, playerSpawnPoint.position + Vector3.up * yOffset, Quaternion.identity);
        var enemyBuilding = Instantiate(enemyMainBuilding, enemySpawnPoint.position + Vector3.up * yOffset, Quaternion.identity);

        playerBuilding.SetBuildingTile(playerTile);
        playerBuilding.Initialize();

        enemyBuilding.SetBuildingTile(enemyTile);
        enemyBuilding.Initialize();

        PlayerMainBuilding = playerBuilding;
        EnemyMainBuilding = enemyBuilding;
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

            if (obstaclePool != null)
                neutralBuilding.transform.parent = obstaclePool;

            BuildingStats buildingStats = neutralBuilding.GetComponent<BuildingStats>();

            building.tile.ChangeSide(side);
            buildingStats.SetBuildingTile(building.tile);

            //Initialize if scriptable object is not null
            // buildingStats.Initialize();
        }
    }
    
    //Abilities Section
    void ReduceAllUnitSpeed()
    {
        if (AllSpeedReduction != null)
        {
            Debug.Log("Speed reduction ON for next 15sec for every unit in game");
            AbilityManager.Instance.AddSpecialAbility(AllSpeedReduction);
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