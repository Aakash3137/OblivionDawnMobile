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

    public void AddUnitKillData(ScenarioOffenseType offenseType, Side side)
    {
        switch (side)
        {
            case Side.Player:
                switch (offenseType)
                {
                    case ScenarioOffenseType.Air:
                        enemyKills.unitKills.airUnitKills++;
                        break;
                    case ScenarioOffenseType.Infantry:
                        enemyKills.unitKills.infantryUnitKills++;
                        break;
                    case ScenarioOffenseType.Melee:
                        enemyKills.unitKills.meleeUnitKills++;
                        break;
                    case ScenarioOffenseType.Tank:
                        enemyKills.unitKills.tankUnitKills++;
                        break;
                }
                enemyTotalUnitKills++;
                break;
            case Side.Enemy:
                switch (offenseType)
                {
                    case ScenarioOffenseType.Air:
                        playerKills.unitKills.airUnitKills++;
                        break;
                    case ScenarioOffenseType.Infantry:
                        playerKills.unitKills.infantryUnitKills++;
                        break;
                    case ScenarioOffenseType.Melee:
                        playerKills.unitKills.meleeUnitKills++;
                        break;
                    case ScenarioOffenseType.Tank:
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
                    case ScenarioOffenseType.Air:
                        enemyKills.offenseBuildingDestroyed.airBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.Infantry:
                        enemyKills.offenseBuildingDestroyed.infantryBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.Melee:
                        enemyKills.offenseBuildingDestroyed.meleeBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.Tank:
                        enemyKills.offenseBuildingDestroyed.tankBuildingDestroyed++;
                        break;
                }
                break;
            case Side.Enemy:
                switch (offenseType)
                {
                    case ScenarioOffenseType.Air:
                        playerKills.offenseBuildingDestroyed.airBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.Infantry:
                        playerKills.offenseBuildingDestroyed.infantryBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.Melee:
                        playerKills.offenseBuildingDestroyed.meleeBuildingDestroyed++;
                        break;
                    case ScenarioOffenseType.Tank:
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
