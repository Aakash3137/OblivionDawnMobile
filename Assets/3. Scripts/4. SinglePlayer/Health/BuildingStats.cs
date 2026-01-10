using UnityEngine;

public class BuildingStats : Stats
{
    public BuildingUpgradeDataSO buildingStats;
    public BuildingUpgradeData buildingData { get; private set; }
    public UpgradeCost[] buildingUpgradeCosts { get; private set; }


    internal override void Start()
    {
        base.Start();

        if (buildingStats == null)
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");

        level = buildingStats.buildingSpawnLevel;
        buildingData = buildingStats.buildingLevelData[level];
        visuals = buildingData.buildingVisuals;
        basicStats = buildingData.buildingBasicStats;

        maxHealth = basicStats.maxHealth;
        currentHealth = maxHealth;

        buildingUpgradeCosts = buildingData.buildingUpgradeCosts;
    }

    // public void UpgradeBuilding()
    // {
    //     //Upgrade building and updating resource generation values
    //     level++;
    //     buildingData = buildingStats.buildingLevelData[level];
    //     maxHealth = buildingData.buildingBasicStats.maxHealth;

    //     visuals = buildingData.buildingVisuals;
    //     basicStats = buildingData.buildingBasicStats;

    //     buildingUpgradeCosts = buildingData.buildingUpgradeCosts;
    // }
    // public bool canUpgrade()
    // {
    //     return PlayerResourceManager.Instance.HasResources(buildingUpgradeCosts);
    // }

    void OnDestroy()
    {
        Tile currentTile = GetComponentInParent<Tile>();
        if (currentTile != null)
            currentTile.hasBuilding = false;
    }
}
