using UnityEngine;

[CreateAssetMenu(fileName = "Defense Upgrade Stats", menuName = "Scenario Stats/Defense Upgrade Stats")]
public class DefenseUpgradeStatsSO : ScriptableObject
{
    [Header("Defense health stats and Resource cost for upgrades")]
    public string defenseBuildingName;
    public ScenarioDefenseType defenseBuildingType;
    public int defenseSpawnLevel;

    [Header("Defense starts at Level 0 and goes up")]
    public DefenseUpgradeData[] defenseLevelData;

    private void OnValidate()
    {
        for (int i = 0; i < defenseLevelData.Length; i++)
        {
            defenseLevelData[i].level = i;
        }
    }
}

[System.Serializable]
public class DefenseUpgradeData
{
    public int level;
    public Visuals defenseVisuals;
    public BasicStats defenseBasicStats;

    [Header("Resource costs required for this upgrade")]
    [Tooltip("Resource Type are auto set from enum values of ScenarioResourceType")]
    public UpgradeCost[] defenseUpgradeCosts;
}