using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct Identity
{
    public string name;
    public int spawnLevel;
    public FactionName faction;
    public int priority;
}
[Serializable]
public class CardDetails
{
    public bool factionUnlocked;
    public bool isUnlocked;

    [ShowIf(nameof(isUnlocked))]
    public bool purchased;
    public int minBuildingLevel;
}
[Serializable]
public class BuildingUpgradeData
{
    public int buildingLevel;
    [Space(30)]
    public BasicStats buildingBasicStats;
    public float buildingBuildTime;
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
