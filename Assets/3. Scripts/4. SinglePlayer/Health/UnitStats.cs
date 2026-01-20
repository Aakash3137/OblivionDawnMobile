using System;
using UnityEngine;

public class UnitStats : Stats
{
    [field: SerializeField]
    public UnitSpawnerScenario spawnerBuilding { get; private set; }
    private UnitUpgradeData unitData;
    private GameObject unitPool;
    public Action onUniqueUnitDied;

    internal override void Start()
    {
        spawnerBuilding = GetComponentInParent<UnitSpawnerScenario>();

        level = spawnerBuilding.unitSpawnLevel;

        unitData = spawnerBuilding.currentUnitLevelData;
        visuals = unitData.unitVisuals;
        basicStats = unitData.unitBasicStats;

        canFly = unitData.unitMobilityStats.canFly;

        side = spawnerBuilding.GetComponent<BuildingStats>().side;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {spawnerBuilding.unitProduceStats.name} ScriptableObject</color>");
            return;
        }

        base.Start();

        unitPool = GameObject.FindWithTag("UnitPool");

        if (unitPool == null)
            Debug.Log("<color=red>No GameObject with tag 'UnitPool' found in scene!</color>");
        else
            transform.parent = unitPool.transform;
    }

    internal override void Die()
    {
        base.Die();
        KillCounterManager.Instance.AddUnitKillData(spawnerBuilding.unitProduceStats.unitType, side);
    }

    private void OnDestroy()
    {
        if (spawnerBuilding.unitProduceStats.isUnique == true)
            onUniqueUnitDied?.Invoke();
    }
}