using UnityEngine;

public class BuildingStats : Stats
{
    [field: SerializeField]
    public BuildingUpgradeDataSO buildingStats { get; private set; }
    public ScenarioBuildingType buildingType { get; private set; }
    public BuildingUpgradeData buildingData { get; private set; }
    public UpgradeCost[] buildingUpgradeCosts { get; private set; }


    internal override void Start()
    {
        if (buildingStats == null)
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");

        level = buildingStats.buildingSpawnLevel;
        buildingData = buildingStats.buildingLevelData[level];
        visuals = buildingData.buildingVisuals;
        basicStats = buildingData.buildingBasicStats;

        buildingUpgradeCosts = buildingData.buildingUpgradeCosts;

        buildingType = buildingStats.buildingType;

        side = GetComponentInParent<Tile>().ownerSide;

        if (visuals.playerUnitMaterial == null)
        {
            Debug.Log($"<color=magenta>Assign materials for {name} on {buildingStats.name} ScriptableObject</color>");
            return;
        }

        base.Start();
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
