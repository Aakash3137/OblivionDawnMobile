using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Unit Data", menuName = "Data/All Unit Data")]
public class AllUnitData : ScriptableObject
{
    public List<Unit> allUnits;

    private void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(FactionName));

        if (allUnits == null)
            allUnits = new List<Unit>();

        while (allUnits.Count < enumValues.Length)
            allUnits.Add(new Unit());

        while (allUnits.Count > enumValues.Length)
            allUnits.RemoveAt(allUnits.Count - 1);

        for (int i = 0; i < enumValues.Length; i++)
        {
            allUnits[i].faction = (FactionName)enumValues.GetValue(i);
        }
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
}