using UnityEngine;

public class BuildingStats : Stats
{
    public BuildingUpgradeDataSO buildingStats;
    BuildingUpgradeCost[] upgradeCosts;

    private void Start()
    {
        if (buildingStats == null)
            Debug.Log($"<color=red>Building {name} missing BuildingStats. Assign the script.</color>");

        Level = 0;
        maxHealth = buildingStats.buildingLevelData[Level].buildingHealth;
        armour = buildingStats.buildingLevelData[Level].buildingArmour;
        currentHealth = maxHealth;
        upgradeCosts = buildingStats.buildingLevelData[Level].buildingUpgradeCosts;        
    }
    public void UpgradeBuilding()
    {
        //Upgrade building and updating resource generation values
        Level++;
        maxHealth = buildingStats.buildingLevelData[Level].buildingHealth;
        armour = buildingStats.buildingLevelData[Level].buildingArmour;
        upgradeCosts = buildingStats.buildingLevelData[Level].buildingUpgradeCosts;
    }
    public BuildingUpgradeCost[] GetUpgradeCosts()
    {
        return upgradeCosts;
    }

    public bool canUpgrade()
    {
        return PlayerResourceManager.Instance.HasResources(upgradeCosts);
    }
}
