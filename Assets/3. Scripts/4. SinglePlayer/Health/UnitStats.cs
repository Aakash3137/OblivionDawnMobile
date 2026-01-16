using UnityEngine;

public class UnitStats : Stats
{
    private UnitSpawnerScenario spawnerBuilding;
    private UnitUpgradeData unitData;

    [Header("Unit Specific Stats (DO NOT EDIT)")]
    public ScenarioOffenseType offenseUnitType;
    public MobilityStats unitMobilityStats;
    public RangeStats rangeStats;
    public VisionAngles visionAngles;
    public AttackTargets attackTargets;
    public FlyStats flyStats;
    private GameObject unitPool;

    internal override void Start()
    {
        spawnerBuilding = GetComponentInParent<UnitSpawnerScenario>();

        offenseUnitType = spawnerBuilding.unitProduceStats.unitType;

        level = spawnerBuilding.unitSpawnLevel;
        unitData = spawnerBuilding.currentUnitLevelData;


        visuals = unitData.unitVisuals;
        basicStats = unitData.unitBasicStats;
        unitMobilityStats = unitData.unitMobilityStats;

        rangeStats = unitData.unitRangeStats;
        visionAngles = unitData.unitVisionAngles;
        attackTargets = unitData.unitAttackTargets;
        flyStats = unitData.unitFlyStats;


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
}