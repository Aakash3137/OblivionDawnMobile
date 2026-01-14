using System;
using UnityEngine;

public class WallStats : Stats
{
    [field: SerializeField]
    public WallUpgradeDataSO wallStats { get; private set; }
    public ScenarioBuildingType wallType { get; private set; }
    public ScenarioDefenseType defenseType { get; private set; }
    public WallUpgradeData wallData { get; private set; }
    public UpgradeCost[] wallUpgradeCosts { get; private set; }


    public Action<float, float> onWallEnableOrDisable;
    public Action onWallDestroyed;
    private WallParent wallParent;

    internal override void Start()
    {
        wallParent = GetComponentInParent<WallParent>();

        if (wallStats == null)
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");

        wallType = wallStats.wallType;
        level = wallStats.wallSpawnLevel;
        wallData = wallStats.wallLevelData[level];

        visuals = wallData.wallVisuals;
        basicStats = wallData.wallBasicStats;
        wallUpgradeCosts = wallData.wallUpgradeCosts;

        side = GetComponentInParent<Tile>().ownerSide;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {wallStats.name} ScriptableObject</color>");
            return;
        }

        base.Start();
    }

    public override void TakeDamage(float amount)
    {
        if (amount <= currentHealth)
            wallParent.DamageWall(amount);
        else
            wallParent.DamageWall(currentHealth);

        base.TakeDamage(amount);
    }
    private void OnEnable()
    {
        if (currentHealth > 0)
            onWallEnableOrDisable?.Invoke(currentHealth, basicStats.maxHealth);
    }
    private void OnDisable()
    {
        if (currentHealth > 0)
            onWallEnableOrDisable?.Invoke(currentHealth, basicStats.maxHealth);
    }
    private void OnDestroy()
    {
        onWallDestroyed?.Invoke();
    }
}
