using UnityEngine;

public class MainBuildingStats : BuildingStats
{
    public MainBuildingUpgradeData mainBuildingData { get; private set; }

    internal override void Start()
    {
        identity = buildingStats.buildingIdentity;

        if (buildingStats is MainBuildingDataSO mainBuildingStats)
        {
            mainBuildingData = mainBuildingStats.mainBuildingUpgradeData[identity.spawnLevel];
            basicStats = mainBuildingData.buildingBasicStats;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing MainBuildingDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();
    }

    internal override void Die()
    {
        base.Die();

        switch (side)
        {
            case Side.Player:
                RTSGameStateManager.Instance.ChangeState(RTSGameState.DEFEAT);
                break;
            case Side.Enemy:
                RTSGameStateManager.Instance.ChangeState(RTSGameState.VICTORY);
                break;
        }
    }
}

