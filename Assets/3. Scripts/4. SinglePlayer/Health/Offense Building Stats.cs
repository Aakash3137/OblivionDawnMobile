using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class OffenseBuildingStats : BuildingStats
{
    public ScenarioOffenseType offenseType { get; private set; }
    public OffenseBuildingUpgradeData offenseBuildingData { get; private set; }
    public override TileEffectType compatibleTileEffectType => TileEffectType.OffenseTile;
    private CubeGridManager cgmInstance;

    // private CharacterDatabase characterDatabase => CharacterDatabase.Instance;

    [ReadOnly]
    public List<UnitStats> producedUnits = new List<UnitStats>();
    [SerializeField, ReadOnly] private UnitStats unit;
    [SerializeField, ReadOnly] private float unitSpawnTime;
    private Vector2Int currentGrid;

    private Tile nearestTile;
    private bool canMaintain => CanMaintain();
    [field: SerializeField, ReadOnly] public bool isProducingUnits { get; private set; }
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

            if (side != Side.Player)
            {
                unit = offenseBuildingSO.unitPrefab;
                unitSpawnTime = unit.unitProduceSO.unitUpgradeData[0].unitSpawnTime;
            }

            basicStats = offenseBuildingData.buildingBasicStats;

            buildTime = offenseBuildingData.buildingBuildTime;

            maxSpawnableUnits = offenseBuildingData.maxSpawnableUnits;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing OffenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();

        currentGrid = CubeGridManager.Instance.WorldToGrid(currentTile.transform.position);
    }

    internal override async Awaitable InitializeOnBuilt()
    {
        isProducingUnits = false;
        await base.InitializeOnBuilt();
        _ = ProduceUnits();
    }

    private async Awaitable ProduceUnits()
    {
        while (currentHealth > 0)
        {
            if (!FulFillSpawnConditions())
            {
                isProducingUnits = false;

                // if(maxSpawnableUnits > producedUnits.Count)
                //  TO DO SHOW ICON FOR BUILDING FULL

                while (!FulFillSpawnConditions())
                    await Awaitable.WaitForSecondsAsync(0.5f, destroyCancellationToken);    // wait for 0.5 seconds till spawn conditions are met

                continue;
            }

            isProducingUnits = true;
            await Awaitable.WaitForSecondsAsync(unitSpawnTime, destroyCancellationToken);
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

    internal override void EnableFunctionality()
    {
        if (isProducingUnits && functionalityUI != null)
            functionalityUI.HideUI();
    }

    internal override void DisableFunctionality()
    {
        if (!isProducingUnits && functionalityUI != null)
            functionalityUI.ShowUI();
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
        return unitSpawnTime;
    }

    internal override void Die()
    {
        base.Die();

        KillCounterManager.Instance.AddOffenseBuildingDestroyedData(offenseType, side);
    }

    public void SetUnitPrefab(UnitStats prefab, float spawnTime)
    {
        unit = prefab;
        unitSpawnTime = spawnTime;
    }

    public void BuffUnitProduction(float buffStrength)
    {
        Debug.Log($"Applying Unit Production Buff currentSpawnTime: {unitSpawnTime}");
        unitSpawnTime = unitSpawnTime / buffStrength;
        Debug.Log($"Applied Unit Production Buff currentSpawnTime: {unitSpawnTime}");
    }
}