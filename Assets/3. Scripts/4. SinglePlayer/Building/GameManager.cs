using System;
using System.Collections.Generic;
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
    private GameAudioType prevAudioType;

    Transform cubeTransform;
    Vector3 size;

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


        cubeTransform = Camera.main.transform.GetChild(0);
        // size is half-extents → divide by 2 for actual size
        size = cubeTransform.localScale * 0.5f;
        _ = InitDynamicSFX();
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

    private async Awaitable InitDynamicSFX()
    {
        while (gameObject.activeInHierarchy)
        {
            await Awaitable.WaitForSecondsAsync(0.2f, destroyCancellationToken);
            PlayDynamicSFX();
        }
    }

    private void PlayDynamicSFX()
    {
        Collider[] hits = new Collider[50];
        LayerMask layerMask = LayerMask.GetMask("PlayerAir", "PlayerGround", "EnemyAir", "EnemyGround");

        int count = Physics.OverlapBoxNonAlloc(cubeTransform.position, size, hits, Quaternion.identity, layerMask);

        GameAudioType currentAudioType = GetAudioType(hits, count);

        if (currentAudioType == prevAudioType) return;

        if (prevAudioType != GameAudioType.None)
            // AudioManager.TryStop(prevAudioType);

            if (currentAudioType != GameAudioType.None)
            {
                AudioManager.Play(currentAudioType);
                prevAudioType = currentAudioType;
                Debug.Log($"<size=20> Playing {currentAudioType} </size>");
            }
    }

    private GameAudioType GetAudioType(Collider[] hits, int count)
    {
        int battleUnits = 0;
        int resourceUnits = 0;
        bool hasMainBuilding = false;

        for (int i = 0; i < count; i++)
        {
            if (hits[i].TryGetComponent(out UnitStats _) || hits[i].TryGetComponent(out DefenseBuildingStats _))
                battleUnits++;
            else if (hits[i].TryGetComponent(out ResourceBuildingStats _))
                resourceUnits++;
            else if (hits[i].TryGetComponent(out MainBuildingStats _))
                hasMainBuilding = true;
        }

        if (battleUnits >= 2) return GameAudioType.BattleSFX;

        if (resourceUnits >= 2) return GameAudioType.ResourceSFX;

        if (hasMainBuilding) return GameAudioType.MainSFX;

        return GameAudioType.None;
    }

    void OnDrawGizmos()
    {
        if (cubeTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(cubeTransform.position, Quaternion.identity, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size * 2f);
        Gizmos.matrix = Matrix4x4.identity; // reset after
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