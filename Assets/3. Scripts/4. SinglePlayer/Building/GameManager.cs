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
    public AllFactionsData data;

    public UnitStance unitStance;

    [Header("Spawn points")]
    public Tile playerTile;
    public Tile enemyTile;

    public Transform obstaclePool;

    [HideInInspector]
    public Transform playerSpawnPoint => playerTile.transform;
    [HideInInspector]
    public Transform enemySpawnPoint => enemyTile.transform;

    public float yOffset = 2f;
    internal FactionName enemyFaction;

    public GameObject defaultObstacle;
    public NeutralBuildingsData[] neutralBuildingsData;

    internal Stats PlayerMainBuilding;
    internal Stats EnemyMainBuilding;

    void Awake() => Instance = this;

    private async Awaitable Start()
    {
        await Awaitable.NextFrameAsync();
        SpawnMainBuilding();
        // SpawnNeutralBuildings();
    }

    private void SpawnMainBuilding()
    {
        var playerFaction = GameData.playerFaction;
        var playerMainBuilding = GetMainBuilding(playerFaction);
        var enemyMainBuilding = GetMainBuilding(enemyFaction);

        var playerBuilding = Instantiate(playerMainBuilding, playerSpawnPoint.position + Vector3.up * yOffset, Quaternion.identity);
        var enemyBuilding = Instantiate(enemyMainBuilding, enemySpawnPoint.position + Vector3.up * yOffset, Quaternion.identity);

        var playerMainBuildingStats = playerBuilding.GetComponent<BuildingStats>();
        var enemyMainBuildingStats = enemyBuilding.GetComponent<BuildingStats>();

        playerMainBuildingStats.SetBuildingTile(playerTile);
        playerMainBuildingStats.Initialize();

        enemyMainBuildingStats.SetBuildingTile(enemyTile);
        enemyMainBuildingStats.Initialize();

        PlayerMainBuilding = playerMainBuildingStats.side == Side.Player ? playerMainBuildingStats.GetComponent<Stats>() : PlayerMainBuilding;
        EnemyMainBuilding = enemyMainBuildingStats.side == Side.Enemy ? enemyMainBuildingStats.GetComponent<Stats>() : EnemyMainBuilding;
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

            building.tile.SetOwner(side);
            buildingStats.SetBuildingTile(building.tile);

            //Initialize if scriptable object is not null
            // buildingStats.Initialize();
        }
    }
    public void SetEnemyFaction(FactionName name)
    {
        enemyFaction = name;
    }

    public GameObject GetMainBuilding(FactionName name)
    {
        switch (name)
        {
            case FactionName.Medieval:
                return data.medievalMainBuilding;
            case FactionName.Present:
                return data.presentMainBuilding;
            case FactionName.Futuristic:
                return data.futureMainBuilding;
            case FactionName.Galvadore:
                return data.galvadoreMainBuilding;
            default: return null;
        }
    }
}

[Serializable]
public struct NeutralBuildingsData
{
    public Tile tile;
    [HideInInspector]
    public Transform tileTransform => tile.transform;
    public GameObject prefab;
    public float yOffset;
}