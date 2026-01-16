using UnityEngine;

public class DefenseWallStats : BuildingStats
{
    [field: SerializeField]
    public WallUpgradeDataSO defenseStats { get; private set; }
    public ScenarioDefenseType defenseType { get; private set; }


    internal override void Start()
    {
        base.Start();

        defenseType = defenseStats.defenseType;
    }
}
