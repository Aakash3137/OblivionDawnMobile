using UnityEngine;
using System.Collections;

public class UnitSpawnerScenario : MonoBehaviour
{
    [Header("Production stats variable")]
    [field: SerializeField]
    public UnitProduceStatsSO unitProduceStats { get; private set; }
    private Vector3 spawnPoint;
    private PlayerResourceManager prmInstance;

    [Header("Building Faction and stats")]
    private BuildingStats buildingStats;
    public bool autoProduce;

    [Header("Tile/Cube variables")]
    private Tile currentTile;
    private Tile nearestTile;
    private CubeGridManager cgmInstance;
    private WaitForSeconds unitBuildTimeDelay;

    [Header("Unit Production variables")]
    public GameObject unitPrefab { get; private set; }
    public int unitSpawnLevel { get; private set; }
    public UnitUpgradeData currentUnitLevelData { get; private set; }
    public float unitBuildTime { get; private set; }
    // public BuildCost[] unitBuildCost { get; private set; }

    public GameObject producedUnit;

    private void Awake()
    {
        prmInstance = PlayerResourceManager.Instance;
        cgmInstance = CubeGridManager.Instance;

        currentTile = GetComponentInParent<Tile>();
        buildingStats = GetComponent<BuildingStats>();
    }

    private void Start()
    {
        unitPrefab = unitProduceStats.unitPrefab;
        unitSpawnLevel = unitProduceStats.unitIdentity.spawnLevel;
        // unitBuildCost = unitProduceStats.unitBuildCost;

        currentUnitLevelData = unitProduceStats.unitUpgradeData[unitSpawnLevel];
        unitBuildTime = currentUnitLevelData.unitBuildTime;

        unitBuildTimeDelay = new WaitForSeconds(unitBuildTime);

        if (unitProduceStats.unitIdentity.isUnique)
            autoProduce = false;

        StartProducingUnits();
    }

    private void StartProducingUnits()
    {
        producedUnit = null;

        if (unitProduceStats != null)
            StartCoroutine(ProductionLoop());
    }

    private IEnumerator ProductionLoop()
    {
        do
        {
            // if (HasResources())
            // {
            //     yield return unitBuildTimeDelay;
            //     SpawnUnit();
            //     //prmInstance.SpendResources(unitUpgradeCosts);
            // }
            yield return unitBuildTimeDelay;
            SpawnUnit();
        }
        while (autoProduce && buildingStats.currentHealth > 0);
    }

    private void SpawnUnit()
    {
        if (unitProduceStats == null) return;

        nearestTile = cgmInstance.GetNearestOpenTile(currentTile);

        if (nearestTile == null)
        {
            Debug.Log($"{name} has no open tile to spawn units on!");
            return;
        }

        spawnPoint = nearestTile.transform.position + Vector3.up * 2f;

        producedUnit = Instantiate(unitPrefab, spawnPoint, Quaternion.identity, transform);

        if (producedUnit != null && !autoProduce)
        {
            producedUnit.GetComponent<UnitStats>().onUniqueUnitDied += StartProducingUnits;
        }
    }

    // public bool HasResources()
    // {
    //     return prmInstance.HasResources(unitBuildCost);
    // }
    private void OnDisable()
    {
        if (producedUnit != null)
            producedUnit.GetComponent<UnitStats>().onUniqueUnitDied -= StartProducingUnits;
    }

    private void OnDestroy()
    {
        //KillCounterManager.Instance.AddOffenseBuildingDestroyedData(unitProduceStats.unitType, buildingStats.side);
    }
}