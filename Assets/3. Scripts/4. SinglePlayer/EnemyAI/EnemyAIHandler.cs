using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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

    [Header("References")]
    [SerializeField]
    private EnemyModeSwitch enemyModeSwitch;

    [SerializeField] private EnemyBuildPanel enemyBuildPanel;
    [SerializeField] internal FactionName enemyFactionName;
    [SerializeField] private EnemyPersonality currentPersonality;

    [SerializeField] private List<EnemyPersonality> AIPersonalities;
    [SerializeField] private AllBuildingData allBuildingData;

    [SerializeField] private float minResourcePercent = 15f; // never drop below 15% of each resource
    [SerializeField] private float emergencyResourceThreshold = 30f; // emergency if below 30
    [SerializeField] private float emergencyGenRateThreshold = 5f; // emergency if gen rate below +5

    private Transform enemyMainBuildingTransform;
    private Transform playerMainBuildingTransform;
    private List<Tile> spawnableTiles = new List<Tile>();

    private int totalBuildingsSpawned = 0;
    private float spawnTimer = 0f;

    private float currentSpawnInterval;

    private int resourceBuildingIndex = 0;
    private bool allResourcesCovered = false;
    [SerializeField]
    private BuildingStats[] resourceTypeToBuilding = new BuildingStats[4];

    [SerializeField, ReadOnly] private int unitBuilt = 0;
    [SerializeField, ReadOnly] private int resourceBuilt = 0;
    [SerializeField, ReadOnly] private int defenseBuilt = 0;
    [SerializeField, ReadOnly] private int wallsBuilt = 0;
    [SerializeField, ReadOnly] private int nonWallDefenseBuilt = 0;

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
    private int initialResourceAmount = 0;
    private bool skipInitialWalls = false;

    // Enemy Stance variable
    public UnitStance CurrentEnemyStance { get; private set; } = UnitStance.Attacking;
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

    private float nextRecheckTime = -999f;

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

        // Check initial resources and set economy targets
        var rm = GetComponent<EnemyResourceManager>();
        if (rm != null && rm.currentResources.Length >= 4)
        {
            float avgResource = 0f;
            for (int i = 0; i < 4; i++)
                avgResource += rm.currentResources[i].resourceAmount;
            avgResource /= 4f;
            initialResourceAmount = Mathf.RoundToInt(avgResource);

            if (initialResourceAmount <= 150)
            {
                earlyResourceTarget = 1;
                skipInitialWalls = true;
            }
            else if (initialResourceAmount <= 250)
            {
                earlyResourceTarget = 2;
            }
            else if (initialResourceAmount <= 350)
            {
                earlyResourceTarget = 3;
            }
            else
            {
                earlyResourceTarget = 2;
            }

            Debug.Log($"[EnemyAI] Initial resources: {initialResourceAmount}, Early target: {earlyResourceTarget}, Skip walls: {skipInitialWalls}");
        }

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

        //UpdateCombatStance();
        //CheckResourceImbalance();

        /*if (!emergencyUnlocked)
        {
            if (KillCounterManager.Instance != null &&
                KillCounterManager.Instance.enemyTotalUnitKills > 5)
            {
                //emergencyUnlocked = true;
                Debug.Log("[EnemyAI][Wall] Emergency system UNLOCKED (mid game reached)");
            }
        }*/
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
                // Debug.Log($"[EnemyAI][Wall] Found wall in deck: {entry.building.name}");
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

        bool playerHasAirUnits = DoesPlayerHaveAirUnits();

        List<BuildingStats> options = new List<BuildingStats>();
        List<float> weights = new List<float>();

        foreach (var entry in deck.defenseBuildingSelections)
        {
            if (!entry.selected || entry.amount <= 0) continue;

            var building = CharacterDatabase.Instance.GetDefenseBuildingPrefab(entry.building);
            if (building == null) continue;

            // Skip anti-air if player has no air units
            if (entry.building != null && entry.building.defenseType == ScenarioDefenseType.AntiAir)
            {
                if (!playerHasAirUnits)
                {
                    Debug.Log($"[AI] Skipping anti-air building {entry.building.name} - player has no air units");
                    continue;
                }
            }

            options.Add(building);
            weights.Add(entry.weight);
        }

        return GetWeightedRandom(options.ToArray(), weights.ToArray());
    }

    bool DoesPlayerHaveAirUnits()
    {
        if (GameplayRegistry.GetOffense(Side.Player, ScenarioOffenseType.AirBuilding).Count > 0)
            return true;

        return false;
    }

    #endregion

    void TrySpawnBuilding()
    {
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
            failedBuildAttempts++;
            if (failedBuildAttempts >= MAX_FAIL_BEFORE_RESOURCE)
            {
                failedBuildAttempts = 0;
                spawnTimer = currentSpawnInterval * 0.5f; // retry sooner
            }
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
            failedBuildAttempts++;
            if (failedBuildAttempts >= MAX_FAIL_BEFORE_RESOURCE)
            {
                failedBuildAttempts = 0;
                spawnTimer = currentSpawnInterval * 0.5f; // retry sooner
            }
            return;
        }

        SpawnBuilding(selectedTile, buildingPrefab, category);
    }

    BuildingStats DecideBuildingUsingPersonality(out BuildingCategory category)
    {
        // =========================
        // STEP 0: EMERGENCY RESOURCE CHECK
        // =========================
        var rm = GetComponent<EnemyResourceManager>();
        if (rm != null && rm.currentResources.Length >= 4 && rm.currentGenerationRates.Length >= 4)
        {
            int emergencyIndex = -1;
            float lowestGenRate = float.MaxValue;

            for (int i = 0; i < 4; i++)
            {
                float amount = rm.currentResources[i].resourceAmount;
                float genRate = rm.currentGenerationRates[i].resourceAmount;

                if (amount < emergencyResourceThreshold && (genRate < 0 || genRate < emergencyGenRateThreshold))
                {
                    if (genRate < lowestGenRate)
                    {
                        lowestGenRate = genRate;
                        emergencyIndex = i;
                    }
                }
            }

            if (emergencyIndex != -1)
            {
                Debug.Log($"[AI] EMERGENCY: Resource {emergencyIndex} below 30 with gen rate {lowestGenRate:F2}");
                category = BuildingCategory.Resource;
                var buildings = GetResourceBuildings();
                if (buildings != null && emergencyIndex < buildings.Length)
                    return buildings[emergencyIndex];
            }
        }

        // =========================
        // STEP 1: EARLY ECONOMY
        // =========================
        if (!earlyEconomyComplete)
        {
            if (!IsEarlyEconomyComplete())
            {
                category = BuildingCategory.Resource;
                return GetLowestResourceBuilding();
            }
            else
            {
                earlyEconomyComplete = true;
            }
        }

        // =========================
        // STEP 2: INITIAL WALL PHASE
        // =========================
        bool canUseWalls = currentPersonality.useSmartWallStrategy && HasWallInDeck() && !skipInitialWalls;

        if (canUseWalls && !initialWallPhaseDone)
        {
            if (wallsBuilt < initialWallsTarget)
            {
                category = BuildingCategory.Defense;
                return GetWallBuilding();
            }
            else
            {
                initialWallPhaseDone = true;
            }
        }

        // =========================
        // STEP 3: PERSONALITY PHASE WITH WEIGHTAGE
        // =========================
        if (rm != null && rm.currentResources.Length >= 4)
        {
            bool allAbove30 = true;
            for (int i = 0; i < 4; i++)
            {
                if (rm.currentResources[i].resourceAmount < emergencyResourceThreshold)
                {
                    allAbove30 = false;
                    break;
                }
            }

            if (allAbove30)
            {
                // Use personality weights
                float unitWeight = currentPersonality.unitBuildingWeight;
                float defenseWeight = currentPersonality.defenseBuildingWeight;

                float totalWeight = unitWeight + defenseWeight;
                if (totalWeight <= 0f)
                {
                    unitWeight = 0.5f;
                    defenseWeight = 0.5f;
                    totalWeight = 1f;
                }

                float unitRatio = unitWeight / totalWeight;
                float currentUnitRatio = (float)unitBuilt / Mathf.Max(1, unitBuilt + nonWallDefenseBuilt);

                if (currentUnitRatio < unitRatio)
                {
                    category = BuildingCategory.Unit;
                    return GetWeightedUnitBuilding();
                }
                else
                {
                    category = BuildingCategory.Defense;
                    return GetWeightedDefenseBuilding();
                }
            }
        }

        // Fallback to old logic
        switch (currentPersonality.playStyle)
        {
            case PlayStyle.Attack:
                category = BuildingCategory.Unit;
                return GetWeightedUnitBuilding();

            case PlayStyle.Defence:
                category = BuildingCategory.Defense;
                return GetWeightedDefenseBuilding();

            case PlayStyle.Mix:
                if (unitBuilt <= defenseBuilt)
                {
                    category = BuildingCategory.Unit;
                    return GetWeightedUnitBuilding();
                }
                else
                {
                    category = BuildingCategory.Defense;
                    return GetWeightedDefenseBuilding();
                }
        }

        // =========================
        // STEP 4: RESOURCE CHECK LOOP
        // =========================

        if (IsResourceLow())
        {
            Debug.Log("[AI] Resource low → building resource");
            category = BuildingCategory.Resource;
            return GetWeightedResourceBuilding();
        }

        // fallback
        category = BuildingCategory.Unit;
        return GetWeightedUnitBuilding();
    }

    bool IsResourceLow()
    {
        var rm = GetComponent<EnemyResourceManager>();
        if (rm == null) return false;

        for (int i = 0; i < 4; i++)
        {
            if (rm.currentResources[i].resourceAmount < minResourcePercent)
                return true;
        }

        return false;
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

        bool playerHasAirUnits = DoesPlayerHaveAirUnits();

        // Calculate wall percentage - walls should not exceed 50% of total defense buildings
        int totalDefense = wallsBuilt + nonWallDefenseBuilt;
        bool blockWalls = false;

        if (totalDefense > 0)
        {
            float wallPercentage = (float)wallsBuilt / totalDefense;
            if (wallPercentage >= 0.50f)
            {
                blockWalls = true;
                Debug.Log($"[AI] Wall percentage {wallPercentage:P0} >= 50%, blocking walls");
            }
        }

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

            // Check if it's a wall using DefenseBuildingDataSO
            bool isWall = entry.building != null && entry.building.defenseType == ScenarioDefenseType.Wall;

            // Block walls if they exceed 33%
            if (isWall && blockWalls)
            {
                Debug.Log($"[AI] Skipping wall {entry.building.name} - exceeds 33% limit");
                continue;
            }

            // Skip anti-air if player has no air units
            if (entry.building != null && entry.building.defenseType == ScenarioDefenseType.AntiAir)
            {
                if (!playerHasAirUnits)
                {
                    Debug.Log($"[AI] Skipping anti-air building {entry.building.name} - player has no air units");
                    continue;
                }
            }

            options.Add(building);
            weights.Add(entry.weight);
        }

        if (options.Count == 0)
        {
            Debug.LogWarning("[AI] No valid defense buildings available after filtering");
            return null;
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

                if (diff01 > 150f)
                    index = sorted[0].index;
                else
                    index = (Random.value < 0.7f)
                        ? sorted[0].index
                        : sorted[1].index;

                return resourceTypeToBuilding[index];
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

        var deck = allBuildingData.GetResourceBuildingsSO(enemyFactionName);

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

    Tile SelectBestTileForCategory(BuildingCategory category, BuildingStats buildingPrefab = null)
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
        if (!resourceCorrectionActive && (max - min) >= 200f)
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
            if ((max - min) < 200f || Time.time > resourceCorrectionEndTime)
            {
                resourceCorrectionActive = false;
                forcedResourceIndexA = -1;
                forcedResourceIndexB = -1;

                Debug.Log("[AI] Resource CORRECTION END");
            }
        }
    }

    float GetResourcePressure()
    {
        var enemyRM = GetComponent<EnemyResourceManager>();
        if (enemyRM == null) return 0f;

        float totalResources = 0f;
        for (int i = 0; i < 4; i++)
            totalResources += enemyRM.currentResources[i].resourceAmount;

        float totalNeed = 0f;
        for (int i = 0; i < 4; i++)
            totalNeed += resourceNeedPercentages[i];

        if (totalNeed <= 0f) return 0f;

        return totalResources / totalNeed;
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