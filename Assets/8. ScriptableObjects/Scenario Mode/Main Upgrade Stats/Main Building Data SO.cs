using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Main Building Data")]
public class MainBuildingDataSO : BuildingDataSO
{
    [Space(50)]
    public MainBuildingUpgradeData[] offenseBuildingUpgradeData;


    internal override void ValidateBase()
    {
        if (offenseBuildingUpgradeData.Length == 0)
            offenseBuildingUpgradeData = new MainBuildingUpgradeData[1];

        for (int i = 0; i < offenseBuildingUpgradeData.Length; i++)
        {
            offenseBuildingUpgradeData[i].buildingLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            if (buildingBuildCost == null || buildingBuildCost.Length != enumValues.Length)
            {
                buildingBuildCost = new BuildCost[enumValues.Length];
            }

            for (int j = 0; j < buildingBuildCost.Length; j++)
            {
                buildingBuildCost[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
            }
        }

        buildingIdentity.spawnLevel = Mathf.Max(0, offenseBuildingUpgradeData.Length - 1);
    }
}

[Serializable]
public struct MainBuildingUpgradeData
{
    public int buildingLevel;
    public BasicStats buildingBasicStats;
    public float buildingBuildTime;
}
