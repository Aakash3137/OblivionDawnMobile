using UnityEngine;

public class DefenseBuildingStats : BuildingStats
{
    public ScenarioDefenseType defenseType { get; private set; }
    public DefenseBuildingUpgradeData defenseBuildingData { get; private set; }

    internal override void Start()
    {
        identity = buildingStats.buildingIdentity;

        if (buildingStats is DefenseBuildingDataSO defenseWallSO)
        {
            defenseType = defenseWallSO.defenseType;
            defenseBuildingData = defenseWallSO.defenseBuildingUpgradeData[identity.spawnLevel];

            basicStats = defenseBuildingData.buildingBasicStats;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing DefenseBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();
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
        return buildingStats as DefenseBuildingDataSO;
    }
}
