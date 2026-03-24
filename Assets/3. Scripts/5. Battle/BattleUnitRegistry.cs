using System.Collections.Generic;
using System.Linq;

public static class BattleUnitRegistry
{
    public static List<UnitStats> Units = new();
    public static List<DefenseBuildingStats> DefenseUnits = new();
    
    public static List<UnitStats> PlayerUnits =>
        Units.Where(u => u.side == Side.Player).ToList();
    
    public static List<UnitStats> EnemyUnits =>
        Units.Where(u => u.side == Side.Enemy).ToList();
}