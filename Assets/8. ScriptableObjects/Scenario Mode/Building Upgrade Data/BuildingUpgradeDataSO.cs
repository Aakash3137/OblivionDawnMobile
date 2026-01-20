using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Upgrade Data", menuName = "Upgrade Data/Building Upgrade Data")]
public class BuildingUpgradeDataSO : ScriptableObject
{
    [Header("Building health stats and Resource cost for upgrades")]
    public string buildingName;
    public ScenarioBuildingType buildingType;
    public FactionName buildingFaction;
    public Side buildingSide;
    public int buildingSpawnLevel;

    [Header("Building starts at Level 0 and goes up")]
    public BuildingUpgradeData[] buildingLevelData;

    internal virtual void ValidateBase()
    {
        if (buildingLevelData.Length == 0)
        {
            buildingLevelData = new BuildingUpgradeData[1];
            buildingLevelData[0] = new BuildingUpgradeData();
        }

        for (int i = 0; i < buildingLevelData.Length; i++)
        {
            buildingLevelData[i].buildingLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            if (buildingLevelData[i].buildingUpgradeCosts == null ||
                buildingLevelData[i].buildingUpgradeCosts.Length != enumValues.Length)
            {
                buildingLevelData[i].buildingUpgradeCosts =
                    new UpgradeCost[enumValues.Length];
            }

            for (int j = 0; j < enumValues.Length; j++)
            {
                buildingLevelData[i].buildingUpgradeCosts[j].resourceType =
                    (ScenarioResourceType)enumValues.GetValue(j);
            }
        }

        buildingSpawnLevel = Mathf.Clamp(buildingSpawnLevel, 0, buildingLevelData.Length - 1);
    }
    private void OnValidate()
    {
        ValidateBase();
    }

}

[Serializable]
public class BuildingUpgradeData
{
    public int buildingLevel;
    public float buildingBuildTime;
    public Visuals buildingVisuals;
    public BasicStats buildingBasicStats;

    [Header("Resource costs required for this upgrade")]
    [Tooltip("Resource Type are auto set from enum values of ScenarioResourceType")]
    public UpgradeCost[] buildingUpgradeCosts;
}