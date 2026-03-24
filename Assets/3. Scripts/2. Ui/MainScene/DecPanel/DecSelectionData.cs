using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "Dec Selection Data", menuName = "Dec Manager/Dec Selection Data")]
public class DecSelectionData : ScriptableObject
{
    public List<DeckData> AllFactionDecData = new List<DeckData>();

    public FactionName CurrentFaction;
    public DecCategory CurrentCategory;

    [field: SerializeField, Space(30)]
    public FactionDeckData[] allFactionsDeckData { get; private set; }
    public int deckIndex = 0;

    public void AddDeckData(DeckData deckData)
    {
        // if(AllFactionDecData.Count > 0)
        // {
        //     AllFactionDecData.Clear();
        // }

        AllFactionDecData.Add(deckData);
    }

    public List<ScriptableObject> GetCurrentFactionDeckData(FactionName factionName, int deckIndex = 0)
    {
        var deckData = allFactionsDeckData[(int)factionName].decks[deckIndex];

        return deckData.deckCardsSO;
    }

    private void OnValidate()
    {
        var enumValues = ScenarioDataTypes._factionEnumValues;

        if (allFactionsDeckData == null || allFactionsDeckData.Length != enumValues.Length)
        {
            var resized = new FactionDeckData[enumValues.Length];

            if (allFactionsDeckData != null)
            {
                int copyCount = Mathf.Min(allFactionsDeckData.Length, resized.Length);
                for (int i = 0; i < copyCount; i++)
                    resized[i] = allFactionsDeckData[i];
            }

            allFactionsDeckData = resized;
        }

        for (int i = 0; i < enumValues.Length; i++)
        {
            if (allFactionsDeckData[i] == null)
                allFactionsDeckData[i] = new FactionDeckData();

            allFactionsDeckData[i].faction = enumValues[i];

            allFactionsDeckData[i].decks ??= new List<Deck>();

            if (allFactionsDeckData[i].decks.Count == 0)
                allFactionsDeckData[i].decks.Add(new Deck());
        }
    }
}

[Serializable]
public class DeckData
{
    public FactionName FactionType;
    public List<UnitProduceStatsSO> SelectedUnitDeck = new List<UnitProduceStatsSO>();
    public List<DefenseBuildingDataSO> SelectedDefenseDec = new List<DefenseBuildingDataSO>();
    public List<ResourceBuildingDataSO> SelectedResourceDeck = new List<ResourceBuildingDataSO>();
}
