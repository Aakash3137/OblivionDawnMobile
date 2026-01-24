using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall UpgradeData SO", menuName = "Upgrade Data/Wall Upgrade Data")]
public class WallUpgradeDataSO : BuildingDataSO
{
    [Space(30)]
    public ScenarioDefenseType defenseType;
    public WallBuildingUpgradeData[] wallBuildingUpgradeData;

    internal override void ValidateBase()
    {
        if (wallBuildingUpgradeData.Length == 0)
            wallBuildingUpgradeData = new WallBuildingUpgradeData[1];

        for (int i = 0; i < wallBuildingUpgradeData.Length; i++)
        {
            wallBuildingUpgradeData[i].buildingLevel = i;

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

        buildingIdentity.spawnLevel = Mathf.Max(0, wallBuildingUpgradeData.Length - 1);

        buildingType = ScenarioBuildingType.DefenseBuilding;
        defenseType = ScenarioDefenseType.Wall;
    }
}

[Serializable]
public class WallBuildingUpgradeData : BuildingUpgradeData
{

}