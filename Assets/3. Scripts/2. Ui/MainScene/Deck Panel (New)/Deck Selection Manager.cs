using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DeckSelectionManager : MonoBehaviour
{
    public static DeckSelectionManager Instance { get; private set; }

    [Space(10)]
    [SerializeField] private DecSelectionData decSelectionDataSO;

    [Space(10)]
    [SerializeField] private SelectedCard selectedCardPrefab;
    [SerializeField] private Transform selectedCardsContainer;
    [SerializeField] private int maxCardCount;

    [Space(10)]
    [ReadOnly][SerializeField] private FactionDeckData[] allFactionsDeckData;
    [Space(10)]
    [ReadOnly][SerializeField] private List<SelectedCard> selectedCards;

    [HideInInspector] public FactionName selectedFaction;

    private int maxEquipCount;
    private int currentDeckIndex;
    private int maxPopulation;
    private int currentPopulation;
    public int AvailablePopulation => maxPopulation - currentPopulation;

    public bool canAddMoreCards => currentDeck.deckCardsSO.Count < maxEquipCount;
    public bool HasPopulationFor(int cost) => currentPopulation + cost <= maxPopulation;

    private Deck currentDeck => allFactionsDeckData[(int)selectedFaction].decks[currentDeckIndex];
    private FactionCardPanel[] factionCardPanels;

    [Space(10)]
    [Header("UI References")]
    [SerializeField] private TMP_Text populationText;

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

        // copy the reference for allFactionsDeckData to one in SO
        allFactionsDeckData = decSelectionDataSO.allFactionsDeckData;
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

    public void RefreshSelectionCards()
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

        SortSelectedCards();
        return true;
    }

    public bool TryDeSelectCard(ScriptableObject cardSO)
    {
        var deck = currentDeck;

        if (!deck.deckCardsSO.Contains(cardSO))
            return true;

        if (deck.deckCardsSO.Count <= 1)
            return false;

        deck.deckCardsSO.Remove(cardSO);

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

        SortSelectedCards();
        return true;
    }
    private void UpdatePopulationUI()
    {
        populationText.SetText($"{currentPopulation}/{maxPopulation}");
    }

    private static int CalculatePopulation(ScriptableObject cardSO)
    {
        int populationCost = cardSO switch
        {
            UnitProduceStatsSO unit => unit.populationCost,
            DefenseBuildingDataSO building => building.populationCost,
            _ => 99
        };

        return populationCost;
    }

    public void LoadDeckData()
    {
        factionCardPanels = DeckPanelManager.Instance.factionCardPanels;

        var factionIndex = (int)selectedFaction;

        for (int j = 0; j < allFactionsDeckData[factionIndex].decks.Count; j++)
        {
            List<ScriptableObject> loadedDeckSO = allFactionsDeckData[factionIndex].decks[j].deckCardsSO;
            var currentFactionCardPanel = factionCardPanels[factionIndex].cardPanels[0];
            List<UpgradeCard> currentFactionCards = currentFactionCardPanel.allCards;

            // Debug.Log($"<color=green>[Deck Selection Manager] currentFactionCards: {loadedDeckSO.Count}</color>");
            // Debug.Log($"<color=green>[Deck Selection Manager] Selected Faction : {selectedFaction}</color>");

            foreach (var card in currentFactionCards)
            {
                if (card == null) continue;
                if (card is DeckCard deckCard)
                {
                    if (deckCard.upgradeDataSO != null && loadedDeckSO.Contains(deckCard.upgradeDataSO))
                        deckCard.EnableSelectionPanel(true);
                    else if (deckCard.upgradeDataSO == null)
                        Debug.Log($"<color=red>[Deck Selection Manager] Card upgradeDataSO not found in loaded deck</color>");
                }
            }

            if (currentFactionCards[0] is DeckCard deckCards)
                deckCards.UpdateButtonInteractivity();
        }
    }

    private void SortSelectedCards()
    {
        var factionIndex = (int)selectedFaction;
        List<ScriptableObject> loadedDeckSO = allFactionsDeckData[factionIndex].decks[currentDeckIndex].deckCardsSO;

        // Sort the selected cards by the population cost in ascending order
        selectedCards.Sort((a, b) => CalculatePopulation(a.upgradeDataSO).CompareTo(CalculatePopulation(b.upgradeDataSO)));
        loadedDeckSO.Sort((a, b) => CalculatePopulation(a).CompareTo(CalculatePopulation(b)));

        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].transform.SetSiblingIndex(i);
        }
    }
}

[Serializable]
public class FactionDeckData
{
    public FactionName faction;
    public List<Deck> decks = new();
}

[Serializable]
public class Deck
{
    public List<ScriptableObject> deckCardsSO = new();
    // public List<ResourceBuildingDataSO> resourceCardsSO = new();
}