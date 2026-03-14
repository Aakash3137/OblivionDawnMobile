using UnityEngine;
using UnityEngine.UI;

public class DeckCard : UpgradeCard
{
    [SerializeField] private Button deckSelectButton;
    [SerializeField] private Button deckDeSelectButton;
    [SerializeField] private GameObject selectionPanel;
    private bool isSelected = false;
    private int cardPopulation;

    private void Start()
    {
        // selectionPanel.SetActive(false);

        cardPopulation = upgradeDataSO switch
        {
            UnitProduceStatsSO unit => unit.populationCost,
            DefenseBuildingDataSO building => building.populationCost,
            _ => 99
        };
    }

    public void OnSelectDeckCard()
    {
        bool selected = DeckSelectionManager.Instance.TrySelectCard(upgradeDataSO);
        selectionPanel.SetActive(selected);
        isSelected = selected;

        UpdateButtonInteractivity();
    }

    public void OnDeSelectCard()
    {
        bool deselected = DeckSelectionManager.Instance.TryDeSelectCard(upgradeDataSO);
        selectionPanel.SetActive(!deselected);
        isSelected = !deselected;

        UpdateButtonInteractivity();
    }

    public void UpdateButtonInteractivity()
    {
        foreach (var upgradeCard in myPanel.allCards)
        {
            if (upgradeCard is DeckCard deckCard)
            {
                deckCard.deckSelectButton.interactable = deckCard.CanAddCard();
            }
        }
    }

    private bool CanAddCard()
    {
        if (isSelected) return false;

        var dsmInstance = DeckSelectionManager.Instance;

        if (!dsmInstance.canAddMoreCards) return false;
        if (!dsmInstance.HasPopulationFor(cardPopulation)) return false;
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

    public void EnableSelectionPanel(bool flag)
    {
        selectionPanel.SetActive(flag);
    }
}