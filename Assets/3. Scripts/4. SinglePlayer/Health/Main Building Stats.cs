using UnityEngine;

public class MainBuildingStats : BuildingStats
{
    public MainBuildingUpgradeData mainBuildingData { get; private set; }

    internal override void Initialize()
    {
        identity = buildingStatsSO.buildingIdentity;

        if (buildingStatsSO is MainBuildingDataSO mainBuildingStats)
        {
            mainBuildingData = mainBuildingStats.mainBuildingUpgradeData[identity.spawnLevel];
            basicStats = mainBuildingData.buildingBasicStats;
            buildTime = mainBuildingData.buildingBuildTime;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing MainBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Initialize();
    }

    internal override void Die()
    {
        base.Die();

        switch (side)
        {
            case Side.Player:
                GameStateManager.Instance.ChangeState(GameState.DEFEAT);
                break;
            case Side.Enemy:
                GameStateManager.Instance.ChangeState(GameState.VICTORY);
                break;
        }
    }
}

