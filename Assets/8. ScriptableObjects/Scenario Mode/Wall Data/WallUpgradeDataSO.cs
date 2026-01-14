using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall UpgradeData SO", menuName = "Upgrade Data/Wall Upgrade Data")]
public class WallUpgradeDataSO : ScriptableObject
{
    [Header("Wall health stats and Resource cost for upgrades")]
    public string wallName;
    public ScenarioBuildingType wallType;
    public ScenarioDefenseType defenseType;
    public FactionName wallFaction;
    public Side wallSide;
    public int wallSpawnLevel;

    [Header("Wall starts at Level 0 and goes up")]
    public WallUpgradeData[] wallLevelData;

    private void ValidateBase()
    {
        if (wallLevelData == null)
        {
            wallLevelData = new WallUpgradeData[1];
            wallLevelData[0] = new WallUpgradeData();
        }

        for (int i = 0; i < wallLevelData.Length; i++)
        {
            wallLevelData[i].wallLevel = i;

            var enumValues = Enum.GetValues(typeof(ScenarioResourceType));

            if (wallLevelData[i].wallUpgradeCosts == null ||
                wallLevelData[i].wallUpgradeCosts.Length != enumValues.Length)
            {
                wallLevelData[i].wallUpgradeCosts =
                    new UpgradeCost[enumValues.Length];
            }

            for (int j = 0; j < enumValues.Length; j++)
            {
                wallLevelData[i].wallUpgradeCosts[j].resourceType =
                    (ScenarioResourceType)enumValues.GetValue(j);
            }
        }

        wallSpawnLevel = Mathf.Clamp(wallSpawnLevel, 0, wallLevelData.Length - 1);

        wallType = ScenarioBuildingType.DefenseBuilding;
        defenseType = ScenarioDefenseType.Wall;
    }
    private void OnValidate()
    {
        ValidateBase();
    }

}
[Serializable]
public class WallUpgradeData
{
    public int wallLevel;
    public float wallBuildTime;
    public Visuals wallVisuals;
    public BasicStats wallBasicStats;

    [Header("Resource costs required for this upgrade")]
    [Tooltip("Resource Type are auto set from enum values of ScenarioResourceType")]
    public UpgradeCost[] wallUpgradeCosts;
}