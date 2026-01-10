using UnityEngine;

public class UnitStats : Stats
{
    private UnitSpawnerScenario spawnerBuilding;
    private UnitUpgradeData unitData;

    [Header("Unit Specific Stats (DO NOT EDIT)")]
    public UnitMobilityStats unitMobilityStats;
    public UnitRangeStats rangeStats;
    public UnitVisionAngles visionAngles;
    public UnitAttackTargets attackTargets;
    public UnitFlyStats flyStats;


    internal override void Start()
    {
        base.Start();

        spawnerBuilding = GetComponentInParent<UnitSpawnerScenario>();
        level = spawnerBuilding.unitSpawnLevel;
        unitData = spawnerBuilding.currentUnitLevelData;

        visuals = unitData.unitVisuals;
        basicStats = unitData.unitBasicStats;
        unitMobilityStats = unitData.unitMobilityStats;
        rangeStats = unitData.unitRangeStats;
        visionAngles = unitData.unitVisionAngles;
        attackTargets = unitData.unitAttackTargets;
        flyStats = unitData.unitFlyStats;

        maxHealth = basicStats.maxHealth;
        currentHealth = maxHealth;

        Renderer renderer = GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            if (visuals.playerUnitMaterial == null)
            {
                Debug.Log($"<color=magenta>Assign materials for {name} on {spawnerBuilding.unitProduceStats.name} ScriptableObject</color>");
                return;
            }

            switch (spawnerBuilding.buildingSide.side)
            {
                case Side.Player:
                    renderer.material = visuals.playerUnitMaterial;
                    break;
                case Side.Enemy:
                    renderer.material = visuals.enemyUnitMaterial;
                    break;
            }
        }
    }
}