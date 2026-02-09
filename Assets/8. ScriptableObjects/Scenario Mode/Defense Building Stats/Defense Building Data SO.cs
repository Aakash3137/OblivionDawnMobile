using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Defense Building Data")]
public class DefenseBuildingDataSO : BuildingDataSO
{
    [Space(30)]
    public ScenarioDefenseType defenseType;
    public bool canAttackBuildings;
    public bool canAttackWalls;
    public VisionAngles defenseVisionAngles;
    public AttackTargets defenseAttackTargets;
    public DefenseBuildingUpgradeData[] defenseBuildingUpgradeData;

    internal override void ValidateBase()
    {
        if (defenseBuildingUpgradeData.Length == 0)
            defenseBuildingUpgradeData = new DefenseBuildingUpgradeData[1];

        for (int i = 0; i < defenseBuildingUpgradeData.Length; i++)
        {
            defenseBuildingUpgradeData[i].buildingLevel = i;

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

        buildingIdentity.spawnLevel = Mathf.Clamp(buildingIdentity.spawnLevel, 0, defenseBuildingUpgradeData.Length - 1);

        // buildingIdentity.name = buildingIdentity.faction.ToString() + " " + defenseType.ToString();
    }
}

[Serializable]
public class DefenseBuildingUpgradeData : BuildingUpgradeData
{
    public AttackStats defenseAttackStats;
    public MobilityStats defenseMobilityStats;
    public RangeStats defenseRangeStats;
}