using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "All Unit Data", menuName = "Data/All Unit Data")]
public class AllUnitData : ScriptableObject
{
    public List<UnitProduceStatsSO> allUnitsSO;

    [field: SerializeField] public List<Unit> allUnits { get; internal set; }

    private void OnValidate()
    {
        Populate();
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
    public List<UnitProduceStatsSO> airUnits;
    public List<UnitProduceStatsSO> meleeUnits;
    public List<UnitProduceStatsSO> aoeRangedUnits;
    public List<UnitProduceStatsSO> rangedUnits;

    public Unit()
    {
        airUnits = new List<UnitProduceStatsSO>();
        meleeUnits = new List<UnitProduceStatsSO>();
        aoeRangedUnits = new List<UnitProduceStatsSO>();
        rangedUnits = new List<UnitProduceStatsSO>();
    }
}