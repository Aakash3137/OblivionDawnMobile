using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BuildingCategory
{
    Unit,
    Resource,
    Defense
}

public class EnemyAIHandler : MonoBehaviour
{
    public static EnemyAIHandler Instance { get; private set; }

    [Header("References")] [SerializeField]
    private EnemyModeSwitch enemyModeSwitch;

    [SerializeField] private EnemyBuildPanel enemyBuildPanel;
    [SerializeField] private AllFactionsData factionData;
    [SerializeField] internal FactionName enemyFactionName;
    [SerializeField] private EnemyPersonality currentPersonality;

    [SerializeField] private List<EnemyPersonality> AIPersonalities;
    [SerializeField] private AllBuildingData allBuildingData;

    [SerializeField] private float minResourcePercent = 15f; // never drop below 15% of each resource

    private Transform enemyMainBuildingTransform;
    private Transform playerMainBuildingTransform;
    private List<Tile> spawnableTiles = new List<Tile>();

    private int totalBuildingsSpawned = 0;
    private float spawnTimer = 0f;

    private float currentSpawnInterval;

    private int resourceBuildingIndex = 0;
    private bool allResourcesCovered = false;

    [Header("READ ONLY")] [SerializeField] private int unitBuilt = 0;
    [SerializeField] private int resourceBuilt = 0;
    [SerializeField] private int defenseBuilt = 0;
    [SerializeField] private int wallsBuilt = 0;
    [SerializeField] private int nonWallDefenseBuilt = 0;

    // New Wall Strategy
    private int initialWallsTarget = 6; // 6-8 walls
    private int emergencyWallsTarget = 5; // 4-6 walls
    private bool initialWallPhaseDone = false;
    private bool emergencyWallTriggered = false;
    private bool emergencyWallPhaseDone = false;
    private int emergencyWallsBuilt = 0;
    private float lastUnitRatio = 1f;

    // Resource need tracking
    [SerializeField] private float[] resourceNeedPercentages = new float[4]; // % out of 100 for each resource
    private float timeSinceLastAnalysis = 0f;
    private const float RECHECK_INTERVAL = 10f;

    // Resource building tracking
    private int[] resourceBuildingCounts = new int[4];
    private bool balancedPhaseComplete = false;
    
    // RESOURCE IMBALANCE CONTROL 
    private bool resourceCorrectionActive = false;
    private float resourceCorrectionEndTime = 0f;
    [SerializeField] private float correctionDuration = 8f;

    private int forcedResourceIndexA = -1;
    private int forcedResourceIndexB = -1;

    //Resource build economical
    [SerializeField] private int earlyResourceTarget = 2;
    private bool earlyEconomyComplete = false;

    // Enemy Stance variable
    public UnitStance CurrentEnemyStance { get; private set; } = UnitStance.Defending;
    private float lastStanceChangeTime = -999f;
    public Image StanceImg;
    public Sprite AttackSprite;
    public Sprite DefendSprite;

    private bool defendLocked = false;
    private float lastDefendEnterTime = -999f;
    [SerializeField] private float defendCooldownAfterAttack = 12f;

    // ===== BUILD FAILURE SYSTEM =====
    // Emergency control
    private bool emergencyUnlocked = false;
    private bool emergencyUsedOnce = false;
    private float lastEmergencyEndTime = -999f;
    [SerializeField] private float emergencyRecoveryBuffer = 10f;

    private int failedBuildAttempts = 0;
    private const int MAX_FAIL_BEFORE_RESOURCE = 2;
    private BuildingCategory lastFailedCategory;

// ===== EMERGENCY STATE =====
    enum EmergencyState
    {
        None,
        Walls,
        Recovery
    }

    private EmergencyState emergencyState = EmergencyState.None;

    private int emergencyUnitsPlaced = 0;
    private int emergencyDefensePlaced = 0;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        GameData.enemyFaction = enemyFactionName;
    }

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

        //Setting spawn interval
        currentSpawnInterval = currentPersonality.spawnInterval;

        // Randomize wall targets
        initialWallsTarget = Random.Range(6, 8); // 6-8
        emergencyWallsTarget = Random.Range(4, 7); // 4-6

        Debug.Log(
            $"[EnemyAI][Wall] Initialized - Initial walls target: {initialWallsTarget}, Emergency walls target: {emergencyWallsTarget}");

        enemyMainBuildingTransform = GameManager.Instance.enemySpawnPoint;
        playerMainBuildingTransform = GameManager.Instance.playerSpawnPoint;

        Invoke(nameof(AnalyzeResourceNeeds), 1.5f);
        Invoke(nameof(InitializeSpawnableTiles), 1f);
    }

    void Update()
    {
        if (enemyModeSwitch == null || enemyModeSwitch.EnemyMode != CurrentEnemyMode.EnemyAIMode)
            return;

        RemoveInvalidSpawnableTiles();

        timeSinceLastAnalysis += Time.deltaTime;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnBuilding();
        }

        UpdateCombatStance();
        CheckResourceImbalance();

        if (!emergencyUnlocked)
        {
            if (KillCounterManager.Instance != null &&
                KillCounterManager.Instance.enemyTotalUnitKills > 5)
            {
                emergencyUnlocked = true;
                Debug.Log("[EnemyAI][Wall] Emergency system UNLOCKED (mid game reached)");
            }
        }
    }

    #region TilesAllocation

    void InitializeSpawnableTiles()
    {
        if (GameManager.Instance.enemySpawnPoint != null)
            enemyMainBuildingTransform = GameManager.Instance.enemySpawnPoint;
        if (GameManager.Instance.playerSpawnPoint != null)
            playerMainBuildingTransform = GameManager.Instance.playerSpawnPoint;

        if (enemyMainBuildingTransform == null || CubeGridManager.Instance == null)
            return;

        Vector2Int mainGrid = CubeGridManager.Instance.WorldToGrid(enemyMainBuildingTransform.position);
        UpdateSpawnableTiles(mainGrid);
    }

    void UpdateSpawnableTiles(Vector2Int buildingGrid)
    {
        foreach (var neighborGrid in CubeGridManager.Instance.GetAllNeighbors(buildingGrid))
        {
            Tile tile = CubeGridManager.Instance.GetTile(neighborGrid);
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
            Tile neighborTile = CubeGridManager.Instance.GetTile(neighbor);
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

    bool IsEarlyEconomyComplete()
    {
        for (int i = 0; i < 4; i++)
        {
            if (resourceBuildingCounts[i] < earlyResourceTarget)
                return false;
        }

        return true;
    }

    #region WallStrategy

    bool HasWallInDeck()
    {
        var deck = GetCurrentFactionDeck();
        if (deck == null)
        {
            Debug.LogWarning("[EnemyAI][Wall] GetCurrentFactionDeck returned null!");
            return false;
        }

        foreach (var entry in deck.defenseBuildingSelections)
        {
            if (!entry.selected) continue;

            var building = CharacterDatabase.Instance.GetDefenseBuildingPrefab(entry.building);
            if (building == null) continue;

            // Check if it's a wall using DefenseBuildingDataSO
            if (entry.building != null && entry.building.defenseType == ScenarioDefenseType.Wall)
            {
                Debug.Log($"[EnemyAI][Wall] Found wall in deck: {entry.building.name}");
                return true;
            }
        }

        Debug.LogWarning("[EnemyAI][Wall] No wall found in deck!");
        return false;
    }

    BuildingStats GetWallBuilding()
    {
        var deck = GetCurrentFactionDeck();
        if (deck == null) return null;

        foreach (var entry in deck.defenseBuildingSelections)
        {
            if (!entry.selected) continue;

            var building = CharacterDatabase.Instance.GetDefenseBuildingPrefab(entry.building);
            if (building == null) continue;

            // Check if it's a wall using DefenseBuildingDataSO
            if (entry.building != null && entry.building.defenseType == ScenarioDefenseType.Wall)
            {
                Debug.Log($"[EnemyAI][Wall] Returning wall building: {building.name}");
                return building;
            }
        }

        Debug.LogWarning("[EnemyAI][Wall] GetWallBuilding returned null!");
        return null;
    }

    BuildingStats GetNonWallDefenseBuilding()
    {
        var deck = GetCurrentFactionDeck();
        if (deck == null) return null;

        List<BuildingStats> options = new List<BuildingStats>();
        List<float> weights = new List<float>();

        foreach (var entry in deck.defenseBuildingSelections)
        {
            if (!entry.selected || entry.amount <= 0) continue;

            var building = CharacterDatabase.Instance.GetDefenseBuildingPrefab(entry.building);
            if (building == null || building is WallStats) continue;

            options.Add(building);
            weights.Add(entry.weight);
        }

        return GetWeightedRandom(options.ToArray(), weights.ToArray());
    }

    #endregion

    void TrySpawnBuilding()
    {
        // ===== FAILURE → RESOURCE RECOVERY =====
        if (failedBuildAttempts >= MAX_FAIL_BEFORE_RESOURCE)
        {
            failedBuildAttempts = 0;

            Debug.Log("[AI] Build failed → switching to resource");

            BuildingStats res = GetWeightedResourceBuilding();
            Tile tile = SelectRandomSpawnableTile();

            if (res != null && tile != null)
            {
                SpawnBuilding(tile, res, BuildingCategory.Resource);
                return;
            }
        }


        if (totalBuildingsSpawned >= currentPersonality.maxEnemyBuildings)
            return;

        if (unitBuilt >= currentPersonality.maxOffenseBuilding)
        {
            currentSpawnInterval = currentPersonality.reduceSpawnTime;
        }

        if (spawnableTiles.Count == 0)
            return;

        BuildingStats buildingPrefab = null;
        BuildingCategory category = BuildingCategory.Unit;

        buildingPrefab = DecideBuildingUsingPersonality(out category);

        if (buildingPrefab == null)
        {
            Debug.LogWarning("[EnemyAI] No building prefab selected");
            return;
        }

        Tile selectedTile = null;
        bool useOptimalTile = Random.value < currentPersonality.tacticalDiscipline;

        if (useOptimalTile && currentPersonality.tacticalPrecision > 0f)
        {
            selectedTile = SelectBestTileForCategory(category, buildingPrefab);
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

        SpawnBuilding(selectedTile, buildingPrefab, category);
    }

    BuildingStats DecideBuildingUsingPersonality(out BuildingCategory category)
    {
        // ===== EARLY ECONOMY =====
        if (!earlyEconomyComplete)
        {
            if (IsEarlyEconomyComplete())
            {
                earlyEconomyComplete = true;
            }
            else
            {
                category = BuildingCategory.Resource;
                return GetLowestResourceBuilding();
            }
        }

        bool isDefensiveStyle =
            currentPersonality.playStyle == PlayStyle.Defence ||
            currentPersonality.playStyle == PlayStyle.Mix;

        bool isAttackStyle =
            currentPersonality.playStyle == PlayStyle.Attack;

        bool canUseWalls =
            currentPersonality.useSmartWallStrategy &&
            HasWallInDeck();

        int myUnits = BattleUnitRegistry.EnemyUnits.Count;
        int playerUnits = Mathf.Max(1, BattleUnitRegistry.PlayerUnits.Count);
        float ratio = (float)myUnits / playerUnits;

        // EMERGENCY SYSTEM (TOP PRIORITY)

        bool canTriggerEmergency =
            emergencyUnlocked &&
            ratio < 0.4f &&
            playerUnits >= 3 &&
            !emergencyWallTriggered &&
            Time.time - lastEmergencyEndTime > emergencyRecoveryBuffer;

        if (canTriggerEmergency)
        {
            emergencyWallTriggered = true;
            emergencyState = EmergencyState.Walls;
            emergencyWallsBuilt = 0;

            Debug.Log("[AI] Emergency TRIGGERED");
        }

        if (emergencyWallTriggered)
        {
            // ===== PHASE 1: WALLS =====
            if (emergencyState == EmergencyState.Walls)
            {
                /*if (emergencyWallsBuilt < emergencyWallsTarget)
                {
                    emergencyWallsBuilt++;

                    category = BuildingCategory.Defense;
                    return GetWallBuilding();
                }*/

                emergencyState = EmergencyState.Recovery;
                emergencyUnitsPlaced = 0;
                emergencyDefensePlaced = 0;
            }

            // ===== PHASE 2: PLAYSTYLE RECOVERY =====

            if (currentPersonality.playStyle == PlayStyle.Attack)
            {
                /*if (emergencyUnitsPlaced < 4)
                {
                    emergencyUnitsPlaced++;
                    category = BuildingCategory.Unit;
                    return GetWeightedUnitBuilding();
                }*/
                if (emergencyDefensePlaced < 2)
                {
                    emergencyDefensePlaced++;
                    category = BuildingCategory.Defense;
                    return GetNonWallDefenseBuilding();
                }

                if (emergencyUnitsPlaced < 3)
                {
                    emergencyUnitsPlaced++;
                    category = BuildingCategory.Unit;
                    return GetWeightedUnitBuilding();
                }
            }
            else if (currentPersonality.playStyle == PlayStyle.Defence)
            {
                if (emergencyDefensePlaced < 4)
                {
                    emergencyDefensePlaced++;
                    category = BuildingCategory.Defense;
                    return GetNonWallDefenseBuilding();
                }
            }
            else // MIX
            {
                if (emergencyDefensePlaced < 2)
                {
                    emergencyDefensePlaced++;
                    category = BuildingCategory.Defense;
                    return GetNonWallDefenseBuilding();
                }

                if (emergencyUnitsPlaced < 3)
                {
                    emergencyUnitsPlaced++;
                    category = BuildingCategory.Unit;
                    return GetWeightedUnitBuilding();
                }
            }

            EndEmergency();
            category = BuildingCategory.Unit;
            return GetWeightedUnitBuilding();
        }

        // INITIAL WALL PHASE

        if (!initialWallPhaseDone && canUseWalls)
        {
            if (wallsBuilt < initialWallsTarget)
            {
                category = BuildingCategory.Defense;
                return GetWallBuilding();
            }
            else
            {
                initialWallPhaseDone = true;
                Debug.Log("[AI] Initial wall phase COMPLETE");
            }
        }

        // FAILURE RECOVERY → RESOURCE

        if (failedBuildAttempts > 0)
        {
            // 50% chance resource, otherwise recover with combat
            if (Random.value < 0.5f)
            {
                category = BuildingCategory.Resource;
                return GetWeightedResourceBuilding();
            }
            else
            {
                category = BuildingCategory.Unit;
                return GetWeightedUnitBuilding();
            }
        }

        // SAFETY RULES

        if (unitBuilt < 2)
        {
            category = BuildingCategory.Unit;
            return GetWeightedUnitBuilding();
        }

        if (wallsBuilt > 5 && nonWallDefenseBuilt < 2)
        {
            category = BuildingCategory.Defense;
            return GetNonWallDefenseBuilding();
        }

// OUTER RATIO CONTROL (5:5)

        int totalCombat = unitBuilt + defenseBuilt;
        int totalRes = resourceBuilt;

        int totalAll = totalCombat + totalRes + 1;

// target 50% resources
        float targetRes = totalAll * 0.5f;
        float targetCombat = totalAll * 0.5f;

        float resDeficit = targetRes - totalRes;
        float combatDeficit = targetCombat - totalCombat;

// If too many resources → force combat
        if (totalRes > totalCombat + 1)
        {
            category = (Random.value < 0.6f) ? BuildingCategory.Unit : BuildingCategory.Defense;
            return category == BuildingCategory.Unit ? GetWeightedUnitBuilding() : GetWeightedDefenseBuilding();
        }

// If too many combat → allow resource
        if (totalCombat > totalRes + 2)
        {
            category = BuildingCategory.Resource;
            return GetWeightedResourceBuilding();
        }

        //DEFAULT SMART AI
        float unit = currentPersonality.unitBuildingWeight;
        float resource = currentPersonality.resourceBuildingWeight;
        float defense = currentPersonality.defenseBuildingWeight;

        float total = unit + resource + defense;

        unit /= total;
        resource /= total;
        defense /= total;

        int totalSoFar = unitBuilt + resourceBuilt + defenseBuilt + 1;

        float unitDeficit = (totalSoFar * unit) - unitBuilt;
        float resourceDeficit = (totalSoFar * resource) - resourceBuilt;
        float defenseDeficit = (totalSoFar * defense) - defenseBuilt;

        var enemyRM = GetComponent<EnemyResourceManager>();

        if (enemyRM != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (enemyRM.currentResources[i].resourceAmount < minResourcePercent)
                {
                    category = BuildingCategory.Resource;
                    return GetWeightedResourceBuilding();
                }
            }
        }

        if (unitDeficit >= resourceDeficit && unitDeficit >= defenseDeficit)
        {
            category = BuildingCategory.Unit;
            return GetWeightedUnitBuilding();
        }

        if (resourceDeficit >= defenseDeficit)
        {
            category = BuildingCategory.Resource;
            return GetWeightedResourceBuilding();
        }

        category = BuildingCategory.Defense;
        return GetWeightedDefenseBuilding();
    }

    void EndEmergency()
    {
        emergencyWallTriggered = false;
        emergencyState = EmergencyState.None;

        lastEmergencyEndTime = Time.time; // THIS FIXES YOUR ERROR + LOGIC

        Debug.Log("[AI] Emergency COMPLETE");
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

    BuildingStats GetLowestResourceBuilding()
    {
        BuildingStats[] buildings = GetResourceBuildings();
        if (buildings == null || buildings.Length == 0)
            return null;

        int lowestIndex = 0;
        int lowestCount = resourceBuildingCounts[0];

        for (int i = 1; i < Mathf.Min(4, buildings.Length); i++)
        {
            if (resourceBuildingCounts[i] < lowestCount)
            {
                lowestCount = resourceBuildingCounts[i];
                lowestIndex = i;
            }
        }

        return buildings[lowestIndex];
    }

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
        if (buildings == null || buildings.Length == 0)
            return null;

        // =========================================================
        // HARD RESOURCE CORRECTION (TOP PRIORITY)
        // =========================================================
        if (resourceCorrectionActive)
        {
            var enemyRM = GetComponent<EnemyResourceManager>();
            if (enemyRM != null && enemyRM.currentResources.Length >= 4)
            {
                float[] res = new float[4];
                for (int i = 0; i < 4; i++)
                    res[i] = enemyRM.currentResources[i].resourceAmount;

                // Sort resources by amount (ascending)
                var sorted = res
                    .Select((value, index) => new { value, index })
                    .OrderBy(x => x.value)
                    .ToList();

                float diff01 = sorted[1].value - sorted[0].value;

                int index;

                // If one resource is critically behind → focus only that
                if (diff01 > 50f)
                {
                    index = sorted[0].index;
                }
                else
                {
                    // Otherwise balance between lowest two
                    index = (Random.value < 0.7f)
                        ? sorted[0].index
                        : sorted[1].index;
                }

                index = Mathf.Clamp(index, 0, buildings.Length - 1);
                return buildings[index];
            }
        }
        
        // ================= BALANCED START =================
        if (currentPersonality.balancedResourceStart && !balancedPhaseComplete)
        {
            int target = currentPersonality.balancedResourceTarget;

            int minCount = int.MaxValue;
            int targetIndex = 0;

            for (int i = 0; i < Mathf.Min(4, buildings.Length); i++)
            {
                if (resourceBuildingCounts[i] < minCount)
                {
                    minCount = resourceBuildingCounts[i];
                    targetIndex = i;
                }
            }

            // Check if balanced phase is complete
            balancedPhaseComplete = true;
            for (int i = 0; i < 4; i++)
            {
                if (resourceBuildingCounts[i] < target)
                {
                    balancedPhaseComplete = false;
                    break;
                }
            }

            return buildings[targetIndex];
        }

        
        // ================= NORMAL AI =================
        // Recheck resource needs every 10 seconds
        if (timeSinceLastAnalysis >= RECHECK_INTERVAL)
        {
            AnalyzeResourceNeeds();
            timeSinceLastAnalysis = 0f;
        }

        bool makeMistake = Random.value < currentPersonality.mistakeProbability;

        if (makeMistake)
        {
            Debug.Log("[EnemyAI] Resource selection mistake triggered!");
            return GetMistakeResourceBuilding(buildings);
        }

        // Use analyzed needs
        if (!allResourcesCovered && buildings.Length > 0)
        {
            BuildingStats building = buildings[resourceBuildingIndex];
            resourceBuildingIndex++;
            if (resourceBuildingIndex >= buildings.Length)
                allResourcesCovered = true;
            return building;
        }

        return GetWeightedRandom(buildings, resourceNeedPercentages);
    }

    BuildingStats GetMistakeResourceBuilding(BuildingStats[] buildings)
    {
        if (buildings.Length == 1)
            return buildings[0];

        // Find least needed resource and pick it (mistake)
        int leastNeededIndex = 0;
        float minNeed = resourceNeedPercentages[0];
        for (int i = 1; i < Mathf.Min(buildings.Length, resourceNeedPercentages.Length); i++)
        {
            if (resourceNeedPercentages[i] < minNeed)
            {
                minNeed = resourceNeedPercentages[i];
                leastNeededIndex = i;
            }
        }

        return buildings[leastNeededIndex];
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

    #region Resource Need Analysis

    void AnalyzeResourceNeeds()
    {
        float[] totalCosts = new float[4];
        var deck = GetCurrentFactionDeck();
        if (deck == null) return;

        // Analyze offense buildings
        foreach (var entry in deck.unitSelections)
        {
            if (!entry.selected || entry.amount <= 0) continue;
            var building = CharacterDatabase.Instance.GetSpawnerBuilding(entry.unit);
            if (building != null && building.buildingStatsSO != null)
            {
                var costs = building.buildingStatsSO.buildingBuildCost;
                for (int i = 0; i < Mathf.Min(costs.Length, 4); i++)
                    totalCosts[i] += costs[i].resourceAmount * entry.weight;
            }
        }

        // Analyze defense buildings
        foreach (var entry in deck.defenseBuildingSelections)
        {
            if (!entry.selected || entry.amount <= 0) continue;
            var building = CharacterDatabase.Instance.GetDefenseBuildingPrefab(entry.building);
            if (building != null && building.buildingStatsSO != null)
            {
                var costs = building.buildingStatsSO.buildingBuildCost;
                for (int i = 0; i < Mathf.Min(costs.Length, 4); i++)
                    totalCosts[i] += costs[i].resourceAmount * entry.weight;
            }
        }

        // Factor in current available resources (lacking resources get higher priority)
        var enemyRM = GetComponent<EnemyResourceManager>();
        if (enemyRM != null && enemyRM.currentResources != null && enemyRM.currentResources.Length >= 4)
        {
            float[] currentAmounts = new float[4];
            for (int i = 0; i < 4; i++)
                currentAmounts[i] = enemyRM.currentResources[i].resourceAmount;

            float totalAvailable = currentAmounts[0] + currentAmounts[1] + currentAmounts[2] + currentAmounts[3];
            if (totalAvailable > 0f)
            {
                for (int i = 0; i < 4; i++)
                {
                    float lackFactor = 1f - (currentAmounts[i] / totalAvailable);
                    totalCosts[i] *= (1f + lackFactor);
                }
            }
        }

        // Convert to percentages
        float sum = totalCosts[0] + totalCosts[1] + totalCosts[2] + totalCosts[3];
        if (sum > 0f)
        {
            for (int i = 0; i < 4; i++)
                resourceNeedPercentages[i] = (totalCosts[i] / sum) * 100f;
        }
        else
        {
            for (int i = 0; i < 4; i++)
                resourceNeedPercentages[i] = 25f;
        }

        // APPLY TARGET RATIO BIAS (2:3:4:2)

        float[] targetRatio = new float[] { 2f, 3f, 4f, 2f };
        float totalTarget = 11f; // sum

        for (int i = 0; i < 4; i++)
        {
            float targetPercent = (targetRatio[i] / totalTarget) * 100f;

            // Blend current analysis with target ratio (soft influence)
            resourceNeedPercentages[i] =
                (resourceNeedPercentages[i] * 0.7f) +
                (targetPercent * 0.3f);
        }

        // Debug.Log($"[EnemyAI] Resource Needs: Gold={resourceNeedPercentages[0]:F1}%, Wood={resourceNeedPercentages[1]:F1}%, Stone={resourceNeedPercentages[2]:F1}%, Food={resourceNeedPercentages[3]:F1}%");
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
        if (allBuildingData == null)
        {
            Debug.Log("<color=red>[EnemyAIHandler] allBuildingData is null");
            return null;
        }

        var deck = allBuildingData.GetFactionResourceBuildingsSO(enemyFactionName);

        var buildings = new BuildingStats[deck.Count];

        for (int i = 0; i < deck.Count; i++)
        {
            buildings[i] = CharacterDatabase.Instance.GetResourceBuildingPrefab(deck[i]);
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
        spawnableTiles.RemoveAll(t => t == null || t.hasBuilding);

        if (spawnableTiles.Count == 0)
            return null;

        return spawnableTiles[
            Random.Range(0, spawnableTiles.Count)];
    }

    Tile SelectBestTileForCategory(
        BuildingCategory category, BuildingStats buildingPrefab = null)
    {
        if (spawnableTiles.Count == 0)
            return null;

        Vector3 enemyPos =
            enemyMainBuildingTransform.position;

        Vector3 playerPos =
            playerMainBuildingTransform.position;

        Vector3 dirToPlayer =
            (playerPos - enemyPos).normalized;

        bool isWall = buildingPrefab != null && buildingPrefab is WallStats;

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

            if (isWall && currentPersonality.useSmartWallStrategy)
            {
                // Walls always go to frontline (middle area toward player)
                score = forwardness;
            }
            else
            {
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

    #region CombatStance

    void UpdateCombatStance()
    {
        if (Time.time - lastStanceChangeTime < currentPersonality.stanceCooldown)
            return;

        int myUnits = BattleUnitRegistry.EnemyUnits.Count;
        int enemyUnits = Mathf.Max(1, BattleUnitRegistry.PlayerUnits.Count);

        float ratio = (float)myUnits / enemyUnits;

        UnitStance newStance = UnitStance.Attacking;

        // ️ DEFAULT: ALWAYS ATTACK (90% CASE)
        bool shouldDefend =
            myUnits < 3 || // too weak
            ratio < 0.3f; // heavily outnumbered

        //  ONLY EMERGENCY DEFENSE (10% CASE)
        if (shouldDefend)
        {
            newStance = UnitStance.Defending;
        }
        else
        {
            newStance = UnitStance.Attacking;
        }

        // OPTIONAL: if player has no units → always attack
        if (BattleUnitRegistry.PlayerUnits.Count == 0)
            newStance = UnitStance.Attacking;

        // STATE UPDATE
        if (newStance != CurrentEnemyStance)
        {
            CurrentEnemyStance = newStance;
            lastStanceChangeTime = Time.time;

            if (StanceImg != null)
            {
                StanceImg.sprite =
                    (CurrentEnemyStance == UnitStance.Attacking)
                        ? AttackSprite
                        : DefendSprite;
            }

            Debug.Log($"[EnemyAI] Stance → {CurrentEnemyStance} (Ratio: {ratio:F2})");
        }
    }

    #endregion

    // ============== SPAWNING METHOD ==============

    void SpawnBuilding(Tile tile, BuildingStats buildingPrefab, BuildingCategory category)
    {
        if (tile == null || buildingPrefab == null || tile.hasBuilding)
            return;

        Vector3 spawnPos = tile.transform.position + Vector3.up * 2f;

        if (enemyBuildPanel.PlaceBuildingAI(buildingPrefab, spawnPos, tile))
        {
            failedBuildAttempts = 0; //  reset on success

            spawnableTiles.Remove(tile);
            Vector2Int tileGrid = CubeGridManager.Instance.WorldToGrid(tile.transform.position);
            UpdateSpawnableTiles(tileGrid);
            totalBuildingsSpawned++;

            switch (category)
            {
                case BuildingCategory.Unit:
                    unitBuilt++;
                    break;

                case BuildingCategory.Resource:
                    resourceBuilt++;

                    var resourceBuildings = GetResourceBuildings();
                    for (int i = 0; i < resourceBuildings.Length; i++)
                    {
                        if (resourceBuildings[i] == buildingPrefab)
                        {
                            resourceBuildingCounts[i]++;
                            break;
                        }
                    }

                    break;

                case BuildingCategory.Defense:
                    defenseBuilt++;

                    var defenseSO = buildingPrefab.buildingStatsSO as DefenseBuildingDataSO;

                    if (defenseSO != null && defenseSO.defenseType == ScenarioDefenseType.Wall)
                        wallsBuilt++;
                    else
                        nonWallDefenseBuilt++;

                    break;
            }
        }
        else
        {
            failedBuildAttempts++; //  track failure
            lastFailedCategory = category;
        }
    }

    //resource Imbalance fixing 
    void CheckResourceImbalance()
    {
        var enemyRM = GetComponent<EnemyResourceManager>();
        if (enemyRM == null || enemyRM.currentResources.Length < 4)
            return;

        float[] res = new float[4];
        for (int i = 0; i < 4; i++)
            res[i] = enemyRM.currentResources[i].resourceAmount;

        float max = res.Max();
        float min = res.Min();

        // Trigger correction
        if (!resourceCorrectionActive && (max - min) >= 100f)
        {
            resourceCorrectionActive = true;
            resourceCorrectionEndTime = Time.time + correctionDuration;

            // Find lowest 2 resources
            var indexed = res
                .Select((value, index) => new { value, index })
                .OrderBy(x => x.value)
                .ToList();

            forcedResourceIndexA = indexed[0].index;
            forcedResourceIndexB = indexed[1].index;

            Debug.Log($"[AI] Resource CORRECTION START → Low: {forcedResourceIndexA}, {forcedResourceIndexB}");
        }

        // Stop condition
        if (resourceCorrectionActive)
        {
            if ((max - min) < 100f || Time.time > resourceCorrectionEndTime)
            {
                resourceCorrectionActive = false;
                forcedResourceIndexA = -1;
                forcedResourceIndexB = -1;

                Debug.Log("[AI] Resource CORRECTION END");
            }
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