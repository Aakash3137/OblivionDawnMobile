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
    public CardDetails cardDetails;

    [Space(5)]
    public Sprite unitIcon;

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

    public int populationCost;

    [Space(20), Header("Unit Upgrade Data")]
    public UnitUpgradeData[] unitUpgradeData;

    private void ValidateBase()
    {
        if (unitUpgradeData == null || unitUpgradeData.Length == 0)
        {
            unitUpgradeData = new UnitUpgradeData[1];
            unitUpgradeData[0] = new UnitUpgradeData();
        }

        for (int i = 0; i < unitUpgradeData.Length; i++)
        {
            unitUpgradeData[i].unitLevel = i;
        }

        canFly = unitType == ScenarioUnitType.Air;

        unitIdentity.spawnLevel = Mathf.Clamp(unitIdentity.spawnLevel, 0, unitUpgradeData.Length - 1);

        if (!hasUpkeep)
            return;

        var enumValues = ScenarioDataTypes._resourceEnumValues;
        int targetLength = enumValues.Length;

        upKeepCost = BuildCostUtils.ResizePreservingData(upKeepCost, targetLength);

        for (int j = 0; j < upKeepCost.Length; j++)
            upKeepCost[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
    }

    private void OnValidate()
    {
        ValidateBase();
    }

    [ShowIf(nameof(hasUpkeep)), Button]
    public void SetUpKeepCost(float increasePercent)
    {
        float discount = 1f + (increasePercent / 100f);

        for (int i = 0; i < upKeepCost.Length; i++)
        {
            if (upKeepCost[i].resourceAmount != 0)
            {
                var amt = Mathf.RoundToInt(upKeepCost[i].resourceAmount * discount);
                upKeepCost[i].resourceAmount = Mathf.Max(amt, 1);
            }
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

public static class BuildCostUtils
{
    public static BuildCost[] ResizePreservingData(BuildCost[] existing, int targetLength)
    {
        if (existing != null && existing.Length == targetLength)
            return existing;

        var resized = new BuildCost[targetLength];

        if (existing != null)
        {
            int copyLength = Mathf.Min(existing.Length, targetLength);
            for (int i = 0; i < copyLength; i++)
            {
                resized[i] = existing[i];
            }
        }

        return resized;
    }
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
    public float minAttackRange;
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

