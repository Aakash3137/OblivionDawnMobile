using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BuildingCategory { Unit, Resource, Defense }

public class EnemyAIHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyModeSwitch enemyModeSwitch;
    [SerializeField] private EnemyBuildPanel enemyBuildPanel;
    [SerializeField] private AllFactionsData factionData;
    [SerializeField] private FactionName enemyFactionName;
    [SerializeField] private EnemyPersonality currentPersonality;

    private Transform enemyMainBuildingTransform;
    private Transform playerMainBuildingTransform;
    private List<Tile> spawnableTiles = new List<Tile>();
    private int totalBuildingsSpawned = 0;
    private float spawnTimer = 0f;
    private int resourceBuildingIndex = 0;
    private bool allResourcesCovered = false;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[EnemyAI] GameManager.Instance is null!");
            return;
        }

        enemyMainBuildingTransform = GameManager.Instance.enemySpawnPoint;
        playerMainBuildingTransform = GameManager.Instance.playerSpawnPoint;

        Invoke(nameof(InitializeSpawnableTiles), 1f);
    }

    void Update()
    {
        if (enemyModeSwitch == null || enemyModeSwitch.EnemyMode != CurrentEnemyMode.EnemyAIMode)
            return;

        RemoveInvalidSpawnableTiles();

        float interval = currentPersonality.spawnInterval;


        spawnTimer += Time.deltaTime;
        if (spawnTimer >= interval)
        {
            spawnTimer = 0f;
            TrySpawnBuilding();
        }
    }

    void InitializeSpawnableTiles()
    {
        if (enemyMainBuildingTransform == null || CubeGridManager.Instance == null)
            return;

        Vector2Int mainGrid = CubeGridManager.Instance.WorldToGrid(enemyMainBuildingTransform.position);
        UpdateSpawnableTiles(mainGrid);
    }

    void UpdateSpawnableTiles(Vector2Int buildingGrid)
    {
        foreach (var neighborGrid in CubeGridManager.Instance.GetAllNeighbors(buildingGrid))
        {
            Tile tile = CubeGridManager.Instance.GetCube(neighborGrid);
            if (tile == null || tile.hasBuilding || tile.ownerSide != Side.Enemy)
                continue;

            if (!AreAllNeighborsEnemy(neighborGrid))
                continue;

            if (!spawnableTiles.Contains(tile))
                spawnableTiles.Add(tile);
        }
    }

    bool AreAllNeighborsEnemy(Vector2Int gridPos)
    {
        foreach (var neighbor in CubeGridManager.Instance.GetAllNeighbors(gridPos))
        {
            Tile neighborTile = CubeGridManager.Instance.GetCube(neighbor);
            if (neighborTile != null && neighborTile.ownerSide == Side.Player)
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
            !AreAllNeighborsEnemy(CubeGridManager.Instance.WorldToGrid(tile.transform.position))
        );
    }

    void TrySpawnBuilding()
    {
        if (totalBuildingsSpawned >= currentPersonality.maxEnemyBuildings)
            return;

        if (spawnableTiles.Count == 0)
            return;

        bool makeMistake = Random.value < currentPersonality.mistakeProbability;
        GameObject buildingPrefab = null;
        BuildingCategory category = BuildingCategory.Unit;

        if (makeMistake)
        {
            buildingPrefab = GetRandomBuildingAny();
        }
        else
        {
            buildingPrefab = DecideBuildingUsingPersonality(out category);
        }

        if (buildingPrefab == null)
        {
            Debug.LogWarning("[EnemyAI] No building prefab selected");
            return;
        }

        Tile selectedTile = null;
        bool useOptimalTile = Random.value < currentPersonality.tacticalDiscipline;

        if (useOptimalTile && currentPersonality.tacticalPrecision > 0f)
        {
            selectedTile = SelectBestTileForCategory(category);
        }
        else
        {
            selectedTile = SelectRandomSpawnableTile();
        }

        if (selectedTile == null)
        {
            Debug.LogWarning("[EnemyAI] No valid tile selected");
            return;
        }

        SpawnBuilding(selectedTile, buildingPrefab);
    }

    GameObject DecideBuildingUsingPersonality(out BuildingCategory category)
    {
        // Normalize category weights
        float total = currentPersonality.unitBuildingWeight
                    + currentPersonality.resourceBuildingWeight
                    + currentPersonality.defenseBuildingWeight;
        float rand = Random.value * total;

        if (rand < currentPersonality.unitBuildingWeight)
        {
            category = BuildingCategory.Unit;
            return GetWeightedUnitBuilding();
        }
        rand -= currentPersonality.unitBuildingWeight;
        if (rand < currentPersonality.resourceBuildingWeight)
        {
            category = BuildingCategory.Resource;
            return GetWeightedResourceBuilding();
        }
        else
        {
            category = BuildingCategory.Defense;
            return GetWeightedDefenseBuilding();
        }
    }

    GameObject GetWeightedUnitBuilding()
    {
        GameObject[] buildings = GetUnitBuildings();
        float[] weights = currentPersonality.unitTypeWeights;
        return GetWeightedRandom(buildings, weights);
    }

    GameObject GetWeightedResourceBuilding()
    {
        GameObject[] buildings = GetResourceBuildings();
        float[] weights = currentPersonality.resourceTypeWeights;
        
        // Handle sequential resource building selection if not all resources covered
        if (!allResourcesCovered && buildings.Length > 0)
        {
            GameObject building = buildings[resourceBuildingIndex];
            resourceBuildingIndex++;
            if (resourceBuildingIndex >= buildings.Length)
                allResourcesCovered = true;
            return building;
        }
        
        return GetWeightedRandom(buildings, weights);
    }

    GameObject GetWeightedDefenseBuilding()
    {
        GameObject[] buildings = GetDefenceBuildings();
        float[] weights = currentPersonality.defenseTypeWeights;
        return GetWeightedRandom(buildings, weights);
    }

    GameObject GetWeightedRandom(GameObject[] options, float[] weights)
    {
        if (options.Length == 0 || weights.Length == 0)
            return null;

        // Ensure weights array matches options length
        float[] normalizedWeights = new float[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            normalizedWeights[i] = (i < weights.Length) ? weights[i] : 1f / options.Length;
        }

        float total = normalizedWeights.Sum();
        float rand = Random.value * total;
        float accum = 0f;

        for (int i = 0; i < options.Length; i++)
        {
            accum += normalizedWeights[i];
            if (rand < accum)
                return options[i];
        }
        return options[options.Length - 1];
    }

    GameObject GetRandomBuildingAny()
    {
        var allBuildings = new List<GameObject>();
        allBuildings.AddRange(GetUnitBuildings());
        allBuildings.AddRange(GetResourceBuildings());
        allBuildings.AddRange(GetDefenceBuildings());
        if (allBuildings.Count == 0) return null;
        return allBuildings[Random.Range(0, allBuildings.Count)];
    }

    Tile SelectRandomSpawnableTile()
    {
        spawnableTiles.RemoveAll(t => t == null || t.hasBuilding);
        if (spawnableTiles.Count == 0) return null;
        return spawnableTiles[Random.Range(0, spawnableTiles.Count)];
    }

    Tile SelectBestTileForCategory(BuildingCategory category)
    {
        if (spawnableTiles.Count == 0) return null;

        Vector3 enemyPos = enemyMainBuildingTransform.position;
        Vector3 playerPos = playerMainBuildingTransform.position;
        Vector3 dirToPlayer = (playerPos - enemyPos).normalized;

        Tile bestTile = null;
        float bestScore = float.MinValue;

        foreach (Tile tile in spawnableTiles)
        {
            if (tile == null || tile.hasBuilding) continue;

            Vector3 tilePos = tile.transform.position;
            float forwardness = Vector3.Dot(tilePos - enemyPos, dirToPlayer);

            float score = 0f;
            switch (category)
            {
                case BuildingCategory.Defense:
                    score = forwardness; // prefer forward (closer to player)
                    break;
                case BuildingCategory.Resource:
                    score = -forwardness; // prefer backward (away from player)
                    break;
                case BuildingCategory.Unit:
                    score = 0f; // neutral
                    break;
            }

            // Add some randomness based on tacticalPrecision
            score = (score * currentPersonality.tacticalPrecision) + 
                    (Random.Range(-1f, 1f) * (1f - currentPersonality.tacticalPrecision));

            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }

        return bestTile ?? SelectRandomSpawnableTile();
    }

    // ============== BUILDING RETRIEVAL METHODS ==============

    GameObject[] GetUnitBuildings()
    {
        switch (enemyFactionName)
        {
            case FactionName.Medieval:
                return new[] { 
                    factionData.medievalAirBuilding, 
                    factionData.medievalMeleeBuilding, 
                    factionData.medievalRangedBuilding, 
                    factionData.medievalAOERangedBuilding
                };
            case FactionName.Present:
                return new[] { 
                    factionData.presentAirBuilding, 
                    factionData.presentMeleeBuilding, 
                    factionData.presentRangedBuilding, 
                    factionData.presentAOERangedBuilding
                };
            case FactionName.Futuristic:
                return new[] { 
                    factionData.futureAirBuilding, 
                    factionData.futureMeleeBuilding, 
                    factionData.futureRangedBuilding, 
                    factionData.futureAOERangedBuilding
                };
            case FactionName.Galvadore:
                return new[] { 
                    factionData.galvadoreAirBuilding, 
                    factionData.galvadoreMeleeBuilding, 
                    factionData.galvadoreRangedBuilding, 
                    factionData.galvadoreAOERangedBuilding
                };
            default:
                Debug.LogWarning($"[EnemyAI] Unknown faction: {enemyFactionName}");
                return new GameObject[0];
        }
    }

    GameObject[] GetResourceBuildings()
    {
        switch (enemyFactionName)
        {
            case FactionName.Medieval:
                return new[] { 
                    factionData.medievalFoodBuilding, 
                    factionData.medievalGoldBuilding, 
                    factionData.medievalMetalBuilding, 
                    factionData.medievalPowerBuilding 
                };
            case FactionName.Present:
                return new[] { 
                    factionData.presentFoodBuilding, 
                    factionData.presentGoldBuilding, 
                    factionData.presentMetalBuilding, 
                    factionData.presentPowerBuilding 
                };
            case FactionName.Futuristic:
                return new[] { 
                    factionData.futureFoodBuilding, 
                    factionData.futureGoldBuilding, 
                    factionData.futureMetalBuilding, 
                    factionData.futurePowerBuilding 
                };
            case FactionName.Galvadore:
                return new[] { 
                    factionData.galvadoreFoodBuilding, 
                    factionData.galvadoreGoldBuilding, 
                    factionData.galvadoreMetalBuilding, 
                    factionData.galvadorePowerBuilding 
                };
            default:
                Debug.LogWarning($"[EnemyAI] Unknown faction: {enemyFactionName}");
                return new GameObject[0];
        }
    }

    GameObject[] GetDefenceBuildings()
    {
        switch (enemyFactionName)
        {
            case FactionName.Medieval:
                return new[] { 
                    factionData.medievalAntiAirBuilding, 
                    factionData.medievalAntiTankBuilding, 
                    factionData.medievalTurretBuilding
                };
            case FactionName.Present:
                return new[] { 
                    factionData.presentAntiAirBuilding, 
                    factionData.presentAntiTankBuilding, 
                    factionData.presentTurretBuilding 
                };
            case FactionName.Futuristic:
                return new[] { 
                    factionData.futureAntiAirBuilding, 
                    factionData.futureAntiTankBuilding, 
                    factionData.futureTurretBuilding 
                };
            case FactionName.Galvadore:
                return new[] { 
                    factionData.galvadoreAntiAirBuilding, 
                    factionData.galvadoreAntiTankBuilding, 
                    factionData.galvadoreTurretBuilding 
                };
            default:
                Debug.LogWarning($"[EnemyAI] Unknown faction: {enemyFactionName}");
                return new GameObject[0];
        }
    }

    // ============== SPAWNING METHOD ==============

    void SpawnBuilding(Tile tile, GameObject buildingPrefab)
    {
        if (tile == null || buildingPrefab == null || tile.hasBuilding)
            return;

        Vector3 spawnPos = tile.transform.position + Vector3.up * 2f;

        if (enemyBuildPanel.PlaceBuildingAI(buildingPrefab, spawnPos, tile))
        {
            spawnableTiles.Remove(tile);
            Vector2Int tileGrid = CubeGridManager.Instance.WorldToGrid(tile.transform.position);
            UpdateSpawnableTiles(tileGrid);
            totalBuildingsSpawned++;

            Debug.Log($"[EnemyAI] Spawned {buildingPrefab.name}. Total: {totalBuildingsSpawned}");
        }
    }

    // ============== PUBLIC METHODS ==============

    public void SetPersonality(EnemyPersonality personality)
    {
        currentPersonality = personality;
        Debug.Log($"[EnemyAI] Personality set to: {personality.personalityName}");
    }

    public void SetFaction(FactionName faction)
    {
        enemyFactionName = faction;
    }
}