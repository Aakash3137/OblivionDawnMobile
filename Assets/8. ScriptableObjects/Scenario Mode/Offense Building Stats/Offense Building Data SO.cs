using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Offense Building Data")]
public class OffenseBuildingDataSO : BuildingDataSO
{
    [Space(30)]
    public ScenarioOffenseType offenseType;
    public UnitStats unitPrefab;
    public List<OffenseBuildingUpgradeData> offenseBuildingUpgradeData;


    internal override void ValidateBase()
    {
        if (offenseBuildingUpgradeData.Count == 0)
            offenseBuildingUpgradeData = new List<OffenseBuildingUpgradeData> { new OffenseBuildingUpgradeData() };

        for (int i = 0; i < offenseBuildingUpgradeData.Count; i++)
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

        buildingIdentity.spawnLevel = Mathf.Clamp(buildingIdentity.spawnLevel, 0, offenseBuildingUpgradeData.Count - 1);

        // buildingIdentity.name = buildingIdentity.faction.ToString() + " " + offenseType.ToString();
    }
}

[Serializable]
public class OffenseBuildingUpgradeData : BuildingUpgradeData
{
    public float unitSpawnTime;
}