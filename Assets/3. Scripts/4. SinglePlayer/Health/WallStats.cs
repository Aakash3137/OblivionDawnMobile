using UnityEngine;

public class WallStats : Stats
{
    public DefenseUpgradeStatsSO defenseStats;
    public DefenseUpgradeData defenseData { get; private set; }
    public UpgradeCost[] defenseUpgradeCosts { get; private set; }

    internal override void Start()
    {
        base.Start();

        if (defenseStats == null)
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");

        level = defenseStats.defenseSpawnLevel;
        defenseData = defenseStats.defenseLevelData[level];
        visuals = defenseData.defenseVisuals;
        basicStats = defenseData.defenseBasicStats;

        maxHealth = basicStats.maxHealth;
        currentHealth = maxHealth;

        defenseUpgradeCosts = defenseData.defenseUpgradeCosts;
    }
}
