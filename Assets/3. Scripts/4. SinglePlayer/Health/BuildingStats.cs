using UnityEngine;

public class BuildingStats : Stats
{
    public BuildingUpgradeDataSO buildingStats;

    private void Start()
    {
        Level = 0;
        maxHealth = buildingStats.buildingLevelData[Level].buildingHealth;
        armour = buildingStats.buildingLevelData[Level].buildingArmour;
        currentHealth = maxHealth;
    }
    public void UpgradeBuilding()
    {
        //Upgrade building and updating resource generation values
        Level++;
        maxHealth = buildingStats.buildingLevelData[Level].buildingHealth;
        armour = buildingStats.buildingLevelData[Level].buildingArmour;
    }
}
