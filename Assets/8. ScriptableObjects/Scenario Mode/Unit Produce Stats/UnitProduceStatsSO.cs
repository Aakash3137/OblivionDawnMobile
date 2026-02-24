using System;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Unit Upgrade Stats", menuName = "Scenario Stats/Unit Upgrade Stats")]
public class UnitProduceStatsSO : ScriptableObject
{
    public Identity unitIdentity;
    public ScenarioUnitType unitType;
    public Visuals unitVisuals;
    public VisionAngles unitVisionAngles;
    public AttackTargets unitAttackTargets;

    [Space(20)]
    public bool hasUpkeep;
    [ShowIf(nameof(hasUpkeep))]
    public BuildCost[] upKeepCost;
    [field: SerializeField, Space(10), ShowIf(nameof(hasUpkeep))]
    public float upKeepTime { get; private set; }

    [Space(20)]
    public OffenseBuildingStats spawnerBuilding;

    public bool canFly;
    [ShowIf(nameof(canFly))]
    public FlyStats unitFlyStats;
    public int unitPopulationCost;
    public Sprite UnitIcon;

    [Space(20), Header("Unit Upgrade Data")]
    public UnitUpgradeData[] unitUpgradeData;

    private void ValidateBase()
    {
        if (unitUpgradeData.Length == 0)
        {
            unitUpgradeData = new UnitUpgradeData[1];
            unitUpgradeData[0] = new UnitUpgradeData();
        }

        for (int i = 0; i < unitUpgradeData.Length; i++)
        {
            unitUpgradeData[i].unitLevel = i;

            if (unitType == ScenarioUnitType.Air)
            {
                canFly = true;
            }
        }

        unitIdentity.spawnLevel = Mathf.Clamp(unitIdentity.spawnLevel, 0, unitUpgradeData.Length - 1);

        var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

        if (upKeepCost == null || upKeepCost.Length != enumValues.Length)
        {
            upKeepCost = new BuildCost[enumValues.Length];
        }

        for (int j = 0; j < enumValues.Length; j++)
        {
            upKeepCost[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
        }

    }
    private void OnValidate()
    {
        ValidateBase();
    }
    [ShowIf(nameof(hasUpkeep)), Button]
    public void SetUpKeepCost(float increasePercent)
    {
        increasePercent = increasePercent / 100f;
        float discount = 1f + increasePercent;
        var baseCosts = upKeepCost;
        for (int i = 0; i < upKeepCost.Length; i++)
        {
            var amt = Mathf.RoundToInt(baseCosts[i].resourceAmount * discount);

            if (upKeepCost[i].resourceAmount != 0)
                upKeepCost[i].resourceAmount = Mathf.Max(amt, 1);
        }
    }
}
[Serializable]
public class UnitUpgradeData
{
    public int unitLevel;
    public float unitBuildTime;
    public BasicStats unitBasicStats;
    public AttackStats unitAttackStats;
    public MobilityStats unitMobilityStats;
    public RangeStats unitRangeStats;
}
[Serializable]
public struct BasicStats
{
    public float maxHealth;
    public float armor;
}

[Serializable]
public struct BuildCost
{
    public ScenarioResourceType resourceType;
    public int resourceAmount;
}

[Serializable]
public struct Visuals
{
    // public Mesh upgradeMesh;
    public Material playerUnitMaterial;
    public Material enemyUnitMaterial;
}

[Serializable]
public struct AttackStats
{
    public float damage;   // apply to take damage for units only
    public float buildingDamage;   // apply to take damage for buildings - include resource, defence, offense buildings
    public float fireRate;
}

[Serializable]
public struct MobilityStats
{
    public float moveSpeed;
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

