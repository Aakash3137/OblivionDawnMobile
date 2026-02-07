using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecManager : MonoBehaviour
{
    [Header("Deck Configuration")]
    [SerializeField] private List<DecSelector> deckList = new();
    [SerializeField] private List<CategorySelector> categoryList = new();

    [Header("Selection Data")]
    [SerializeField] private DecSelectionData selectionData;
    [SerializeField] private DecSelector CurrentDec;
    [SerializeField] private DecCategory CurrentCategory;

    [Header("Inventory Reference")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("UI")]
    [SerializeField] private Image selectedFactionIcon;
    [SerializeField] private Canvas _Canvas;
    [SerializeField] internal TMP_Text diamondtext;
    [SerializeField] internal Userdata _Profile;
    [SerializeField] private TMP_Text Coins;
    [SerializeField] private Button OffenseBtn, DefenseBtn;

    //private DecCategory currentCategory;

    private void OnEnable()
    {
        diamondtext.text = _Profile.Diamonds.ToString();
        Coins.text = _Profile.Coins.ToString();

        OffenseBtn.onClick.RemoveAllListeners();
        OffenseBtn.onClick.AddListener(OnOffenseSelected);

        DefenseBtn.onClick.RemoveAllListeners();
        DefenseBtn.onClick.AddListener(OnDefenseSelected);

        if (deckList.Count > 0)
            SelectDeck(deckList[0]);
    }

    // =========================
    // FACTION BUTTON
    // =========================
    public void OnClickButton(GameObject clickedButton)
    {
        Debug.Log("OnCLick");
        foreach (var deck in deckList)
        {
            deck.InActiveObj.SetActive(true);
            deck._Checked.SetActive(false);
        }

        DecSelector selected = deckList.Find(d => d.InActiveObj == clickedButton);
        if (selected == null) return;

        SelectDeck(selected);
    }

    private void SelectDeck(DecSelector selected)
    {
        Debug.Log("Select Deck: " + selected);
        selected.InActiveObj.SetActive(false);
        selected._Checked.SetActive(true);

        if (selected.FactionSprite != null)
            selectedFactionIcon.sprite = selected.FactionSprite;

        selectionData.CurrentFaction = selected._FactionName;

        CurrentDec = selected;

        DeckData deckData = GetOrCreateDeckData(selected._FactionName);

        deckData.SelectedUnitDeck = selected.UnitCards;
        deckData.SelectedDefenseDec = selected.DefenseCards;
        selectionData.AddDeckData(deckData);

        if (CurrentCategory != DecCategory.Defense)
        {
            SelectCategory(DecCategory.Offense);
        }
        else
        {
            SelectCategory(DecCategory.Defense);
        }
    }

    // =========================
    // CATEGORY BUTTONS
    // =========================
    public void OnOffenseSelected()
    {
        SelectCategory(DecCategory.Offense);
        CurrentCategory = DecCategory.Offense;
    }

    public void OnDefenseSelected()
    {
        SelectCategory(DecCategory.Defense);
        CurrentCategory = DecCategory.Defense;
    }

    private void SelectCategory(DecCategory decCategory)
    {
        foreach (CategorySelector category in categoryList)
        {
            category.InActiveObj.SetActive(true);
            category._Checked.gameObject.SetActive(false);
        }

        CategorySelector _Category = categoryList.Find(x => x._Name == decCategory);

        if (_Category != null)
        {
            _Category.InActiveObj.SetActive(false);
            _Category._Checked.gameObject.SetActive(true);
        }

        if (decCategory == DecCategory.Defense)
        {
            StartCoroutine(BuildDefenseEquippedDec(CurrentDec.DefenseCards));
        }
        else
        {
            StartCoroutine(BuildUnitEquippedDeck(CurrentDec.UnitCards));
        }

        RefreshAllCards(CurrentDec);

        //RefreshAllCards();
    }

    // =========================
    // EQUIPPED
    // =========================
    private IEnumerator BuildUnitEquippedDeck(List<UnitProduceStatsSO> cards)
    {
        yield return null;
        Debug.Log("Build Equipped Deck: " + cards.Count);
        inventoryManager.ClearEquipped();

        int count = Mathf.Min(4, cards.Count);
        for (int i = 0; i < count; i++)
        {
            var card = cards[i];

            inventoryManager.AddEquippedItem(
                card.unitIdentity.name,
                card.unitType.ToString(),
                _Canvas,
                GetUnitStats(card),
                selectionData.CurrentFaction,
                true,
                CurrentCategory,
                cards[i],
                null
            );
        }
    }

    private IEnumerator BuildDefenseEquippedDec(List<DefenseBuildingDataSO> cards)
    {
        yield return null;
        int count = Mathf.Min(4, cards.Count);
        for (int i = 0; i < count; i++)
        {
            var card = cards[i];

            inventoryManager.AddEquippedItem(
                card.buildingIdentity.name,
                card.buildingType.ToString(),
                _Canvas,
                "GetUnitStats(card)",
                selectionData.CurrentFaction,
                true,
                CurrentCategory,
                null,
                cards[i]
            );
        }
    }

    // =========================
    // ALL CARDS
    // =========================
    private void RefreshAllCards(DecSelector _dec)
    {
        inventoryManager.ClearUnequipped();
        Debug.Log("Current Dec" + CurrentCategory);
        if (CurrentCategory == DecCategory.Offense)
            BuildAllUnitCardsInventory(_dec.UnitCards);
        else
            BuildAllDefenseInventory(_dec.DefenseCards);
    }

    private void BuildAllUnitCardsInventory(List<UnitProduceStatsSO> cards)
    {
        Debug.Log("Current Dec Category: " + CurrentCategory);
        foreach (var card in cards)
        {
            inventoryManager.AddUnequippedItem(
                card.unitIdentity.name,
                card.unitType.ToString(),
                _Canvas,
                GetUnitStats(card),
                selectionData.CurrentFaction,
                false,
                CurrentCategory,
                card,
                null
            );
        }
    }

    private void BuildAllDefenseInventory(List<DefenseBuildingDataSO> buildings)
    {
        Debug.Log("Current Dec Category: " + CurrentCategory);
        foreach (var building in buildings)
        {
            inventoryManager.AddUnequippedItem(
                building.name,
                "Defense",
                _Canvas,
                GetBuildingStats(building),
                selectionData.CurrentFaction,
                false,
                CurrentCategory,
                null,
                building
            );
        }
    }

    // =========================
    // HELPERS
    // =========================

    private string GetUnitStats(UnitProduceStatsSO card)
    {
        return
            $"UnitLevel: {card.unitIdentity.spawnLevel + 1}\n" +
            $"Health: {card.unitUpgradeData[0].unitBasicStats.maxHealth}\n" +
            $"Armor: {card.unitUpgradeData[0].unitBasicStats.armor}\n" +
            $"Attack Range: {card.unitUpgradeData[0].unitRangeStats.attackRange}";
    }

    private string GetBuildingStats(DefenseBuildingDataSO card)
    {
        return
            $"Building Level: ";
    }

    private DeckData GetCurrentDeck()
    {
        return selectionData.AllFactionDecData
            .Find(d => d.FactionType == selectionData.CurrentFaction);
    }

    private DeckData GetOrCreateDeckData(FactionName faction)
    {
        var deck = selectionData.AllFactionDecData
            .Find(d => d.FactionType == faction);

        if (deck == null)
        {
            deck = new DeckData { FactionType = faction };
            selectionData.AddDeckData(deck);
        }

        return deck;
    }
}

// =========================
// SUPPORT
// =========================

public enum DecCategory
{
    Offense,
    Defense
}


[System.Serializable]
public class DecSelector
{
    public FactionName _FactionName;
    public GameObject _Checked;
    public GameObject InActiveObj;
    public Sprite FactionSprite;
    public List<UnitProduceStatsSO> UnitCards = new();
    public List<DefenseBuildingDataSO> DefenseCards = new();
}

[System.Serializable]
public class CategorySelector
{
    public DecCategory _Name;
    public GameObject _Checked;
    public GameObject InActiveObj;
}
