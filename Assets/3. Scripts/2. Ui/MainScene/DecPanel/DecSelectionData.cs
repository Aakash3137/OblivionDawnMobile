using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "Dec Selection Data", menuName = "Dec Manager/Dec Selection Data")]
public class DecSelectionData : ScriptableObject
{
    public List<DeckData> AllFactionDecData = new List<DeckData>();
    public List<DeckData> AllFactionDecDatatem = new List<DeckData>();

    public FactionName CurrentFaction;

    public void AddDeckData(DeckData deckData)
    {
        if(AllFactionDecData.Count > 0)
        {
            AllFactionDecData.Clear();
        }

        AllFactionDecData.Add(deckData);
    }
}

[System.Serializable]
public class DeckData
{
    public FactionName FactionType;
    public List<UnitProduceStatsSO> SelectedUnitDeck = new List<UnitProduceStatsSO>();
    public List<DefenseBuildingDataSO> SelectedDefenseDec= new List<DefenseBuildingDataSO>();
    public List<ResourceBuildingDataSO> SelectedResourceDeck = new List<ResourceBuildingDataSO>();
}

[System.Serializable]
public class TempDeckData
{
    public FactionName FactionType;
    public List<UnitProduceStatsSO> SelectedUnitDeck = new List<UnitProduceStatsSO>();
    public List<DefenseBuildingDataSO> SelectedDefenseDec= new List<DefenseBuildingDataSO>();
    public List<ResourceBuildingDataSO> SelectedResourceDeck = new List<ResourceBuildingDataSO>();
}
