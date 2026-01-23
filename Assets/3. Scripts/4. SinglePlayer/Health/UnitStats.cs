using System;
using UnityEngine;

public class UnitStats : Stats
{
    [Header("Unit Settings")]
    [SerializeField]
    private bool canFly;

    public override bool CanFly => canFly;

    [field: SerializeField]
    public UnitSpawnerScenario spawnerBuilding { get; private set; }
    public UnitProduceStatsSO unitProduceStats;
    private UnitUpgradeData unitData;
    private GameObject unitPool;
    public Action onUniqueUnitDied;

    internal override void Start()
    {
        spawnerBuilding = GetComponentInParent<UnitSpawnerScenario>();

        unitProduceStats = spawnerBuilding.unitProduceStats;

        level = spawnerBuilding.unitSpawnLevel;

        unitData = spawnerBuilding.currentUnitLevelData;
        basicStats = unitData.unitBasicStats;

        visuals = unitProduceStats.unitVisuals;

        canFly = unitProduceStats.canFly;

        side = spawnerBuilding.GetComponent<BuildingStats>().side;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {unitProduceStats.name} ScriptableObject</color>");
        }

        faction = unitProduceStats.unitIdentity.faction;
        targetPriority = unitProduceStats.unitIdentity.priority;

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
        KillCounterManager.Instance.AddUnitKillData(unitProduceStats.unitType, side);
    }

    private void OnDestroy()
    {
        if (unitProduceStats.unitIdentity.isUnique == true)
            onUniqueUnitDied?.Invoke();
    }
}