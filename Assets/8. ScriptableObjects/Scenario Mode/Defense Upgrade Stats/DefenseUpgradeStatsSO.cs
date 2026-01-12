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
    public int defenseSpawnLevel;

    [Header("Defense starts at Level 0 and goes up")]
    public DefenseUpgradeData[] defenseLevelData;

    private void ValidateBase()
    {
        if (defenseLevelData == null)
        {
            defenseLevelData = new DefenseUpgradeData[1];
            defenseLevelData[0] = new DefenseUpgradeData();
        }

        for (int i = 0; i < defenseLevelData.Length; i++)
        {
            defenseLevelData[i].defenseLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            if (defenseLevelData[i].defenseUpgradeCosts == null ||
                defenseLevelData[i].defenseUpgradeCosts.Length != enumValues.Length)
            {
                defenseLevelData[i].defenseUpgradeCosts = new UpgradeCost[enumValues.Length];
            }

            for (int j = 0; j < enumValues.Length; j++)
            {
                defenseLevelData[i].defenseUpgradeCosts[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
            }
        }

        defenseSpawnLevel = Mathf.Clamp(defenseSpawnLevel, 0, defenseLevelData.Length - 1);

        if (defenseType == ScenarioDefenseType.Wall)
        {
            projectilePrefab = null;
        }

        defenseSpawnLevel = Mathf.Clamp(defenseSpawnLevel, 0, defenseLevelData.Length - 1);
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
    public Visuals defenseVisuals;
    public BasicStats defenseBasicStats;
    public RangeStats defenseRangeStats;
    public VisionAngles defenseVisionAngles;
    public AttackTargets defenseAttackTargets;

    [Header("Resource costs required for this upgrade")]
    [Tooltip("Resource Type are auto set from enum values of ScenarioResourceType")]
    public UpgradeCost[] defenseUpgradeCosts;
}