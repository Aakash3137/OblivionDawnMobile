using UnityEngine;
using System.Collections;

public class UnitSpawnerScenario : MonoBehaviour
{
    [Header("Production Settings")]
    public UnitProduceStatsSO unitProduceStats;   // ScriptableObject reference
    public Vector3 spawnPoint;     // empty child where units appear
    public bool autoProduce = true;  // keep producing continuously

    [SerializeField] private Material playerUnitMaterial;
    [SerializeField] private Material enemyUnitMaterial;

    [Header(" EDITOR VIEW ONLY ")]
    [SerializeField] private int unitSpawnLevel;
    public float unitBuildTime;
    private SideScenario buildingSide;   // building's faction marker
    private Tile currentTile;
    private Tile nearestTile;
    private Transform unitPool;      // found by tag "UnitPool"
    private UnitStats unitStats;
    private BuildingStats buildingStats;    // Reference to the faction Building    

    private void Start()
    {
        buildingSide = GetComponent<SideScenario>();
        currentTile = GetComponentInParent<Tile>();

        if (buildingSide == null)
            Debug.LogError($"Building {name} missing UnitSide. Assign Player/Enemy.");

        buildingStats = GetComponent<BuildingStats>();

        if (buildingStats == null)
            Debug.LogError($"Building {name} missing BuildingStats. Assign the script.");

        unitStats = unitProduceStats.unitPrefab.GetComponent<UnitStats>();
        unitSpawnLevel = buildingStats.Level;

        unitBuildTime = unitProduceStats.unitLevelData[unitSpawnLevel].unitBuildTime;
        unitStats.maxHealth = unitProduceStats.unitLevelData[unitSpawnLevel].unitHealth;
        unitStats.armour = unitProduceStats.unitLevelData[unitSpawnLevel].unitArmour;
        unitStats.unitAttackDamage = unitProduceStats.unitLevelData[unitSpawnLevel].unitAttackDamage;
        unitStats.unitAttackRange = unitProduceStats.unitLevelData[unitSpawnLevel].unitAttackRange;
        unitStats.unitFireRate = unitProduceStats.unitLevelData[unitSpawnLevel].unitFireRate;

        // Find UnitPool by tag
        GameObject poolObj = GameObject.FindGameObjectWithTag("UnitPool");
        if (poolObj != null)
            unitPool = poolObj.transform;
        else
            Debug.LogError("No GameObject with tag 'UnitPool' found in scene!");

        if (unitProduceStats != null)
            StartCoroutine(ProductionLoop());
    }

    private IEnumerator ProductionLoop()
    {
        while (autoProduce && buildingStats.currentHealth > 0)
        {
            yield return new WaitForSeconds(unitBuildTime);
            SpawnUnit();
        }
    }

    private void SpawnUnit()
    {
        if (unitProduceStats == null) return;

        nearestTile = CubeGridManager.Instance.GetNearestOpenTile(currentTile);

        if (nearestTile == null)
        {
            Debug.LogWarning($"{name} has no open tile to spawn units on!");
            return;
        }

        spawnPoint = nearestTile.transform.position + Vector3.up * 2f;


        GameObject unit = Instantiate(unitProduceStats.unitPrefab, spawnPoint, Quaternion.identity);

        if (unitPool != null)
            unit.transform.SetParent(unitPool);

        // Assign side
        SideScenario unitSide = unit.GetComponent<SideScenario>();
        if (unitSide != null && buildingSide != null)
        {
            unitSide.side = buildingSide.side;
            //SideManager manager = FindAnyObjectByType<SideManager>();
            //if (manager != null) manager.SetSide(unit, unitSide.side);

            Renderer renderer = unit.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                switch (unitSide.side)
                {
                    case Side.Player:
                        renderer.material = playerUnitMaterial;
                        break;
                    case Side.Enemy:
                        renderer.material = enemyUnitMaterial;
                        break;
                }
            }
        }

        // Assign stats
        UnitStats stats = unit.GetComponent<UnitStats>();
        if (stats != null)
        {
            stats.maxHealth = unitStats.maxHealth;
            stats.currentHealth = stats.maxHealth;
            stats.armour = unitStats.armour;
            stats.attackDamage = unitStats.unitAttackDamage;
        }

        // Assign AI combat properties
        AIUnit ai = unit.GetComponent<AIUnit>();
        if (ai != null)
        {
            ai.attackDamage = unitStats.unitAttackDamage;
            ai.attackRange = unitStats.unitAttackRange;
            ai.attackInterval = unitStats.unitFireRate;
        }

        //// Debug.Log($"{buildingSide?.side} building produced {unitProduceStats.unitName} inside UnitPool");
    }

    public void UpgradeProduction()
    {
        unitSpawnLevel++;
        unitBuildTime = unitProduceStats.unitLevelData[unitSpawnLevel].unitBuildTime;
        unitStats.maxHealth = unitProduceStats.unitLevelData[unitSpawnLevel].unitHealth;
        unitStats.armour = unitProduceStats.unitLevelData[unitSpawnLevel].unitArmour;
        unitStats.unitAttackDamage = unitProduceStats.unitLevelData[unitSpawnLevel].unitAttackDamage;
        unitStats.unitAttackRange = unitProduceStats.unitLevelData[unitSpawnLevel].unitAttackRange;
        unitStats.unitFireRate = unitProduceStats.unitLevelData[unitSpawnLevel].unitFireRate;

        Debug.Log($"{buildingSide?.side} building upgraded to level {unitSpawnLevel}");
    }
}