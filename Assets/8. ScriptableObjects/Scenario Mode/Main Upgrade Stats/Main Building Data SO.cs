using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Main Building Data")]
public class MainBuildingDataSO : BuildingDataSO
{
    [Space(30)]
    public MainBuildingUpgradeData[] mainBuildingUpgradeData;


    internal override void ValidateBase()
    {
        if (mainBuildingUpgradeData.Length == 0)
            mainBuildingUpgradeData = new MainBuildingUpgradeData[1];

        for (int i = 0; i < mainBuildingUpgradeData.Length; i++)
        {
            mainBuildingUpgradeData[i].buildingLevel = i;

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

        buildingIdentity.spawnLevel = Mathf.Clamp(buildingIdentity.spawnLevel, 0, mainBuildingUpgradeData.Length - 1);
    }
}

[Serializable]
public class MainBuildingUpgradeData : BuildingUpgradeData
{

}
