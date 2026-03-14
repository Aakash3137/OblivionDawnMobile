using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Main Building Data")]
public class MainBuildingDataSO : BuildingDataSO
{
    [Space(20)]
    public List<MainBuildingUpgradeData> mainBuildingUpgradeData;


    internal override void ValidateBase()
    {
        if (mainBuildingUpgradeData.Count == 0)
            mainBuildingUpgradeData = new List<MainBuildingUpgradeData> { new MainBuildingUpgradeData() };

        for (int i = 0; i < mainBuildingUpgradeData.Count; i++)
        {
            mainBuildingUpgradeData[i].buildingLevel = i;

            // var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            // if (mainBuildingUpgradeData[i].starterResources == null || mainBuildingUpgradeData[i].starterResources.Length != enumValues.Length)
            // {
            //     mainBuildingUpgradeData[i].starterResources = new BuildCost[enumValues.Length];
            // }

            // for (int j = 0; j < mainBuildingUpgradeData[i].starterResources.Length; j++)
            // {
            //     mainBuildingUpgradeData[i].starterResources[j].resourceType = (ScenarioResourceType)enumValues.GetValue(j);
            // }
        }

        buildingIdentity.spawnLevel = Mathf.Clamp(buildingIdentity.spawnLevel, 0, mainBuildingUpgradeData.Count - 1);

        // buildingIdentity.name = buildingIdentity.faction.ToString() + " " + buildingType.ToString();

        base.ValidateBase();
    }
}

[Serializable]
public class MainBuildingUpgradeData : BuildingUpgradeData
{
    [Range(4, 8)] public int maxDeckEquipCount;
    [Range(10, 50)] public int maxPopulation;
    [Range(500, 1000)] public int starterResources;
}
