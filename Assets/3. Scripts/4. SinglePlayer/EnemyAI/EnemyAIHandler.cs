using System.Collections.Generic;
using UnityEngine;

public enum EnemyAIMode
{
    FullAttack,
    SemiAttacking,
    Defence,
    OnlyDefence
}

public class EnemyAIHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyModeSwitch enemyModeSwitch;
    [SerializeField] private EnemyBuildPanel enemyBuildPanel;
    [SerializeField] private AllFactionsData factionData;
    [SerializeField] private FactionName enemyFactionName;

    [Header("AI Behavior Mode")]
    [SerializeField] private EnemyAIMode aiMode = EnemyAIMode.SemiAttacking;

    [Header("Full Attack Mode Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float fullAttack_UnitBuildingRatio = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] private float fullAttack_ResourceBuildingRatio = 0.2f;

    [Header("Semi Attacking Mode Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float semiAttack_UnitBuildingRatio = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float semiAttack_ResourceBuildingRatio = 0.3f;
    [Range(0f, 1f)]
    [SerializeField] private float semiAttack_DefenceBuildingRatio = .1f;

    [Header("Defence Mode Settings")]
    [SerializeField] private int defence_InitialDefenceCount = 3;
    [Range(0f, 1f)]
    [SerializeField] private float defence_ResourceRatio = 0.4f;
    [Range(0f, 1f)]
    [SerializeField] private float defence_AttackRatio = 0.6f;

    [Header("Only Defence Mode Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float onlyDefence_DefenceRatio = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] private float onlyDefence_ResourceRatio = 0.2f;
    [SerializeField] private bool onlyDefence_AllowAttackBuildings = false;
    [SerializeField] private int onlyDefence_AttackBuildingsAfterDefenceCount = 6;
    
    [Header("Resources")]
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemyBuildings = 30;
    [SerializeField] private bool reduceSpawningAfterMax = false;
    [SerializeField] private float reducedSpawnInterval = 15f;

    private Transform enemyMainBuildingTransform;
    private List<Tile> spawnableTiles = new List<Tile>();
    private int defenceBuildingsSpawned = 0;
    private int totalBuildingsSpawned = 0;
    private float spawnTimer = 0f;
    private int resourceBuildingIndex = 0;
    private bool allResourcesCovered = false;

    void Start()
    {
        Debug.Log("[EnemyAI] Start called");

        if (GameManager.Instance == null)
        {
            Debug.LogError("[EnemyAI] MainBuildingSpawner.Instance is null!");
            return;
        }

        enemyMainBuildingTransform = GameManager.Instance.enemySpawnPoint;
        // GameDebug.Log($"[EnemyAI] Enemy main building transform: {enemyMainBuildingTransform}");

        Invoke(nameof(InitializeSpawnableTiles), 1f);
    }

    void Update()
    {
        if (enemyModeSwitch == null)
            return;

        if (enemyModeSwitch.EnemyMode != CurrentEnemyMode.EnemyAIMode)
            return;

        RemoveInvalidSpawnableTiles();

        float currentInterval = (reduceSpawningAfterMax && totalBuildingsSpawned >= maxEnemyBuildings)
            ? reducedSpawnInterval
            : spawnInterval;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentInterval)
        {
            spawnTimer = 0f;
            TrySpawnBuilding();
        }
    }

    // Initialize spawnable tiles around the enemy main building. 
    void InitializeSpawnableTiles()
    {
        // GameDebug.Log("[EnemyAI] InitializeSpawnableTiles called");

        if (enemyMainBuildingTransform == null)
        {
            Debug.LogError("[EnemyAI] enemyMainBuildingTransform is null!");
            return;
        }

        if (CubeGridManager.Instance == null)
        {
            Debug.LogError("[EnemyAI] CubeGridManager.Instance is null!");
            return;
        }

        Vector2Int mainBuildingGrid = CubeGridManager.Instance.WorldToGrid(enemyMainBuildingTransform.position);
        // GameDebug.Log($"[EnemyAI] Main building grid: {mainBuildingGrid}");

        UpdateSpawnableTiles(mainBuildingGrid);
        // GameDebug.Log($"[EnemyAI] Spawnable tiles count: {spawnableTiles.Count}");
    }

    void UpdateSpawnableTiles(Vector2Int buildingGrid)
    {
        List<Vector2Int> neighbors = CubeGridManager.Instance.GetAllNeighbors(buildingGrid);
        // GameDebug.Log($"[EnemyAI] Checking {neighbors.Count} neighbors for grid {buildingGrid}");

        foreach (var neighborGrid in neighbors)
        {
            GameObject tileObj = CubeGridManager.Instance.GetCube(neighborGrid);
            if (tileObj == null) continue;

            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null || tile.hasBuilding || tile.ownerSide != Side.Enemy)
                continue;

            //All neighbors must be enemy
            if (!AreAllNeighborsEnemy(neighborGrid))
                continue;

            if (!spawnableTiles.Contains(tile))
            {
                spawnableTiles.Add(tile);
                // GameDebug.Log($"[EnemyAI] Added spawnable tile at {neighborGrid}");
            }
        }
    }

    bool AreAllNeighborsEnemy(Vector2Int gridPos)
    {
        List<Vector2Int> neighbors = CubeGridManager.Instance.GetAllNeighbors(gridPos);

        foreach (var neighbor in neighbors)
        {
            GameObject neighborObj = CubeGridManager.Instance.GetCube(neighbor);
            if (neighborObj == null)
                continue;

            Tile neighborTile = neighborObj.GetComponent<Tile>();
            if (neighborTile == null)
                continue;

            //  If any neighbor is Player tile fail
            if (neighborTile.ownerSide == Side.Player)
                return false;
        }

        return true;
    }

    void RemoveInvalidSpawnableTiles()
    {
        spawnableTiles.RemoveAll(tile =>
            tile == null ||
            tile.hasBuilding ||
            tile.ownerSide != Side.Enemy ||
            !AreAllNeighborsEnemy(
                CubeGridManager.Instance.WorldToGrid(tile.transform.position)
            )
        );
    }


    void TrySpawnBuilding()
    {
        if (!reduceSpawningAfterMax && totalBuildingsSpawned >= maxEnemyBuildings)
        {
            // GameDebug.Log($"[EnemyAI] Max buildings reached ({maxEnemyBuildings}). Spawning stopped.");
            return;
        }

        // GameDebug.Log($"[EnemyAI] TrySpawnBuilding - Spawnable tiles: {spawnableTiles.Count}");

        if (spawnableTiles.Count == 0)
        {
            // GameDebug.LogWarning("[EnemyAI] No spawnable tiles available!");
            return;
        }

        Tile selectedTile = SelectSpawnTile();
        if (selectedTile == null)
        {
            Debug.LogWarning("[EnemyAI] Failed to select spawn tile!");
            return;
        }

        GameObject buildingPrefab = DecideBuildingToSpawn();
        if (buildingPrefab == null)
        {
            Debug.LogError("[EnemyAI] Failed to decide building to spawn!");
            return;
        }

        // GameDebug.Log($"[EnemyAI] Spawning {buildingPrefab.name} at tile {selectedTile.transform.position}");
        SpawnBuilding(selectedTile, buildingPrefab);
    }

    Tile SelectSpawnTile()
    {
        spawnableTiles.RemoveAll(t => t == null || t.hasBuilding);

        if (spawnableTiles.Count == 0)
            return null;

        return spawnableTiles[Random.Range(0, spawnableTiles.Count)];
    }

    GameObject DecideBuildingToSpawn()
    {
        switch (aiMode)
        {
            case EnemyAIMode.FullAttack:
                return DecideFullAttackBuilding();
            case EnemyAIMode.SemiAttacking:
                return DecideSemiAttackBuilding();
            case EnemyAIMode.Defence:
                return DecideDefenceBuilding();
            case EnemyAIMode.OnlyDefence:
                return DecideOnlyDefenceBuilding();
            default:
                return null;
        }
    }

    GameObject DecideFullAttackBuilding()
    {
        float rand = Random.value;
        if (rand < fullAttack_UnitBuildingRatio)
            return GetRandomUnitBuilding();
        else
            return GetRandomResourceBuilding();
    }

    GameObject DecideSemiAttackBuilding()
    {
        float rand = Random.value;
        if (rand < semiAttack_UnitBuildingRatio)
            return GetRandomUnitBuilding();
        else if (rand < semiAttack_UnitBuildingRatio + semiAttack_ResourceBuildingRatio)
            return GetRandomResourceBuilding();
        else
            return GetRandomDefenceBuilding();
    }

    GameObject DecideDefenceBuilding()
    {
        if (defenceBuildingsSpawned < defence_InitialDefenceCount)
        {
            defenceBuildingsSpawned++;
            return GetRandomDefenceBuilding();
        }

        float rand = Random.value;
        
        if (rand < defence_ResourceRatio)
            return GetRandomResourceBuilding();
        else
            return GetRandomUnitBuilding();
    }

    GameObject DecideOnlyDefenceBuilding()
    {
        if (onlyDefence_AllowAttackBuildings && defenceBuildingsSpawned >= onlyDefence_AttackBuildingsAfterDefenceCount)
        {
            float rand = Random.value;
            if (rand < 0.3f)
                return GetRandomUnitBuilding();
        }

        float rand2 = Random.value;
        if (rand2 < onlyDefence_DefenceRatio)
        {
            defenceBuildingsSpawned++;
            return GetRandomDefenceBuilding();
        }
        else
            return GetRandomResourceBuilding();
    }

    GameObject GetRandomUnitBuilding()
    {
        GameObject[] buildings = GetUnitBuildings();
        return buildings[Random.Range(0, buildings.Length)];
    }

    GameObject GetRandomResourceBuilding()
    {
        GameObject[] buildings = GetResourceBuildings();
        
        if (!allResourcesCovered)
        {
            GameObject building = buildings[resourceBuildingIndex];
            resourceBuildingIndex++;
            if (resourceBuildingIndex >= buildings.Length)
                allResourcesCovered = true;
            
            return building;
        }
        return buildings[Random.Range(0, buildings.Length)];
    }

    GameObject GetRandomDefenceBuilding()
    {
        GameObject[] buildings = GetDefenceBuildings();
        return buildings[Random.Range(0, buildings.Length)];
    }

    GameObject[] GetUnitBuildings()
    {
        switch (enemyFactionName)
        {
            case FactionName.Medieval:
                return new[] { factionData.medievalAirBuilding, factionData.medievalInfantryBuilding, factionData.medievalTankBuilding };
            case FactionName.Present:
                return new[] { factionData.presentAirBuilding, factionData.presentInfantryBuilding, factionData.presentTankBuilding };
            case FactionName.Futuristic:
                return new[] { factionData.futureAirBuilding, factionData.futureInfantryBuilding, factionData.futureTankBuilding };
            case FactionName.Galvadore:
                return new[] { factionData.galvadoreAirBuilding, factionData.galvadoreInfantryBuilding, factionData.galvadoreTankBuilding };
            default:
                return new GameObject[0];
        }
    }

    GameObject[] GetResourceBuildings()
    {
        switch (enemyFactionName)
        {
            case FactionName.Medieval:
                return new[] { factionData.medievalFoodBuilding, factionData.medievalGoldBuilding, factionData.medievalMetalBuilding, factionData.medievalPowerBuilding };
            case FactionName.Present:
                return new[] { factionData.presentFoodBuilding, factionData.presentGoldBuilding, factionData.presentMetalBuilding, factionData.presentPowerBuilding};
            case FactionName.Futuristic:
                return new[] { factionData.futureFoodBuilding, factionData.futureGoldBuilding, factionData.futureMetalBuilding, factionData.futurePowerBuilding };
            case FactionName.Galvadore:
                return new[] { factionData.galvadoreFoodBuilding, factionData.galvadoreGoldBuilding, factionData.galvadoreMetalBuilding, factionData.galvadorePowerBuilding };
            default:
                return new GameObject[0];
        }
    }

    GameObject[] GetDefenceBuildings()
    {
        switch (enemyFactionName)
        {
            case FactionName.Medieval:
                return new[] { factionData.medievalAntiAirBuilding, factionData.medievalAntiTankBuilding, factionData.pastTurretBuilding };
            case FactionName.Present:
                return new[] { factionData.presentAntiAirBuilding, factionData.presentAntiTankBuilding, factionData.presentTurretBuilding };
            case FactionName.Futuristic:
                return new[] { factionData.futureAntiAirBuilding, factionData.futureAntiTankBuilding, factionData.futureTurretBuilding };
            case FactionName.Galvadore:
                return new[] { factionData.galvadoreAntiAirBuilding, factionData.galvadoreAntiTankBuilding, factionData.galvadoreTurretBuilding };
            default:
                return new GameObject[0];
        }
    }

    void SpawnBuilding(Tile tile, GameObject buildingPrefab)
    {
        if (tile == null || buildingPrefab == null || tile.hasBuilding)
        {
            Debug.LogWarning("[EnemyAI] Cannot spawn building - invalid tile or prefab");
            return;
        }

        Vector3 spawnPos = tile.transform.position + Vector3.up * 2f;
        enemyBuildPanel.PlaceBuildingAI(buildingPrefab, spawnPos, tile);
        
       // Instantiate(buildingPrefab, spawnPos, Quaternion.identity, tile.transform);
       tile.SetBuildingPlaced();

        spawnableTiles.Remove(tile);
        Vector2Int tileGrid = CubeGridManager.Instance.WorldToGrid(tile.transform.position);
        UpdateSpawnableTiles(tileGrid);

        totalBuildingsSpawned++;
        // GameDebug.Log($"[EnemyAI] Building spawned! Total: {totalBuildingsSpawned}, Spawnable tiles: {spawnableTiles.Count}");
    }
    
}
