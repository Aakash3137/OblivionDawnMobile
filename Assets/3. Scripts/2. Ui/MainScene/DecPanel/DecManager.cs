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
    [SerializeField] private DecCategory CurrentCategory = DecCategory.Offense;

    [Header("Inventory Reference")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("UI")]
    [SerializeField] private Image selectedFactionIcon;
    [SerializeField] private Canvas _Canvas;
    [SerializeField] internal TMP_Text diamondtext;
    [SerializeField] internal Userdata _Profile;
    [SerializeField] private TMP_Text Coins;
    [SerializeField] private Button OffenseBtn, DefenseBtn, ResourceBtn;

    // =========================
    // LIFECYCLE
    // =========================
    private void OnEnable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
        SetInitialData();
    }

    private void OnDisable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    // =========================
    // INITIALIZE DATA
    // =========================
    public void InitializeAllFactionDecks()
    {
        selectionData.AllFactionDecData.Clear();

        foreach (var dec in deckList)
        {
            DeckData deckData = new DeckData
            {
                FactionType = dec._FactionName,
                SelectedUnitDeck = GetFirst4(dec.UnitCards),
                SelectedDefenseDec = GetFirst4(dec.DefenseCards),
                SelectedResourceDeck = GetFirst4(dec.ResourceCards)
            };

            selectionData.AddDeckData(deckData);
        }
    }

    private List<T> GetFirst4<T>(List<T> list)
    {
        return list.GetRange(0, Mathf.Min(4, list.Count));
    }

    private void SetInitialData()
    {
        diamondtext.text = _Profile.Diamonds.ToString();
        Coins.text = _Profile.Coins.ToString();

        OffenseBtn.onClick.RemoveAllListeners();
        DefenseBtn.onClick.RemoveAllListeners();
        ResourceBtn.onClick.RemoveAllListeners();

        OffenseBtn.onClick.AddListener(() => OnCategorySelected(DecCategory.Offense));
        DefenseBtn.onClick.AddListener(() => OnCategorySelected(DecCategory.Defense));
        ResourceBtn.onClick.AddListener(() => OnCategorySelected(DecCategory.Resource));

        ShowFaction();
    }

    void ShowFaction()
    {
        int index = (int)selectionData.CurrentFaction;
        Debug.Log($"Faction Name" + index);
        CurrentCategory = selectionData.CurrentCategory;
        // Safety check
        if (index < 0 || index >= deckList.Count ||
            index >= selectionData.AllFactionDecData.Count)
        {
            Debug.LogError("Invalid faction index!");
            return;
        }

        SelectDeck(deckList[index]);

        var factionData = selectionData.AllFactionDecData[index];

        CurrentDec.UnitCards = factionData.SelectedUnitDeck;
        CurrentDec.DefenseCards = factionData.SelectedDefenseDec;
        CurrentDec.ResourceCards = factionData.SelectedResourceDeck;
    }

    // =========================
    // FACTION SELECTION
    // =========================
    public void OnClickButton(GameObject clickedButton)
    {
        foreach (var deck in deckList)
        {
            deck.InActiveObj.SetActive(true);
            deck._Checked.SetActive(false);
        }

        DecSelector selected = deckList.Find(d => d.InActiveObj == clickedButton);
        if (selected == null)
            return;

        selectionData.CurrentFaction = selected._FactionName;
        ShowFaction();
    }

    private void SelectDeck(DecSelector selected)
    {
        selected.InActiveObj.SetActive(false);
        selected._Checked.SetActive(true);

        if (selected.FactionSprite != null)
            selectedFactionIcon.sprite = selected.FactionSprite;

        selectionData.CurrentFaction = selected._FactionName;
        CurrentDec = selected;

        SelectCategory(CurrentCategory);
    }

    // =========================
    // CATEGORY HANDLING
    // =========================
    private void OnCategorySelected(DecCategory category)
    {
        selectionData.CurrentCategory = CurrentCategory = category;
        SelectCategory(category);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

    private void SelectCategory(DecCategory category)
    {
        foreach (CategorySelector cat in categoryList)
        {
            cat.InActiveObj.SetActive(true);
            cat._Checked.SetActive(false);
        }

        CategorySelector selected =
            categoryList.Find(x => x._Name == category);

        if (selected != null)
        {
            selected.InActiveObj.SetActive(false);
            selected._Checked.SetActive(true);
        }

        inventoryManager.ClearEquipped();

        switch (category)
        {
            case DecCategory.Offense:
                StartCoroutine(BuildUnitEquippedDeck(CurrentDec));
                break;

            case DecCategory.Defense:
                StartCoroutine(BuildDefenseEquippedDeck(CurrentDec));
                break;

            case DecCategory.Resource:
                StartCoroutine(BuildResourceEquippedDeck(CurrentDec));
                break;
        }


        foreach (var _Data in deckList)
        {
            if (CurrentDec._FactionName == _Data._FactionName)
            {
                RefreshAllCards(_Data);
            }
        }
    }

    // =========================
    // EQUIPPED CARDS
    // =========================
    private IEnumerator BuildUnitEquippedDeck(DecSelector deck)
    {
        yield return null;

        for (int i = 0; i < Mathf.Min(4, deck.UnitCards.Count); i++)
        {
            var card = deck.UnitCards[i];

            inventoryManager.AddEquippedItem(
                card.unitIdentity.name,
                card.unitType.ToString(),
                _Canvas,
                GetUnitStats(card),
                selectionData.CurrentFaction,
                true,
                CurrentCategory,
                deck
            );
        }
    }

    private IEnumerator BuildDefenseEquippedDeck(DecSelector deck)
    {
        yield return null;

        for (int i = 0; i < Mathf.Min(4, deck.DefenseCards.Count); i++)
        {
            var card = deck.DefenseCards[i];

            inventoryManager.AddEquippedItem(
                card.buildingIdentity.name,
                "Defense",
                _Canvas,
                GetBuildingStats(card),
                selectionData.CurrentFaction,
                true,
                CurrentCategory,
                deck
            );
        }
    }

    private IEnumerator BuildResourceEquippedDeck(DecSelector deck)
    {
        yield return null;

        for (int i = 0; i < Mathf.Min(4, deck.ResourceCards.Count); i++)
        {
            var card = deck.ResourceCards[i];

            inventoryManager.AddEquippedItem(
                card.buildingIdentity.name,
                "Resource",
                _Canvas,
                GetResourceStats(card),
                selectionData.CurrentFaction,
                true,
                CurrentCategory,
                deck
            );
        }
    }

    // =========================
    // ALL CARDS
    // =========================
    private void RefreshAllCards(DecSelector dec)
    {
        inventoryManager.ClearUnequipped();

        switch (CurrentCategory)
        {
            case DecCategory.Offense:
                BuildAllUnitCardsInventory(dec);
                break;

            case DecCategory.Defense:
                BuildAllDefenseInventory(dec);
                break;

            case DecCategory.Resource:
                BuildAllResourceInventory(dec);
                break;
        }
    }

    private void BuildAllUnitCardsInventory(DecSelector _Dec)
    {
        foreach (var card in _Dec.UnitCards)
        {
            inventoryManager.AddUnequippedItem(
                card.unitIdentity.name,
                card.unitType.ToString(),
                _Canvas,
                GetUnitStats(card),
                selectionData.CurrentFaction,
                false,
                CurrentCategory,
                _Dec
            );
        }
    }

    private void BuildAllDefenseInventory(DecSelector dec)
    {
        foreach (var card in dec.DefenseCards)
        {
            inventoryManager.AddUnequippedItem(
                card.buildingIdentity.name,
                "Defense",
                _Canvas,
                GetBuildingStats(card),
                selectionData.CurrentFaction,
                false,
                CurrentCategory,
                dec
            );
        }
    }

    private void BuildAllResourceInventory(DecSelector _Dec)
    {
        foreach (var card in _Dec.ResourceCards)
        {
            inventoryManager.AddUnequippedItem(
                card.buildingIdentity.name,
                "Resource",
                _Canvas,
                GetResourceStats(card),
                selectionData.CurrentFaction,
                false,
                CurrentCategory,
                _Dec
            );
        }
    }

    // =========================
    // STATS HELPERS
    // =========================
    private string GetUnitStats(UnitProduceStatsSO card)
    {
        return
            $"Level: {card.unitIdentity.spawnLevel + 1}\n" +
            $"Health: {card.unitUpgradeData[0].unitBasicStats.maxHealth}\n" +
            $"Armor: {card.unitUpgradeData[0].unitBasicStats.armor}\n" +
            $"Range: {card.unitUpgradeData[0].unitRangeStats.attackRange}";
    }

    private string GetBuildingStats(DefenseBuildingDataSO card)
    {
        return $"Level: {card.buildingIdentity.spawnLevel + 1}\n" +
            $"Health: {card.defenseBuildingUpgradeData[0].defenseAttackStats.damage}\n" +
            $"Armor: {card.defenseBuildingUpgradeData[0].defenseAttackStats.fireRate}\n" +
            $"Range: {card.defenseBuildingUpgradeData[0].defenseRangeStats.attackRange}";
    }

    private string GetResourceStats(ResourceBuildingDataSO card)
    {
        return
            $"Level: {card.buildingIdentity.spawnLevel + 1}\n" +
            $"Health: {card.resourceBuildingUpgradeData[0].buildingBasicStats.maxHealth}\n" +
            $"Armor: {card.resourceBuildingUpgradeData[0].buildingBasicStats.armor}\n" +
            $"Range: {card.resourceBuildingUpgradeData[0].buildingBuildTime}";

    }
}

// =========================
// SUPPORT
// =========================
public enum DecCategory
{
    Offense,
    Defense,
    Resource
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
    public List<ResourceBuildingDataSO> ResourceCards = new();
}

[System.Serializable]
public class CategorySelector
{
    public DecCategory _Name;
    public GameObject _Checked;
    public GameObject InActiveObj;
}
