using UnityEngine;
using System.Collections;

public class UnitSpawnerScenario : MonoBehaviour
{
    [Header("Production Settings")]
    public UnitProduceStatsSO unitProduceStats;   // ScriptableObject reference
    public Transform spawnPoint;     // empty child where units appear
    public bool autoProduce = true;  // keep producing continuously
    private SideScenario buildingSide;   // building's faction marker
    private Transform unitPool;      // found by tag "UnitPool"
    private UnitStats unitStats;
    public int unitSpawnLevel;
    // Reference to the faction Building
    private BuildingStats buildingStats;

    private void Start()
    {
        buildingSide = GetComponent<SideScenario>();

        if (buildingSide == null)
            Debug.LogError($"Building {name} missing UnitSide. Assign Player/Enemy.");

        buildingStats = GetComponent<BuildingStats>();

        if (buildingStats == null)
            Debug.LogError($"Building {name} missing BuildingStats. Assign the script.");

        unitStats = unitProduceStats.unitPrefab.GetComponent<UnitStats>();
        unitSpawnLevel = buildingStats.Level;

        unitStats.unitBuildTime = unitProduceStats.unitLevelData[unitSpawnLevel].unitBuildTime;
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
            yield return new WaitForSeconds(unitStats.unitBuildTime);
            SpawnUnit();
        }
    }

    private void SpawnUnit()
    {
        if (unitProduceStats == null || spawnPoint == null) return;

        GameObject unit = Instantiate(unitProduceStats.unitPrefab, spawnPoint.position, Quaternion.identity);

        if (unitPool != null)
            unit.transform.SetParent(unitPool);

        // Assign side
        SideScenario unitSide = unit.GetComponent<SideScenario>();
        if (unitSide != null && buildingSide != null)
        {
            unitSide.side = buildingSide.side;
            SideManager manager = FindAnyObjectByType<SideManager>();
            if (manager != null) manager.SetSide(unit, unitSide.side);
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
        unitStats.unitBuildTime = unitProduceStats.unitLevelData[unitSpawnLevel].unitBuildTime;
        unitStats.maxHealth = unitProduceStats.unitLevelData[unitSpawnLevel].unitHealth;
        unitStats.armour = unitProduceStats.unitLevelData[unitSpawnLevel].unitArmour;
        unitStats.unitAttackDamage = unitProduceStats.unitLevelData[unitSpawnLevel].unitAttackDamage;
        unitStats.unitAttackRange = unitProduceStats.unitLevelData[unitSpawnLevel].unitAttackRange;
        unitStats.unitFireRate = unitProduceStats.unitLevelData[unitSpawnLevel].unitFireRate;

        Debug.Log($"{buildingSide?.side} building upgraded to level {unitSpawnLevel}");
    }
}