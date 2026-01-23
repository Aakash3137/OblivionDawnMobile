using UnityEngine;
using System.Collections;

public class SpawnerComponent : MonoBehaviour
{
    private Vector3 spawnPoint;
    [Header("Tile/Cube variables")]
    private Tile currentTile;
    private Tile nearestTile;
    private CubeGridManager cgmInstance;
    private WaitForSeconds unitBuildTimeDelay;
    public float unitBuildTime { get; private set; }

    public GameObject producedUnit;
    public bool autoProduce;
    private BuildingStats buildingStats;
    private PlayerResourceManager prmInstance;

    [Header("Unit Production variables")]
    public GameObject unitPrefab { get; private set; }
    public int unitSpawnLevel { get; private set; }
    public UnitUpgradeData currentUnitLevelData { get; private set; }

    // public BuildCost[] unitBuildCost { get; private set; }

    public void Initialize(UnitProduceStatsSO produceStats)
    {
        currentTile = GetComponentInParent<Tile>();
        buildingStats = GetComponent<BuildingStats>();

        prmInstance = PlayerResourceManager.Instance;
        cgmInstance = CubeGridManager.Instance;

        unitPrefab = produceStats.unitPrefab;
        unitSpawnLevel = produceStats.unitIdentity.spawnLevel;
        // unitBuildCost = produceStats.unitBuildCost;

        currentUnitLevelData = produceStats.unitUpgradeData[unitSpawnLevel];
        unitBuildTime = currentUnitLevelData.unitBuildTime;

        unitBuildTimeDelay = new WaitForSeconds(unitBuildTime);

        if (produceStats.unitIdentity.isUnique)
            autoProduce = false;

        if (produceStats != null)
            StartProducingUnits();
    }

    private void StartProducingUnits()
    {
        producedUnit = null;
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
        nearestTile = cgmInstance.GetNearestOpenTile(currentTile);

        if (nearestTile == null)
        {
            Debug.Log($"{name} has no open tile to spawn units on!");
            return;
        }

        spawnPoint = nearestTile.transform.position + Vector3.up * 2f;

        producedUnit = Instantiate(unitPrefab, spawnPoint, Quaternion.identity, transform);

        if (producedUnit != null)
        {
            producedUnit.GetComponent<BasicStatsComponent>().onUniqueUnitDied += StartProducingUnits;
        }
    }

    // public bool HasResources()
    // {
    //     return prmInstance.HasResources(unitBuildCost);
    // }

    private void OnDisable()
    {
        if (producedUnit != null)
            producedUnit.GetComponent<BasicStatsComponent>().onUniqueUnitDied -= StartProducingUnits;


    }

}