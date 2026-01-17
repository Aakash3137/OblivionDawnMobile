using System;
using UnityEngine;

public class WallStats : Stats
{
    [field: SerializeField]
    public WallUpgradeDataSO wallStats { get; private set; }
    public ScenarioBuildingType wallType { get; private set; }
    public ScenarioDefenseType defenseType { get; private set; }
    public BuildingUpgradeData wallData { get; private set; }
    public UpgradeCost[] wallUpgradeCosts { get; private set; }


    public Action<float, float> onWallEnableOrDisable;
    public Action onWallDestroyed;
    private WallParent wallParent;

    internal override void Start()
    {
        wallParent = GetComponentInParent<WallParent>();

        if (wallStats == null)
        {
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");
            return;
        }

        wallType = wallStats.buildingType;
        level = wallStats.buildingSpawnLevel;
        wallData = wallStats.buildingLevelData[level];

        visuals = wallData.buildingVisuals;
        basicStats = wallData.buildingBasicStats;
        wallUpgradeCosts = wallData.buildingUpgradeCosts;

        side = GetComponentInParent<BuildingStats>().side;

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
