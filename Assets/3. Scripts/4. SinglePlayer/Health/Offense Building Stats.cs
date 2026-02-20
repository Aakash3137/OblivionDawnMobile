using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class OffenseBuildingStats : BuildingStats
{
    public ScenarioOffenseType offenseType { get; private set; }
    public OffenseBuildingUpgradeData offenseBuildingData { get; private set; }
    private CubeGridManager cgmInstance;
    // private CharacterDatabase characterDatabase => CharacterDatabase.Instance;

    [ReadOnly]
    public UnitStats producedUnit;
    private UnitStats unit;
    private Vector2Int currentGrid;

    private Tile nearestTile;
    private WaitForSeconds waitTime;
    public bool isProducing { get; private set; }
    private BuildingSkeleton buildingSkeleton;

    internal override void Initialize()
    {
        identity = buildingStatsSO.buildingIdentity;

        isProducing = true;

        cgmInstance = CubeGridManager.Instance;

        if (buildingStatsSO is OffenseBuildingDataSO offenseBuildingSO)
        {
            offenseType = offenseBuildingSO.offenseType;
            offenseBuildingData = offenseBuildingSO.offenseBuildingUpgradeData[identity.spawnLevel];

            buildCost = offenseBuildingSO.buildingBuildCost;
            waitTime = new WaitForSeconds(offenseBuildingData.unitSpawnTime);

            basicStats = offenseBuildingData.buildingBasicStats;

            unit = offenseBuildingSO.unitPrefab;

            buildTime = offenseBuildingData.buildingBuildTime;

            if (unit != null && unit.unitProduceSO.unitIdentity.isUnique)
                isProducing = false;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing OffenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();

        currentGrid = CubeGridManager.Instance.WorldToGrid(currentTile.transform.position);

        // StartProducingUnits();
    }

    private void StartProducingUnits()
    {
        producedUnit = null;

        if (offenseBuildingData != null)
            StartCoroutine(ProductionLoop());
    }

    private void StartProductionLoop()
    {
        producedUnit = null;
        isProducing = true;
        if (offenseBuildingData != null)
            StartCoroutine(ProductionLoop());
    }
    private void StopProduction()
    {
        isProducing = false;
        StopAllCoroutines();
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
            yield return waitTime;
            SpawnUnit();
        }
        while (isProducing && currentHealth > 0);

        // Debug.Log("<color=magenta>Produced Unique unit from " + name + ".</color>");

        if (currentHealth <= 0)
            Debug.Log("<color=red>Health is 0 for " + name + ".</color>");
    }

    private void SpawnUnit()
    {
        // nearestTile = cgmInstance.GetNearestOpenTile(currentTile, currentGrid);

        nearestTile = cgmInstance.GetNearestOpenTile(currentGrid, side, transform.position);

        if (nearestTile == null)
        {
            Debug.Log($"{name} has no open tile to spawn units on!");
            return;
        }

        Vector3 spawnPoint = nearestTile.transform.position + Vector3.up * 2f;

        producedUnit = Instantiate(unit, spawnPoint, Quaternion.identity, transform);

        producedUnit.spawnerBuilding = this;

        producedUnit.Initialize();

        if (producedUnit != null && !isProducing)
        {
            producedUnit.GetComponent<UnitStats>().onUniqueUnitDied += StartProducingUnits;
        }
    }

    public float GetUnitSpawnTime()
    {
        return offenseBuildingData.unitSpawnTime;
    }

    internal override void Die()
    {
        base.Die();

        KillCounterManager.Instance.AddOffenseBuildingDestroyedData(offenseType, side);
    }
    private void OnEnable()
    {
        if (buildingSkeleton == null)
            buildingSkeleton = GetComponent<BuildingSkeleton>();

        buildingSkeleton.onBuildingBuilt += StartProductionLoop;
    }

    private void OnDisable()
    {
        if (producedUnit != null)
            producedUnit.GetComponent<UnitStats>().onUniqueUnitDied -= StartProducingUnits;

        if (buildingSkeleton != null)
            buildingSkeleton.onBuildingBuilt -= StartProductionLoop;
    }

    public void SetUnitPrefab(UnitStats prefab)
    {
        unit = prefab;
    }
}
