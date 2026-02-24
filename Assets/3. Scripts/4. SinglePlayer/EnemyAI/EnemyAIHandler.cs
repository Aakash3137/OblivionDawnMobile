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
    [SerializeField] internal FactionName enemyFactionName;
    [SerializeField] private EnemyPersonality currentPersonality;

    [SerializeField] private List<EnemyPersonality> AIPersonalities;
    [SerializeField] private DecSelectionData AIDecSelectionData;
    
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

        if (MenuManager.Instance != null)
        {
            AIPersonalityEnum chosen = MenuManager.Instance.SelectedPersonalityFromMenu();
            currentPersonality = AIPersonalities.Find(p => p.personalityName == chosen);
        }
        
        enemyMainBuildingTransform = GameManager.Instance.enemySpawnPoint;
        playerMainBuildingTransform = GameManager.Instance.playerSpawnPoint;
        
        GameManager.Instance.EnemyFactionName = enemyFactionName;

        Invoke(nameof(InitializeSpawnableTiles), 1f);
    }

    void Update()
    {
        
        if (enemyModeSwitch == null || enemyModeSwitch.EnemyMode != CurrentEnemyMode.EnemyAIMode)
            return;

        if (currentPersonality == null)
            return;

        RemoveInvalidSpawnableTiles();
        
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentPersonality.spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnBuilding();
        }
    }

    #region TilesAllocation

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
    
    #endregion 
    
    void TrySpawnBuilding()
    {
        if (totalBuildingsSpawned >= currentPersonality.maxEnemyBuildings)
            return;

        if (spawnableTiles.Count == 0)
            return;

        bool makeMistake = Random.value < currentPersonality.mistakeProbability;
        BuildingStats buildingPrefab = null;
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

    BuildingStats DecideBuildingUsingPersonality(out BuildingCategory category)
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
    
    #region Personality Deck Access

    FactionDeckSelection GetCurrentFactionDeck()
    {
        if (currentPersonality == null)
            return null;

        return currentPersonality.factionDeckSelections
            .Find(f => f.faction == enemyFactionName);
    }

    #endregion

 #region Weighted Building Selection

    BuildingStats GetWeightedUnitBuilding()
    {
        var deck = GetCurrentFactionDeck();
        if (deck == null)
            return null;

        List<BuildingStats> options = new List<BuildingStats>();
        List<float> weights = new List<float>();

        foreach (var entry in deck.unitSelections)
        {
            if (!entry.selected || entry.amount <= 0)
                continue;

            var building =
                CharacterDatabase.Instance
                .GetSpawnerBuilding(entry.unit);

            if (building == null)
                continue;

            options.Add(building);
            weights.Add(entry.weight);
        }

        return GetWeightedRandom(
            options.ToArray(),
            weights.ToArray());
    }

    BuildingStats GetWeightedDefenseBuilding()
    {
        var deck = GetCurrentFactionDeck();
        if (deck == null)
            return null;

        List<BuildingStats> options = new List<BuildingStats>();
        List<float> weights = new List<float>();

        foreach (var entry in deck.defenseBuildingSelections)
        {
            if (!entry.selected || entry.amount <= 0)
                continue;

            var building =
                CharacterDatabase.Instance
                .GetDefenseBuildingPrefab(entry.building);

            if (building == null)
                continue;

            options.Add(building);
            weights.Add(entry.weight);
        }

        return GetWeightedRandom(
            options.ToArray(),
            weights.ToArray());
    }

    BuildingStats GetWeightedResourceBuilding()
    {
        BuildingStats[] buildings = GetResourceBuildings();
        float[] weights = currentPersonality.resourceTypeWeights;

        if (!allResourcesCovered && buildings.Length > 0)
        {
            BuildingStats building =
                buildings[resourceBuildingIndex];

            resourceBuildingIndex++;

            if (resourceBuildingIndex >= buildings.Length)
                allResourcesCovered = true;

            return building;
        }

        return GetWeightedRandom(buildings, weights);
    }

    BuildingStats GetWeightedRandom(
        BuildingStats[] options,
        float[] weights)
    {
        if (options == null ||
            weights == null ||
            options.Length == 0 ||
            weights.Length == 0)
            return null;

        int count = Mathf.Min(options.Length, weights.Length);

        float total = 0f;
        for (int i = 0; i < count; i++)
            total += weights[i];

        if (total <= 0f)
            return options[Random.Range(0, count)];

        float rand = Random.value * total;
        float accum = 0f;

        for (int i = 0; i < count; i++)
        {
            accum += weights[i];
            if (rand <= accum)
                return options[i];
        }

        return options[count - 1];
    }

    #endregion
    
    // ============== BUILDING RETRIEVAL METHODS ==============

  #region Building Retrieval

    OffenseBuildingStats[] GetUnitBuildings()
    {
        var deck = GetCurrentFactionDeck();
        if (deck == null)
            return new OffenseBuildingStats[0];

        List<OffenseBuildingStats> buildings =
            new List<OffenseBuildingStats>();

        foreach (var entry in deck.unitSelections)
        {
            if (!entry.selected || entry.amount <= 0)
                continue;

            var building =
                CharacterDatabase.Instance
                .GetSpawnerBuilding(entry.unit);

            if (building != null)
                buildings.Add(building);
        }

        return buildings.ToArray();
    }

    BuildingStats[] GetResourceBuildings()
    {
        var deck =
            AIDecSelectionData
            .AllFactionDecData[(int)enemyFactionName]
            .SelectedResourceDeck;

        if (deck == null || deck.Count == 0)
            return new BuildingStats[0];

        BuildingStats[] buildings =
            new BuildingStats[deck.Count];

        for (int i = 0; i < deck.Count; i++)
        {
            buildings[i] =
                CharacterDatabase.Instance
                .GetResourceBuildingPrefab(deck[i]);
        }

        return buildings;
    }

    BuildingStats[] GetDefenceBuildings()
    {
        var deck = GetCurrentFactionDeck();
        if (deck == null)
            return new BuildingStats[0];

        List<BuildingStats> buildings =
            new List<BuildingStats>();

        foreach (var entry in deck.defenseBuildingSelections)
        {
            if (!entry.selected || entry.amount <= 0)
                continue;

            var building =
                CharacterDatabase.Instance
                .GetDefenseBuildingPrefab(entry.building);

            if (building != null)
                buildings.Add(building);
        }

        return buildings.ToArray();
    }

    BuildingStats GetRandomBuildingAny()
    {
        var all = new List<BuildingStats>();

        all.AddRange(GetUnitBuildings());
        all.AddRange(GetResourceBuildings());
        all.AddRange(GetDefenceBuildings());

        if (all.Count == 0)
            return null;

        return all[Random.Range(0, all.Count)];
    }

    #endregion
    
    // ============== TILE SELECTION ==============
    
    #region Tile Selection

    Tile SelectRandomSpawnableTile()
    {
        spawnableTiles.RemoveAll(
            t => t == null || t.hasBuilding);

        if (spawnableTiles.Count == 0)
            return null;

        return spawnableTiles[
            Random.Range(0, spawnableTiles.Count)];
    }

    Tile SelectBestTileForCategory(
        BuildingCategory category)
    {
        if (spawnableTiles.Count == 0)
            return null;

        Vector3 enemyPos =
            enemyMainBuildingTransform.position;

        Vector3 playerPos =
            playerMainBuildingTransform.position;

        Vector3 dirToPlayer =
            (playerPos - enemyPos).normalized;

        Tile bestTile = null;
        float bestScore = float.MinValue;

        foreach (Tile tile in spawnableTiles)
        {
            if (tile == null || tile.hasBuilding)
                continue;

            Vector3 tilePos = tile.transform.position;
            float forwardness =
                Vector3.Dot(tilePos - enemyPos, dirToPlayer);

            float score = 0f;

            switch (category)
            {
                case BuildingCategory.Defense:
                    score = forwardness;
                    break;

                case BuildingCategory.Resource:
                    score = -forwardness;
                    break;

                case BuildingCategory.Unit:
                    score = 0f;
                    break;
            }

            score =
                (score * currentPersonality.tacticalPrecision) +
                (Random.Range(-1f, 1f) *
                 (1f - currentPersonality.tacticalPrecision));

            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }

        return bestTile ?? SelectRandomSpawnableTile();
    }

    #endregion


    // ============== SPAWNING METHOD ==============

    void SpawnBuilding(Tile tile, BuildingStats buildingPrefab)
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