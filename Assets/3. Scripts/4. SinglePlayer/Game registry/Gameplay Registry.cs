using System.Collections.Generic;

public static class GameplayRegistry
{
    // One registry per entity type
    private static EntityRegistry<UnitStats, Side, ScenarioUnitType> _units =
        new(s => s.side, s => s.unitType, ScenarioDataTypes._unitEnumValues);

    private static EntityRegistry<OffenseBuildingStats, Side, ScenarioOffenseType> _offense =
        new(s => s.side, s => s.offenseType, ScenarioDataTypes._offenseEnumValues);

    private static EntityRegistry<DefenseBuildingStats, Side, ScenarioDefenseType> _defense =
        new(s => s.side, s => s.defenseType, ScenarioDataTypes._defenseEnumValues);

    private static EntityRegistry<ResourceBuildingStats, Side, ScenarioResourceType> _resource =
        new(s => s.side, s => s.resourceType, ScenarioDataTypes._resourceEnumValues);

    // Public surface — same shape as before
    public static List<UnitStats> AllUnits => _units.All;
    public static List<OffenseBuildingStats> AllOffenseBuildings => _offense.All;
    public static List<DefenseBuildingStats> AllDefenseBuildings => _defense.All;
    public static List<ResourceBuildingStats> AllResourceBuildings => _resource.All;

    public static List<UnitStats> GetUnits(Side side) => _units.BySide(side);
    public static List<UnitStats> GetUnits(Side side, ScenarioUnitType type) => _units.BySideAndType(side, type);
    public static List<UnitStats> GetUnits(ScenarioUnitType type) => _units.ByType(type);

    public static List<OffenseBuildingStats> GetOffense(Side side) => _offense.BySide(side);
    public static List<OffenseBuildingStats> GetOffense(Side side, ScenarioOffenseType type) => _offense.BySideAndType(side, type);
    public static List<OffenseBuildingStats> GetOffense(ScenarioOffenseType type) => _offense.ByType(type);

    public static List<DefenseBuildingStats> GetDefense(Side side) => _defense.BySide(side);
    public static List<DefenseBuildingStats> GetDefense(Side side, ScenarioDefenseType type) => _defense.BySideAndType(side, type);
    public static List<DefenseBuildingStats> GetDefense(ScenarioDefenseType type) => _defense.ByType(type);

    public static List<ResourceBuildingStats> GetResource(Side side) => _resource.BySide(side);
    public static List<ResourceBuildingStats> GetResource(Side side, ScenarioResourceType type) => _resource.BySideAndType(side, type);
    public static List<ResourceBuildingStats> GetResource(ScenarioResourceType type) => _resource.ByType(type);

    // Register / Unregister
    public static void Register(UnitStats unitStats) => _units.Register(unitStats);
    public static void Unregister(UnitStats unitStats) => _units.Unregister(unitStats);

    public static void Register(BuildingStats buildingStats)
    {
        switch (buildingStats)
        {
            case OffenseBuildingStats s: _offense.Register(s); break;
            case DefenseBuildingStats s: _defense.Register(s); break;
            case ResourceBuildingStats s: _resource.Register(s); break;
        }
    }

    public static void Unregister(BuildingStats buildingStats)
    {
        switch (buildingStats)
        {
            case OffenseBuildingStats s: _offense.Unregister(s); break;
            case DefenseBuildingStats s: _defense.Unregister(s); break;
            case ResourceBuildingStats s: _resource.Unregister(s); break;
        }
    }
}