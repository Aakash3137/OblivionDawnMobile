using System;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public enum PlayStyle
{
    Aggressive = 0,
    Defensive = 1,
    Balanced = 2
}

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

    [Header("Wall Strategy")]
    public bool useSmartWallStrategy = false;
    public Difficulty difficulty = Difficulty.Medium;
    public PlayStyle playStyle = PlayStyle.Balanced;

    [Header("Limits")]
    public bool reduceSpawningTime = false;
    public int maxOffenseBuilding = 5;
    public float reduceSpawnTime = 10f;

    public int maxEnemyBuildings = 150;

    [Header("Economy")]
    public bool balancedResourceStart = true;
    public int balancedResourceTarget = 3;

    [Header("Resource Type Weights (Food, Gold, Metal, Power)")]
    public float[] resourceTypeWeights = new float[4] { 0.25f, 0.25f, 0.25f, 0.25f };

    [Header("Deck Selection")]

    [Min(1)] public int maxDeckSlots = 8;

    public AllUnitData allUnitData;
    public AllBuildingData allBuildingData;

    public List<FactionDeckSelection> factionDeckSelections = new List<FactionDeckSelection>();

    private bool _isValidating;

    private void OnValidate()
    {
        if (_isValidating) return;
        _isValidating = true;

        try
        {
            if (!Application.isPlaying)
            {
                ValidateDeckLimits(); 
            }
        }
        finally
        {
            _isValidating = false;
        }
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
        List<UnitProduceStatsSO> allUnits = allUnitData.GetUnitsSO(block.faction);

       // block.unitSelections.RemoveAll(u => !allUnits.Contains(u.unit));

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
        List<DefenseBuildingDataSO> allBuildings = allBuildingData.GetDefenseBuildingsSO(block.faction);

       // block.defenseBuildingSelections.RemoveAll  (b => !allBuildings.Contains(b.building));
      
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
    
#if UNITY_EDITOR
    [ContextMenu("Rebuild Deck (Safe)")]
    public void RebuildDeck()
    {
        SyncDeck();
        ValidateDeckLimits();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
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