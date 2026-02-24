using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyPersonality", menuName = "AI/Enemy Personality")]
public class EnemyPersonality : ScriptableObject
{
    public AIPersonalityEnum personalityName;

    [Header("Building Category Weights")]
    [Range(0f, 1f)] public float unitBuildingWeight = 0.5f;
    [Range(0f, 1f)] public float resourceBuildingWeight = 0.3f;
    [Range(0f, 1f)] public float defenseBuildingWeight = 0.2f;
    

    [Header("Thinking")]
    public float spawnInterval = 5f;

    [Header("Strategic Behavior")]
    [Range(0f, 1f)] public float mistakeProbability = 0.1f;

    [Header("Map Knowledge")]
    [Range(0, 1)] public float tacticalDiscipline = 0.8f;
    [Range(0, 1)] public float tacticalPrecision = 0.8f;

    [Header("Limits")]
    public int maxEnemyBuildings = 150;
    
    [Header("Resource Type Weights (Food, Gold, Metal, Power)")]
    public float[] resourceTypeWeights = new float[4] { 0.25f, 0.25f, 0.25f, 0.25f };

    [Header("Deck Selection")]
    
    [Min(1)] public int maxDeckSlots = 8;
    
    public AllUnitData allUnitData;
    public AllBuildingData allBuildingData;

    public List<FactionDeckSelection> factionDeckSelections = new List<FactionDeckSelection>();

    private void OnValidate()
    {
        SyncDeck();
        ValidateDeckLimits();
    }

    void SyncDeck()
    {
        if (allUnitData == null || allBuildingData == null)
            return;

        foreach (FactionName faction in Enum.GetValues(typeof(FactionName)))
        {
            FactionDeckSelection block = factionDeckSelections.Find(f => f.faction == faction);

            if (block == null)
            {
                block = new FactionDeckSelection();
                block.faction = faction;
                factionDeckSelections.Add(block);
            }

            SyncUnits(block);
            SyncDefense(block);
        }
    }

    void SyncUnits(FactionDeckSelection block)
    {
        var factionData = allUnitData.allUnits.Find(u => u.faction == block.faction);
        if (factionData == null) return;

        List<UnitProduceStatsSO> allUnits = new List<UnitProduceStatsSO>();

        if (factionData.airUnits != null) allUnits.AddRange(factionData.airUnits);
        if (factionData.meleeUnits != null) allUnits.AddRange(factionData.meleeUnits);
        if (factionData.rangedUnits != null) allUnits.AddRange(factionData.rangedUnits);
        if (factionData.aoeRangedUnits != null) allUnits.AddRange(factionData.aoeRangedUnits);

        block.unitSelections.RemoveAll(u => !allUnits.Contains(u.unit));

        foreach (var unit in allUnits)
        {
            if (!block.unitSelections.Exists(e => e.unit == unit))
            {
                block.unitSelections.Add(new UnitToggleEntry
                {
                    unit = unit,
                    selected = false,
                    amount = 0,
                    UnitsSpawnLevel = 0,
                    weight = 0.25f
                });
            }
        }
    }

    void SyncDefense(FactionDeckSelection block)
    {
        var factionData = allBuildingData.defenseBuildings.Find(b => b.faction == block.faction);
        if (factionData == null) return;

        List<DefenseBuildingDataSO> allBuildings = new List<DefenseBuildingDataSO>();

        if (factionData.antiAirBuildings != null) allBuildings.AddRange(factionData.antiAirBuildings);
        if (factionData.antiTankBuildings != null) allBuildings.AddRange(factionData.antiTankBuildings);
        if (factionData.turretBuildings != null) allBuildings.AddRange(factionData.turretBuildings);
        if (factionData.wallBuildings != null) allBuildings.AddRange(factionData.wallBuildings);

        block.defenseBuildingSelections.RemoveAll(b => !allBuildings.Contains(b.building));

        foreach (var building in allBuildings)
        {
            if (!block.defenseBuildingSelections.Exists(e => e.building == building))
            {
                block.defenseBuildingSelections.Add(new DefenseBuildingToggleEntry
                {
                    building = building,
                    selected = false,
                    amount = 0,
                    UnitsSpawnLevel = 0,
                    weight = 0.33f
                });
            }
        }
    }

    void ValidateDeckLimits()
    {
        foreach (var faction in factionDeckSelections)
        {
            int total = 0;

            foreach (var unit in faction.unitSelections)
            {
                unit.UnitsSpawnLevel = Mathf.Clamp(unit.UnitsSpawnLevel, 0, 10);
                if (unit.selected) total += unit.amount;
            }

            foreach (var defense in faction.defenseBuildingSelections)
            {
                defense.UnitsSpawnLevel = Mathf.Clamp(defense.UnitsSpawnLevel, 0, 10);
                if (defense.selected) total += defense.amount;
            }

            if (total > maxDeckSlots)
            {
                   ClampDeckAmounts(faction, total);
            }
        }
    }

    void ClampDeckAmounts(FactionDeckSelection faction, int currentTotal)
    {
        int excess = currentTotal - maxDeckSlots;
        
        for (int i = faction.unitSelections.Count - 1; i >= 0 && excess > 0; i--)
        {
            var unit = faction.unitSelections[i];
            if (unit.selected && unit.amount > 0)
            {
                int reduction = Mathf.Min(unit.amount, excess);
                unit.amount -= reduction;
                excess -= reduction;
            }
        }
        
        for (int i = faction.defenseBuildingSelections.Count - 1; i >= 0 && excess > 0; i--)
        {
            var defense = faction.defenseBuildingSelections[i];
            if (defense.selected && defense.amount > 0)
            {
                int reduction = Mathf.Min(defense.amount, excess);
                defense.amount -= reduction;
                excess -= reduction;
            }
        }
    }
}

[Serializable]
public class FactionDeckSelection
{
    public FactionName faction;
    public List<UnitToggleEntry> unitSelections = new List<UnitToggleEntry>();
    public List<DefenseBuildingToggleEntry> defenseBuildingSelections = new List<DefenseBuildingToggleEntry>();
}

[Serializable]
public class UnitToggleEntry
{
    public UnitProduceStatsSO unit;
    public bool selected;
    [Min(0)] public int amount;
    [Min(0)] public int UnitsSpawnLevel;
    [Range(0f, 1f)] public float weight = 0.25f;
}

[Serializable]
public class DefenseBuildingToggleEntry
{
    public DefenseBuildingDataSO building;
    public bool selected;
    [Min(0)] public int amount;
    [Min(0)] public int UnitsSpawnLevel;
    [Range(0f, 1f)] public float weight = 0.33f;
}