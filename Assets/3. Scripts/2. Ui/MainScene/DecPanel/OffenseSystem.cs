using System.Collections.Generic;
using UnityEngine;

public static class OffenseSystem
{
    private static readonly Dictionary<FactionName, List<UnitProduceStatsSO>> _offenseUnits
        = new Dictionary<FactionName, List<UnitProduceStatsSO>>();

    // Call this ONCE during game boot / data init
    public static void RegisterFactionUnits(
        FactionName faction,
        List<UnitProduceStatsSO> units)
    {
        if (units == null) return;

        if (_offenseUnits.ContainsKey(faction))
            _offenseUnits[faction] = units;
        else
            _offenseUnits.Add(faction, units);
    }

    // Used by DecManager
    public static List<UnitProduceStatsSO> GetUnits(FactionName faction)
    {
        if (_offenseUnits.TryGetValue(faction, out var list))
            return list;

        return new List<UnitProduceStatsSO>();
    }
}