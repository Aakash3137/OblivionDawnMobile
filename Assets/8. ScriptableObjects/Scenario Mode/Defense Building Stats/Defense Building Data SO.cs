using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Building Data SO", menuName = "Scenario Stats/Defense Building Data")]
public class DefenseBuildingDataSO : BuildingDataSO
{
    [Space(30)]
    public ScenarioDefenseType defenseType;
    public bool canAttackBuildings;
    public bool canAttackWalls;
    public VisionAngles defenseVisionAngles;
    public AttackTargets defenseAttackTargets;
    public int populationCost;
    [Space(20)]
    public List<DefenseBuildingUpgradeData> defenseBuildingUpgradeData;

    internal override void ValidateBase()
    {
        if (defenseBuildingUpgradeData.Count == 0)
            defenseBuildingUpgradeData = new List<DefenseBuildingUpgradeData> { new DefenseBuildingUpgradeData() };

        for (int i = 0; i < defenseBuildingUpgradeData.Count; i++)
        {
            defenseBuildingUpgradeData[i].buildingLevel = i + 1;
        }

        buildingIdentity.spawnLevel = Mathf.Clamp(buildingIdentity.spawnLevel, 0, defenseBuildingUpgradeData.Count - 1);

        // buildingIdentity.name = buildingIdentity.faction.ToString() + " " + defenseType.ToString();
        base.ValidateBase();
    }
}

[Serializable]
public class DefenseBuildingUpgradeData : BuildingUpgradeData
{
    public AttackStats defenseAttackStats;
    public RangeStats defenseRangeStats;
}