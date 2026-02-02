using System.Collections.Generic;
using UnityEngine;

public static class DefenseSystem
{
    private static readonly Dictionary<FactionName, List<DefenseBuildingDataSO>> _defenseBuildings
        = new Dictionary<FactionName, List<DefenseBuildingDataSO>>();

    // Call this ONCE during game boot / data init
    public static void RegisterFactionBuildings(
        FactionName faction,
        List<DefenseBuildingDataSO> buildings)
    {
        if (buildings == null) return;

        if (_defenseBuildings.ContainsKey(faction))
            _defenseBuildings[faction] = buildings;
        else
            _defenseBuildings.Add(faction, buildings);
    }

    // Used by DecManager
    public static List<DefenseBuildingDataSO> GetBuildings(FactionName faction)
    {
        if (_defenseBuildings.TryGetValue(faction, out var list))
            return list;

        return new List<DefenseBuildingDataSO>();
    }
}
