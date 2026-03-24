using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dec Selection Data", menuName = "Dec Manager/Dec Selection Data")]
public class DecSelectionData : ScriptableObject
{
    [field: SerializeField, Space(10)]
    public FactionDeckData[] allFactionsDeckData { get; private set; }
    public int deckIndex = 0;

    private List<ScriptableObject> GetAllCardsInDeck(FactionName faction) => allFactionsDeckData[(int)faction].decks[deckIndex].deckCardsSO;

    public List<UnitProduceStatsSO> GetUnitsSOInDeck(FactionName faction)
    {
        var allCardsSO = GetAllCardsInDeck(faction);
        var unitCardSO = new List<UnitProduceStatsSO>();

        foreach (var card in allCardsSO)
        {
            if (card is UnitProduceStatsSO unitSO)
                unitCardSO.Add(unitSO);
        }

        return unitCardSO;
    }

    public List<DefenseBuildingDataSO> GetDefensesSOInDeck(FactionName faction)
    {
        var allCardsSO = GetAllCardsInDeck(faction);
        var defenseCardSO = new List<DefenseBuildingDataSO>();

        foreach (var card in allCardsSO)
        {
            if (card is DefenseBuildingDataSO defenseSO)
                defenseCardSO.Add(defenseSO);
        }

        return defenseCardSO;
    }

    private void OnValidate()
    {
        ValidateBase();
    }

    private void ValidateBase()
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
