public enum AbilityTargetType
{
    Self,
    Area,
    UnitName,
    UnitClass,
    All  // Targets all units of the caster's side
}
/*
Some abilities affect:
 
Self
Single unit
Area
Unit class (melee, ranged, etc.)
All - All units on player or enemy side

TargetType = UnitClass
UnitClass = Melee
*/