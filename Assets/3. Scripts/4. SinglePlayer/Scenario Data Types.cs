
using System;

public enum FactionName { Medieval = 0, Present = 1, Futuristic = 2, Galvadore = 3 }
public enum ScenarioResourceType { Food = 0, Gold = 1, Metal = 2, Power = 3 }
public enum ScenarioUnitType { Air = 0, Melee = 1, AOERanged = 2, Ranged = 3 }
public enum ScenarioOffenseType { AirBuilding = 0, MeleeBuilding = 1, AOERangedBuilding = 2, RangedBuilding = 3 }
public enum ScenarioDefenseType { AntiAir = 0, AntiTank = 1, Turret = 2, Wall = 3 }

public static class ScenarioDataTypes
{
    public static readonly Array _factionEnumValues = Enum.GetValues(typeof(FactionName));
    public static readonly Array _resourceEnumValues = Enum.GetValues(typeof(ScenarioResourceType));
    public static readonly Array _unitEnumValues = Enum.GetValues(typeof(ScenarioUnitType));
    public static readonly Array _offenseEnumValues = Enum.GetValues(typeof(ScenarioOffenseType));
    public static readonly Array _defenseEnumValues = Enum.GetValues(typeof(ScenarioDefenseType));

}