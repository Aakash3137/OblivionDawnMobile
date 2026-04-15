using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "All Unit Data", menuName = "Data/All Unit Data")]
public class AllUnitData : ScriptableObject
{
    public List<UnitProduceStatsSO> allUnitsSO;

    public List<Unit> allUnits { get; internal set; }

    private void Awake()
    {
        Populate();
        SortUnitData();
    }
    private void OnValidate()
    {
        //  need this for editor mode Upgrade Data Editor Window
        Populate();
        SortUnitData();
    }

    public void Populate()
    {
        // Clear Previous Data and repopulate
        var enumValues = ScenarioDataTypes._factionEnumValues;

        allUnits = new List<Unit>();

        foreach (FactionName faction in enumValues)
        {
            allUnits.Add(new Unit { faction = faction });
        }

        AddUnitsDataSO();
    }

    private void SortUnitData()
    {
        allUnitsSO.Sort(CompareUnitSO);
    }

    private static int CompareUnitSO(UnitProduceStatsSO a, UnitProduceStatsSO b)
    {
        int order = a.unitIdentity.faction.CompareTo(b.unitIdentity.faction);

        // if(both are same then compare by UnitType)
        if (order == 0)
            order = a.unitType.CompareTo(b.unitType);

        // if(both are same then compare by scriptable Object name)
        if (order == 0)
            order = a.name.CompareTo(b.name);

        return order;
    }

    private void AddUnitsDataSO()
    {
        foreach (var unitSO in allUnitsSO)
        {
            var faction = unitSO.unitIdentity.faction;

            switch (unitSO.unitType)
            {
                case ScenarioUnitType.Air:
                    allUnits[(int)faction].airUnits.Add(unitSO);
                    break;
                case ScenarioUnitType.AOERanged:
                    allUnits[(int)faction].aoeRangedUnits.Add(unitSO);
                    break;
                case ScenarioUnitType.Melee:
                    allUnits[(int)faction].meleeUnits.Add(unitSO);
                    break;
                case ScenarioUnitType.Ranged:
                    allUnits[(int)faction].rangedUnits.Add(unitSO);
                    break;
            }
        }
    }

    public List<UnitProduceStatsSO> GetFactionUnitsSO(FactionName faction)
    {
        var unitScriptables = new List<UnitProduceStatsSO>();

        foreach (var unitSO in allUnitsSO)
        {
            if (unitSO.unitIdentity.faction == faction)
            {
                unitScriptables.Add(unitSO);
            }
        }

        return unitScriptables;
    }

}

[Serializable]
public class Unit
{
    public FactionName faction;
    public List<UnitProduceStatsSO> meleeUnits;
    public List<UnitProduceStatsSO> rangedUnits;
    public List<UnitProduceStatsSO> aoeRangedUnits;
    public List<UnitProduceStatsSO> airUnits;

    public Unit()
    {
        airUnits = new();
        meleeUnits = new();
        aoeRangedUnits = new();
        rangedUnits = new();
    }
}