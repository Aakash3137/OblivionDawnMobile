using UnityEngine;

public class DefenseStats : BuildingStats
{
    [field: SerializeField]
    public DefenseUpgradeStatsSO defenseStats { get; private set; }
    public ScenarioDefenseType defenseType { get; private set; }
    public DefenseUpgradeData defenseData { get; private set; }


    internal override void Start()
    {
        base.Start();

        defenseType = defenseStats.defenseType;
        defenseData = defenseStats.defenseLevelData[level];
    }

    internal override void OnDestroy()
    {
        currentTile.ClearOccupant();
        currentTile.hasBuilding = false;

        KillCounterManager.Instance.AddDefenseBuildingDestroyedData(defenseType, side);
    }
}
