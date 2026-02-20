using UnityEngine;

public class DefenseBuildingStats : BuildingStats
{
    public ScenarioDefenseType defenseType { get; private set; }
    public DefenseBuildingUpgradeData defenseBuildingData { get; private set; }

    internal override void Initialize()
    {
        identity = buildingStatsSO.buildingIdentity;

        if (buildingStatsSO is DefenseBuildingDataSO defenseWallSO)
        {
            defenseType = defenseWallSO.defenseType;
            defenseBuildingData = defenseWallSO.defenseBuildingUpgradeData[identity.spawnLevel];
            buildTime = defenseBuildingData.buildingBuildTime;

            basicStats = defenseBuildingData.buildingBasicStats;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing DefenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();
    }

    internal override void Die()
    {
        base.Die();

        KillCounterManager.Instance.AddDefenseBuildingDestroyedData(defenseType, side);
    }

    public DefenseBuildingUpgradeData GetBuildingData()
    {
        return defenseBuildingData;
    }
    public DefenseBuildingDataSO GetBuildingSO()
    {
        return buildingStatsSO as DefenseBuildingDataSO;
    }
}
