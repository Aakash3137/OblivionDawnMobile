using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Upgrade Stats", menuName = "Scenario Stats/Unit Upgrade Stats")]
public class UnitProduceStatsSO : ScriptableObject
{
    [Header("Unit health stats and Resource cost for upgrades")]
    public string unitName;
    public ScenarioOffenseType unitType;
    public GameObject unitPrefab;
    public GameObject projectilePrefab;
    public FactionName unitFaction;
    public Side unitSide;
    public int unitPopulationCost;
    public int unitSpawnLevel;
    public bool isUnique;

    [Header("Unit starts at Level 0 and goes up")]
    public UnitUpgradeData[] unitLevelData;

    private void ValidateBase()
    {
        if (unitLevelData == null)
        {
            unitLevelData = new UnitUpgradeData[1];
            unitLevelData[0] = new UnitUpgradeData();
        }

        for (int i = 0; i < unitLevelData.Length; i++)
        {
            unitLevelData[i].unitLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            if (unitLevelData[i].unitUpgradeCosts == null ||
                unitLevelData[i].unitUpgradeCosts.Length != enumValues.Length)
            {
                unitLevelData[i].unitUpgradeCosts = new UpgradeCost[enumValues.Length];
            }

            for (int j = 0; j < enumValues.Length; j++)
            {
                unitLevelData[i].unitUpgradeCosts[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
            }

            if (unitType == ScenarioOffenseType.Air)
            {
                unitLevelData[i].unitMobilityStats.canFly = true;
            }
            else
            {
                unitLevelData[i].unitMobilityStats.flySpeed = 0f;
            }
        }

        unitSpawnLevel = Mathf.Clamp(unitSpawnLevel, 0, unitLevelData.Length - 1);
    }
    private void OnValidate()
    {
        ValidateBase();
    }
}
[Serializable]
public class UnitUpgradeData
{
    public int unitLevel;
    public float unitBuildTime;
    public Visuals unitVisuals;
    public BasicStats unitBasicStats;
    public MobilityStats unitMobilityStats;
    public RangeStats unitRangeStats;
    public VisionAngles unitVisionAngles;
    public AttackTargets unitAttackTargets;
    public FlyStats unitFlyStats;

    [Header("Resource costs required for this upgrade")]
    [Tooltip("Resource Type are auto set from enum values of ScenarioResourceType")]
    public UpgradeCost[] unitUpgradeCosts;
}
[Serializable]
public struct BasicStats
{
    public float maxHealth;
    public float armour;
}

[Serializable]
public struct Visuals
{
    public Mesh upgradeMesh;
    public Material playerUnitMaterial;
    public Material enemyUnitMaterial;
}

[Serializable]
public struct UpgradeCost
{
    public ScenarioResourceType resourceType;
    public int resourceCost;
}

[Serializable]
public struct MobilityStats
{
    public bool canFly;
    public float moveSpeed;
    public float flySpeed;
}

[Serializable]
public struct AttackTargets
{
    public bool canAttackAir;
    public bool canAttackGround;
}

[Serializable]
public struct VisionAngles
{
    public float narrowViewAngle;
    public float wideViewAngle;
}

[Serializable]
public struct RangeStats
{
    public float detectionRange;
    public float attackRange;
}

[Serializable]
public struct FlyStats
{
    public float flyHeight;
    public float climbAngle;
    public float turnSpeed;
    public float bankAngle;
    public float evadeRadius;
    public float attackAngleTolerance;
}

