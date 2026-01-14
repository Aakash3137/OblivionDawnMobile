using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Defense Upgrade Stats", menuName = "Scenario Stats/Defense Upgrade Stats")]
public class DefenseUpgradeStatsSO : ScriptableObject
{
    [Header("Defense health stats and Resource cost for upgrades")]
    public string defenseBuildingName;
    public ScenarioDefenseType defenseType;
    public GameObject projectilePrefab;
    public FactionName defenseFaction;
    public Side defenseSide;

    [Header("Defense starts at Level 0 and goes up")]
    public DefenseUpgradeData[] defenseLevelData;

    private void ValidateBase()
    {
        if (defenseLevelData == null)
        {
            defenseLevelData = new DefenseUpgradeData[1];
            defenseLevelData[0] = new DefenseUpgradeData();
        }

        if (defenseType == ScenarioDefenseType.Wall)
        {
            projectilePrefab = null;
        }

    }
    private void OnValidate()
    {
        ValidateBase();
    }
}

[Serializable]
public class DefenseUpgradeData
{
    public int defenseLevel;
    public float defenseBuildTime;
    public RangeStats defenseRangeStats;
    public VisionAngles defenseVisionAngles;
    public AttackTargets defenseAttackTargets;
}