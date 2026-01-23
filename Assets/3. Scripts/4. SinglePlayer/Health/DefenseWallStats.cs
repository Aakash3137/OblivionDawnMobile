using UnityEngine;

public class DefenseWallStats : BuildingStats
{
    [field: SerializeField]
    public ScenarioDefenseType defenseType { get; private set; }


    internal override void Start()
    {
        if (buildingStats is WallUpgradeDataSO defenseWallStats)
        {
            defenseType = defenseWallStats.defenseType;
        }
        else
        {
            Debug.Log($"<color=#FAFA00>Building {name} missing WallUpgradeDataSO. Assign correct ScriptableObject.</color>");
        }

        base.Start();
    }
}
