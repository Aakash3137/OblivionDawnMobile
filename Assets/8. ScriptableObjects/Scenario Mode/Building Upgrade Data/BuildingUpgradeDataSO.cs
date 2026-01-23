using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Upgrade Data", menuName = "Upgrade Data/Building Upgrade Data")]
public class BuildingUpgradeDataSO : ScriptableObject
{
    public Identity buildingIdentity;
    [Header("Name")]
    public string buildingName;
    [Header("Prefabs")]
    [Header("Enums")]
    public ScenarioBuildingType buildingType;
    public FactionName buildingFaction;
    [Header("Building stats")]
    public int targetPriority;
    public int buildingSpawnLevel;
    public BuildCost[] buildingBuildCost;

    public Visuals buildingVisuals;

    [Header("Building starts")]
    public BuildingUpgradeData[] buildingUpgradeData;

    internal virtual void ValidateBase()
    {
        if (buildingUpgradeData.Length == 0)
        {
            buildingUpgradeData = new BuildingUpgradeData[1];
            buildingUpgradeData[0] = new BuildingUpgradeData();
        }

        for (int i = 0; i < buildingUpgradeData.Length; i++)
        {
            buildingUpgradeData[i].buildingLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            // if (buildingLevelData[i].buildingUpgradeCosts == null ||
            //     buildingLevelData[i].buildingUpgradeCosts.Length != enumValues.Length)
            // {
            //     buildingLevelData[i].buildingUpgradeCosts =
            //         new UpgradeCost[enumValues.Length];
            // }

            // for (int j = 0; j < enumValues.Length; j++)
            // {
            //     buildingLevelData[i].buildingUpgradeCosts[j].resourceType =
            //         (ScenarioResourceType)enumValues.GetValue(j);
            // }

            if (buildingBuildCost == null || buildingBuildCost.Length != enumValues.Length)
            {
                buildingBuildCost = new BuildCost[enumValues.Length];
            }

            for (int j = 0; j < buildingBuildCost.Length; j++)
            {
                buildingBuildCost[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
            }
        }

        buildingSpawnLevel = Mathf.Clamp(buildingSpawnLevel, 0, buildingUpgradeData.Length - 1);
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
    public BasicStats buildingBasicStats;
}