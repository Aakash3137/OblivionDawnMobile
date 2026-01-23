using System;
using UnityEngine;

public class WallStats : Stats
{
    [field: SerializeField]
    public WallUpgradeDataSO wallStats { get; private set; }
    public ScenarioBuildingType wallType { get; private set; }
    public ScenarioDefenseType defenseType { get; private set; }
    public WallBuildingUpgradeData wallData { get; private set; }


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
        level = wallStats.buildingIdentity.spawnLevel;
        visuals = wallStats.buildingVisuals;

        wallData = wallStats.wallBuildingUpgradeData[level];

        basicStats = wallData.buildingBasicStats;

        side = GetComponentInParent<BuildingStats>().side;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {wallStats.name} ScriptableObject</color>");
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
        // This event is to destroy wall Parent if the wall Parent current HP goes to zero
        onWallDestroyed?.Invoke();
    }
}
