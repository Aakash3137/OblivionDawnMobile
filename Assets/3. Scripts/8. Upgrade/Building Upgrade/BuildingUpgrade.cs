
public class BuildingUpgrade
{
    public int maxLevel = 20;
    public void UpgradeNext(BuildingDataSO building, DecManager _Dec = null)
    {
        int next = -1;

        if (building is OffenseBuildingDataSO offenseBuilding)
        {
            next = UpgradeOffenseBuildingStats(offenseBuilding);
        }
        else if (building is MainBuildingDataSO mainBuilding)
        {
            next = UpgradeMainBuildingStats(mainBuilding);
        }
        else if (building is DefenseBuildingDataSO defenseBuilding)
        {
            next = UpgradeDefenseBuildingStats(defenseBuilding);
        }
        else if (building is ResourceBuildingDataSO resourceBuilding)
        {
            next = UpgradeResourceBuildingStats(resourceBuilding);
        }


        // if (_Dec != null && next >= 0)
        //     _Dec.diamondtext.text = (_Dec._Profile.Diamonds -= StatUpgrade.UpgradeCost(next)).ToString();
    }

    private int UpgradeOffenseBuildingStats(OffenseBuildingDataSO building)
    {
        int next = building.offenseBuildingUpgradeData.Count;

        if (next > maxLevel)
            return -1;

        var last = building.offenseBuildingUpgradeData[next - 1];

        var cur = new OffenseBuildingUpgradeData();

        cur.buildingLevel = next;

        // Upgrading stats from StatUpgrades
        cur.buildingBasicStats.maxHealth = StatUpgrade.MaxHealth(last.buildingBasicStats.maxHealth, cur.buildingLevel, maxLevel);
        cur.buildingBasicStats.armor = StatUpgrade.Armour(last.buildingBasicStats.armor, cur.buildingLevel, maxLevel);
        cur.buildingBuildTime = StatUpgrade.BuildTime(last.buildingBuildTime, cur.buildingLevel, maxLevel);

        cur.unitSpawnTime = StatUpgrade.BuildTime(last.unitSpawnTime, cur.buildingLevel, maxLevel);

        building.offenseBuildingUpgradeData.Add(cur);

        building.offenseBuildingUpgradeData[next] = cur;

        building.buildingIdentity.spawnLevel = next;

        return next;
    }
    private int UpgradeDefenseBuildingStats(DefenseBuildingDataSO building)
    {
        int next = building.defenseBuildingUpgradeData.Count;

        if (next > maxLevel)
            return -1;

        var last = building.defenseBuildingUpgradeData[next - 1];

        var cur = new DefenseBuildingUpgradeData();

        cur.buildingLevel = next;

        // Upgrading stats from StatUpgrades
        cur.buildingBasicStats.maxHealth = StatUpgrade.MaxHealth(last.buildingBasicStats.maxHealth, cur.buildingLevel, maxLevel);
        cur.buildingBasicStats.armor = StatUpgrade.Armour(last.buildingBasicStats.armor, cur.buildingLevel, maxLevel);
        cur.buildingBuildTime = StatUpgrade.BuildTime(last.buildingBuildTime, cur.buildingLevel, maxLevel);

        cur.defenseAttackStats.damage = StatUpgrade.Damage(last.defenseAttackStats.damage, cur.buildingLevel, maxLevel);
        cur.defenseAttackStats.fireRate = StatUpgrade.Damage(last.defenseAttackStats.fireRate, cur.buildingLevel, maxLevel);
        cur.defenseRangeStats.attackRange = StatUpgrade.AttackRange(last.defenseRangeStats.attackRange, cur.buildingLevel, maxLevel);


        building.defenseBuildingUpgradeData.Add(cur);

        building.defenseBuildingUpgradeData[next] = cur;

        building.buildingIdentity.spawnLevel = next;

        return next;
    }
    private int UpgradeResourceBuildingStats(ResourceBuildingDataSO building)
    {
        int next = building.resourceBuildingUpgradeData.Count;

        if (next > maxLevel)
            return -1;

        var last = building.resourceBuildingUpgradeData[next - 1];

        var cur = new ResourceBuildingUpgradeData();

        cur.buildingLevel = next;

        // Upgrading stats from StatUpgrades
        cur.buildingBasicStats.maxHealth = StatUpgrade.MaxHealth(last.buildingBasicStats.maxHealth, cur.buildingLevel, maxLevel);
        cur.buildingBasicStats.armor = StatUpgrade.Armour(last.buildingBasicStats.armor, cur.buildingLevel, maxLevel);
        cur.buildingBuildTime = StatUpgrade.BuildTime(last.buildingBuildTime, cur.buildingLevel, maxLevel);

        cur.resourceAmountPerBatch = StatUpgrade.Resource(last.resourceAmountPerBatch, cur.buildingLevel, maxLevel);
        // cur.resourceTimeToProduce = StatUpgrade.BuildTime(last.resourceTimeToProduce, cur.buildingLevel, maxLevel);
        cur.resourceAmountCapacity = StatUpgrade.Resource(last.resourceAmountCapacity, cur.buildingLevel, maxLevel);


        building.resourceBuildingUpgradeData.Add(cur);

        building.resourceBuildingUpgradeData[next] = cur;

        building.buildingIdentity.spawnLevel = next;

        return next;
    }

    private int UpgradeMainBuildingStats(MainBuildingDataSO building)
    {
        int next = building.mainBuildingUpgradeData.Count;

        if (next > maxLevel)
            return -1;

        var last = building.mainBuildingUpgradeData[next - 1];

        var cur = new MainBuildingUpgradeData();

        cur.buildingLevel = next;

        // Upgrading stats from StatUpgrades
        cur.buildingBasicStats.maxHealth = StatUpgrade.MaxHealth(last.buildingBasicStats.maxHealth, cur.buildingLevel, maxLevel);
        cur.buildingBasicStats.armor = StatUpgrade.Armour(last.buildingBasicStats.armor, cur.buildingLevel, maxLevel);
        cur.buildingBuildTime = StatUpgrade.BuildTime(last.buildingBuildTime, cur.buildingLevel, maxLevel);

        for (int i = 0; i < cur.starterResources.Length; i++)
            cur.starterResources[i].resourceCost = StatUpgrade.Resource(last.starterResources[i].resourceCost, cur.buildingLevel, maxLevel);

        building.mainBuildingUpgradeData.Add(cur);

        building.mainBuildingUpgradeData[next] = cur;

        building.buildingIdentity.spawnLevel = next;

        return next;
    }
}
