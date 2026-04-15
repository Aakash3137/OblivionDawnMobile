using System.Collections.Generic;

public static class GameplayRegistry
{
    public static Dictionary<Side, List<UnitStats>> UnitsDictionary = new();
    public static Dictionary<Side, List<OffenseBuildingStats>> OffenseDictionary = new();
    public static Dictionary<Side, List<DefenseBuildingStats>> DefenseDictionary = new();
    public static Dictionary<Side, List<ResourceBuildingStats>> ResourceDictionary = new();
    static GameplayRegistry()
    {
        var enumValues = System.Enum.GetValues(typeof(Side));

        foreach (Side side in enumValues)
        {
            UnitsDictionary[side] = new();
            OffenseDictionary[side] = new();
            DefenseDictionary[side] = new();
            ResourceDictionary[side] = new();
        }
    }
}