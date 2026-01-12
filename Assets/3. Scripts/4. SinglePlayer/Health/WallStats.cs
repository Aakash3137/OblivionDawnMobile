using UnityEngine;

public class WallStats : Stats
{
    [field: SerializeField]
    public DefenseUpgradeStatsSO defenseStats { get; private set; }
    public ScenarioDefenseType defenseType { get; private set; }
    public DefenseUpgradeData defenseData { get; private set; }
    public UpgradeCost[] defenseUpgradeCosts { get; private set; }

    private WallParent wallParent;


    internal override void Start()
    {
        wallParent = GetComponentInParent<WallParent>();

        if (defenseStats == null)
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");

        level = defenseStats.defenseSpawnLevel;
        defenseData = defenseStats.defenseLevelData[level];
        visuals = defenseData.defenseVisuals;
        basicStats = defenseData.defenseBasicStats;
        defenseUpgradeCosts = defenseData.defenseUpgradeCosts;

        defenseType = defenseStats.defenseType;

        side = GetComponentInParent<Tile>().ownerSide;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {defenseStats.name} ScriptableObject</color>");
            return;
        }

        base.Start();
    }

    public override void TakeDamage(float amount)
    {
        wallParent.DamageWall(amount);

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
