
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FactionDisplayCard : MonoBehaviour
{
    [SerializeField] private DecSelectionData selectedDeckData;
    [SerializeField] private AllBuildingData buildingsData;
    [SerializeField] private SelectedCard selectedCardPrefab;
    [SerializeField] private Transform selectedCardsRoot;
    [SerializeField] private Transform resourceCardsRoot;
    [ReadOnly] public FactionName faction;
    private List<ScriptableObject> selectedSO;
    private List<SelectedCard> instantiatedCards = new List<SelectedCard>();
    [SerializeField] private int maxCardCount = 8;

    private void Start()
    {
        resourceCardsRoot.parent.gameObject.SetActive(false);

        var currentFactionDeckData = selectedDeckData.allFactionsDeckData[(int)faction];
        var selectedDeck = currentFactionDeckData.decks[selectedDeckData.selectedDeckIndex];
        selectedSO = selectedDeck.deckCardsSO;

        CreateCards();
        RefreshCards();
    }

    private void CreateCards()
    {
        for (int i = 0; i < maxCardCount; i++)
        {
            var card = Instantiate(selectedCardPrefab, selectedCardsRoot);
            instantiatedCards.Add(card);
        }
    }

    private void RefreshCards()
    {
        foreach (var card in instantiatedCards)
        {
            try
            {
                card.SetSelectedCard(selectedSO[instantiatedCards.IndexOf(card)]);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                card.HideCard();
            }
        }
    }

}
