using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class OffenseBuildingStats : BuildingStats
{
    public ScenarioOffenseType offenseType { get; private set; }
    public OffenseBuildingUpgradeData offenseBuildingData { get; private set; }
    private CubeGridManager cgmInstance;

    [ReadOnly]
    public UnitStats producedUnit;
    private UnitStats unit;

    private Tile nearestTile;
    private WaitForSeconds waitTime;
    public bool autoProduce { get; private set; }

    internal override void Start()
    {
        identity = buildingStats.buildingIdentity;

        autoProduce = true;

        cgmInstance = CubeGridManager.Instance;

        if (buildingStats is OffenseBuildingDataSO offenseBuildingSO)
        {
            offenseType = offenseBuildingSO.offenseType;
            offenseBuildingData = offenseBuildingSO.offenseBuildingUpgradeData[identity.spawnLevel];

            buildCost = offenseBuildingSO.buildingBuildCost;
            waitTime = new WaitForSeconds(offenseBuildingData.unitSpawnTime);

            basicStats = offenseBuildingData.buildingBasicStats;

            unit = offenseBuildingSO.unitPrefab;

            if (unit != null && unit.unitProduceSO.unitIdentity.isUnique)
                autoProduce = false;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing OffenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();

        StartProducingUnits();
    }

    private void StartProducingUnits()
    {
        producedUnit = null;

        if (offenseBuildingData != null)
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
            yield return waitTime;
            SpawnUnit();
        }
        while (autoProduce && currentHealth > 0);
    }

    private void SpawnUnit()
    {
        nearestTile = cgmInstance.GetNearestOpenTile(currentTile);

        if (nearestTile == null)
        {
            Debug.Log($"{name} has no open tile to spawn units on!");
            return;
        }

        Vector3 spawnPoint = nearestTile.transform.position + Vector3.up * 2f;

        producedUnit = Instantiate(unit, spawnPoint, Quaternion.identity, transform);

        producedUnit.spawnerBuilding = this;

        if (producedUnit != null && !autoProduce)
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

    private void OnDisable()
    {
        if (producedUnit != null)
            producedUnit.GetComponent<UnitStats>().onUniqueUnitDied -= StartProducingUnits;
    }
}
