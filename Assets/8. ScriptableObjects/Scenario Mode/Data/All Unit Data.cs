using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Unit Data", menuName = "Data/All Unit Data")]
public class AllUnitData : ScriptableObject
{
    [SerializeField] private List<UnitProduceStatsSO> _allUnitsSO;

    private ScriptableRegistry<UnitProduceStatsSO, FactionName, ScenarioUnitType> _unitsSO =
        new(so => so.unitIdentity.faction, so => so.unitType, ScenarioDataTypes._unitEnumValues);

    public List<UnitProduceStatsSO> AllUnitsSO => _unitsSO.All;
    public List<UnitProduceStatsSO> GetUnitsSO(FactionName faction) => _unitsSO.ByFaction(faction);
    public List<UnitProduceStatsSO> GetUnitsSO(ScenarioUnitType type) => _unitsSO.ByType(type);
    public List<UnitProduceStatsSO> GetUnitsSO(FactionName faction, ScenarioUnitType type) => _unitsSO.ByFactionAndType(faction, type);

    private void Awake()
    {
        Populate();
    }
    private void OnValidate()
    {
        //  need this for editor mode Upgrade Data Editor Window
        Populate();
    }

    public void Populate()
    {
        _allUnitsSO.Sort(CompareUnitSO);

        foreach (var unitSO in _allUnitsSO)
        {
            _unitsSO.Register(unitSO);
        }
    }

    private static int CompareUnitSO(UnitProduceStatsSO a, UnitProduceStatsSO b)
    {
        int order = a.unitIdentity.faction.CompareTo(b.unitIdentity.faction);

        // if(both are same then compare by UnitType)
        if (order == 0)
            order = a.unitType.CompareTo(b.unitType);

        // if(both are same then compare by scriptable Object name so that first added is first in the list)
        if (order == 0)
            order = a.name.CompareTo(b.name);

        return order;
    }

}