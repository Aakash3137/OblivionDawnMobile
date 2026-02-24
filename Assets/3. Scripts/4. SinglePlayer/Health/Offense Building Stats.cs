using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class OffenseBuildingStats : BuildingStats
{
    public ScenarioOffenseType offenseType { get; private set; }
    public OffenseBuildingUpgradeData offenseBuildingData { get; private set; }
    private CubeGridManager cgmInstance;
    // private CharacterDatabase characterDatabase => CharacterDatabase.Instance;

    [ReadOnly]
    public List<UnitStats> producedUnits = new List<UnitStats>();
    [SerializeField, ReadOnly] private UnitStats unit;
    private Vector2Int currentGrid;

    private Tile nearestTile;
    private WaitForSeconds waitTime;
    private bool canMaintain => CanMaintain();
    [field: SerializeField, ReadOnly] public bool isProducing { get; private set; }
    private int maxSpawnableUnits;

    internal override void Initialize()
    {
        identity = buildingStatsSO.buildingIdentity;

        cgmInstance = CubeGridManager.Instance;

        if (buildingStatsSO is OffenseBuildingDataSO offenseBuildingSO)
        {
            offenseType = offenseBuildingSO.offenseType;
            offenseBuildingData = offenseBuildingSO.offenseBuildingUpgradeData[identity.spawnLevel];

            buildCost = offenseBuildingSO.buildingBuildCost;

            waitTime = new WaitForSeconds(offenseBuildingData.unitSpawnTime);

            unit = offenseBuildingSO.unitPrefab;

            basicStats = offenseBuildingData.buildingBasicStats;

            buildTime = offenseBuildingData.buildingBuildTime;
            buildingWaitTime = new WaitForSeconds(buildTime);

            maxSpawnableUnits = offenseBuildingData.maxSpawnableUnits;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing OffenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();

        currentGrid = CubeGridManager.Instance.WorldToGrid(currentTile.transform.position);

        StartProduction();
    }

    private void StartProduction()
    {
        StartCoroutine(ProduceUnits());
    }

    private IEnumerator ProduceUnits()
    {
        isProducing = false;
        yield return new WaitForSeconds(buildTime);

        while (currentHealth > 0)
        {
            if (canMaintain && maxSpawnableUnits > producedUnits.Count)
            {
                isProducing = true;
            }
            else
            {
                isProducing = false;
                if (functionalityUI != null)
                    functionalityUI.ShowUI();
                yield return new WaitUntil(FulFillSpawnConditions);
                continue;
            }

            isProducing = true;
            if (functionalityUI != null)
                functionalityUI.HideUI();

            yield return waitTime;

            // if (canMaintain)
            SpawnUnit();
        }
    }

    private bool FulFillSpawnConditions()
    {
        return canMaintain && maxSpawnableUnits > producedUnits.Count;
    }

    private void SpawnUnit()
    {
        if (producedUnits.Count >= maxSpawnableUnits)
            return;

        if (TryGetSpawnPosition(out Vector3 spawnPoint))
        {
            var intUnit = Instantiate(unit, spawnPoint, Quaternion.identity, transform);
            intUnit.spawnerBuilding = this;
            intUnit.Initialize();
            producedUnits.Add(intUnit);
        }
        else
        {
            Debug.Log("<color=red>[OffenseBuildingStats] No spawn position found</color>");
        }
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        nearestTile = cgmInstance.GetNearestOpenTile(currentGrid, side, transform.position);

        if (nearestTile != null)
        {
            spawnPosition = nearestTile.transform.position + Vector3.up * 2f;
            return true;
        }

        spawnPosition = Vector3.zero;
        return false;
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

    public void SetUnitPrefab(UnitStats prefab)
    {
        unit = prefab;
    }
}
