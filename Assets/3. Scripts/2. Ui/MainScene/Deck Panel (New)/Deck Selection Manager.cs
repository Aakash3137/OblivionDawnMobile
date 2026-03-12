using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DeckSelectionManager : MonoBehaviour
{
    public static DeckSelectionManager Instance { get; private set; }

    [Space(10)]
    [SerializeField] private SelectedCard selectedCardPrefab;
    [SerializeField] private Transform selectedCardsContainer;
    [SerializeField] private int maxCardCount;

    [Space(10)]
    [ReadOnly][SerializeField] private FactionDeckData[] allFactionsDeckData;
    [ReadOnly][SerializeField] private List<SelectedCard> selectedCards;

    [Space(10)]
    [Header("UI References")]
    [SerializeField] private TMP_Text populationText;

    [HideInInspector] public FactionName selectedFaction;

    private int maxEquipCount;
    private int currentDeckIndex;
    private int maxPopulation;
    private int currentPopulation;

    private Deck currentDeck => allFactionsDeckData[(int)selectedFaction].decks[currentDeckIndex];

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        GenerateCards();
    }

    public void GenerateCards()
    {
        selectedCards = new List<SelectedCard>(maxCardCount);

        for (int i = 0; i < maxCardCount; i++)
        {
            var card = Instantiate(selectedCardPrefab, selectedCardsContainer);
            selectedCards.Add(card);
        }
    }

    public void SetVariables()
    {
        var cityCenterSO = DeckPanelManager.Instance.cityCenterScriptables[(int)selectedFaction];
        var cityCenterData = cityCenterSO.mainBuildingUpgradeData[cityCenterSO.buildingIdentity.spawnLevel];

        maxEquipCount = cityCenterData.maxDeckEquipCount;
        maxPopulation = cityCenterData.maxPopulation;

        currentPopulation = 0;
        foreach (var cardSO in currentDeck.deckCardsSO)
            currentPopulation += CalculatePopulation(cardSO);

        UpdatePopulationUI();
    }

    public void RefreshCards()
    {
        // Hide all first
        foreach (var card in selectedCards)
            card.HideCard();

        var deck = currentDeck;

        for (int i = 0; i < maxEquipCount; i++)
        {
            selectedCards[i].ShowCard();

            if (i < deck.deckCardsSO.Count)
                selectedCards[i].SetSelectedCard(deck.deckCardsSO[i]);
            else
                selectedCards[i].UnsetSelectedCard();
        }
    }

    public bool TrySelectCard(ScriptableObject cardSO)
    {
        var deck = currentDeck;

        if (deck.deckCardsSO.Count >= maxEquipCount)
            return false;

        int addedPop = CalculatePopulation(cardSO);
        if (currentPopulation + addedPop > maxPopulation)
            return false;

        deck.deckCardsSO.Add(cardSO);
        currentPopulation += addedPop;
        UpdatePopulationUI();

        // Fill the first empty active slot
        foreach (var card in selectedCards)
        {
            if (card.isActive && card.upgradeDataSO == null)
            {
                card.SetSelectedCard(cardSO);
                break;
            }
        }

        return true;
    }

    public bool TryDeSelectCard(ScriptableObject cardSO)
    {
        var deck = currentDeck;

        if (!deck.deckCardsSO.Remove(cardSO))
            return false;

        currentPopulation -= CalculatePopulation(cardSO);
        UpdatePopulationUI();

        foreach (var card in selectedCards)
        {
            if (card.isActive && card.upgradeDataSO == cardSO)
            {
                card.UnsetSelectedCard();
                break;
            }
        }

        return true;
    }

    private void UpdatePopulationUI()
    {
        populationText.SetText($"{currentPopulation}/{maxPopulation}");
    }

    private static int CalculatePopulation(ScriptableObject cardSO)
    {
        if (cardSO is DefenseBuildingDataSO defense) return defense.populationCost;
        if (cardSO is UnitProduceStatsSO unit) return unit.populationCost;
        return 0;
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
            ref var factionData = ref allFactionsDeckData[i];
            factionData.faction = (FactionName)enumValues.GetValue(i);

            factionData.decks ??= new List<Deck>();

            if (factionData.decks.Count == 0)
                factionData.decks.Add(new Deck());
        }
    }
}

[Serializable]
public class FactionDeckData
{
    public FactionName faction;
    public List<Deck> decks;
}

[Serializable]
public class Deck
{
    public List<ScriptableObject> deckCardsSO = new();
}