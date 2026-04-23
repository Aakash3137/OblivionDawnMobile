using UnityEngine;
using UnityEngine.UI;

public class DeckCard : UpgradeCard
{
    [SerializeField] private Button deckSelectButton;
    [SerializeField] private Button deckDeSelectButton;
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private bool isSelected = false;
    private DeckSelectionManager dsmInstance;

    public override void InitializeCard()
    {
        base.InitializeCard();
        dsmInstance = DeckSelectionManager.Instance;
        deckSelectButton.interactable = CanAddCard();
    }
    public void OnSelectDeckCard()
    {
        bool selected = dsmInstance.TrySelectCard(upgradeDataSO);
        selectionPanel.SetActive(selected);
        isSelected = selected;

        RefreshAllCardsInteractivity();
    }

    public void OnDeSelectCard()
    {
        bool deselected = dsmInstance.TryDeSelectCard(upgradeDataSO);
        selectionPanel.SetActive(!deselected);
        isSelected = !deselected;

        RefreshAllCardsInteractivity();
    }
    private void RefreshAllCardsInteractivity()
    {
        foreach (DeckCard deckCard in myPanel.allCards)
        {
            deckCard.deckSelectButton.interactable = deckCard.CanAddCard();
        }
    }

    private bool CanAddCard()
    {
        if (isSelected)
        {
            // Debug.Log($"<size=20><color=red>[Deck Card] Can't add card {upgradeDataSO.name} Reason : Card Already Selected</color></size>");
            return false;
        }
        if (!dsmInstance.canAddMoreCards)
        {
            // Debug.Log($"<size=20><color=red>[Deck Card] Can't add card {upgradeDataSO.name} Reason : Deck Full</color></size>");
            return false;
        }
        if (!dsmInstance.HasPopulationFor(cardPopulation))
        {
            // Debug.Log($"<size=20><color=red>[Deck Card] Can't add card {upgradeDataSO.name} Reason : Not enough population</color></size>");
            return false;
        }
        return true;
    }

    internal override void AddListeners()
    {
        base.AddListeners();
        deckSelectButton.onClick.AddListener(OnSelectDeckCard);
        deckDeSelectButton.onClick.AddListener(OnDeSelectCard);
    }

    internal override void RemoveListeners()
    {
        base.RemoveListeners();
        deckSelectButton.onClick.RemoveListener(OnSelectDeckCard);
        deckDeSelectButton.onClick.RemoveListener(OnDeSelectCard);
    }

    public void SelectionPanelEnabled(bool enable)
    {
        isSelected = enable;
        selectionPanel.SetActive(enable);
        deckSelectButton.interactable = CanAddCard();
    }
}