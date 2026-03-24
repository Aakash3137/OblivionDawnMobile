
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
        CreateCards();
        resourceCardsRoot.parent.gameObject.SetActive(false);

        var currentFactionDeckData = selectedDeckData.allFactionsDeckData[(int)faction];
        var selectedDeck = currentFactionDeckData.decks[selectedDeckData.deckIndex];
        selectedSO = selectedDeck.deckCardsSO;

        RefreshCards();
    }

    private void OnEnable()
    {
        if (selectedSO != null)
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
        for (int i = 0; i < instantiatedCards.Count; i++)
        {
            try
            {
                instantiatedCards[i].SetSelectedCard(selectedSO[i]);
                instantiatedCards[i].EnablePopulationPanel();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                instantiatedCards[i].HideCard();
            }
        }
    }

}
