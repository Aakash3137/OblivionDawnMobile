
using System;

public enum FactionName { Medieval = 0, Present = 1, Futuristic = 2, Galvadore = 3 }
public enum ScenarioResourceType { Food = 0, Gold = 1, Metal = 2, Power = 3 }
public enum ScenarioUnitType { Air = 3, Melee = 0, AOERanged = 2, Ranged = 1 }
public enum ScenarioOffenseType { AirBuilding = 3, MeleeBuilding = 0, AOERangedBuilding = 2, RangedBuilding = 1 }
public enum ScenarioDefenseType { AntiAir = 3, AntiTank = 2, Turret = 1, Wall = 0 }

public static class ScenarioDataTypes
{
    public static readonly FactionName[] _factionEnumValues = (FactionName[])Enum.GetValues(typeof(FactionName));
    public static readonly ScenarioResourceType[] _resourceEnumValues = (ScenarioResourceType[])Enum.GetValues(typeof(ScenarioResourceType));
    public static readonly ScenarioUnitType[] _unitEnumValues = (ScenarioUnitType[])Enum.GetValues(typeof(ScenarioUnitType));
    public static readonly ScenarioOffenseType[] _offenseEnumValues = (ScenarioOffenseType[])Enum.GetValues(typeof(ScenarioOffenseType));
    public static readonly ScenarioDefenseType[] _defenseEnumValues = (ScenarioDefenseType[])Enum.GetValues(typeof(ScenarioDefenseType));

}