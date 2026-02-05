using UnityEngine;

public class KillCounterManager : MonoBehaviour
{
    public static KillCounterManager Instance;

    public KillData playerKills;
    public KillData enemyKills;

    [Header("Total Kills")]
    public int playerTotalBuildingKills;
    public int enemyTotalBuildingKills;

    public int playerTotalUnitKills;
    public int enemyTotalUnitKills;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ResetKills()
    {
        playerKills = new KillData();
        enemyKills = new KillData();
    }

    public void AddUnitKillData(ScenarioUnitType offenseType, Side side)
    {
        switch (side)
        {
            case Side.Player:
                switch (offenseType)
                {
                    case ScenarioUnitType.Air:
                        enemyKills.unitKills.airUnitKills++;
                        break;
                    case ScenarioUnitType.Melee:
                        enemyKills.unitKills.infantryUnitKills++;
                        break;
                    case ScenarioUnitType.AOERanged:
                        enemyKills.unitKills.meleeUnitKills++;
                        break;
                    case ScenarioUnitType.Ranged:
                        enemyKills.unitKills.tankUnitKills++;
                        break;
                }
                enemyTotalUnitKills++;
                break;
            case Side.Enemy:
                switch (offenseType)
                {
                    case ScenarioUnitType.Air:
                        playerKills.unitKills.airUnitKills++;
                        break;
                    case ScenarioUnitType.Melee:
                        playerKills.unitKills.infantryUnitKills++;
                        break;
                    case ScenarioUnitType.AOERanged:
                        playerKills.unitKills.meleeUnitKills++;
                        break;
                    case ScenarioUnitType.Ranged:
                        playerKills.unitKills.tankUnitKills++;
                        break;
                }
                playerTotalUnitKills++;
                break;
        }
    }

    public void AddBuildingDestroyedData(ScenarioBuildingType buildingType, Side side)
    {
        switch (side)
        {
            case Side.Player:
                switch (buildingType)
                {
                    case ScenarioBuildingType.MainBuilding:
                        enemyKills.buildingDestroyed.mainBuildingDestroyed++;
                        break;
                    case ScenarioBuildingType.DefenseBuilding:
                        enemyKills.buildingDestroyed.defenseBuildingDestroyed++;
                        break;
                    case ScenarioBuildingType.OffenseBuilding:
                        enemyKills.buildingDestroyed.offenseBuildingDestroyed++;
                        break;
                    case ScenarioBuildingType.ResourceBuilding:
                        enemyKills.buildingDestroyed.resourceBuildingDestroyed++;
                        break;
                }
                enemyTotalBuildingKills++;
                break;
            case Side.Enemy:
                switch (buildingType)
                {
                    case ScenarioBuildingType.MainBuilding:
                        playerKills.buildingDestroyed.mainBuildingDestroyed++;
                        break;
                    case ScenarioBuildingType.DefenseBuilding:
                        playerKills.buildingDestroyed.defenseBuildingDestroyed++;
                        break;
                    case ScenarioBuildingType.OffenseBuilding:
                        playerKills.buildingDestroyed.offenseBuildingDestroyed++;
                        break;
                    case ScenarioBuildingType.ResourceBuilding:
                        playerKills.buildingDestroyed.resourceBuildingDestroyed++;
                        break;
                }
                playerTotalBuildingKills++;
                break;
        }
    }

    public void AddDefenseBuildingDestroyedData(ScenarioDefenseType defenseType, Side side)
    {
        switch (side)
        {
            case Side.Player:
                switch (defenseType)
                {
                    case ScenarioDefenseType.AntiAir:
                        enemyKills.defenseBuildingDestroyed.antiAirDestroyed++;
                        break;
                    case ScenarioDefenseType.AntiTank:
                        enemyKills.defenseBuildingDestroyed.antiTankDestroyed++;
                        break;
                    case ScenarioDefenseType.Turret:
                        enemyKills.defenseBuildingDestroyed.turretDestroyed++;
                        break;
                    case ScenarioDefenseType.Wall:
                        enemyKills.defenseBuildingDestroyed.wallDestroyed++;
                        break;
                }
                break;
            case Side.Enemy:
                switch (defenseType)
                {
                    case ScenarioDefenseType.AntiAir:
                        playerKills.defenseBuildingDestroyed.antiAirDestroyed++;
                        break;
                    case ScenarioDefenseType.AntiTank:
                        playerKills.defenseBuildingDestroyed.antiTankDestroyed++;
                        break;
                    case ScenarioDefenseType.Turret:
                        playerKills.defenseBuildingDestroyed.turretDestroyed++;
                        break;
                    case ScenarioDefenseType.Wall:
                        playerKills.defenseBuildingDestroyed.wallDestroyed++;
                        break;
                }

                break;
        }
    }

    public void AddResourceBuildingDestroyedData(ScenarioResourceType resourceType, Side side)
    {
        switch (side)
        {
            case Side.Player:
                switch (resourceType)
                {
                    case ScenarioResourceType.Food:
                        enemyKills.resourceBuildingDestroyed.foodBuildingDestroyed++;
                        break;
                    case ScenarioResourceType.Gold:
                        enemyKills.resourceBuildingDestroyed.goldBuildingDestroyed++;
                        break;
                    case ScenarioResourceType.Metal:
                        enemyKills.resourceBuildingDestroyed.metalBuildingDestroyed++;
                        break;
                    case ScenarioResourceType.Power:
                        enemyKills.resourceBuildingDestroyed.powerBuildingDestroyed++;
                        break;
                }
                break;
            case Side.Enemy:
                switch (resourceType)
                {
                    case ScenarioResourceType.Food:
                        playerKills.resourceBuildingDestroyed.foodBuildingDestroyed++;
                        break;
                    case ScenarioResourceType.Gold:
                        playerKills.resourceBuildingDestroyed.goldBuildingDestroyed++;
                        break;
                    case ScenarioResourceType.Metal:
                        playerKills.resourceBuildingDestroyed.metalBuildingDestroyed++;
                        break;
                    case ScenarioResourceType.Power:
                        playerKills.resourceBuildingDestroyed.powerBuildingDestroyed++;
                        break;
                }
                break;
        }
    }
    public void AddOffenseBuildingDestroyedData(ScenarioOffenseType offenseType, Side side)
    {
        switch (side)
        {
            case Side.Player:
                switch (offenseType)
                {
                    case ScenarioOffenseType.AirBuilding:
                        enemyKills.offenseBuildingDestroyed.airBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.MeleeBuilding:
                        enemyKills.offenseBuildingDestroyed.infantryBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.AOERangedBuilding:
                        enemyKills.offenseBuildingDestroyed.meleeBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.RangedBuilding:
                        enemyKills.offenseBuildingDestroyed.tankBuildingDestroyed++;
                        break;
                }
                break;
            case Side.Enemy:
                switch (offenseType)
                {
                    case ScenarioOffenseType.AirBuilding:
                        playerKills.offenseBuildingDestroyed.airBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.MeleeBuilding:
                        playerKills.offenseBuildingDestroyed.infantryBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.AOERangedBuilding:
                        playerKills.offenseBuildingDestroyed.meleeBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.RangedBuilding:
                        playerKills.offenseBuildingDestroyed.tankBuildingDestroyed++;
                        break;
                }
                break;
        }
    }
}

[System.Serializable]
public class KillData
{
    public UnitKillData unitKills;
    public BuildingDestroyedData buildingDestroyed;
    public OffenseBuildingDestroyedData offenseBuildingDestroyed;
    public DefenseBuildingDestroyedData defenseBuildingDestroyed;
    public ResourceBuildingDestroyedData resourceBuildingDestroyed;
}

[System.Serializable]
public struct UnitKillData
{
    public int infantryUnitKills;
    public int airUnitKills;
    public int tankUnitKills;
    public int meleeUnitKills;
}

[System.Serializable]
public struct BuildingDestroyedData
{
    public int mainBuildingDestroyed;
    public int offenseBuildingDestroyed;
    public int defenseBuildingDestroyed;
    public int resourceBuildingDestroyed;
}

[System.Serializable]
public struct OffenseBuildingDestroyedData
{
    public int infantryBuildingDestroyed;
    public int airBuildingDestroyed;
    public int tankBuildingDestroyed;
    public int meleeBuildingDestroyed;
}

[System.Serializable]
public struct DefenseBuildingDestroyedData
{
    public int antiTankDestroyed;
    public int antiAirDestroyed;
    public int turretDestroyed;
    public int wallDestroyed;
}

[System.Serializable]
public struct ResourceBuildingDestroyedData
{
    public int foodBuildingDestroyed;
    public int goldBuildingDestroyed;
    public int metalBuildingDestroyed;
    public int powerBuildingDestroyed;
}
