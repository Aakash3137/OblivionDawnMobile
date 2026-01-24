using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class UnitStats : Stats
{
    [Header("Assign Unit Produce Stats")]
    public UnitProduceStatsSO unitProduceSO;

    public ScenarioUnitType unitType { get; private set; }
    public int unitPopulationCost { get; private set; }
    private GameObject unitPool;

    public VisionAngles unitVisionAngles { get; private set; }
    public AttackTargets unitAttackTargets { get; private set; }

    [SerializeField, ReadOnly]
    private bool canFly;
    [ShowIf(nameof(canFly)), ReadOnly]
    public FlyStats unitFlyStats;
    public override bool CanFly => canFly;

    public OffenseBuildingStats spawnerBuilding { private get; set; }
    public UnitUpgradeData unitData { get; private set; }
    public Action onUniqueUnitDied;


    internal override void Start()
    {
        identity = unitProduceSO.unitIdentity;
        unitData = unitProduceSO.unitUpgradeData[identity.spawnLevel];
        basicStats = unitData.unitBasicStats;

        visuals = unitProduceSO.unitVisuals;
        canFly = unitProduceSO.canFly;

        side = spawnerBuilding.side;

        unitVisionAngles = unitProduceSO.unitVisionAngles;
        unitAttackTargets = unitProduceSO.unitAttackTargets;

        unitPopulationCost = unitProduceSO.unitPopulationCost;
        unitType = unitProduceSO.unitType;

        if (canFly)
            unitFlyStats = unitProduceSO.unitFlyStats;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {unitProduceSO.name} ScriptableObject</color>");
        }

        base.Start();

        unitPool = GameObject.FindWithTag("UnitPool");

        if (unitPool == null)
            Debug.Log("<color=green>No GameObject with tag 'UnitPool' found in scene!</color>");
        else
            transform.parent = unitPool.transform;
    }

    public void FireWeapon()
    {

    }

    public void Fly()
    {

    }

    internal override void Die()
    {
        base.Die();
        KillCounterManager.Instance.AddUnitKillData(unitType, side);
    }

    private void OnDestroy()
    {
        if (unitProduceSO.unitIdentity.isUnique == true)
            onUniqueUnitDied?.Invoke();
    }
}